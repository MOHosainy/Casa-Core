using CommunityToolkit.Mvvm.Input;
//using Foundation;
using Microsoft.Extensions.DependencyInjection;

//using Foundation;
using MauiStoreApp.Services;
using MauiStoreApp.ViewModels;

namespace MauiStoreApp.Views;


public partial class CheckoutPage : ContentPage
{


    public CheckoutPage(CartService cartService)
    {
        InitializeComponent();
        BindingContext = new CheckoutViewModel(cartService);
    }




}