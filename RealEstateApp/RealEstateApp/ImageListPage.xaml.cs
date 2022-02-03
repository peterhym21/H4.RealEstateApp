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
            Posistion = new List<string>();
            foreach (var item in _property.ImageUrls)
            {
                Posistion.Add(item.ToString());
            }
        }


        SensorSpeed speed = SensorSpeed.Game;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Accelerometer.IsMonitoring)
                return;

            Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;
            Accelerometer.Start(speed);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (!Accelerometer.IsMonitoring)
                return;

            Accelerometer.ShakeDetected -= Accelerometer_ShakeDetected;
            Accelerometer.Stop();
        }


        void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            if (TheCarousel.Position < Posistion.Count - 1)
                TheCarousel.Position = TheCarousel.Position++;
            else
                TheCarousel.Position = 0;
        }


    }
}