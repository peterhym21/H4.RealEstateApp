using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.IO;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyDetailPage : ContentPage
    {

        public bool PlayBool  { get; set; }

        public PropertyDetailPage(PropertyListItem propertyListItem)
        {
            InitializeComponent();

            Property = propertyListItem.Property;

            IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

            PlayBool = true;

            BindingContext = this;
        }

        public Agent Agent { get; set; }

        public Property Property { get; set; }

        private async void EditProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }


        #region TTS

        
        private async void PlayDiscription_Clicked(object sender, EventArgs e)
        {
            await SpeakNowDefaultSettings();
        }

        CancellationTokenSource cts;
        public async Task SpeakNowDefaultSettings()
        {
            cts = new CancellationTokenSource();
            await TextToSpeech.SpeakAsync(Property.Description, cts.Token);
        }

        private void CancelSpeech_Clicked(object sender, EventArgs e)
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();
        }
        #endregion

        private void Image_OnTabGestureRecognizerTapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ImageListPage(Property));
        }

        #region sms Call Email



        private async void Email_OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var attachmentFilePath = Path.Combine(folder, "property.txt");
            File.WriteAllText(attachmentFilePath, $"{Property.Address}");
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Test",
                    Body = "test",
                    To = new() { Property.Vendor.Email },
                    Attachments = new()
                    {
                        new EmailAttachment(attachmentFilePath)
                    }
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }

        }

        private async void Phone_OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Call, Message?", "Cancel", null, "Call", "Message");
            switch (action)
            {
                case "Call":
                    PlacePhoneCall();
                    break;
                case "Message":
                    await SendSms();
                    break;
                default:
                    break;
            }
        }

        public void PlacePhoneCall()
        {
            try
            {
                PhoneDialer.Open(Property.Vendor.Phone);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
        public async Task SendSms()
        {
            try
            {
                var message = new SmsMessage("test", new[] { Property.Vendor.Phone });
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Sms is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        #endregion


        private async void MapMarkedAlt_Clicked(object sender, EventArgs e)
        {
            var location = new Location((double)Property.Latitude, (double)Property.Longitude);
            try
            {
                await Map.OpenAsync(location);
            }
            catch (Exception ex)
            {
                // No map application available to open
            }
        }

        private async void Directions_Clicked(object sender, EventArgs e)
        {
            var location = new Location((double)Property.Latitude, (double)Property.Longitude);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            await Map.OpenAsync(location, options);
        }

    }
}