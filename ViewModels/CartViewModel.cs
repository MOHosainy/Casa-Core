

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Models;
using MauiStoreApp.Services;
using MauiStoreApp.Views;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using Supabase.Gotrue;

namespace MauiStoreApp.ViewModels
{
    public partial class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly AuthService _authService;
        //public IAsyncRelayCommand InitCommand { get; }

        bool isFirstRun = true;
        public int TotalQuantity => CartItems?.Sum(i => i.Quantity) ?? 0;

        public decimal TotalPrice => CartItems?.Sum(i => i.Quantity * i.Product.Price) ?? 0m;


        public CartViewModel(CartService cartService, AuthService authService)
        {
            //InitCommand = new AsyncRelayCommand(Init);

            _cartService = cartService;
            _authService = authService;
            CartItems = new ObservableCollection<CartItemDetail>(_cartService.GetCartItems());
            //InitCommand = new AsyncRelayCommand(Init);

            _cartService.CartUpdated += OnCartUpdated; // ✅ متابعة تغييرات السلة

            CalculateTotals();
        }




        private void OnCartUpdated()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //SyncCartItems();
                CartItems.Clear();

                foreach (var item in _cartService.GetCartItems())
                    CartItems.Add(item);

                CalculateTotals();
            });
        }








        private void CalculateTotals()
        {
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(TotalPrice));
        }







        [ObservableProperty]
        public bool isUserLoggedIn;

        [ObservableProperty]
        private bool isBusyWithCartModification;
        //bool isFirstRun = true;

        public ObservableCollection<CartItemDetail> CartItems { get; private set; }

        [RelayCommand]
        public async Task Init()
        {


            await _cartService.LoadCartFromStorageAsync();
            SyncCartItems();
            CalculateTotals();

            if (isFirstRun)
            {
                await GetCartByUserIdAsync();
                isFirstRun = false;
            }
            else
            {
                SyncCartItems();
            }

            // guard null
            //IsUserLoggedIn = _authService?.IsUserLoggedIn ?? false;
            //IsUserLoggedIn = _authService.IsUserLoggedIn;
            IsUserLoggedIn = true; // ✅ مؤقتًا للعرض فقط

        }

        private async Task GetCartByUserIdAsync()
        {
            try
            {
                if (!(_authService?.IsUserLoggedIn ?? false))
                    return;

                if (IsBusy)
                    return;

                var userIdStr = await SecureStorage.GetAsync("userId");
                if (!int.TryParse(userIdStr, out int userId))
                {
                    Debug.WriteLine("Failed to get or parse userId from SecureStorage.");
                    // don't block app if no user id
                    return;
                }

                IsBusy = true;

                await _cartService.RefreshCartItemsByUserIdAsync(userId);

                SyncCartItems();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get cart: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to retrieve cart.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToLoginPage()
        {
            await Shell.Current.GoToAsync("LoginPage");
        }

        //[RelayCommand]
        //public async Task DeleteCart()
        //{
        //    if (IsBusy || IsBusyWithCartModification) return;

        //    try
        //    {
        //        var userResponse = await Shell.Current.DisplayAlert("Confirm", "Are you sure you want to delete the cart?", "Yes", "No");
        //        if (!userResponse) return;

        //        if (CartItems.Count > 0)
        //        {
        //            IsBusyWithCartModification = true;

        //            var response = await _cartService.DeleteCartAsync();

        //            if (response != null && response.IsSuccessStatusCode)
        //            {
        //                CartItems.Clear();
        //                CalculateTotals();
        //                var toast = Toast.Make("Cart deleted successfully.", ToastDuration.Short);
        //                await toast.Show();
        //            }
        //            else
        //            {
        //                await Shell.Current.DisplayAlert("Error", "Failed to delete cart.", "OK");
        //            }
        //        }
        //        else
        //        {
        //            await Shell.Current.DisplayAlert("Error", "No cart found.", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Unable to delete cart: {ex.Message}");
        //        await Shell.Current.DisplayAlert("Error", "Failed to delete cart.", "OK");
        //    }
        //    finally
        //    {
        //        IsBusyWithCartModification = false;
        //    }
        //}




























































        [RelayCommand]
        public void IncreaseProductQuantity(Product product)
        {
            try
            {
                if (product == null) return;
                _cartService.IncreaseProductQuantity(product.Id);
                SyncCartItems();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to increase product quantity: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "Failed to increase product quantity.", "OK");
            }
        }

        [RelayCommand]
        public void DecreaseProductQuantity(Product product)
        {
            try
            {
                if (product == null) return;
                _cartService.DecreaseProductQuantity(product.Id);
                SyncCartItems();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to decrease product quantity: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "Failed to decrease product quantity.", "OK");
            }
        }






        //[RelayCommand]
        //private async Task GoToCheckoutAsync()
        //{
        //    // // معناه route مطلق
        //    await Shell.Current.GoToAsync("CheckoutPage");
        //}


        
        [RelayCommand]
        private async Task GoToCheckoutAsync()
        {
            try
            {
                //await Shell.Current.GoToAsync(nameof(CheckoutPage));
                await Shell.Current.GoToAsync("CheckoutRoute");
                //await Shell.Current.GoToAsync("CheckoutRoute");


            }
            catch (Exception ex)
            {           
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }







        [RelayCommand]
        public async Task DeleteCart()
        {
            if (IsBusy || IsBusyWithCartModification) return;

            try
            {
                var confirmed = await Shell.Current.DisplayAlert("Confirm", "Are you sure you want to delete the cart?", "Yes", "No");
                if (!confirmed) return;

                IsBusyWithCartModification = true;

                var success = await _cartService.DeleteCartAsync();

                if (success)
                {
                    CartItems.Clear();
                    var toast = Toast.Make("Cart deleted successfully.", ToastDuration.Short);
                    await toast.Show();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to delete cart.", "OK");
                }
            }
            finally
            {
                IsBusyWithCartModification = false;
            }
        }

        [RelayCommand]
        public async Task AddProduct(Product product)
        {
            var userIdStr = await SecureStorage.GetAsync("userId");
            if (!int.TryParse(userIdStr, out int userId)) return;

            await _cartService.AddProductToCartAsync(product, userId);
        }












































        [RelayCommand]
        private void SyncCartItems()
        {
            // replace entire collection to keep UI consistent
            try
            {
                var updatedCartItems = _cartService.GetCartItems() ?? new System.Collections.Generic.List<CartItemDetail>();
                CartItems.Clear();
                foreach (var updatedItem in updatedCartItems)
                {
                    CartItems.Add(updatedItem);
                    OnPropertyChanged(nameof(CartItems));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncCartItems error: {ex.Message}");
            }
        }
    }
}
