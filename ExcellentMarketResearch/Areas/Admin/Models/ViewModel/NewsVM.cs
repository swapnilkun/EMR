using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class NewsVM
    {

        public int NewsId { get; set; }
        [Required(ErrorMessage = "News Title should not be empty")]
        public string NewsTitle { get; set; }
        public string NewsURL { get; set; }
        [Required(ErrorMessage = "News Details should not be empty")]
        public string NewsDetail { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        [Required(ErrorMessage = "Category name should not be empty")]
        public int CategoryId { get; set; }
        public string NewsImage { get; set; }
        public DateTime NewsPublishingDate { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
    }
}