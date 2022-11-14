using Ejercicio2_3PMO2.Models;
using MediaManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ejercicio2_3PMO2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListAudio : ContentPage
    {
        public ListAudio()
        {
            InitializeComponent();
        }
        
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            await CrossMediaManager.Current.Stop();
        }

        private async void SwipeItem_Invoked(object sender, EventArgs e)
        {
            SwipeItem item = sender as SwipeItem;
            Audio item2 = (Audio)item.BindingContext;
            await CrossMediaManager.Current.Play(item2.path);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ListaAudio.ItemsSource = await App.BaseDatos.GetListAudios();
        }
    }
}