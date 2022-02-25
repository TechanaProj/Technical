using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public class ReportService
    {
        public static string EncodeServerName(string serverName)
        {

            byte[] bytes = GetBytesFromUrl(serverName);
            return Convert.ToBase64String(bytes, 0, bytes.Length);


        }
        static public byte[] GetBytesFromUrl(string url)
        {
            byte[] b = null;
            HttpWebRequest myReq = null;
            WebResponse myResp = null;
            Stream stream = null;
            try
            {
                myReq = (HttpWebRequest)WebRequest.Create(url);
                myResp = myReq.GetResponse();
                stream = myResp.GetResponseStream();
                using (BinaryReader br = new BinaryReader(stream))
                {
                    //b = br.ReadBytes(500000); 2130702268       Int32.MaxValue  = 2147483647             
                    b = br.ReadBytes(2130702268);
                    br.Close();
                }
            }
            catch (Exception ex)
            {

            }
            myResp.Close();
            return b;
        }
    }

}

