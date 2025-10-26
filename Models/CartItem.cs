using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASACORE.Models
{
    public class CartItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public double price { get; set; }
        public string image { get; set; }
        public string category { get; set; }
    }

}
