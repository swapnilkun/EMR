using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Models.ViewModel
{
    public class NewsDetailsVM
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public int RowNumber { get; set; }
        public string NewsTitle { get; set; }
        public int NewsId { get; set; }
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NewsPublishingDate { get; set; }
        public string NewsImage { get; set; }
        public string NewsDetail { get; set; }
        public string NewsURL { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
    }
}