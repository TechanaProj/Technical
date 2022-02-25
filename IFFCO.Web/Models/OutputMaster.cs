using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class OutputMaster
    {
        public OutputMaster()
        {
            DailyTechOutput1 = new HashSet<DailyTechOutput1>();
        }

        public string OpCode { get; set; }
        public string OpName { get; set; }
        public string OpUnit { get; set; }
        public string PlCode { get; set; }
        public string PlOutput { get; set; }
        public string FoCode { get; set; }

        public PlantMaster PlCodeNavigation { get; set; }
        public ICollection<DailyTechOutput1> DailyTechOutput1 { get; set; }
    }
}
