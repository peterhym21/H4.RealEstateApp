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
    public partial class PropertyListPage : ContentPage
    {
        IRepository Repository;
        public ObservableCollection<PropertyListItem> PropertiesCollection { get; set; } = new ObservableCollection<PropertyListItem>();

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            LoadProperties();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadProperties();
        }

        void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            LoadProperties();
            list.IsRefreshing = false;
        }

        async Task LoadProperties()
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();
            Location location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                location = await GetCurrentLocation();
            }

            foreach (Property item in items)
            {
                var distance = await GetDistanceAsync(item.Address, location);
                PropertiesCollection.Add(new PropertyListItem(item, distance));
            }


        }

        private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PropertyDetailPage(e.Item as PropertyListItem));
        }

        private async void AddProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage());
        }



        public async Task<double> GetDistanceAsync(string address, Location mylocation)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();

                if (location != null)
                {
                    double distance = location.CalculateDistance(mylocation, DistanceUnits.Kilometers);
                    return distance;
                }
                else
                {
                    return 0;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                return 0;
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
                return 0;
            }
        }


        CancellationTokenSource cts;
        public async Task<Location> GetCurrentLocation()
        {
            try
            {

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                Location location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    return location;
                }
                else
                {
                    return null;
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                return null;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                return null;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                return null;
            }
            catch (Exception ex)
            {
                // Unable to get location
                return null;
            }
        }



        private async void SortPropertys_Clicked(object sender, System.EventArgs e)
        {
            ItemsListView.IsRefreshing = true;

            await LoadProperties();
            PropertiesCollection = new ObservableCollection<PropertyListItem>(PropertiesCollection.OrderBy(x => x.Distance));

            ItemsListView.IsRefreshing = false;

        }


    }
}