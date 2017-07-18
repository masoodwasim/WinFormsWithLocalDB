using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsLocalDB
{
   public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }
        public string Region { get; set; }
        public string Product { get; set; }
        public string Supplier { get; set; }
        public string Team { get; set; }
    }
}
