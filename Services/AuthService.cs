



using Supabase;
using Supabase.Gotrue;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System.Text;


namespace MauiStoreApp.Services
{
    public class AuthService
    {
        private readonly Supabase.Client _supabase;

        private Session _currentSession;
        private User _currentUser;

        public Session CurrentSession => _currentSession;
        public User CurrentUser => _currentUser;
        public bool IsUserLoggedIn { get; private set; }
        public string CurrentUserEmail { get; private set; }

        private const string SessionKey = "supabase_session";

        private readonly CartService _cartService;


        public AuthService(CartService cartService)
        {
            var url = "https://phbarflogerpotdqiwrp.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBoYmFyZmxvZ2VycG90ZHFpd3JwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjE5NDY1NjksImV4cCI6MjA3NzUyMjU2OX0.FUC7B6BJFWcFl-w2I2CLjkLb3YyCVzlCrR9tKEdyJ5M";
            _supabase = new Supabase.Client(url, key, new SupabaseOptions { AutoConnectRealtime = false });
            _cartService = cartService;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var result = await _supabase.Auth.SignUp(email, password);

                Session session = result as Session;
                if (session == null)
                {
                    var prop = result?.GetType().GetProperty("Session");
                    session = prop?.GetValue(result) as Session;
                }

                if (session == null || session.User == null)
                    return false;

                _currentSession = session;
                _currentUser = session.User;
                await SaveSessionAsync(email);
                return true;
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException ex)
            {
                if (ex.Message.Contains("user_already_exists"))
                {
                    await Shell.Current.DisplayAlert("تنبيه ⚠️", "هذا البريد مسجّل بالفعل ✅", "حسناً");
                    return false;
                }

                await Shell.Current.DisplayAlert("خطأ ❌", "حدث خطأ أثناء التسجيل!\n" + ex.Message, "موافق");
                return false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("خطأ ❌", ex.Message, "موافق");
                return false;
            }
        }







        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _supabase.Auth.SignIn(email, password);

                if (result == null)
                    return false;

                Session session = result as Session;
                if (session == null)
                {
                    var prop = result.GetType().GetProperty("Session");
                    session = prop?.GetValue(result) as Session;
                }

                if (session == null || session.User == null)
                    return false;

                _currentSession = session;
                _currentUser = session.User;

