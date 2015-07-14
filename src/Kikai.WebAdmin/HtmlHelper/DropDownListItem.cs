using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Kikai.WebAdmin.HtmlHelper
{
    public class DropDownListItem : IDisplayable
    {
        public String Label { get; set; }

        public int Code { get; set; }

        public bool Chosen { get; set; }

        public string Text
        {
            get { return Label; }
        }

        public int Value
        {
            get { return Code; }
        }

        public bool IsCheckedOrSelected
        {
            get { return Chosen; }
        }

        public static ICollection<SelectListItem> GenerateListOf<T>(IEnumerable<T> collection) where T : IDisplayable
        {
            var list = new List<SelectListItem>();

            foreach (var item in collection)
            {
                SelectListItem sItem = new SelectListItem();
                sItem.Text = item.Text;
                sItem.Value = item.Value.ToString();
                sItem.Selected = item.IsCheckedOrSelected;
                list.Add(sItem);
            }

            return list;
        }
    }
}