using IFFCO.HRMS.Repository.Pattern;
using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class RefEnergy : Entity
    {
       
        public string Plant { get; set; }
        public string PlantCode { get; set; }
        public double? ReEnergy { get; set; }
        public decimal? CreatedBy { get; set; }
        public DateTime? CreationDatetime { get; set; }
        public DateTime? ModifiedDatetime { get; set; }
        public decimal? ModifiedBy { get; set; }
        
    }
}
