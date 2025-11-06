//using MauiStoreApp.Models;
//using Newtonsoft.Json;
//using System.Collections.ObjectModel;

//namespace MauiStoreApp.Services
//{
//    /// <summary>
//    /// Provides services for managing the shopping cart.
//    /// </summary>
//    public class CartService : BaseService
//    {
//        private readonly ProductService _productService;
//        private List<CartItemDetail> _cartItems = new List<CartItemDetail>(); // Internal cart items collection
//        private int? cartId = null;

//        //public static object Instance { get; internal set; }

//        public event Action CartUpdated;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="CartService"/> class.
//        /// </summary>
//        /// <param name="productService">The product service to use for retrieving product details.</param>
//        public CartService(ProductService productService)
//        {
//            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
//        }

//        /// <summary>
//        /// Retrieves a specific cart based on the provided cart ID.
//        /// </summary>
//        /// <param name="cartId">The ID of the cart to be retrieved.</param>
//        /// <returns>The cart corresponding to the provided cart ID.</returns>
//        public async Task<Cart> GetCartAsync(int cartId)
//        {
//            return await GetAsync<Cart>($"carts/{cartId}");
//        }

//        /// <summary>
//        /// Retrieves the current cart items from the service's local cache.
//        /// </summary>
//        /// <returns>A list of cart items.</returns>
//        public List<CartItemDetail> GetCartItems()
//        {
//            return _cartItems;
//        }

//        /// <summary>
//        /// Refreshes the cart items based on a specific user's ID.
//        /// </summary>
//        /// <param name="userId">The ID of the user whose cart items are to be refreshed.</param>
//        /// <returns>A task that represents the asynchronous refresh operation.</returns>
//        public async Task RefreshCartItemsByUserIdAsync(int userId)
//        {
//            var carts = await GetAsync<List<Cart>>($"carts/user/{userId}");
//            var cart = carts?.FirstOrDefault();

//            cartId = cart?.Id;

//            if (cart != null)
//            {
//                _cartItems.Clear();

//                foreach (var cartProduct in cart.Products)
//                {
//                    var productDetails = await _productService.GetProductByIdAsync(cartProduct.ProductId);
//                    _cartItems.Add(new CartItemDetail
//                    {
//                        Product = productDetails,
//                        Quantity = cartProduct.Quantity,
//                    });
//                }
//            }
//        }

//        /// <summary>
//        /// Deletes the current cart and clears local cart items if the operation is successful.
//        /// </summary>
//        /// <returns>The HTTP response indicating the result of the delete operation.</returns>
//        public async Task<HttpResponseMessage> DeleteCartAsync()
//        {
//            var response = await DeleteAsync($"carts/{cartId}");
//            if (response.IsSuccessStatusCode)
//            {
//                _cartItems.Clear();
//            }
//            return response;
//            //CartUpdated?.Invoke();
//            _ = SaveCartLocallyAsync();
//        }

//        /// <summary>
//        /// Adds a product to the current cart or updates its quantity if it already exists in the cart.
//        /// </summary>
//        /// <param name="product">The instance of the product to be added to the cart.</param>
















//        public void AddProductToCart(Product product)
//        {
//            var existingCartItem = _cartItems.FirstOrDefault(item => item.Product.Id == product.Id);

//            if (existingCartItem != null)
//                existingCartItem.Quantity++;
//            else
//                _cartItems.Add(new CartItemDetail { Product = product, Quantity = 1 });

//            CartUpdated?.Invoke();
//            //CartUpdated?.Invoke();
//            _ = SaveCartLocallyAsync();
//        }

//        public void IncreaseProductQuantity(int? productId)
//        {
//            var existingCartItem = _cartItems.First(i => i.Product.Id == productId.Value);
//            existingCartItem.Quantity++;

//            CartUpdated?.Invoke();
//        }

//        public void DecreaseProductQuantity(int? productId)
//        {
//            var existingCartItem = _cartItems.First(i => i.Product.Id == productId.Value);
//            existingCartItem.Quantity--;

//            if (existingCartItem.Quantity <= 0)
//                _cartItems.Remove(existingCartItem);

//            CartUpdated?.Invoke();
//            //CartUpdated?.Invoke();
//            _ = SaveCartLocallyAsync();
//        }


































//        private async Task SaveCartLocallyAsync()
//        {
//            var json = JsonConvert.SerializeObject(_cartItems);
//            await SecureStorage.SetAsync("localCart", json);
//        }
//        public async Task LoadCartFromStorageAsync()
//        {
//            try
//            {
//                var json = await SecureStorage.GetAsync("localCart");
//                if (!string.IsNullOrEmpty(json))
//                {
//                    _cartItems = JsonConvert.DeserializeObject<List<CartItemDetail>>(json);
//                    CartUpdated?.Invoke();
//                }
//            }
//            catch { }
//        }


