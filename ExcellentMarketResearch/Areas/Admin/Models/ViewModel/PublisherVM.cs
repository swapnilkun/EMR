using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class PublisherVM
    {

        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Company name should not be empty")]
        public string PublisherName { get; set; }
        
        [Required(ErrorMessage = "Contact name should not be empty")]
        public string ContactName { get; set; }
        
        [Required(ErrorMessage = "Contact number should not be empty")]
        public string PublisherContactNumber { get; set; }
       
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }
        
        [Required(ErrorMessage = "Address should not be empty")]
        public string Address { get; set; }
        
        public string Title { get; set; }
        public string PublisherUrl { get; set; }
        public string Description { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}