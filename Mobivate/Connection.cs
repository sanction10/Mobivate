using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Mobivate
{
    public class Connection
    {
        const string Authentication = "http://app.mobivatebulksms.com/bulksms/xmlapi/login/{0}/{1}";
        const string SendSingleSMS = "http://app.mobivatebulksms.com/bulksms/xmlapi/{0}/send/sms/single";
        const string SendBatchSMS = "http://app.mobivatebulksms.com/bulksms/xmlapi/{0}/send/sms/batch";
        const string RoutesUrl = "http://app.mobivatebulksms.com/bulksms/xmlapi/{0}/entity/user.UserRoutePricing/all/visible";

        private string _username;
        private string _password;
        private CookieContainer _cookieContainer;

        public string SessionID { get; set; }

        public Connection()
        {
            // create cookie container for session information
            _cookieContainer = new CookieContainer();
        }

        public Connection(string username, string password)
            : this()
        {
            _username = username;
            _password = password;
        }

        public bool Connect()
        {
            try
            {
                XDocument xDoc = XDocument.Parse(Post(string.Empty, RequestType.Login));
                SessionID = xDoc.Root.Descendants("session").First().Value;
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        public string Post(string xml, RequestType type)
        {
            // attempt to connect to the authentication service is not connected
            if (!IsConnected() && type != RequestType.Login)
                Connect();

            var postData = HttpUtility.ParseQueryString(string.Empty);
            postData.Add(new NameValueCollection
                {
                    { "xml", xml },
                });

            string url = string.Empty;

            // check request type and build url accordingly
            switch (type)
            {
                case RequestType.Login:
                    url = string.Format(Authentication, _username, _password);
                    break;
                case RequestType.BatchMessage:
                    throw new NotImplementedException();
                case RequestType.SingleMessage:
                    url = string.Format(SendSingleSMS, SessionID);
                    break;
                case RequestType.Routes:
                    url = string.Format(RoutesUrl, SessionID);
                    break;
                default:
                    throw new Exception("Request type required");
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = _cookieContainer;

            // if xml data exists post request and data
            if (!string.IsNullOrEmpty(postData["xml"]))
            {
                webRequest.Method = "POST";

                using (var s = webRequest.GetRequestStream())
                using (var sw = new StreamWriter(s))
                    sw.Write(postData.ToString());
            }
            else
                webRequest.Method = "GET";


            webRequest.ContentType = "application/x-www-form-urlencoded";

            // important specific user agent exists or requests fails
            webRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

            string xmlResponse = string.Empty;

            using (var webResponse = webRequest.GetResponse())
            {
                var responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    throw new Exception();

                using (var reader = new StreamReader(responseStream))
                {
                    xmlResponse = reader.ReadToEnd();
                }
            }

            return xmlResponse;
        }

        public bool IsConnected()
        {
            // check if session id has been generated
            return !string.IsNullOrEmpty(SessionID);
        }
    }
}
