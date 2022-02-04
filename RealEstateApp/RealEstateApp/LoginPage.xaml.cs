using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void ButtonLogin_Clicked(object sender, EventArgs e)
        {

        }

        private async Task LoginAsync()
        {

        }


    }
}