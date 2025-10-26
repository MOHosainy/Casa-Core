//using System.Diagnostics;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using MauiStoreApp.Models;
//using MauiStoreApp.Services;

//namespace MauiStoreApp.ViewModels
//{
//    /// <summary>
//    /// The view model for the login page.
//    /// </summary>
//    public partial class LoginViewModel : BaseViewModel
//    {
//        private readonly AuthService _authService;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
//        /// </summary>
//        /// <param name="authService">The authentication service.</param>
//        public LoginViewModel(AuthService authService)
//        {
//            _authService = authService;
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="LoginViewModel"/> class. This empty constructor is used for design-time data.
//        /// </summary>
//        public LoginViewModel()
//        {
//        }

//        /// <summary>
//        /// Gets or sets the username.
//        /// </summary>
//        [ObservableProperty]
//        string username;

//        /// <summary>
//        /// Gets or sets the password.
//        /// </summary>
//        [ObservableProperty]
//        string password;

//        /// <summary>
//        /// Gets or sets a value indicating whether the password is visible.
//        /// </summary>
//        [ObservableProperty]
//        bool isPasswordVisible;

//        private LoginResponse loginResponse = new LoginResponse();

//        /// <summary>
//        /// Attempts to log in the user.
//        /// </summary>
//        [RelayCommand]
//        public async Task Login()
//        {
//            if (IsBusy)
//                return;

//            try
//            {
//                IsBusy = true;
//                loginResponse = await _authService.LoginAsync(Username, Password);

//                if (loginResponse != null)
//                {
//                    Debug.WriteLine($"Login successful. Token: {loginResponse.Token}");

//                    // save token to secure storage
//                    await SecureStorage.Default.SetAsync("token", loginResponse.Token);

//                    // save user id to secure storage
//                    await SecureStorage.Default.SetAsync("userId", loginResponse.UserId.ToString());

//                    _authService.IsUserLoggedIn = true;

//                    var navigationStack = Shell.Current.Navigation.NavigationStack;

//                    if (navigationStack.Count >= 2)
//                    {
//                        // go to previous page
//                        await Shell.Current.Navigation.PopAsync();
//                    }
//                    else
//                    {
//                        // if there is no previous page, navigate to home page
//                        await Shell.Current.GoToAsync("//HomePage");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        /// <summary>
//        /// Toggles the password visibility.
//        /// </summary>
//        [RelayCommand]
//        public void TogglePasswordVisibility()
//        {
//            this.IsPasswordVisible = !this.IsPasswordVisible;
//        }
//    }
//}








using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;

namespace MauiStoreApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        public LoginViewModel()
        {
            // dummy service لو مش متاح
            _authService = new AuthService();
        }

        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        bool isPasswordVisible;

        [RelayCommand]
        //public async Task Login()
        //{
        //    if (IsBusy)
        //        return;

        //    try
        //    {
        //        IsBusy = true;

        //        // ✅ هنا هنحط تحقق بسيط بدل API
        //        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        //        {
        //            await App.Current.MainPage.DisplayAlert("خطأ", "من فضلك ادخل اسم المستخدم وكلمة المرور", "حسناً");
        //            return;
        //        }

        //        // تحقق محلي مؤقت
        //        if (Username == "admin" && Password == "1234")
        //        {
        //            Debug.WriteLine("Login successful - mock login");

        //            await SecureStorage.Default.SetAsync("token", "dummy_token_123");
        //            await SecureStorage.Default.SetAsync("userId", "1");

        //            _authService.IsUserLoggedIn = true;

        //            await Shell.Current.GoToAsync("//HomePage");
        //        }
        //        else
        //        {
        //            await App.Current.MainPage.DisplayAlert("فشل", "اسم المستخدم أو كلمة المرور غير صحيحة", "حسناً");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        await App.Current.MainPage.DisplayAlert("خطأ", ex.Message, "حسناً");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}



        //[RelayCommand]
        public async Task Login()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await App.Current.MainPage.DisplayAlert("خطأ", "من فضلك ادخل اسم المستخدم وكلمة المرور", "حسناً");
                    return;
                }

                // تحقق محلي مؤقت
                if (Username == "admin" && Password == "1234")
                {
                    await SecureStorage.Default.SetAsync("token", "dummy_token_123");
                    await SecureStorage.Default.SetAsync("userId", "1");

                    _authService.IsUserLoggedIn = true;

                    // ✅ عرض توستر
                    var toast = CommunityToolkit.Maui.Alerts.Toast.Make(
                        "تم تسجيل الدخول بنجاح ✅",
                        CommunityToolkit.Maui.Core.ToastDuration.Short,
                        14);

                    await toast.Show();

                    // انتقال للصفحة الرئيسية
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("فشل", "اسم المستخدم أو كلمة المرور غير صحيحة", "حسناً");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("خطأ", ex.Message, "حسناً");
            }
            finally
            {
                IsBusy = false;
            }
        }










        [RelayCommand]
        public void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
