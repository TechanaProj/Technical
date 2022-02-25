using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class AdmEmpprgAccess
    {
        public string Empid { get; set; }
        public string Projectid { get; set; }
        public string Moduleid { get; set; }
        public string Programid { get; set; }
        public string PrivSelect { get; set; }
        public string PrivInsert { get; set; }
        public string PrivUpdate { get; set; }
        public string PrivDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Programtype { get; set; }
    }
}
