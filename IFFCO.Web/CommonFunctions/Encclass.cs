using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using IFFCO.HRMS.Entities.AppConfig;
using Devart.Data.Oracle;
using System.Data;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public static class Encclass
    {

        private static string Key = "DBAL24062020IFCO";
        private static byte[] GetByte(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static byte[] EncryptString(string data)
        {
            byte[] byteData = GetByte(data);
            SymmetricAlgorithm algo = SymmetricAlgorithm.Create();
            //SymmetricAlgorithm algo = (SymmetricAlgorithm)CryptoConfig.CreateFromName(Key);
            algo.Key = GetByte(Key);
            algo.GenerateIV();

            MemoryStream mStream = new MemoryStream();
            mStream.Write(algo.IV, 0, algo.IV.Length);

            CryptoStream myCrypto = new CryptoStream(mStream, algo.CreateEncryptor(), CryptoStreamMode.Write);
            myCrypto.Write(byteData, 0, byteData.Length);
            myCrypto.FlushFinalBlock();

            return mStream.ToArray();
        }

        public static string DecryptString(byte[] data)
        {
            SymmetricAlgorithm algo = SymmetricAlgorithm.Create();
            algo.Key = GetByte(Key);
            MemoryStream mStream = new MemoryStream();

            byte[] byteData = new byte[algo.IV.Length];
            Array.Copy(data, byteData, byteData.Length);
            algo.IV = byteData;
            int readFrom = 0;
            readFrom += algo.IV.Length;

            CryptoStream myCrypto = new CryptoStream(mStream, algo.CreateDecryptor(), CryptoStreamMode.Write);
            myCrypto.Write(data, readFrom, data.Length - readFrom);
            myCrypto.FlushFinalBlock();

            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        public static string GetEncryptedQueryString(string plainText)
        {

            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plainText;
        }


        public static string GetEncryptedQueryStringOLD(string data)
        {
            return UrlEncodeBase64(Convert.ToBase64String(EncryptString(data)));
        }
        public static string GetDecryptedQueryString(string encryptedText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";

            //encryptedText = encryptedText.Replace("%2f", "/");
            //encryptedText = encryptedText.Replace("%3d", "=");
            byte[] cipherBytes = Convert.FromBase64String(UrlDecodeBase64(encryptedText));
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encryptedText;
        }
        public static string GetDecryptedQueryStringOLD(string data)
        {
            // byte[] byteData = Convert.FromBase64String(data.Replace(" ", "+"));
            byte[] byteData = Convert.FromBase64String(UrlDecodeBase64(data));
            return DecryptString(byteData);
        }
        public static string UrlEncodeBase64(string base64Input)
        {
            return base64Input.Replace('+', '.').Replace('/', '_').Replace('=', '-');
        }

        public static string UrlDecodeBase64_OLD(string encodedBase64Input)
        {
            return encodedBase64Input.Replace('.', '+').Replace('_', '/').Replace('-', '=');
        }
        public static string UrlDecodeBase64(string encodedBase64Input)
        {
            return encodedBase64Input.Replace("%21", "!").Replace("%22", "").Replace("%23", "#").Replace("%24", "$").Replace("%25", "%").Replace("%26", "&").Replace("%27", "'").Replace("%28", "(").Replace("%29", ")").Replace("%2a", "*").Replace("%2b", "+").Replace("%2c", ",").Replace("%2d", "-").Replace("%2e", ".").Replace("%2f", "/").Replace("%3a", ":").Replace("%3b", ";").Replace("%3c", "<").Replace("%3d", "=").Replace("%3e", ">").Replace("%3f", "?");

        }

        //public static void ReportLog(string modulename, string reportname, string name, string QueryString, string personalNo, string fullClientIp, string clientIp)
        //{
        //    try
        //    {
        //        string connstring = new AppConfiguration().ConnectionString;
        //        string runtime = Convert.ToDateTime(DateTime.Now).ToString();

        //        using (OracleConnection con = new OracleConnection(connstring))
        //        {
        //            string str = "insert into DESP_REPORT_LOG (MODULENAME, REPORTNAME, RUNTIME, R_NAME,REPORT_PARAMETERS,PERSONAL_NO,FULLCLIENTIP,CLIENTIP)Values('" + modulename + "','" + reportname + "',sysdate,'" + name + "','" + QueryString + "','" + personalNo + "','" + fullClientIp + "','" + clientIp + "' )";

        //            con.Open();
        //            OracleCommand cmd = new OracleCommand(str, con);
        //            cmd.CommandType = CommandType.Text;
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        try
        //        {
        //            string connstring = new AppConfiguration().ConnectionString;
        //            string runtime = Convert.ToDateTime(DateTime.Now).ToString();

        //            using (OracleConnection con = new OracleConnection(connstring))
        //            {
        //                string str = "insert into DESP_REPORT_LOG (MODULENAME, REPORTNAME, R_NAME,PERSONAL_NO,FULLCLIENTIP,CLIENTIP) Values ('" + modulename + "','" + reportname + "','" + name + "','" + personalNo + "','" + fullClientIp + "','" + clientIp + "')";


        //                con.Open();
        //                OracleCommand cmd = new OracleCommand(str, con);
        //                cmd.CommandType = CommandType.Text;
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }
        //        catch (Exception excep)
        //        {
        //        }
        //   }
        //}

    }

}