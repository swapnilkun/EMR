using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
        public List<Category> ChildCategory { get; set; }
        public int? ParentCategoryId { get; set; }
        public int Count { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
    }
}