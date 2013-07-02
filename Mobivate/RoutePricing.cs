using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobivate
{
    public class RoutePricing
    {
        public decimal Cost { get; set; }

        public string Currency { get; set; }

        public string CountryCode { get; set; }

        public Guid CountryID { get; set; }

        public string CountryName { get; set; }

        public string Description { get; set; }

        public Guid ID { get; set; }

        public decimal Amount { get; set; }

        public Guid UserRouteID { get; set; }

        public Guid UserID { get; set; }
    }
}
