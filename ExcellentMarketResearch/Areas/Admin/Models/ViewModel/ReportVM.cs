using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class ReportVM
    {

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public int ReportId { get; set; }

        [Display(Name = "Report Tilte")]
        [Required(ErrorMessage = "Report Title Can not be empty !..")]
        [StringLength(5000, MinimumLength = 8, ErrorMessage = "Title must be between 4 and 5000 char")]
        public string ReportTitle { get; set; }

        [Display(Name = "Report URL")]
        [StringLength(2000, MinimumLength = 4, ErrorMessage = "URL must be between 4 and 150 char")]
        public string ReportUrl { get; set; }

        [AllowHtml]
        [Display(Name = "Full Description")]
        [Required(ErrorMessage = "Full description can not be empty !..")]
        //[StringLength(Max,ErrorMessage = "Description must be between 0 and 2000 char")]
        public string FullDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Summary")]
        //  [StringLength(2000, ErrorMessage = "Summary must be between 0 and 2000 char")]
        //[Required(ErrorMessage = "Summary detail can not be empty !..")]
        public string Summary { get; set; }

        [AllowHtml]
        [Display(Name = "Table Of Content")]
        // [StringLength(2000, ErrorMessage = "TableofContent must be between 0 and 2000 char")]
        [Required(ErrorMessage = "Table of content can not be empty !..")]
        public string TableofContent { get; set; }

        [AllowHtml]
        [Display(Name = "List Of Chart")]
        //   [StringLength(2000, ErrorMessage = "ListofCharts must be between 0 and 2000 char")]
        public string ListofCharts { get; set; }

        [AllowHtml]
        [Display(Name = "List Of Figure")]
        // [StringLength(2000, ErrorMessage = "ListOfFigure must be between 0 and 2000 char")]
        public string ListOfFigure { get; set; }

        [AllowHtml]
        [Display(Name = "Methodology")]
        //    [StringLength(2000, ErrorMessage = "Methodology must be between 0 and 2000 char")]
        public string Methodology { get; set; }

        [AllowHtml]
        [Display(Name = "Free Analysis")]
        //   [StringLength(2000, ErrorMessage = "FreeAnalysis must be between 0 and 2000 char")]
        public string FreeAnalysis { get; set; }

        [AllowHtml]
        [Display(Name = " MetaTitle")]
        [StringLength(2000, ErrorMessage = "MetaTitle must be between 0 and 200 char")]
        public string MetaTitle { get; set; }

        [AllowHtml]
        [Display(Name = "MetaKeywords")]
        [StringLength(150, ErrorMessage = "MetaKeywords must be between 0 and 150 char")]
        public string MetaKeywords { get; set; }

        [AllowHtml]
        [Display(Name = "Meta Description")]
        //     [StringLength(2000, ErrorMessage = " Meta Description must be between 0 and 2000 char")]
        public string MetaDescription { get; set; }


        [Display(Name = "ReportImage")]
        // [Required(ErrorMessage = "Image can not be empty !..")]
        public string ReportImage { get; set; }

        [Display(Name = "Publishing Date")]
        [Required(ErrorMessage = "Publishing date can not be empty !..")]
        public System.DateTime PublishingDate { get; set; }


        [Display(Name = "Price for Single User")]
        [Required(ErrorMessage = "Price can not be empty !..")]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
        [RegularExpression("[0-9]+(.[0-9][0-9]?)?", ErrorMessage = " Price should be decimal")]
        public decimal PriceSingleUser { get; set; }


        [Display(Name = " Price For Enterprise License ")]
        //  [Required(ErrorMessage = "Price  can not be empty !..")]
        [RegularExpression("[0-9]+(.[0-9][0-9]?)?", ErrorMessage = " Price should be decimal")]
        public decimal? PriceMultiUser { get; set; }

        [Display(Name = "Price For Corporate User")]
        // [Required(ErrorMessage = "Price  can not be empty !..")]
        [RegularExpression("[0-9]+(.[0-9][0-9]?)?", ErrorMessage = " Price should be decimal")]
        public decimal? PriceCUL { get; set; }

        [Display(Name = "Discount Percentage")]
        //   [Required(ErrorMessage = "Price  can not be empty !..")]
        [RegularExpression("[0-9]+(.[0-9][0-9]?)?", ErrorMessage = " Price should be decimal")]
        public decimal? DiscountPercentage { get; set; }

        [Display(Name = "Number Of Page")]
        [Required(ErrorMessage = "Number Of pages can not be empty !..")]
        public int NumberOfPage { get; set; }

        [Display(Name = "Publishing Name")]
        //  [Required(ErrorMessage = "Publishing name can not be empty !..")]
        public int PublisherId { get; set; }

        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "Category name is not  selected !..")]
        public int CategoryId { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Report Type should not be empty !..")]
        public int ReportTypeId { get; set; }


        [Required(ErrorMessage = "Delivery Type should not be empty !..")]
        public int DeliveryTypeId { get; set; }

        [Display(Name = "IsUpComming")]
        public bool IsUpcomming { get; set; }

        public string CategoryName { get; set; }

        public string CategoryUrl { get; set; }

        public string ShortCatDesc { get; set; }

        public string LongCatDesc { get; set; }

        public string ReportTypeName { get; set; }

        public string PublisherName { get; set; }

        public string PublishingUrl { get; set; }

        public string DeliveryTypeName { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string LongPublisherDesc { get; set; }

        public string ShortPublisherDesc { get; set; }

        public int? pagesize { get; set; }
        public int RepImageCat { get; set; }
        //public List<ReportDeliveryType> GetDeliverytype()
        //{
        //    var x = db.ReportDeliveryTypes.ToList();
        //    return x;
        //}
        public IEnumerable<ReportVM> RelatedReports { get; set; }

       // public Category Category { get; set; }

        public List<ReportType> GetReportType()
        {
            var doc = db.ReportTypes.ToList();
            return doc;
        }

        public List<ReportDeliveryType> GetDeliveryType()
        {
            var doc = db.ReportDeliveryTypes.ToList();
            return doc;
        }

        public List<PublisherMaster> GetPublisher()
        {
            var publisher = db.PublisherMasters.ToList();
            return publisher;
        }

        public List<CategoryMaster> GetParentCategory()
        {

            //  var getparent = db.CategoryMasters.Where(x => x.ParentCategoryId ==0).OrderBy(x=>x.CategoryName).ToList();
            var getparent = (from l in db.CategoryMasters
                             orderby l.CategoryName ascending
                             where l.ParentCategoryId == 0
                             select l).ToList();
            return getparent;
        }

        public List<SelectListItem> GetPageSize()
        {
            
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "20", Value = "20" });
            items.Add(new SelectListItem() { Text = "50", Value = "50" });
            items.Add(new SelectListItem() { Text = "100", Value = "100" });
            items.Add(new SelectListItem() { Text = "200", Value = "200" });
            items.Add(new SelectListItem() { Text = "400", Value = "400" });
            items.Add(new SelectListItem() { Text = "600", Value = "600" });
            return items;

        }

    }
}