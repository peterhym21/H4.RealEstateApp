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
    public partial class CompassPage : ContentPage
    {

        public string CurrentAspect { get; set; }
        public double RotationAngle { get; set; }
        public string CurrentHeading { get; set; }

        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;

        private readonly Property _property;

        public CompassPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
        public CompassPage(Property property)
        {
            InitializeComponent();
            _property = property;
            BindingContext = this;

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Compass.IsMonitoring)
                return;

            Compass.ReadingChanged += Compass_ReadingChanged;
            Compass.Start(speed);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (!Compass.IsMonitoring)
                return;

            Compass.ReadingChanged -= Compass_ReadingChanged;
            Compass.Stop();
        }



        void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;
            CurrentHeading = data.HeadingMagneticNorth.ToString();
            RotationAngle = (int)data.HeadingMagneticNorth * - 1;

            CurrentAspect = RotationAngle switch
            {
                > 45 and < 135 => "West",
                > 135 and < 225 => "South",
                > 225 and < 315 => "East",
                // > 315 and < 45
                _ => "North",
            };

            // Process Heading Magnetic North
        }

        private async void SaveAspect_Clicked(object sender, EventArgs e)
        {
            _property.Aspect = CurrentAspect;
            await Navigation.PopModalAsync();
        }


    }
}