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

        public CompassPage()
        {
            InitializeComponent();
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

            if (data.HeadingMagneticNorth >= 315 && data.HeadingMagneticNorth <= 45)
            {
                CurrentAspect = "Nord";
            }
            if (data.HeadingMagneticNorth >= 45 && data.HeadingMagneticNorth <= 135)
            {
                CurrentAspect = "Øst";
            }
            if (data.HeadingMagneticNorth >= 135 && data.HeadingMagneticNorth <= 225)
            {
                CurrentAspect = "syd";
            }
            if (data.HeadingMagneticNorth >= 225 && data.HeadingMagneticNorth <= 315)
            {
                CurrentAspect = "Vest";
            }

            // Process Heading Magnetic North
        }


    }
}