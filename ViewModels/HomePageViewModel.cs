using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Models;
using MauiStoreApp.Services;
using MauiStoreApp.Views;
using System.Linq; // تأكد إنها موجودة

namespace MauiStoreApp.ViewModels
{
    /// <summary>
    /// Represents the view model for the home page.
    /// </summary>
    public partial class HomePageViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly RecentlyViewedProductsService _recentlyViewedProductsService;

        /// <summary>
        /// Gets the products.
        /// </summary>
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        private bool isFirstRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        /// <param name="categoryService">The category service.</param>
        /// <param name="recentlyViewedProductsService">The recently viewed products service.</param>
        public HomePageViewModel(ProductService productService, CategoryService categoryService, RecentlyViewedProductsService recentlyViewedProductsService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            isFirstRun = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class. This empty constructor is used for design-time data.
        /// </summary>
        public HomePageViewModel()
        {
        }

        /// <summary>
        /// Initializes the home page view model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task Init()
        {
            if (isFirstRun)
            {
                await GetProductsAsync();
                await GetCategoriesAsync();
                _recentlyViewedProductsService.LoadProducts();
                isFirstRun = false;
            }
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        private async Task GetCategoriesAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }


        //private async Task GetProductsAsync()
        //{
        //    if (IsBusy)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        IsBusy = true;

        //        var products = await _productService.GetProductsAsync();
        //        Products.Clear();
        //        foreach (var product in products)
        //        {
        //            Products.Add(product);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Unable to get products: {ex.Message}");
        //        await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}













        private async Task GetProductsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var products = await _productService.GetProductsAsync();
                _allProducts = products.ToList();

                Products.Clear();
                foreach (var product in _allProducts)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get products: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
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
        }

        /// <summary>
        /// Handles the category tapped event.
        /// </summary>
        /// <param name="category">The tapped category.</param>
        [RelayCommand]
        private async Task CategoryTapped(Category category)
        {
            if (category == null)
            {
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "Category", category },
            };

            await Shell.Current.GoToAsync($"{nameof(CategoryPage)}", true, navigationParameter);
        }










private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilterProducts();
            }
        }
    }

    private List<Product> _allProducts = new(); // الأصلية من الـ API

    private void FilterProducts()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            // رجّع كل المنتجات
            Products.Clear();
            foreach (var item in _allProducts)
                Products.Add(item);
        }
        else
        {
            var filtered = _allProducts
                .Where(p => p.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Products.Clear();
            foreach (var item in filtered)
                Products.Add(item);
        }
    }

}
}
