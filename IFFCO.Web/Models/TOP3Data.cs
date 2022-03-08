using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Models
{
    public class TOP3Data
    {

        public string FIN_YEAR { get; set; }
        public string PLANT_UNIT { get; set; }
        public int S_NO { get; set; }
        public string PLANT_CATALYST { get; set; }
        public string TYPE { get; set; }
        public string SUPPLIER { get; set; }
        public double? QTY { get; set; }
        public double? DENSITY { get; set; }
        public double? LIFE_GURANTEED { get; set; }
        public string CHARG_DATE { get; set; }
        public string REPLACE_DATE { get; set; }
        public string EXPECTED_LIFE { get; set; }
        public string PRE_CHARGE_DATE { get; set; }
        public string PRE_REPLACE_DATE { get; set; }
    }
}