                await SaveSessionAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login Error: " + ex.Message);
                return false;
            }
        }


        public async Task LogoutAsync()
        {
            try
            {
                await _supabase.Auth.SignOut();
            }
            catch
            {
                // ignore errors signing out
            }

            _currentSession = null;
            _currentUser = null;
            IsUserLoggedIn = false;
            CurrentUserEmail = null;
            SecureStorage.Remove("userEmail");
            SecureStorage.Remove("isLoggedIn");
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("supabase_session");


            await _cartService.ClearCartAsync();

            // 🔄 عمل Refresh للـ UI
            Application.Current.MainPage = new AppShell();
        }




        public async Task<bool> TryRestoreFullSessionAsync()
        {


            try
            {
                //if (!SecureStorage.Default.ContainsKey(SessionKey))
                //    return false;

                //if (string.IsNullOrEmpty(await SecureStorage.GetAsync(SessionKey)))
                //    return false;


                var savedSessionJson = await SecureStorage.Default.GetAsync(SessionKey);
                if (string.IsNullOrEmpty(savedSessionJson))
                    return false;



                var logged = await SecureStorage.GetAsync("isLoggedIn");
                if (logged != "true")
                    return false;






                var json = await SecureStorage.GetAsync(SessionKey);
                if (string.IsNullOrEmpty(json))
                    return false;

                var session = JsonConvert.DeserializeObject<Session>(json);
                if (session == null)
                    return false;

                //await MauiProgram.SupabaseClient.Auth.SetSession(session);
                await MauiProgram.SupabaseClient.Auth.SetSession(session.AccessToken, session.RefreshToken);


                return MauiProgram.SupabaseClient.Auth.CurrentSession != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task StoreSessionAsync(Session session)
        {
            if (session == null) return;

            var json = JsonConvert.SerializeObject(session);
            await SecureStorage.SetAsync(SessionKey, json);
        }


        public async Task<bool> DeleteAccountAsync()
        {
            // يتطلب أن يكون هناك session فعّال لأننا سنستخدم الـ access token
            if (_currentSession == null || _currentSession.AccessToken == null)
                return false;

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentSession.AccessToken}");
                client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                var requestBody = new { user_id = _currentUser?.Id };
                var jsonBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

                var url = "https://phbarflogerpotdqiwrp.supabase.co/functions/v1/delete_user";
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return false;

                await LogoutAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Delete account error: " + ex.Message);
                return false;
            }
        }

        private async Task SaveSessionAsync(string email)
        {
            // خزّن حاجة بسيطة تدل على تسجيل الدخول (لا تخزن التوكن لو مش محتاج)
            await SecureStorage.SetAsync("userEmail", email);
            await SecureStorage.SetAsync("isLoggedIn", "true");

            if (_currentSession != null)
            {
                await SecureStorage.SetAsync("access_token", _currentSession.AccessToken);
                await SecureStorage.SetAsync("refresh_token", _currentSession.RefreshToken);
            }

            IsUserLoggedIn = true;
            CurrentUserEmail = email;
        }

        public async Task RestoreSessionAsync()
        {
            var loggedIn = await SecureStorage.GetAsync("isLoggedIn");
            var email = await SecureStorage.GetAsync("userEmail");

            if (loggedIn == "true" && !string.IsNullOrEmpty(email))
            {
                // note: this does not automatically restore server session object.
                // If you need the actual Supabase session you must persist the access token
                // and rehydrate _currentSession from it (advanced).
                IsUserLoggedIn = true;
                CurrentUserEmail = email;
            }
            else
            {
                IsUserLoggedIn = false;
                CurrentUserEmail = null;
            }
        }












        //public bool IsUserLoggedIn => _currentSession != null;

        //public async Task Logout()
        //{
        //    _currentSession = null;
        //    _currentUser = null;

        //    await SecureStorage.SetAsync("supabase_session", "");
        //}
        //public static CartService Instance { get; } = new CartService();


        //public async Task Logout()
        //{
        //    // Clear session + user
        //    _currentSession = null;
        //    _currentUser = null;
        //    IsUserLoggedIn = false;

        //    // Clear stored session
        //    SecureStorage.Remove("supabase_session");

        //    // Clear local cart  
        //    Preferences.Remove("local_cart"); // لو بتخزن محليًا
        //    CartService.Instance.LoadCart();


        //    CartService.Instance.ClearCart();

        //    // 🗑 مسح بيانات المستخدم المحفوظة
        //    Preferences.Clear();






        //    //CartService.Instance.ClearCart();

        //    CartService.Instance.ClearCart();   
        //    // Optionally Reload UI
        //    Application.Current.MainPage = new AppShell();
        //}





























        public async Task Logout()
        {
            // Clear session + user
            _currentSession = null;
            _currentUser = null;
            IsUserLoggedIn = false;

            // 🗑 Clear ONLY login values
            SecureStorage.Remove("supabase_session");
            SecureStorage.Remove("userEmail");
            SecureStorage.Remove("isLoggedIn");

            // 🗑 Clear local cart from storage + memory
            Preferences.Remove("local_cart");
            //CartService.Instance.ClearCartMemory(); // ✅ new method
            //CartService.Instance.LoadCart(); // ✅ reload empty
            //CartService.Instance.ClearCart();

            // 🔄 Refresh App UI
            Application.Current.MainPage = new AppShell();
        }




        //public void ClearCart()
        //{
        //    CartItems.Clear();
        //    SaveCart();
        //}



    }
}
