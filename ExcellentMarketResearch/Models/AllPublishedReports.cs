using ExcellentMarketResearch.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Models
{
    public class AllPublishedReports
    {
        ReportDetailsVM r = new ReportDetailsVM();

        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public int RowNumber { get; set; }
        public string NewsTitle { get; set; }
        public int NewsId { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime PublishingDate { get; set; }
        public string DeliveryType { get; set; }
        public int DeliveryId { get; set; }
        public string DocType { get; set; }
        public int NumberOfPage { get; set; }
        public string FullDescription { get; set; }
        public string CategoryName { get; set; }
        public decimal PriceSingleUser { get; set; }
        public int CategoryId { get; set; }

        public string Tempfulldesc { get; set; }
        //public List<CategoryMaster> GetCategories()
        //{
        //    var x = r.GetCategories();
        //    return x;
        //}
        //public List<PublisherMaster> GetPublisher()
        //{
        //    var p = r.GetPublisher();
        //    return p;
        //}
 
    }
}