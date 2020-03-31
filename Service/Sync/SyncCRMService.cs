using Service.Sync.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Utils.SQL;

namespace Service.Sync
{
    public class SyncCRMService
    {
        private Database _db = new Database();
        private string crm_db_link = System.Configuration.ConfigurationManager.AppSettings["CRM_DB_LINK"];

        public void Execute()
        {
            try
            {
                SyncCustomerData();
                SyncProductData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 客户数据

        /// <summary>
        /// 同步客户数据
        /// </summary>
        private void SyncCustomerData()
        {
            var sspc_crm_cuslist = InitSSPC_CRM_CUSTOMERS();
            var sspc_crm_usercustomers = InitSSPC_CRM_UserCustomers();
            var sspc_eorder_cuslist = InitSSPC_EOrder_CUSTOMERS();
            var sspc_eorder_renyuans = InitSSPC_EOrder_Renyuans();
            var sspc_eorder_usercustomerExcel = InitSSPC_EOrder_KehuExcels();
            var sspc_crm_cusshiplist = InitSSPC_CRM_CusShips();
            var sspc_eorder_cusshiplist = InitSSPC_EOrder_CusShips();
            var sspc_eorder_usercustomers = InitSSPC_EOrder_UserCustomers();

            var sspc_eorder_cuslistAdd = new List<tbl_eorder_cussale>();
            var sspc_eorder_cusshiplistAdd = new List<tbl_eorder_cusship>();
            var sspc_eorder_userCustomerAdd = new List<tbl_eorder_usercustomer>();
            var sspc_eorder_renyuanAdd = new List<mstrenyuan>();
            var sspc_eorder_cuslistUpdate = new List<tbl_eorder_cussale>();
            var sspc_eorder_userCustomerDelete = new List<tbl_eorder_usercustomer>();
            var sspc_eorder_cusshiplistDelete = new List<tbl_eorder_cusship>();

            foreach (var item in sspc_crm_cuslist)
            {

                #region 处理经销商基本信息
                //获取eorder中对应的经销商数据
                var sspc_eorder_cus = sspc_eorder_cuslist.FirstOrDefault(p => p.KUNNR == item.KUNNR && p.VKORG == "4033");
                if (sspc_eorder_cus == null)
                {
                    //如果没有，则需要新增
                    sspc_eorder_cuslistAdd.Add(item);
                }
                else
                {
                    //如果有，验证是否有变更修改
                    if (GetChange(sspc_eorder_cus, item))
                    {
                        item.ID = sspc_eorder_cus.ID;
                        sspc_eorder_cuslistUpdate.Add(item);
                    }
                }
                #endregion

                //看此经销商是否存在对应的商务
                if (sspc_eorder_usercustomers.Any(p => p.CustomerCode == item.KUNNR && p.CompanyCode == "4033"))
                {
                    var tmpeucs = sspc_eorder_usercustomers.FindAll(p => p.CustomerCode == item.KUNNR && p.CompanyCode == "4033");
                    sspc_eorder_userCustomerDelete.AddRange(tmpeucs);
                }
                var tmpucs = sspc_crm_usercustomers.FindAll(p => p.KUNNR == item.KUNNR);
                var rennms = tmpucs.Select(p => p.ADName).Distinct().ToArray();
                var tmpusers = sspc_eorder_renyuans.FindAll(p => rennms.Contains(p.ADName) && p.IsEmployee == "1" && p.ShiFouTingYon == 0 && p.ShiFouZaiZhi == 1);
                foreach (var tu in tmpusers)
                {
                    //添加商务人员-经销商信息
                    sspc_eorder_userCustomerAdd.Add(new tbl_eorder_usercustomer
                    {
                        ID = Guid.NewGuid(),
                        CompanyCode = "4033",
                        CustomerCode = item.KUNNR,
                        Renyuanid = tu.RenYuanID
                    });
                }

                if (tmpucs.Count != tmpusers.Count)
                    Console.WriteLine($"{tmpucs.Count},{tmpusers.Count}");

                var tmpusercoustomer = sspc_eorder_usercustomerExcel.FirstOrDefault(p => p.经销商名称 == item.ZNAME1);

                #region 处理经销商的对应操作人员

                //检查经销商商业人员是否存在，如果不存在，则需要新增
                var customerRenyuan = sspc_eorder_renyuans.FirstOrDefault(p => p.BeiZhu == item.ZNAME1);
                if (customerRenyuan == null)
                {
                    sspc_eorder_renyuanAdd.Add(new mstrenyuan
                    {
                        LoginID = tmpusercoustomer == null || string.IsNullOrWhiteSpace(tmpusercoustomer.经销商简称) ? item.ZNAME1 : tmpusercoustomer.经销商简称,
                        LoginPW = "1234",
                        ShiFouTingYon = 0,
                        BeiZhu = item.ZNAME1,
                        RenYuanBH = item.KUNNR,
                        Sap_code = item.KUNNR,
                        RenYuanNM = item.ZNAME1,
                        western_name = item.ZNAME1,
                        ZhiWu = "12101435",
                        Address = item.ZADDRESS,
                        ShiFouZaiZhi = 1,
                        UseMPW = 0,
                        CreateUser = 1000000,
                        CreateTime = DateTime.Now,
                        OnlineFlag = false,
                        CompanyCode = "4033",
                        IsEmployee = "2",
                        CusCode = $"4033|{item.KUNNR},"
                    });
                }

                #endregion

                #region 处理经销商的Ship信息

                //找到eorder中经销商的货运信息
                var sspc_eorder_cusships = sspc_eorder_cusshiplist.FindAll(p => p.KUNNR == item.KUNNR);
                //找到crm中经销商的货运信息
                var sspc_crm_cusships = sspc_crm_cusshiplist.FindAll(p => p.KUNNR == item.KUNNR);

                if (sspc_eorder_cusships.Count > 0)
                {
                    sspc_eorder_cusshiplistDelete.AddRange(sspc_eorder_cusships);
                }
                if (sspc_crm_cusships.Count > 0)
                {
                    if (!sspc_eorder_cusshiplistAdd.Any(p => p.KUNNR == item.KUNNR))
                        sspc_eorder_cusshiplistAdd.AddRange(sspc_crm_cusships);
                }

                #endregion
            }

            #region 执行数据库操作

            if (sspc_eorder_cuslistAdd.Count > 0
                || sspc_eorder_cuslistUpdate.Count > 0
                || sspc_eorder_cusshiplistDelete.Count > 0
                || sspc_eorder_cusshiplistAdd.Count > 0
                || sspc_eorder_userCustomerDelete.Count > 0
                || sspc_eorder_userCustomerAdd.Count > 0
                || sspc_eorder_renyuanAdd.Count > 0)
            {
                //写入经销商表
                using (var scope = _db.GetTransaction())
                {
                    // 其他任务处理 …
                    if (sspc_eorder_cuslistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_cuslistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_cussales", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_cuslistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    if (sspc_eorder_cuslistUpdate.Count > 0)
                    {
                        foreach (var item in sspc_eorder_cuslistUpdate)
                        {
                            _db.Update(item);
                        }
                    }
                    if (sspc_eorder_cusshiplistDelete.Count > 0)
                        _db.Delete<tbl_eorder_cusship>(new Sql($" where id in ({string.Join(",", sspc_eorder_cusshiplistDelete.Select(p => "'" + p.ID + "'"))})"));
                    if (sspc_eorder_cusshiplistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_cusshiplistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_cusship", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_cusshiplistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    //if (sspc_eorder_userCustomerDelete.Count > 0)
                    //    _db.Delete<tbl_eorder_usercustomer>(new Sql($" where id in ({string.Join(",", sspc_eorder_userCustomerDelete.Select(p => "'" + p.ID + "'"))})"));
                    //if (sspc_eorder_userCustomerAdd.Count > 0)
                    //{
                    //    int pi = 0;
                    //    int ps = 50;
                    //    var addtmp = sspc_eorder_userCustomerAdd.Skip(pi * ps).Take(ps).ToList();
                    //    while (addtmp.Count() > 0)
                    //    {
                    //        _db.InsertList("user_bfp.tbl_eorder_usercustomer", "ID", false, addtmp);
                    //        pi++;
                    //        addtmp = sspc_eorder_userCustomerAdd.Skip(pi * ps).Take(ps).ToList();
                    //    }
                    //}
                    //if (sspc_eorder_renyuanAdd.Count > 0)
                    //{
                    //    int pi = 0;
                    //    int ps = 25;
                    //    var addtmp = sspc_eorder_renyuanAdd.Skip(pi * ps).Take(ps).ToList();
                    //    while (addtmp.Count() > 0)
                    //    {
                    //        _db.InsertList("user_bfp.mstrenyuan", "RenyuanID", true, addtmp);
                    //        pi++;
                    //        addtmp = sspc_eorder_renyuanAdd.Skip(pi * ps).Take(ps).ToList();
                    //    }
                    //}
                    // Commit
                    scope.Complete();
                }
            }

            #endregion
        }

        /// <summary>
        ///加载SSPC-CRM客户数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_cussale> InitSSPC_CRM_CUSTOMERS()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_CUSTOMERS", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_cussale>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM客户SHIP数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_cusship> InitSSPC_CRM_CusShips()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_CusShips", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_cusship>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM客户-商务人员对应数据
        /// </summary>
        /// <returns></returns>
        private List<CRM_UserCustomer> InitSSPC_CRM_UserCustomers()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_UserCustomers", GetCRMHashTable());
            var list = _db.Query<CRM_UserCustomer>(sql);
            return list.ToList();
        }

        /// <summary>
        ///加载SSPC-EOrder客户数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_cussale> InitSSPC_EOrder_CUSTOMERS()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_CUSTOMERS", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_cussale>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载0622商务人员提供的excel数据
        /// </summary>
        /// <returns></returns>
        private List<T_ExcelCustomer20180622> InitSSPC_EOrder_KehuExcels()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_KehuExcels", GetSSPCHashTable());
            var list = _db.Query<T_ExcelCustomer20180622>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载系统中的所有人员信息
        /// </summary>
        /// <returns></returns>
        private List<mstrenyuan> InitSSPC_EOrder_Renyuans()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_Renyuans", GetSSPCHashTable());
            var list = _db.Query<mstrenyuan>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-EOrder客户SHIP数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_cusship> InitSSPC_EOrder_CusShips()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_CusShips", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_cusship>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-EOrder客户-商务人员对应数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_usercustomer> InitSSPC_EOrder_UserCustomers()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_UserCustomers", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_usercustomer>(sql);
            return list.ToList();
        }

        #endregion

        #region 产品数据

        /// <summary>
        /// 同步客户数据
        /// </summary>
        private void SyncProductData()
        {
            var sspc_crm_productlist = InitSSPC_CRM_Products();
            var sspc_crm_productmvkelist = InitSSPC_CRM_ProductMvkes();
            var sspc_crm_productmarmlist = InitSSPC_CRM_ProductMarms();
            var sspc_crm_cusproductlist = InitSSPC_CRM_CusProducts();
            var sspc_crm_cusproductpricelist = InitSSPC_CRM_CusProductPrices();

            var sspc_eorder_productlist = InitSSPC_EOrder_Products();
            var sspc_eorder_productmvkelist = InitSSPC_EOrder_ProductMvkes();
            var sspc_eorder_productmarmlist = InitSSPC_EOrder_ProductMarms();
            var sspc_eorder_cusproductlist = InitSSPC_EOrder_CusProducts();
            var sspc_eorder_cusproductpricelist = InitSSPC_EOrder_CusProductPrices();

            var sspc_eorder_productlistAdd = new List<tbl_eorder_matnr>();
            var sspc_eorder_productmvkelistAdd = new List<tbl_eorder_mvke>();
            var sspc_eorder_productmarmlistAdd = new List<tbl_eorder_marm>();
            var sspc_eorder_cusproductlistAdd = new List<tbl_eorder_list>();
            var sspc_eorder_cusproductpricelistAdd = new List<tbl_eorder_price>();

            var sspc_eorder_productlistUpdate = new List<tbl_eorder_matnr>();
            var sspc_eorder_productmvkelistUpdate = new List<tbl_eorder_mvke>();
            var sspc_eorder_productmarmlistUpdate = new List<tbl_eorder_marm>();
            var sspc_eorder_cusproductlistUpdate = new List<tbl_eorder_list>();
            var sspc_eorder_cusproductpricelistUpdate = new List<tbl_eorder_price>();

            var sspc_eorder_productlistDelete = new List<tbl_eorder_matnr>();
            var sspc_eorder_productmvkelistDelete = new List<tbl_eorder_mvke>();
            var sspc_eorder_productmarmlistDelete = new List<tbl_eorder_marm>();
            var sspc_eorder_cusproductlistDelete = new List<tbl_eorder_list>();
            var sspc_eorder_cusproductpricelistDelete = new List<tbl_eorder_price>();

            #region 处理产品主数据

            //同步产品主数据
            foreach (var item in sspc_crm_productlist)
            {
                if (sspc_eorder_productlist.Any(p => p.MATNR == item.MATNR))
                {
                    var tmp = sspc_eorder_productlist.FirstOrDefault(p => p.MATNR == item.MATNR);
                    if (GetChange(tmp, item))
                    {
                        item.ID = tmp.ID;
                        sspc_eorder_productlistUpdate.Add(item);
                    }
                }
                else
                    sspc_eorder_productlistAdd.Add(item);
            }
            foreach (var item in sspc_eorder_productlist)
            {
                if (!sspc_crm_productlist.Any(p => p.MATNR == item.MATNR))
                    sspc_eorder_productlistDelete.Add(item);
            }

            #endregion

            #region 处理销售单位数据

            //同步产品销售单位数据
            foreach (var item in sspc_crm_productmvkelist)
            {
                if (sspc_eorder_productmvkelist.Any(p => p.MATNR == item.MATNR))
                {
                    var tmp = sspc_eorder_productmvkelist.FirstOrDefault(p => p.MATNR == item.MATNR);
                    if (GetChange(tmp, item))
                    {
                        item.ID = tmp.ID;
                        sspc_eorder_productmvkelistUpdate.Add(item);
                    }
                }
                else
                    sspc_eorder_productmvkelistAdd.Add(item);
            }
            foreach (var item in sspc_eorder_productmvkelist)
            {
                if (!sspc_crm_productmvkelist.Any(p => p.MATNR == item.MATNR))
                    sspc_eorder_productmvkelistDelete.Add(item);
            }

            #endregion

            #region 处理销售单位比例数据

            foreach (var item in sspc_crm_productmarmlist)
            {
                if (sspc_eorder_productmarmlist.Any(p => p.MATNR == item.MATNR))
                {
                    var tmp = sspc_eorder_productmarmlist.FirstOrDefault(p => p.MATNR == item.MATNR);
                    if (GetChange(tmp, item))
                    {
                        item.ID = tmp.ID;
                        sspc_eorder_productmarmlistUpdate.Add(item);
                    }
                }
                else
                    sspc_eorder_productmarmlistAdd.Add(item);
            }
            foreach (var item in sspc_eorder_productmarmlist)
            {
                if (!sspc_crm_productmarmlist.Any(p => p.MATNR == item.MATNR))
                    sspc_eorder_productmarmlistDelete.Add(item);
            }

            #endregion

            #region 处理产品与经销商之间的关系数据

            foreach (var item in sspc_crm_cusproductlist)
            {
                if (sspc_eorder_cusproductlist.Any(p => p.MATNR == item.MATNR && p.KUNNR == item.KUNNR))
                {
                    var tmp = sspc_eorder_cusproductlist.FirstOrDefault(p => p.MATNR == item.MATNR && p.KUNNR == item.KUNNR);
                    if (GetChange(tmp, item))
                    {
                        item.ID = tmp.ID;
                        sspc_eorder_cusproductlistUpdate.Add(item);
                    }
                }
                else
                    sspc_eorder_cusproductlistAdd.Add(item);
            }
            foreach (var item in sspc_eorder_cusproductlist)
            {
                if (!sspc_crm_cusproductlist.Any(p => p.MATNR == item.MATNR && p.KUNNR == item.KUNNR))
                    sspc_eorder_cusproductlistDelete.Add(item);
            }

            #endregion

            #region 处理产品与经销商的销售价格数据

            foreach (var item in sspc_crm_cusproductpricelist)
            {
                if (sspc_eorder_cusproductpricelist.Any(p => p.MATNR == item.MATNR && p.KUNNR == item.KUNNR))
                {
                    var tmp = sspc_eorder_cusproductpricelist.FirstOrDefault(p => p.MATNR == item.MATNR && p.KUNNR == item.KUNNR);
                    if (GetChange(tmp, item))
                    {
                        item.ID = tmp.ID;
                        sspc_eorder_cusproductpricelistUpdate.Add(item);
                    }
                    else if (tmp.KBETR != item.KBETR)
                    {
                        item.ID = tmp.ID;
                        sspc_eorder_cusproductpricelistUpdate.Add(item);
                    }
                }
                else
                    sspc_eorder_cusproductpricelistAdd.Add(item);
            }
            foreach (var item in sspc_eorder_cusproductpricelist)
            {
                if (!sspc_crm_cusproductpricelist.Any(p => p.MATNR == item.MATNR && p.KUNNR == item.KUNNR))
                    sspc_eorder_cusproductpricelistDelete.Add(item);
            }

            #endregion

            #region 执行数据库操作

            if (sspc_eorder_productlistAdd.Count > 0
                || sspc_eorder_productmvkelistAdd.Count > 0
                || sspc_eorder_productmarmlistAdd.Count > 0
                || sspc_eorder_cusproductlistAdd.Count > 0
                || sspc_eorder_cusproductpricelistAdd.Count > 0

                || sspc_eorder_productlistDelete.Count > 0
                || sspc_eorder_productmvkelistDelete.Count > 0
                || sspc_eorder_productmarmlistDelete.Count > 0
                || sspc_eorder_cusproductlistDelete.Count > 0
                || sspc_eorder_cusproductpricelistDelete.Count > 0

                || sspc_eorder_productlistUpdate.Count > 0
                || sspc_eorder_productmvkelistUpdate.Count > 0
                || sspc_eorder_productmarmlistUpdate.Count > 0
                || sspc_eorder_cusproductlistUpdate.Count > 0
                || sspc_eorder_cusproductpricelistUpdate.Count > 0)
            {
                //写入
                using (var scope = _db.GetTransaction())
                {
                    if (sspc_eorder_productlistUpdate.Count > 0)
                    {
                        foreach (var item in sspc_eorder_productlistUpdate)
                        {
                            _db.Update(item);
                        }
                    }
                    if (sspc_eorder_productmvkelistUpdate.Count > 0)
                    {
                        foreach (var item in sspc_eorder_productmvkelistUpdate)
                        {
                            _db.Update(item);
                        }
                    }
                    if (sspc_eorder_productmarmlistUpdate.Count > 0)
                    {
                        foreach (var item in sspc_eorder_productmarmlistUpdate)
                        {
                            _db.Update(item);
                        }
                    }
                    if (sspc_eorder_cusproductlistUpdate.Count > 0)
                    {
                        foreach (var item in sspc_eorder_cusproductlistUpdate)
                        {
                            _db.Update(item);
                        }
                    }
                    if (sspc_eorder_cusproductpricelistUpdate.Count > 0)
                    {
                        foreach (var item in sspc_eorder_cusproductpricelistUpdate)
                        {
                            _db.Update(item);
                        }
                    }

                    if (sspc_eorder_productlistDelete.Count > 0)
                        _db.Delete<tbl_eorder_matnr>(new Sql($" where id in ({string.Join(",", sspc_eorder_productlistDelete.Select(p => "'" + p.ID + "'"))})"));
                    if (sspc_eorder_productmvkelistDelete.Count > 0)
                        _db.Delete<tbl_eorder_mvke>(new Sql($" where id in ({string.Join(",", sspc_eorder_productmvkelistDelete.Select(p => "'" + p.ID + "'"))})"));
                    if (sspc_eorder_productmarmlistDelete.Count > 0)
                        _db.Delete<tbl_eorder_marm>(new Sql($" where id in ({string.Join(",", sspc_eorder_productmarmlistDelete.Select(p => "'" + p.ID + "'"))})"));
                    if (sspc_eorder_cusproductlistDelete.Count > 0)
                        _db.Delete<tbl_eorder_list>(new Sql($" where id in ({string.Join(",", sspc_eorder_cusproductlistDelete.Select(p => "'" + p.ID + "'"))})"));
                    if (sspc_eorder_cusproductpricelistDelete.Count > 0)
                        _db.Delete<tbl_eorder_price>(new Sql($" where id in ({string.Join(",", sspc_eorder_cusproductpricelistDelete.Select(p => "'" + p.ID + "'"))})"));

                    if (sspc_eorder_productlistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_productlistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_matnr", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_productlistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    if (sspc_eorder_productmvkelistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_productmvkelistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_mvke", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_productmvkelistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    if (sspc_eorder_productmarmlistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_productmarmlistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_marm", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_productmarmlistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    if (sspc_eorder_cusproductlistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_cusproductlistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_list", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_cusproductlistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    if (sspc_eorder_cusproductpricelistAdd.Count > 0)
                    {
                        int pi = 0;
                        int ps = 50;
                        var addtmp = sspc_eorder_cusproductpricelistAdd.Skip(pi * ps).Take(ps).ToList();
                        while (addtmp.Count() > 0)
                        {
                            _db.InsertList("user_bfp.tbl_eorder_price", "ID", false, addtmp);
                            pi++;
                            addtmp = sspc_eorder_cusproductpricelistAdd.Skip(pi * ps).Take(ps).ToList();
                        }
                    }
                    // Commit
                    scope.Complete();
                }
            }

            #endregion
        }

        /// <summary>
        ///加载SSPC-CRM产品数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_matnr> InitSSPC_CRM_Products()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_Products", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_matnr>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM产品销售单位数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_mvke> InitSSPC_CRM_ProductMvkes()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_ProductMvkes", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_mvke>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM产品销售单位比例数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_marm> InitSSPC_CRM_ProductMarms()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_ProductMarms", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_marm>(sql);
            return list.ToList();
        }

        /// <summary>
        ///加载SSPC-CRM客户产品数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_list> InitSSPC_CRM_CusProducts()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_CusProducts", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_list>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM客户产品价格数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_price> InitSSPC_CRM_CusProductPrices()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_CRM_CusProductPrices", GetCRMHashTable());
            var list = _db.Query<tbl_eorder_price>(sql);
            return list.ToList();
        }

        /// <summary>
        ///加载SSPC-CRM产品数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_matnr> InitSSPC_EOrder_Products()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_Products", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_matnr>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM产品销售单位数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_mvke> InitSSPC_EOrder_ProductMvkes()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_ProductMvkes", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_mvke>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM产品销售单位比例数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_marm> InitSSPC_EOrder_ProductMarms()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_ProductMarms", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_marm>(sql);
            return list.ToList();
        }

        /// <summary>
        ///加载SSPC-CRM客户产品数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_list> InitSSPC_EOrder_CusProducts()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_CusProducts", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_list>(sql);
            return list.ToList();
        }

