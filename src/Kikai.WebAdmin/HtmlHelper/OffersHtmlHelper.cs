using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kikai.WebAdmin.HtmlHelper
{
    public class OffersHtmlHelper
    {

        internal static List<DropDownListItem> _OfferStatusList;
        internal static List<DropDownListItem> _OfferTypeList;
        internal static List<DropDownListItem> _RecordsPerPageList;
        internal static List<DropDownListItem> _PublishedAttributesList;

        public static List<DropDownListItem> OfferStatusList
        {
            get
            {
                if (_OfferStatusList == null)
                {
                    _OfferStatusList = new List<DropDownListItem>();
                    _OfferStatusList.Add(new DropDownListItem() { Code = 4, Label = "All", Chosen = false });
                    _OfferStatusList.Add(new DropDownListItem() { Code = 1, Label = "Active", Chosen = true });
                    _OfferStatusList.Add(new DropDownListItem() { Code = 0, Label = "Inactive", Chosen = false });
                    _OfferStatusList.Add(new DropDownListItem() { Code = 2, Label = "Suspended", Chosen = false });
                    _OfferStatusList.Add(new DropDownListItem() { Code = 3, Label = "Pending", Chosen = false });
                }
                return _OfferStatusList;
            }
        }

        public static List<DropDownListItem> OfferTypeList
        {
            get
            {
                if (_OfferTypeList == null)
                {
                    _OfferTypeList = new List<DropDownListItem>();
                    _OfferTypeList.Add(new DropDownListItem() { Code = 2, Label = "All", Chosen = false });
                    _OfferTypeList.Add(new DropDownListItem() { Code = 0, Label = "Live", Chosen = true });
                    _OfferTypeList.Add(new DropDownListItem() { Code = 1, Label = "Test", Chosen = false });
                }
                return _OfferTypeList;
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
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 50, Label = "50", Chosen = true });
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 100, Label = "100", Chosen = false });
                    _RecordsPerPageList.Add(new DropDownListItem() { Code = 500, Label = "500", Chosen = false });
                }
                return _RecordsPerPageList;
            }
        }

        public static List<DropDownListItem> PublishedAttributes
        {
            get
            {
                    _PublishedAttributesList = new List<DropDownListItem>();
                    var publishedAtts = new AttributeRepository().SelectPublishedAttributes().ToList();
                    publishedAtts.RemoveAll(item => new Constants().SupportedAttributes.Contains(item.Id));
                    _PublishedAttributesList.Add(new DropDownListItem() { Code = 0, Label = "- Choose -", Chosen = true });
                    for (int i = 0; i < publishedAtts.Count(); i++)
                    {
                        _PublishedAttributesList.Add(new DropDownListItem() { Code = i + 1, Label = publishedAtts[i].Id, Chosen = false });
                    }
                    _PublishedAttributesList.Add(new DropDownListItem() { Code = -2, Label = "------------------------", Chosen = false});
                    _PublishedAttributesList.Add(new DropDownListItem() { Code = -1, Label = "+ Publish Attributes", Chosen = false });
                return _PublishedAttributesList;
            }
        }

        public static ICollection<SelectListItem> GetOfferStatusList(string Chosen)
        {
            var dropDownList = DropDownListItem.GenerateListOf<DropDownListItem>(OffersHtmlHelper.OfferStatusList);
            if (!String.IsNullOrEmpty(Chosen))
            {
                dropDownList.Where(d => d.Selected == true).First().Selected = false;
                dropDownList.Where(d => d.Value == Chosen).First().Selected = true;
            }
            return dropDownList;
        }

        public static ICollection<SelectListItem> GetOfferTypeList(string Chosen)
        {
            var dropDownList = DropDownListItem.GenerateListOf<DropDownListItem>(OffersHtmlHelper.OfferTypeList);
            if (!String.IsNullOrEmpty(Chosen))
            {
                dropDownList.Where(d => d.Selected == true).First().Selected = false;
                dropDownList.Where(d => d.Value == Chosen).First().Selected = true;
            }
            return dropDownList;
        }

        public static ICollection<SelectListItem> GetRecordsPerPageList(string Chosen)
        {
            var dropDownList = DropDownListItem.GenerateListOf<DropDownListItem>(OffersHtmlHelper.RecordsPerPageList);
            if (!String.IsNullOrEmpty(Chosen))
            {
                dropDownList.Where(d => d.Selected == true).First().Selected = false;
                dropDownList.Where(d => d.Value == Chosen).First().Selected = true;
            }
            return dropDownList;
        }

        public static ICollection<SelectListItem> GetPublishedAttributes(List<RespondentAttributeObject> attributes)
        {
            var dropDownList = DropDownListItem.GenerateListOf<DropDownListItem>(OffersHtmlHelper.PublishedAttributes);
            var filteredDropDownList = dropDownList;
            foreach (var att in attributes)
            {
                if (dropDownList.Contains(dropDownList.SingleOrDefault(c => c.Text == att.Ident)))
                    dropDownList.Remove(dropDownList.Single(c => c.Text == att.Ident));
            }
            
            return dropDownList;
        }

    }
}