using Mobivate.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Mobivate.Xml;

namespace Mobivate
{
    public class Api
    {
        private List<RoutePricing> _routes;
        private Connection _connection;

        public Api()
        {
            _connection = new Connection();
        }

        public Api(string username, string password)
        {
            _connection = new Connection(username, password);
        }

        public void Send(SingleMessage message)
        {
            string xml = ToXml<SingleMessage>(message);
            _connection.Post(xml, RequestType.SingleMessage);
        }

        private string ToXml<T>(T message)
        {
            var serializer = new XmlSerializer(typeof(T));
            string xml = string.Empty;

            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                serializer.Serialize(writer, message);

                Byte[] buffer = new Byte[memory.Length];
                buffer = memory.ToArray();
                xml = System.Text.Encoding.UTF8.GetString(buffer);
            }
            return xml;
        }

        public void Send(BatchMessage message)
        {
            string xml = ToXml<BatchMessage>(message);
            _connection.Post(xml, RequestType.BatchMessage);
        }

        public void Send(string originator, string number, string body, string reference, bool autoroute = true, Guid? userRouteId = null)
        {
            RoutePricing route = GetRoute(number);

            if (route == null)
                throw new Exception("Invalid route");

            Send(new SingleMessage()
            {
                Body = body,
                Recipient = number,
                Originator = originator,
                RouteId = route.UserRouteID,
                Reference = string.IsNullOrEmpty(reference) ? Guid.NewGuid().ToString() : reference
            });
        }

        private RoutePricing GetRoute(string number)
        {
            // populate routes for re-use per connection
            PopulateRoutes();

            RoutePricing pricing = _routes.FirstOrDefault(r => number.StartsWith(r.CountryCode));

            // check if the pricing route is available for the area code
            if (pricing != null)
                return pricing;

            return null;
        }

        private void PopulateRoutes(bool reset = false)
        {
            // if required re-populate routes
            if (_routes == null || reset)
            {
                // make request and get xml
                string xml = _connection.Post(string.Empty, RequestType.Routes);

                // parse string and create xml document
                XDocument xDoc = XDocument.Parse(xml);

                // query specific node for relevant user route pricing information
                List<XElement> xRoutes = ((XElement)xDoc.Root.Element("entitylist")).Elements("userroutepricing").ToList();

                List<RoutePricing> routes = new List<RoutePricing>();

                foreach (var xRoute in xRoutes)
                {
                    // build dynamic class
                    dynamic route = xRoute.ToDynamic();

                    // populate concrete class from dynamic class
                    routes.Add(new RoutePricing()
                    {
                        Cost = decimal.Parse(route.cost.amount.Value),
                        CountryCode = route.countryCode.Value,
                        CountryID = Guid.Parse(route.countryId.Value),
                        CountryName = route.countryName.Value,
                        Description = route.description.Value,
                        ID = Guid.Parse(route.id.Value),
                        Amount = decimal.Parse(route.price.amount.Value),
                        UserID = Guid.Parse(route.userId.Value),
                        UserRouteID = Guid.Parse(route.userRouteId.Value)
                    });
                }

                _routes = routes;
            }
            
            // possibly implement caching in future
        }

        public List<RoutePricing> GetRoutes()
        {
            PopulateRoutes();
            return _routes;
        }
    }
}
