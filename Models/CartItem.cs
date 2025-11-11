using MauiStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//namespace MauiStoreApp.Models

namespace CASACORE.Models
{
    //public class CartItem
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public int quantity { get; set; }
    //    public double price { get; set; }
    //    public string image { get; set; }
    //    public string category { get; set; }

    //    //public int Id { get; set; }
    //    public int CartId { get; set; }
    //    public int ProductId { get; set; }
    //    public int Quantity { get; set; }
    //}


    public class CartItemDetail
    {
        public Product Product { get; set; }
        public int Quantity { get; set; } = 1;
        public int Price { get; internal set; }
        public decimal TotalPrice { get; internal set; }
    }

}
