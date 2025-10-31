using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OURSTORE.Models
{

    public class UserModel
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // بيانات افتراضية مطلوبة من fakestoreapi
        public object Name { get; set; } = new { firstname = "user", lastname = "default" };
        public string Email { get; set; } = "user@example.com";
        public string Phone { get; set; } = "000000000";
        public object Address { get; set; } = new
        {
            city = "cairo",
            street = "main street",
            number = 1,
            zipcode = "00000",
            geolocation = new { lat = "0.0", lon = "0.0" }
        };
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; internal set; }
    }


  

}
