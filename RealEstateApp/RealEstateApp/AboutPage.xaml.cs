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
    public partial class AboutPage : ContentPage
    {
        public float Pitch { get; set; }
        public float Volume { get; set; }
        public AboutPage()
        {
            
            InitializeComponent();
            Preferences.Set("Pitch", Pitch);
            Preferences.Set("Volume", Volume);
        }
    }
}