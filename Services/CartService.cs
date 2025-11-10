
using MauiStoreApp.Models;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace MauiStoreApp.Services
{
    public class CartService : BaseService
    {

        private static CartService _instance;
        public static CartService Instance => _instance ??= new CartService(new ProductService());



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

        //public async Task<HttpResponseMessage> DeleteCartAsync()
        //{

        //    //if (string.IsNullOrEmpty(cartId))
        //    if (!cartId.HasValue)
        //    {
        //        Debug.WriteLine("❌ cartId is null or empty — cannot delete cart.");
        //        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        //    }




        //    var response = await DeleteAsync($"carts/{cartId.Value}"); // use .Value

        //    if (response.IsSuccessStatusCode)
        //    {
        //        _cartItems.Clear();
        //        CartUpdated?.Invoke();
        //        await SaveCartLocallyAsync();
        //        cartId = null;
        //    }
        //    return response;
        //}

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

        //private async Task SaveCartLocallyAsync()
        //{
        //    var json = JsonConvert.SerializeObject(_cartItems);
        //    await SecureStorage.SetAsync("localCart", json);
        //}

        //public async Task LoadCartFromStorageAsync()
        //{
        //    try
        //    {
        //        var json = await SecureStorage.GetAsync("localCart");
        //        if (!string.IsNullOrEmpty(json))
        //            _cartItems = JsonConvert.DeserializeObject<List<CartItemDetail>>(json);

        //        CartUpdated?.Invoke();
        //    }
        //    catch { }
        //}

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
































        // قائمة المنتجات الموجودة في السلة
        //private List<CartItemDetail> _cartItems = new List<CartItemDetail>();

        //// رقم السلة الحالي (nullable int)
        //private int? cartId = null;

        //// حدث للإشعارات عند تحديث السلة
        //public event Action CartUpdated;




        public async Task<bool> DeleteCartAsync()
        {
            if (!cartId.HasValue)
            {
                _cartItems.Clear();
                CartUpdated?.Invoke();
                await SaveCartLocallyAsync();
                return true;
            }

            var response = await DeleteAsync($"carts/{cartId.Value}");
            if (response.IsSuccessStatusCode)
            {
                _cartItems.Clear();
                CartUpdated?.Invoke();
                await SaveCartLocallyAsync();
                cartId = null;
                return true;
            }
            return false;
        }

        // حفظ محل


        // حفظ السلة محليًا
        private async Task SaveCartLocallyAsync()
        {
            var json = JsonConvert.SerializeObject(_cartItems);
            await SecureStorage.SetAsync("localCart", json);
        }

        // تحميل السلة من التخزين المحلي
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




















        // إضافة منتج للسلة مع إنشاء السلة أول مرة إذا لم تكن موجودة
        public async Task AddProductToCartAsync(Product product, int userId)
        {
            if (!cartId.HasValue)
                await CreateCartForUserAsync(userId); // إنشاء السلة على السيرفر

            var item = _cartItems.FirstOrDefault(i => i.Product.Id == product.Id);
            if (item != null)
                item.Quantity++;
            else
                _cartItems.Add(new CartItemDetail { Product = product, Quantity = 1 });

            CartUpdated?.Invoke();
            await SaveCartLocallyAsync();
        }


        // داخل CartService
        public async Task<Cart> CreateCartForUserAsync(int userId)
        {
            var newCart = new Cart
            {
                UserId = userId,
                Products = new List<CartProduct>()
            };

            var json = JsonConvert.SerializeObject(newCart);

            using var client = new HttpClient();
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://phbarflogerpotdqiwrp.supabase.co", content);

            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync();
            var createdCart = JsonConvert.DeserializeObject<Cart>(resultJson);

            cartId = createdCart.Id; // 🔑 تعيين cartId للسلة الجديدة

            return createdCart;
        }
















    }
}
