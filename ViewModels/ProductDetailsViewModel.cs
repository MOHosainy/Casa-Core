using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Models;
using MauiStoreApp.Services;
using MauiStoreApp.Views;

namespace MauiStoreApp.ViewModels
{
    /// <summary>
    /// Represents the view model for the product details page.
    /// </summary>
    [QueryProperty(nameof(Product), "Product")]
    public partial class ProductDetailsViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private readonly CartService _cartService;
        private readonly RecentlyViewedProductsService _recentlyViewedProductsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsViewModel"/> class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        /// <param name="cartService">The cart service.</param>
        /// <param name="recentlyViewedProductsService">The recently viewed products service.</param>
        public ProductDetailsViewModel(ProductService productService, CartService cartService, RecentlyViewedProductsService recentlyViewedProductsService)
        {
            _productService = productService;
            _cartService = cartService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsViewModel"/> class. This empty constructor is used for design-time data.
        /// </summary>
        public ProductDetailsViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the current product.
        /// </summary>
        [ObservableProperty]
        Product product;

        /// <summary>
        /// Gets the cross-sell products.
        /// </summary>
        public ObservableCollection<Product> CrossSellProducts { get; private set; } = new ObservableCollection<Product>();

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task Init()
        {
            await GetProductByIdAsync();
            await GetCrossSellProductsAsync();
        }

        private async Task GetProductByIdAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var product = await _productService.GetProductByIdAsync(Product.Id);
                Product = product;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get product: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to get products.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
       


        private async Task GetCrossSellProductsAsync()
        {
            if (IsBusy || Product == null)
            {
                return;
            }

            try
            {
                IsBusy = true;
                var crossSellProducts = await _productService.GetProductsByCategoryAsync(Product.Category);
                CrossSellProducts.Clear();
                foreach (var crossSellProduct in crossSellProducts)
                {
                    if (crossSellProduct.Id != Product.Id) // exclude the current product
                    {
                        CrossSellProducts.Add(crossSellProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get cross-sell products: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to get cross-sell products.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Handles the product tapped event.
        /// </summary>
        /// <param name="product">The tapped product.</param>
        [RelayCommand]
        private async Task ProductTapped(Product product)
        {
            IsBusy = true;

            if (product == null)
            {
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "Product", product },
            };

            _recentlyViewedProductsService.AddProduct(product);

            await Shell.Current.GoToAsync($"{nameof(ProductDetailsPage)}", true, navigationParameter);

            IsBusy = false;
        }

        /// <summary>
        /// Shares the product.
        /// </summary>
        /// <param name="product">The product to share.</param>
        [RelayCommand]
        private async Task ShareProduct(Product product)
        {
            if (product == null)
                return;

            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = product.Image,
                Title = product.Title,
                Text = "Hey, check out this product I found on AStore!",
            });




        }

        /// <summary>
        /// Adds the product to the cart.
        /// </summary>
        /// <param name="product">The product to add to the cart.</param>
        //[RelayCommand]



        [RelayCommand]
        public async Task AddToCart(Product product)
        {
            if (product == null) return;

            _cartService.AddProductToCart(product);

            await Shell.Current.GoToAsync("//MainTabs/CartPage");
            //await Shell.Current.GoToAsync("CartPage");

            var toast = Toast.Make("✅ Product added to cart ( تمت إضافة المنتج إلى عربة التسوّق ) ", ToastDuration.Short);
            await toast.Show();
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
