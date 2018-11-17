using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class ExcelReportVM
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public string FullDescription { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PublishingDate { get; set; }
    }
}