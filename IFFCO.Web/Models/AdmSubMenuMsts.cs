using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class AdmSubMenuMsts
    {
        public string Moduleid { get; set; }
        public string SubMenuId { get; set; }
        public string SubMenuName { get; set; }
        public string ParentMenuId { get; set; }
        public decimal? DisplayOrder { get; set; }
        public string Projectid { get; set; }
    }
}
