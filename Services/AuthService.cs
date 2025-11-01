using Supabase;
using Supabase.Gotrue;
using System.Threading.Tasks;

namespace MauiStoreApp.Services
{




    public class AuthService
    {
        private readonly Supabase.Client _client;

        public bool IsUserLoggedIn { get;  set; }  // ✅ دي خاصية نستخدمها للمتابعة المنطقية

        public AuthService()
        {
            var url = "https://phbarflogerpotdqiwrp.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBoYmFyZmxvZ2VycG90ZHFpd3JwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjE5NDY1NjksImV4cCI6MjA3NzUyMjU2OX0.FUC7B6BJFWcFl-w2I2CLjkLb3YyCVzlCrR9tKEdyJ5M";
            _client = new Supabase.Client(url, key);
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var session = await _client.Auth.SignUp(email, password);
                IsUserLoggedIn = session?.User != null;
                return IsUserLoggedIn;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var session = await _client.Auth.SignIn(email, password);
                IsUserLoggedIn = session?.User != null;
                return IsUserLoggedIn;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _client.Auth.SignOut();
            IsUserLoggedIn = false; // ✅ بمجرد تسجيل الخروج نغير القيمة
        }
    }

}
