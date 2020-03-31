using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using Aspose.Slides.Charts;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Utils.Excel
{
    public class ExportExcel
    {

        protected void ExportData(string strContent, string FileName)
        {

            FileName = FileName + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Charset = "gb2312";
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //this.Page.EnableViewState = false; 
            // 添加头信息，为"文件下载/另存为"对话框指定默认文件名 
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");
            // 把文件流发送到客户端 
            HttpContext.Current.Response.Write("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\">");
            HttpContext.Current.Response.Write(strContent);
            HttpContext.Current.Response.Write("</body></html>");
            // 停止页面的执行 
            //Response.End();
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="obj"></param>
        public void ExportData(GridView obj)
        {
            try
            {
                string style = "";
                if (obj.Rows.Count > 0)
                {
                    style = @"<style> .text { mso-number-format:\@; } </script> ";
                }
                else
                {
                    style = "no data.";
                }

                HttpContext.Current.Response.ClearContent();
                DateTime dt = DateTime.Now;
                string filename = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=ExportData" + filename + ".xls");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                obj.RenderControl(htw);
                HttpContext.Current.Response.Write(style);
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
            }
            catch
            {
            }
        }



        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="FileName"></param>
        /// <param name="fileSize"></param>
        /// <param name="data"></param>
        public void FileStreamDownload(HttpRequest request, HttpResponse response, string FileName, string fileSize, byte[] data)
        {
            //response.ContentType = "application/download";
            //response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.ContentType = "application/vnd.ms-excel";
            
            string fileName = string.Empty;
            string needEncodeBrowsers = "Firefox,Safari";
            if (string.IsNullOrEmpty(needEncodeBrowsers))
            {
                //Firefox,Safari必须要Encode
                needEncodeBrowsers = "Firefox,Safari";
            }
            if (needEncodeBrowsers.ToLower().IndexOf(request.Browser.Browser.ToLower()) >= 0)
            {
                fileName = FileName.Replace(" ", "_");
            }
            else
            {
                fileName = HttpUtility.UrlEncode(FileName.Replace(" ", "_"));
            }
            try
            {
                response.Clear();
                response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                response.AddHeader("Content-Length", fileSize);
                response.BinaryWrite(data);

                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存为文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dt"></param>
        /// <param name="colsWidth"></param>
        /// <returns></returns>
        public bool SaveAsFile(string filename, System.Data.DataTable dt, int[] colsWidth = null)
        {
            try
            {
                var data = GenerateExcel(dt, colsWidth);

                //保存为Excel文件  
                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Datatable生成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colsWidth">列宽</param>
        /// <returns></returns>
        public byte[] GenerateExcel(System.Data.DataTable dt, int[] colsWidth = null)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet("Data");

            //设置列宽
            if (colsWidth != null)
            {
                for (int i = 0; i < colsWidth.Length; i++)
                {
                    sheet.SetColumnWidth(i, colsWidth[i] * 256);
                }

            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }
            }

            //列头
            IRow row = sheet.CreateRow(0);
            ICellStyle titleStyle = book.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.FillBackgroundColor = HSSFColor.LightBlue.Index;
            IFont titleFont = book.CreateFont();
            titleFont.Boldweight = (short)FontBoldWeight.Bold;
            titleStyle.SetFont(titleFont);
            titleStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            titleStyle.BottomBorderColor = HSSFColor.Black.Index;
            titleStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Dotted;
            titleStyle.LeftBorderColor = HSSFColor.Black.Index;
            titleStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Dotted;
            titleStyle.RightBorderColor = HSSFColor.Black.Index;
            titleStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            titleStyle.TopBorderColor = HSSFColor.Black.Index;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                row.CreateCell(i).CellStyle = titleStyle;
                row.GetCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            sheet.CreateFreezePane(0, 1, 0, 1);
            //数据
            ICellStyle dataStyle = book.CreateCellStyle();
            dataStyle.Alignment = HorizontalAlignment.Center;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row2 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    row2.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                    row2.GetCell(j).CellStyle = dataStyle;
                }
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);

            book = null;
            ms.Close();
            ms.Dispose();
            return ms.ToArray();
        }

        /// <summary>
        /// Datatable生成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colsWidth">列宽</param>
        /// <returns></returns>
        public byte[] GenerateExcelTwoSheet(System.Data.DataTable dt1, System.Data.DataTable dt2, int[] colsWidth1 = null, int[] colsWidth2 = null)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("Data1");
            ISheet sheet2 = book.CreateSheet("Data2");
            //设置列宽
            if (colsWidth1 != null)
            {
                for (int i = 0; i < colsWidth1.Length; i++)
                {
                    sheet1.SetColumnWidth(i, colsWidth1[i] * 256);
                }

            }
            else
            {
                for (int i = 0; i < dt1.Columns.Count; i++)
                {
                    sheet1.SetColumnWidth(i, 20 * 256);
                }
            }

            //列头
            IRow row = sheet1.CreateRow(0);
            row.Height = 600;
            ICellStyle titleStyle1 = book.CreateCellStyle();
            titleStyle1.Alignment = HorizontalAlignment.Center;
            titleStyle1.VerticalAlignment = VerticalAlignment.Center;
            titleStyle1.FillBackgroundColor = HSSFColor.LightBlue.Index;
            titleStyle1.WrapText = true;
            IFont titleFont1 = book.CreateFont();
            titleFont1.Boldweight = (short)FontBoldWeight.Bold;
            titleStyle1.SetFont(titleFont1);
            titleStyle1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            titleStyle1.BottomBorderColor = HSSFColor.Black.Index;
            titleStyle1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Dotted;
            titleStyle1.LeftBorderColor = HSSFColor.Black.Index;
            titleStyle1.BorderRight = NPOI.SS.UserModel.BorderStyle.Dotted;
            titleStyle1.RightBorderColor = HSSFColor.Black.Index;
            titleStyle1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            titleStyle1.TopBorderColor = HSSFColor.Black.Index;
            for (int i = 0; i < dt1.Columns.Count; i++)
            {
                row.CreateCell(i).CellStyle = titleStyle1;
                row.GetCell(i).SetCellValue(dt1.Columns[i].ColumnName);
            }
            sheet1.CreateFreezePane(0, 1, 0, 1);
            //数据
            ICellStyle dataStyle1 = book.CreateCellStyle();
            dataStyle1.Alignment = HorizontalAlignment.Center;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                IRow row2 = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    row2.CreateCell(j).SetCellValue(dt1.Rows[i][j].ToString());
                    row2.GetCell(j).CellStyle = dataStyle1;
                }
            }



            if (colsWidth2 != null)
            {
                for (int i = 0; i < colsWidth2.Length; i++)
                {
                    sheet2.SetColumnWidth(i, colsWidth2[i] * 256);
                }

            }
            else
            {
                for (int i = 0; i < dt2.Columns.Count; i++)
                {
                    sheet2.SetColumnWidth(i, 20 * 256);
                }
            }

            //列头
            IRow row3 = sheet2.CreateRow(0);
            row3.Height = 600;
            ICellStyle titleStyle = book.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.VerticalAlignment = VerticalAlignment.Center;
            titleStyle.FillBackgroundColor = HSSFColor.LightBlue.Index;
            titleStyle.WrapText = true;
            IFont titleFont = book.CreateFont();
            titleFont.Boldweight = (short)FontBoldWeight.Bold;
            titleStyle.SetFont(titleFont);
            titleStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            titleStyle.BottomBorderColor = HSSFColor.Black.Index;
            titleStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Dotted;
            titleStyle.LeftBorderColor = HSSFColor.Black.Index;
            titleStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Dotted;
            titleStyle.RightBorderColor = HSSFColor.Black.Index;
            titleStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            titleStyle.TopBorderColor = HSSFColor.Black.Index;
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                row3.CreateCell(i).CellStyle = titleStyle;
                row3.GetCell(i).SetCellValue(dt2.Columns[i].ColumnName);
            }
            sheet2.CreateFreezePane(0, 1, 0, 1);
            //数据
            ICellStyle dataStyle = book.CreateCellStyle();
            dataStyle.Alignment = HorizontalAlignment.Center;
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                IRow row2 = sheet2.CreateRow(i + 1);
                for (int j = 0; j < dt2.Columns.Count; j++)
                {
                    row2.CreateCell(j).SetCellValue(dt2.Rows[i][j].ToString());
                    row2.GetCell(j).CellStyle = dataStyle;
                }
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);

            book = null;
            ms.Close();
            ms.Dispose();
            return ms.ToArray();
        }
    }
}