        /// <summary>
        /// 加载SSPC-CRM客户产品价格数据
        /// </summary>
        /// <returns></returns>
        private List<tbl_eorder_price> InitSSPC_EOrder_CusProductPrices()
        {
            string sql = SQLLoaderComponent.GetSQLQuery("SyncCRM", "InitSSPC_EOrder_CusProductPrices", GetSSPCHashTable());
            var list = _db.Query<tbl_eorder_price>(sql);
            return list.ToList();
        }

        #endregion

        #region 人员

        #endregion

        private Hashtable GetCRMHashTable()
        {
            Hashtable ht = new Hashtable();
            ht.Add("CRM_DB_LINK", crm_db_link);
            return ht;
        }

        public Hashtable GetSSPCHashTable()
        {
            return new Hashtable();
        }

        private bool GetChange<T>(T t1, T t2)
        {
            if (t1 == null || t2 == null)
                return false;

            System.Reflection.PropertyInfo[] properties = typeof(T).GetProperties();
            if (properties.Length <= 0)
                return false;

            var typearr = new string[] { "int", "float", "string" };
            var result = false;
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                //if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("string"))
                var typename = item.PropertyType.Name.ToLower();
                if (typearr.Contains(typename))
                {

                    string name = item.Name;
                    object oldValue = item.GetValue(t1, null);
                    object newValue = item.GetValue(t2, null);
                    string oldValue_string = oldValue == null ? "" : oldValue.ToString().Trim();
                    string newValue_string = newValue == null ? "" : newValue.ToString().Trim();
                    if (!oldValue_string.Equals(newValue_string))
                        result = true;
                }
            }
            return result;
        }
    }
}