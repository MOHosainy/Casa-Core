//using System.Text;
//using System.Text.Json;
//using MauiStoreApp.Models;

//namespace MauiStoreApp.Services
//{
//    /// <summary>
//    /// This class is responsible for authenticating the user and storing the token and userId in secure storage.
//    /// </summary>
//    public class AuthService : BaseService
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="AuthService"/> class.
//        /// </summary>
//        public AuthService()
//        {
//        }

//        /// <summary>
//        /// Gets or sets a value indicating whether the user is logged in.
//        /// </summary>
//        /// <remarks>
//        /// Checks if the token is stored in secure storage. If set to false, removes the token and userId from secure storage.
//        /// </remarks>
//        public bool IsUserLoggedIn
//        {
//            get
//            {
//                var token = SecureStorage.GetAsync("token").Result;
//                return !string.IsNullOrEmpty(token);
//            }

//            set
//            {
//                if (!value)
//                {
//                    // remove token and userId from secure storage if IsUserLoggedIn is set to false
//                    SecureStorage.Remove("token");
//                    SecureStorage.Remove("userId");
//                }
//            }
//        }

//        /// <summary>
//        /// Logs the user in.
//        /// </summary>
//        /// <param name="username">The username of the user.</param>
//        /// <param name="password">The password of the user.</param>
//        /// <returns>A task of type <see cref="LoginResponse"/>.</returns>
//        public async Task<LoginResponse> LoginAsync(string username, string password)
//        {
//            var request = new LoginRequest
//            {
//                Username = username,
//                Password = password,
//            };

//            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

//            var response = await _httpClient.PostAsync("auth/login", content);

//            response.EnsureSuccessStatusCode();

//            var responseContent = await response.Content.ReadAsStringAsync();
//            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);

//            // fetch all users
//            var usersResponse = await _httpClient.GetAsync("users");
//            usersResponse.EnsureSuccessStatusCode();
//            var usersResponseContent = await usersResponse.Content.ReadAsStringAsync();
//            var users = JsonSerializer.Deserialize<List<User>>(usersResponseContent);

//            // find the matching user and set the UserId in LoginResponse
//            var user = users.FirstOrDefault(u => u.Username == username);
//            if (user != null)
//            {
//                loginResponse.UserId = user.Id;
//            }

//            return loginResponse;
//        }
//    }
//}




































































using System.Text.Json;
using OURSTORE.Models;

namespace MauiStoreApp.Services
{
    public class AuthService : BaseService
    {
        public bool IsUserLoggedIn
        {
            get
            {
                var token = SecureStorage.GetAsync("token").Result;
                return !string.IsNullOrEmpty(token);
            }
            set
            {
                if (!value)
                {
                    SecureStorage.Remove("token");
                    SecureStorage.Remove("username");
                    SecureStorage.Remove("password");
                }
            }
        }

        // ✅ تسجيل الدخول محليًا
        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            var savedUsername = await SecureStorage.Default.GetAsync("username");
            var savedPassword = await SecureStorage.Default.GetAsync("password");

            if (savedUsername == username && savedPassword == password)
            {
                var token = Guid.NewGuid().ToString(); // توليد توكن عشوائي رمزي
                await SecureStorage.Default.SetAsync("token", token);

                return new LoginResponse
                {
                    Token = token,
                    UserId = 1 // ممكن تضيف ID وهمي
                };
            }

            return null; // لو البيانات غلط
        }

        // ✅ تسجيل مستخدم جديد
        public async Task<bool> RegisterAsync(UserModel user)
        {
            // تأكد إن كل القيم موجودة
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return false;

            // خزن البيانات في SecureStorage (كمثال محلي)
            await SecureStorage.Default.SetAsync("username", user.Username);
            await SecureStorage.Default.SetAsync("password", user.Password);

            // توليد Token مبدئي
            await SecureStorage.Default.SetAsync("token", Guid.NewGuid().ToString());

            return true;
        }
    }
}
