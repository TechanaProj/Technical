using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class PlantMaster
    {
        public PlantMaster()
        {
            OutputMaster = new HashSet<OutputMaster>();
            ParameterMaster = new HashSet<ParameterMaster>();
        }

        public string PlCode { get; set; }
        public string PlName { get; set; }

        public ICollection<OutputMaster> OutputMaster { get; set; }
        public ICollection<ParameterMaster> ParameterMaster { get; set; }
    }
}
