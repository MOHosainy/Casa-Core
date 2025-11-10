//using CommunityToolkit.Mvvm.Input;
//using MauiStoreApp.Services;
//using MauiStoreApp.ViewModels;
//using OURSTORE.ViewModels;

//namespace MauiStoreApp.Views;

//public partial class CheckoutPage : ContentPage
//{


//    public CheckoutPage(CheckoutViewModel viewModel)
//    {
//        InitializeComponent();
//        BindingContext = viewModel;
//    }





//}








using MauiStoreApp.Services;
using MauiStoreApp.ViewModels;
//using OURSTORE.ViewModels;

namespace MauiStoreApp.Views;

public partial class CheckoutPage : ContentPage
{

    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // ??? ??? ??? CartService ???????? ?? CartPage
    }

    //public CheckoutPage()  // default constructor ????? ?? Shell
    //{
    //    InitializeComponent();
    //    var cartService = CartService.Instance;
    //    BindingContext = new CheckoutViewModel(cartService);
    //}
}
