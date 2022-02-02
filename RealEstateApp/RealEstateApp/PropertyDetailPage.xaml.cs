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

        private async void OnTabGestureRecognizerTapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ImageListPage(Property));
        }




    }
}