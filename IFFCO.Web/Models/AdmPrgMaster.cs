using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class AdmPrgMaster
    {
        public string Projectid { get; set; }
        public string Moduleid { get; set; }
        public string Programtype { get; set; }
        public string Programid { get; set; }
        public string Programname { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Ismainform { get; set; }
        public string SubMenuName { get; set; }
        public decimal? DisplayOrder { get; set; }
        public string ActiveInactive { get; set; }
        public string FreeAccess { get; set; }
    }
}
