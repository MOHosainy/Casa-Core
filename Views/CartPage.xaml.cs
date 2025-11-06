using CommunityToolkit.Mvvm.Messaging;
using MauiStoreApp.ViewModels;
using OURSTORE.Messages;
using System.Diagnostics;

namespace MauiStoreApp.Views;

public partial class CartPage : ContentPage
{
	public CartPage(CartViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
   
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CartViewModel vm)
            vm.InitCommand.Execute(null);
    }



}