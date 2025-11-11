

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using MauiStoreApp.Services;

namespace MauiStoreApp.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private bool isUserLoggedIn;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string avatarInitial;

        [ObservableProperty]
        private bool isBusy;

        public ProfilePageViewModel(AuthService authService)
        {
            //_authService = new AuthService();
            _authService = authService;
            InitCommand = new AsyncRelayCommand(Init);
            LoginCommand = new AsyncRelayCommand(Login);
            LogoutCommand = new AsyncRelayCommand(Logout);
            DeleteAccountCommand = new AsyncRelayCommand(DeleteAccount);
        }

        public IAsyncRelayCommand InitCommand { get; }
        public IAsyncRelayCommand LoginCommand { get; }
        public IAsyncRelayCommand LogoutCommand { get; }
        public IAsyncRelayCommand DeleteAccountCommand { get; }

        private async Task Init()
        {
            await _authService.TryRestoreFullSessionAsync();
            await LoadUser();
        }




        private async Task LoadUser()
        {
            IsBusy = true;

            var logged = await SecureStorage.GetAsync("isLoggedIn");
            var savedEmail = await SecureStorage.GetAsync("userEmail");


            if (logged == "true" && !string.IsNullOrEmpty(savedEmail))
            {
                //IsUserLoggedIn = true;
                // 👇 split to get text before @
                var username = savedEmail.Split('@')[0];

                // 👇 replace dots with spaces & remove digits
                var cleanName = new string(username.Replace('.', ' ')
                                                   .Where(char.IsLetter)
                                                   .ToArray());

                if (string.IsNullOrEmpty(cleanName))
                    cleanName = "User"; // احتياطي لو الاسم كله أرقام

                Email = cleanName; // عرض الاسم بدل من الميل
                AvatarInitial = char.ToUpper(cleanName[0]).ToString();  // نعرض الاسم بدل أول حرف فقط

                IsUserLoggedIn = true;
            }
            else
            {
                Email = "";
                AvatarInitial = "";
                IsUserLoggedIn = false;
            }









            IsBusy = false;
        }



        private async Task Login()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }

        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("(LogOut) تسجيل الخروج", "(Do you really want to log out?) هل تريد بالفعل تسجيل الخروج؟", "نعم", "إلغاء");

            if (!confirm) return;
            IsBusy = true;

            //await SecureStorage.Default.RemoveAsync("isLoggedIn");
            //await SecureStorage.Default.RemoveAsync("userEmail");
            SecureStorage.Remove("isLoggedIn");
            SecureStorage.Remove("userEmail");
            SecureStorage.Remove("supabase_session"); // ✅ اهم واحدة
            await _authService.LogoutAsync();
            IsUserLoggedIn = false;
            Email = "";
            AvatarInitial = "";
            //await SecureStorage.Default.RemoveAsync("supabase_session");
            //await LoadUser();
            await Shell.Current.GoToAsync("//LoginPage");
            IsBusy = false;
        }
















        private async Task DeleteAccount()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Log out ( تسجيل الخروج)",
                "Are you sure you want to LogOut your account? (هل تريد الخروج بالفعل)",
                "Yes", "Cancel");

            if (!confirm)
                return;

            bool deleted = await _authService.DeleteAccountAsync();

            if (deleted)
            {
                await Shell.Current.DisplayAlert("Info", "Account deleted successfully ✅", "OK");
                await Shell.Current.GoToAsync("//RegisterPage"); // ✅ يرجّعه للتسجيل
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete account ❌", "OK");
            }
        }























        private string _currentLang = Preferences.Get("AppLanguage", "ar");
        public string CurrentLang
        {
            get => _currentLang;
            set
            {
                if (SetProperty(ref _currentLang, value))
                    OnPropertyChanged(nameof(CurrentFlowDirection));
            }
        }

        public FlowDirection CurrentFlowDirection =>
            CurrentLang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        [RelayCommand]
        private void ChangeLanguages(string lang)
        {
            // 1. احفظ اللغة الجديدة
            CurrentLang = lang;
            Preferences.Set("AppLanguage", lang);

            // 2. طبق اللغة والاتجاه العام للتطبيق
            App.SetAppLanguage(lang);

            // 3. أعد تحميل الواجهة لتطبيق النصوص والاتجاه
            Application.Current.MainPage = new AppShell();
        }





    }
}

