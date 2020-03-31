using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;

namespace Utils.ExtentHelper
{
    public static class AjaxHelperExt
    {

        #region 查询表单
        public static MvcForm BeginSearchFrom(this AjaxHelper _ajaxHelper, string _actionName, object _htmlAttributes, string _updateTargetId = "list", string _searchFormId = "searchForm")
        {
            _actionName = _actionName ?? "List";
            _htmlAttributes = _htmlAttributes ?? new { @class = "form-horizontal", @id = _searchFormId };
            _updateTargetId = _updateTargetId ?? "list";
            return _ajaxHelper.BeginForm(_actionName, new { }, new AjaxOptions() { UpdateTargetId = _updateTargetId, InsertionMode = InsertionMode.Replace, OnBegin = "ajax.loading", OnComplete = "ajax.loadingClose" }, _htmlAttributes);
        }

        public static MvcForm BeginSearchFrom(this AjaxHelper _ajaxHelper, string _updateTargetId = "list", string _searchFormId = "searchForm")
        {



            return _ajaxHelper.BeginSearchFrom(null, null, _updateTargetId, _searchFormId);
        }
        #endregion
        #region 保存表单
        public static MvcForm BeginSaveFrom(this AjaxHelper _ajaxHelper, string _actionName, object _htmlAttributes)
        {
            _actionName = _actionName ?? "Edit";
            _htmlAttributes = _htmlAttributes ?? new { @class = "form-horizontal", @id = "saveForm" };
            return _ajaxHelper.BeginForm(_actionName, new { }, new AjaxOptions() { OnBegin = "ajax.loading", OnComplete = "ajax.loadingClose", OnFailure = "ajax.updateFailure", OnSuccess = "ajax.updateSuccess" }, _htmlAttributes);
        }
        public static MvcForm BeginSaveFrom(this AjaxHelper _ajaxHelper)
        {


            return _ajaxHelper.BeginSaveFrom(null, null);
        }
        #endregion

    }
}
