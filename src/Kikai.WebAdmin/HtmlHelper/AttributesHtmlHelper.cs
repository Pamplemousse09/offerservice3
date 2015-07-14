using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kikai.WebAdmin.HtmlHelper
{
    public static class AttributesHtmlHelper
    {
        internal static List<DropDownListItem> _AttributeStatusList;
        internal static List<DropDownListItem> _RecordsPerPageList;

        public static List<DropDownListItem> AttributeStatusList
        {
            get
            {
                if (_AttributeStatusList == null)
                {
                    _AttributeStatusList = new List<DropDownListItem>();
                    _AttributeStatusList.Add(new DropDownListItem() { Code = 2, Label = "All", Chosen = true });
                    _AttributeStatusList.Add(new DropDownListItem() { Code = 0, Label = "Unpublished", Chosen = false });
                    _AttributeStatusList.Add(new DropDownListItem() { Code = 1, Label = "Published", Chosen = false });
                }
                return _AttributeStatusList;
            }
        }

        public static List<DropDownListItem> RecordsPerPageList
        {
            get
            {
                if (_RecordsPerPageList == null)
                {
                    _RecordsPerPageList = new List<DropDownListItem>();
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 5, Label = "5", Chosen = false });
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 20, Label = "20", Chosen = false });
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 50, Label = "50", Chosen = false });
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 100, Label = "100", Chosen = true });
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 500, Label = "500", Chosen = false });
                }
                return _RecordsPerPageList;
            }
        }

        public static ICollection<SelectListItem> GetAttributeStatusList(string Chosen)
        {
            var dropDownList =  DropDownListItem.GenerateListOf<DropDownListItem>(AttributesHtmlHelper.AttributeStatusList);
            if (!String.IsNullOrEmpty(Chosen))
            {
                dropDownList.Where(d => d.Selected == true).FirstOrDefault().Selected = false;
                if (dropDownList.Where(d => d.Value == Chosen).Count() == 1)
                    dropDownList.Where(d => d.Value == Chosen).FirstOrDefault().Selected = true;
            }
            return dropDownList;
        }

        public static ICollection<SelectListItem> GetRecordsPerPageList(string Chosen)
        {
            var dropDownList = DropDownListItem.GenerateListOf<DropDownListItem>(AttributesHtmlHelper.RecordsPerPageList);
            if (!String.IsNullOrEmpty(Chosen))
            {
                dropDownList.Where(d => d.Selected == true).FirstOrDefault().Selected = false;
                if (dropDownList.Where(d => d.Value == Chosen).Count() == 1)
                    dropDownList.Where(d => d.Value == Chosen).FirstOrDefault().Selected = true;
            }
            return dropDownList;
        }
    }
}