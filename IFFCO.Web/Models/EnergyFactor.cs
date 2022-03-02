using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Models
{
    public class EnergyFactor
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PrCode { get; set; }
        public string EFFUnit { get; set; }   
        public double PrValue { get; set; }
      
    }
}
