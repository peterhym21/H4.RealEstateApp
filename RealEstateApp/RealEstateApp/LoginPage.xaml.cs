using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async void ButtonLogin_Clicked(object sender, EventArgs e)
        {
            LoginResult result = await LoginAsync();
            if (result.Success)
            {
                await DisplayAlert("Welcome", "Welcome back", "thanks");
            }
            else
            {
                await DisplayAlert("Out!", "Get the fuck out of here!", "sorry");
            }
        }

        public async Task<LoginResult> LoginAsync()
        {
            if (await SecureStorage.GetAsync("accessToken") != null)
            {
                LoginButton.IsVisible = false;
                LogoutButton.IsVisible = true;
                return new LoginResult()
                {
                    Success = true,
                    AccessToken = await SecureStorage.GetAsync("accessToken"),
                    RefreshToken = await SecureStorage.GetAsync("refreshToken")
                };
            }

            if (Username == "bofa" && Password == "admin")
            {
                LoginButton.IsVisible = false;
                LogoutButton.IsVisible = true;
                LoginResult result = new LoginResult()
                {
                    Success = true,
                    AccessToken = "bruh_acessToken",
                    RefreshToken = "bofa_deez_tokens"
                };

                await SecureStorage.SetAsync("accessToken", result.AccessToken);
                await SecureStorage.SetAsync("refreshToken", result.RefreshToken);
                return result;
            }

            return new LoginResult()
            {
                Success = false
            };
        }
        private void LogoutButton_OnClicked(object sender, EventArgs e)
        {
            SecureStorage.RemoveAll();
            LoginButton.IsVisible = true;
            LogoutButton.IsVisible = false;
        }


    }
}