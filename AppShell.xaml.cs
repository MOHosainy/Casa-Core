using MauiStoreApp.Views;

namespace MauiStoreApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(nameof(ProductDetailsPage), typeof(ProductDetailsPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
            Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));


            //Routing.RegisterRoute("CartPage", typeof(CartPage));
        }
    }
}