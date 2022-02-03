using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeightCalculatorPage : ContentPage
    {

        public double CurrentPressure { get; set; }
        public double CurrentAltitude { get; set; }

        SensorSpeed speed = SensorSpeed.UI;

        public ObservableCollection<BarometerMeasurement> BarometerMeasurements { get; set; } = new ObservableCollection<BarometerMeasurement>();

        public HeightCalculatorPage()
        {
            InitializeComponent();
            BindingContext = this;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Barometer.IsMonitoring)
                return;

            Barometer.ReadingChanged += Barometer_ReadingChanged;
            Barometer.Start(speed);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (!Barometer.IsMonitoring)
                return;

            Barometer.ReadingChanged += Barometer_ReadingChanged;
            Barometer.Stop();
        }

        void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            var data = e.Reading;
            // Process Pressure
            CurrentPressure = data.PressureInHectopascals;

            CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / 1013.25, 0.190284));
            // 1013.25 Average sea-level pressure in pha

        }

        private void SaveMesurment_Clicked(object sender, EventArgs e)
        {
            if (BarometerMeasurements.Count >= 1)
            {
                BarometerMeasurements.Add(new BarometerMeasurement
                {
                    Altitude = CurrentAltitude,
                    Label = LabelName.Text,
                    Pressure = CurrentPressure,
                    HeightChange = (CurrentAltitude - BarometerMeasurements.Last().Altitude)
                });
            }
            else
            {
                BarometerMeasurements.Add(new BarometerMeasurement
                {
                    Altitude = CurrentAltitude,
                    Label = LabelName.Text,
                    Pressure = CurrentPressure,
                    HeightChange = 0
                });
            }

        }

    }
}