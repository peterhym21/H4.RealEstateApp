using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPropertyPage : ContentPage
    {
        private IRepository Repository;

        #region PROPERTIES
        public ObservableCollection<Agent> Agents { get; }

        private Property _property;
        public Property Property
        {
            get => _property;
            set
            {
                _property = value;
                if (_property.AgentId != null)
                {
                    SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
                }

            }
        }

        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (Property != null)
                {
                    _selectedAgent = value;
                    Property.AgentId = _selectedAgent?.Id;
                }
            }
        }

        

        public bool FlashOnOff { get; set; } = false;

        public string StatusMessage { get; set; }

        public string BatteryMessage { get; set; } 

        public Color StatusColor { get; set; } = Color.White;
        #endregion

        public AddEditPropertyPage(Property property = null)
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agents = new ObservableCollection<Agent>(Repository.GetAgents());

            if (property == null)
            {
                Title = "Add Property";
                Property = new Property();
            }
            else
            {
                Title = "Edit Property";
                Property = property;
            }

            BindingContext = this;
            BatteryStateChange();
        }

        #region SaveCancel

        
        private async void SaveProperty_Clicked(object sender, System.EventArgs e)
        {
            if (IsValid() == false)
            {
                StatusMessage = "Please fill in all required fields";
                StatusColor = Color.Red;
                try
                {
                    var duration = TimeSpan.FromSeconds(1);
                    Vibration.Vibrate(duration);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // Feature not supported on device
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                }
            }
            else
            {
                Repository.SaveProperty(Property);
                await Navigation.PopToRootAsync();
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        #endregion


        #region Location

        CancellationTokenSource cts;
        private async void GetCurrentLocation_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                var profiles = Connectivity.ConnectionProfiles;
                if (profiles.Contains(ConnectionProfile.WiFi))
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    cts = new CancellationTokenSource();
                    var location = await Geolocation.GetLocationAsync(request, cts.Token);

                    if (location != null)
                    {
                        Property.Latitude = location.Latitude;
                        Property.Longitude = location.Longitude;
                    }
                }
                else
                {
                    StatusMessage = "Network Required to use geolocation";
                    StatusColor = Color.Red;
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private async void GetAddressFromLocation_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                var profiles = Connectivity.ConnectionProfiles;
                if (profiles.Contains(ConnectionProfile.WiFi))
                {
                    double lat = Property.Latitude ?? 0;
                    double lon = Property.Longitude ?? 0;

                    var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        var geocodeAddress =
                            $"{placemark.SubThoroughfare} {placemark.Thoroughfare}, {placemark.Locality} {placemark.PostalCode}, {placemark.CountryName}";

                        Property.Address = geocodeAddress;
                    }
                }
                else
                {
                    StatusMessage = "Network Required to use geolocation";
                    StatusColor = Color.Red;
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
            }
        }

        #endregion


        private async void BatteryStateChange()
        {
            var level = Battery.ChargeLevel; // returns 0.0 to 1.0 or 1.0 when on AC or no battery.

            var state = Battery.State;
            var status = Battery.EnergySaverStatus;


            if (state == BatteryState.Charging)
            {
                BatteryMessage = "Charging your battery";
                StatusColor = Color.Yellow;
            }
            if (level <= 0.2)
            {
                BatteryMessage = "Your battery i getting Low";
                StatusColor = Color.Red;
            }
            if (status == EnergySaverStatus.On)
            {
                BatteryMessage = "Your are in battery saving mode";
                StatusColor = Color.Green;
            }

        }


        private async void FlashLight_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                if (!FlashOnOff)
                {
                    FlashOnOff = true;
                }
                else
                {
                    FlashOnOff = false;
                }

                if (FlashOnOff)
                {
                    await Flashlight.TurnOnAsync();
                }
                if (!FlashOnOff)
                {
                    await Flashlight.TurnOffAsync();
                }
                

                
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to turn on/off flashlight
            }
        }


        private void GetAspect_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new CompassPage(Property));
        }



    }
}