//        public ObservableCollection<CartItemDetail> CartItems { get; set; }
//             = new ObservableCollection<CartItemDetail>();

//        public static CartService Instance { get; } = new CartService();
//        public CartService()
//        {
//            LoadCart();
//        }

//        public void LoadCart()
//        {
//            var json = Preferences.Get("local_cart", null);

//            if (string.IsNullOrEmpty(json))
//            {
//                CartItems = new ObservableCollection<CartItemDetail>();
//                return;
//            }

//            CartItems = JsonConvert.DeserializeObject<ObservableCollection<CartItemDetail>>(json)
//                       ?? new ObservableCollection<CartItemDetail>();
//        }





//        public void SaveCart()
//        {
//            var json = JsonConvert.SerializeObject(CartItems);
//            Preferences.Set("local_cart", json);
//        }

//        public void ClearCart()
//        {
//            _cartItems.Clear();
//            CartItems.Clear();
//            SaveCart();
//        }





//    }
//}









using MauiStoreApp.Models;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace MauiStoreApp.Services
{
    public class CartService : BaseService
    {
        private readonly ProductService _productService;
        private List<CartItemDetail> _cartItems = new();
        private int? cartId = null;

        public event Action CartUpdated;

        public CartService(ProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public List<CartItemDetail> GetCartItems()
        {
            return _cartItems;
        }

        public async Task RefreshCartItemsByUserIdAsync(int userId)
        {
            var carts = await GetAsync<List<Cart>>($"carts/user/{userId}");
            var cart = carts?.FirstOrDefault();

            cartId = cart?.Id;
            _cartItems.Clear();

            if (cart != null)
            {
                foreach (var cartProduct in cart.Products)
                {
                    var productDetails = await _productService.GetProductByIdAsync(cartProduct.ProductId);
                    _cartItems.Add(new CartItemDetail
                    {
                        Product = productDetails,
                        Quantity = cartProduct.Quantity
                    });
                }
            }

            CartUpdated?.Invoke();
            await SaveCartLocallyAsync();
        }

        public async Task<HttpResponseMessage> DeleteCartAsync()
        {
            var response = await DeleteAsync($"carts/{cartId}");
            if (response.IsSuccessStatusCode)
            {
                _cartItems.Clear();
                CartUpdated?.Invoke();
                await SaveCartLocallyAsync();
            }
            return response;
        }

        public void AddProductToCart(Product product)
        {
            var item = _cartItems.FirstOrDefault(i => i.Product.Id == product.Id);
            if (item != null)
                item.Quantity++;
            else
                _cartItems.Add(new CartItemDetail { Product = product, Quantity = 1 });

            CartUpdated?.Invoke();
            _ = SaveCartLocallyAsync();
        }

        public void IncreaseProductQuantity(int productId)
        {
            var item = _cartItems.First(i => i.Product.Id == productId);
            item.Quantity++;
            CartUpdated?.Invoke();
            _ = SaveCartLocallyAsync();
        }

        public void DecreaseProductQuantity(int productId)
        {
            var item = _cartItems.First(i => i.Product.Id == productId);
            item.Quantity--;

            if (item.Quantity <= 0)
                _cartItems.Remove(item);

            CartUpdated?.Invoke();
            _ = SaveCartLocallyAsync();
        }

        private async Task SaveCartLocallyAsync()
        {
            var json = JsonConvert.SerializeObject(_cartItems);
            await SecureStorage.SetAsync("localCart", json);
        }

        public async Task LoadCartFromStorageAsync()
        {
            try
            {
                var json = await SecureStorage.GetAsync("localCart");
                if (!string.IsNullOrEmpty(json))
                    _cartItems = JsonConvert.DeserializeObject<List<CartItemDetail>>(json);

                CartUpdated?.Invoke();
            }
            catch { }
        }

        // ✅ دالة مسح السلة الصحيحة
        public void ClearCart()
        {
            _cartItems.Clear();
            CartUpdated?.Invoke();
            _ = SaveCartLocallyAsync();
        }





        public async Task<Cart> GetCartAsync(int cartId)
        {
            try
            {
                // تحميل السلة من التخزين المحلي
                var cartJson = Preferences.Get("local_cart", null);

                if (string.IsNullOrEmpty(cartJson))
                    return null;

                var cart = System.Text.Json.JsonSerializer.Deserialize<Cart>(cartJson);

                return cart;
            }
            catch
            {
                return null;
            }
        }

        public async Task ClearCartAsync()
        {
            _cartItems.Clear();
            CartUpdated?.Invoke();
            SecureStorage.Remove("localCart");
        }











    }
}
