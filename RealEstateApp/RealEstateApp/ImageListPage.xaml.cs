using RealEstateApp.Models;
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
    public partial class ImageListPage : ContentPage
    {
        public List<string> Posistion { get; set; }

        private readonly Property _property;
        public ImageListPage(Property property)
        {
            InitializeComponent();
            _property = property;

            GetImageUrls();

            BindingContext = this;
        }



        private void GetImageUrls()
        {
            foreach (var item in _property.ImageUrls)
            {
               Posistion.Add(item);
            }
        }




    }
}