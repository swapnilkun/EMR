using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name should not be empty!..")]
        public string CategoryName { get; set; }

        [Display(Name = "Category URL")]
        [StringLength(150, MinimumLength = 4, ErrorMessage = "URL must be between 4 and 150 char")]

        public string CategoryURL { get; set; }
        public string Domain { get; set; }
        public string MetaTitle { get; set; }
        public string Keywords { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string MetaDescription { get; set; }

        public int? ParentCategoryId { get; set; }

        public string ParentName { get; set; }

        public string CategoryIcon { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<CategoryVM> categorylist { get; set; }

        //public List<CategoryMaster> GetParent()
        //{
        //    // var parentcat = db.CategoryMasters.ToList();
        //    var parentcat = (from l in db.CategoryMasters
        //                     orderby l.CategoryName ascending
        //                     select l).ToList();
        //    return parentcat;
        //}
    }
}