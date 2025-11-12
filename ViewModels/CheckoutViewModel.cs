using MauiStoreApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiStoreApp.ViewModels
{
    public partial class CheckoutViewModel : BaseViewModel
    {
        private readonly CartService _cartService;

        public ObservableCollection<CartItemDetail> CartItems { get; private set; } = new();
        //public int TotalQuantity { get; private set; }
        //public decimal TotalPrice { get; private set; }

        [ObservableProperty]
        private decimal totalPrice;

        [ObservableProperty]
        private int totalQuantity;

        public CheckoutViewModel(CartService cartService)
        {
            _cartService = cartService;
            LoadCheckoutData();
            _cartService.CartUpdated += OnCartUpdated;
        }

        private void OnCartUpdated()
        {
            LoadCheckoutData();
        }


      



        private void LoadCheckoutData()
        {
            var items = _cartService.GetCartItems() ?? new List<CartItemDetail>();
            CartItems.Clear();

            foreach (var item in items)
                CartItems.Add(item);

            TotalQuantity = CartItems.Sum(item => item.Quantity);
            TotalPrice = CartItems.Sum(item => item.Quantity * item.Product.Price);

            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(CartItems));
        }



        [RelayCommand]
        private async Task GoBackAsync()
        {
            // يعيد إلى الصفحة السابقة
            //await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync("//CartPage");
        }



            







        [RelayCommand]
        public async Task ConfirmOrder()
        {
            if (!CartItems.Any())
            {
                await Shell.Current.DisplayAlert("Error ( حدث خطا ) ", "Cart is empty! ( السله فارغه )", "OK ( تم ) ");
                return;
            }

            //_cartService.ClearCart();
            //LoadCheckoutData();
            await _cartService.ClearCartAsync(); // هتمسح من التخزين كمان

            // ✅ 3. فضّي القائمة المعروضة
            CartItems.Clear();

            await Shell.Current.DisplayAlert("✅ Success (تم بنجاح)", "Order has been submitted (  تم تقديم الطلب ) ", "OK");
            await Shell.Current.GoToAsync("//HomePage");

        }
    }
}
