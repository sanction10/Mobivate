using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mobivate.Message
{
    [XmlRoot("message")]
    public class SingleMessage
    {
        [XmlElement("originator")]
        public string Originator { get; set; }

        [XmlElement("recipient")]
        public string Recipient { get; set; }
        
        [XmlElement("body")]
        public string Body { get; set; }

        [XmlElement("routeId")]
        public Guid RouteId { get; set; }

        [XmlElement("reference")]
        public string Reference { get; set; }
    }
}
