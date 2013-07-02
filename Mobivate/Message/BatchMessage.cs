using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobivate.Message
{
    public class BatchMessage
    {
        public bool DetailedResponse { get; set; }

        public bool FilteredOptouts { get; set; }

        public Guid RouteID { get; set; }

        public string Originator { get; set; }

        public string Body { get; set; }

        public List<Recipient> Recipients { get; set; }
    }
}
