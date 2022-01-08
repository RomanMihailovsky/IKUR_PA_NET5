using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IKUR_PA_NET5.Models
{
    public static class SMSSender
    {
        private const string url_sms = "https://lcab.smsprofi.ru/API/XML/send.php";

        public static string Send(string tonumber, string textsms, string smsid)
        {

            WebResponse result = null;
            WebRequest req = null;
            Stream newStream = null;
            Stream ReceiveStream = null;
            StreamReader sr = null;
            string strOut = "";

            string xml = "<?xml version='1.0' encoding='UTF-8'?> " +
                        "<data> " +
                        "<login>ikur_nebo</login> " +
                        "<password>ikur_134</password> " +
                        "<source>AO IKUR</source>" +
                        "<action>send</action> " +
                        $"<text>{textsms}</text> " +
                        $"<to number='{tonumber}'></to> " +
                        $"<smsid>{smsid}</smsid> " +
                        //  "<channel>ID</channel> " +
                        "</data>";

            try
            {
                req = WebRequest.Create(url_sms);
                req.Method = "POST";
                req.Timeout = 120000;
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] SomeBytes = null;
                SomeBytes = UTF8Encoding.UTF8.GetBytes(xml);
                req.ContentLength = SomeBytes.Length;
                newStream = req.GetRequestStream();
                newStream.Write(SomeBytes, 0, SomeBytes.Length);
                newStream.Close();

                result = req.GetResponse();
                ReceiveStream = result.GetResponseStream();
                Encoding encode = Encoding.UTF8;
                sr = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    strOut += str;
                    count = sr.Read(read, 0, 256);
                }

                int pos = strOut.IndexOf("</code>");
                string sCode = "";
                if (pos > 0)
                {
                    sCode = strOut.Substring(strOut.IndexOf("<code>") + 6, pos - strOut.IndexOf("<code>") - 6);
                }

                if (sCode == "1")
                {
                    return "Ok";
                }
                else
                {
                    return strOut; 
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}. Ответ сервера сообщений: {strOut}";
            }
            finally
            {
                if (newStream != null)
                    newStream.Close();
                if (ReceiveStream != null)
                    ReceiveStream.Close();
                if (sr != null)
                    sr.Close();
                if (result != null)
                    result.Close();
            }

        }

    }
}
