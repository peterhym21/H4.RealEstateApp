﻿using RealEstateApp.Models;
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
using Newtonsoft.Json;

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
            var settings = new SpeechOptions()
            {
                Volume = Preferences.Get("Pitch", 0.5f),
                Pitch = Preferences.Get("Volume", 0.5f)
        };
            cts = new CancellationTokenSource();
            await TextToSpeech.SpeakAsync(Property.Description, settings, cts.Token);
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


        #region Maps

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
        #endregion


        #region Link

        private async void Link_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync("https://www.erdetfredag.dk/", BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
            }
        }


        private async void ExternalLink_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync("https://www.erdetfredag.dk/", BrowserLaunchMode.External);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
            }
        }
        private async void PDF_Clicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(Property.ContractFilePath)
            });
        }

        #endregion


        #region share Copy

        private async void ShareText_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = Property.NeighbourhoodUrl,
                Subject = "A property you may be interested in",
                Text = $"Address: {Property.Address}, Price: {Property.Price}, Beds: {Property.Beds}",
                Title = "Share Property"
            });
        }

        private async void ShareFile_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareFileRequest
            {

                Title = "Share Property Contract",
                File = new ShareFile(Property.ContractFilePath)
            });
        }

        private async void CopyClipboard_Clicked(object sender, EventArgs e)
        {

            await Clipboard.SetTextAsync(JsonConvert.SerializeObject(Property));
        }



        #endregion

    }
}