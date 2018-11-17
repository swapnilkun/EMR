using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class RoleActionVM
    {

        public int RAId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int[] ActionId { get; set; }
        public string ActionName { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}