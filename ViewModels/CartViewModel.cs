

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
using Microsoft.Maui.Storage;

namespace MauiStoreApp.ViewModels
{
    public partial class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly AuthService _authService;
        //public IAsyncRelayCommand InitCommand { get; }

        bool isFirstRun = true;


        public CartViewModel(CartService cartService, AuthService authService)
        {
            //InitCommand = new AsyncRelayCommand(Init);

            _cartService = cartService;
            _authService = authService;
            CartItems = new ObservableCollection<CartItemDetail>(_cartService.GetCartItems());
            //InitCommand = new AsyncRelayCommand(Init);

            _cartService.CartUpdated += OnCartUpdated; // ✅ متابعة تغييرات السلة


        }
        private void OnCartUpdated()
        {
            CartItems = new ObservableCollection<CartItemDetail>(_cartService.GetCartItems());
            OnPropertyChanged(nameof(CartItems));

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

        [RelayCommand]
        public async Task DeleteCart()
        {
            if (IsBusy || IsBusyWithCartModification) return;

            try
            {
                var userResponse = await Shell.Current.DisplayAlert("Confirm", "Are you sure you want to delete the cart?", "Yes", "No");
                if (!userResponse) return;

                if (CartItems.Count > 0)
                {
                    IsBusyWithCartModification = true;

                    var response = await _cartService.DeleteCartAsync();

                    if (response != null && response.IsSuccessStatusCode)
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
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No cart found.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to delete cart: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to delete cart.", "OK");
            }
            finally
            {
                IsBusyWithCartModification = false;
            }
        }

        [RelayCommand]
        public void IncreaseProductQuantity(Product product)
        {
            try
            {
                if (product == null) return;
                _cartService.IncreaseProductQuantity(product.Id);
                SyncCartItems();
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to decrease product quantity: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "Failed to decrease product quantity.", "OK");
            }
        }

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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncCartItems error: {ex.Message}");
            }
        }
    }
}
