using Ejercicio2_3PMO2.Controllers;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ejercicio2_3PMO2
{
    public partial class App : Application
    {

        static BaseDatos basedatos;
        public static BaseDatos BaseDatos
        {
            get
            {
                if (basedatos == null)
                {
                    basedatos = new BaseDatos(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AudiosDB.db3"));
                }
                return basedatos;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
