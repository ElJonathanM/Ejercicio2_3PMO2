using Ejercicio2_3PMO2.Models;
using Ejercicio2_3PMO2.Views;
using MediaManager;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ejercicio2_3PMO2
{
    public partial class MainPage : ContentPage
    {

        Random rnd = new Random();
        AudioRecorderService recorder;
        AudioPlayer player;
        bool isTimerRunning = false;
        bool isTimer2Running = false;
        bool seReprodujo = false;
        int seconds = 0, minutes = 0;
        public string fileName { get; set; }
        int ultimoIDAUDIO = 0;

        public MainPage()
        {
            InitializeComponent();
            int random = rnd.Next(10, 9999);
            sacarUltimoIdAudio();
            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample_" + ultimoIDAUDIO + "" + random + ".wav");
            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(180),//3 minutos
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };
            player = new AudioPlayer();
            player.FinishedPlaying += Finish_Playing;
        }

        private async void bntRecord_Clicked(object sender, EventArgs e)
        {
            if (!recorder.IsRecording)
            {
                seconds = 0; minutes = 0;
                isTimerRunning = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                    seconds++;
                    if (seconds.ToString().Length == 1) { lblSeconds.Text = "0" + seconds.ToString(); }
                    else { lblSeconds.Text = seconds.ToString(); }
                    if (seconds == 60)
                    {
                        minutes++;
                        seconds = 0;
                        if (minutes.ToString().Length == 1) { lblMinutes.Text = "0" + minutes.ToString(); }
                        else { lblMinutes.Text = minutes.ToString(); }
                        lblSeconds.Text = "00";
                    }
                    return isTimerRunning;
                });

                //DETECTA LO DEL SILECIO
                recorder.StopRecordingOnSilence = IsSilence.IsToggled;
                var audioRecordTask = await recorder.StartRecording();
                lblinfo.Text = "GRABANDO";

                bntRecord.IsEnabled = false;
                bntRecord.BackgroundColor = Color.Silver;
                bntPlay.IsEnabled = false;
                bntPlay.BackgroundColor = Color.Silver;
                bntStop.IsEnabled = true;
                bntStop.BackgroundColor = Color.FromHex("#7cbb45");
                await audioRecordTask;
            }
        }

        private async void bntStop_Clicked(object sender, EventArgs e)
        {
            StopRecording();
            await recorder.StopRecording();
        }

        private async void bntPlay_Clicked(object sender, EventArgs e)
        {
            try
            {
                var stream = recorder.GetAudioFileStream();
                bool doesExist = File.Exists(fileName);
                if (doesExist)
                {
                    
                    String[] filee = fileName.Split('/'); 
                    String nombrerch = filee[filee.Length - 1];  
                    String respuesta = GetNumbers(nombrerch);
                    if (respuesta.Length <= 0)
                    {
                        fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample" + rnd + ".wav");
                    }
                    else
                    {
                        int numero = Int32.Parse(respuesta);
                        int numeronuevo = numero += 2;
                        fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample" + numeronuevo + ".wav");
                    }
                }
                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) { stream.CopyTo(fileStream); }
               

                if (fileName != null)
                {
                    StopRecording();
                    await CrossMediaManager.Current.Play(fileName);
                    lblinfo.Text = "REPRODUCIENDO";
                    seReprodujo = true;
                    //--TIMER DE REPRODUCCION
                    seconds = 0;
                    minutes = 0;
                    isTimer2Running = true;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                        if (CrossMediaManager.Current.IsStopped()) { isTimer2Running = false; StopRecording(); }
                        seconds++;
                        if (seconds.ToString().Length == 1) { lblSeconds.Text = "0" + seconds.ToString(); }
                        else { lblSeconds.Text = seconds.ToString(); }
                        if (seconds == 60)
                        {
                            minutes++;
                            seconds = 0;
                            if (minutes.ToString().Length == 1) { lblMinutes.Text = "0" + minutes.ToString(); }
                            else { lblMinutes.Text = minutes.ToString(); }
                            lblSeconds.Text = "00";
                        }
                        return isTimer2Running;
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void btnsaveSQLite_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtdescricion.Text))
            {
                await DisplayAlert("ALERTA", "DEBES INGRESAR UNA DESCRIPCION PARA GUARDAR TU AUDIO!", "OK");
            }
            else
            {
                var stream = recorder.GetAudioFileStream();
                if (seReprodujo != true) 
                {
                    bool doesExist = File.Exists(fileName);
                    if (doesExist)
                    {
                        
                        String[] filee = fileName.Split('/'); 
                        String nombrerch = filee[filee.Length - 1];  
                        String respuesta = GetNumbers(nombrerch);
                        if (respuesta.Length <= 0)
                        {
                            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample" + rnd + ".wav");
                        }
                        else
                        {
                            int numero = Int32.Parse(respuesta);
                            int numeronuevo = numero += 2;
                            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample" + numeronuevo + ".wav");
                        }
                    }
                    using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                        var mic = new Audio
                        {
                            path = fileName.ToString(),
                            descripcion = txtdescricion.Text,
                            fecha = DateTime.Now
                        };
                        var resultado = await App.BaseDatos.guardaAudios(mic);
                        if (resultado != 0)
                        {
                            await DisplayAlert("Aviso", "Audio Guardado!! en\n" + mic.path, "OK");
                            btnsaveSQLite.IsEnabled = false;
                            txtdescricion.Text = "";
                            StopRecording();
                        }
                    }
                }
                else 
                {
                    var mic = new Audio
                    {
                        path = fileName.ToString(),
                        descripcion = txtdescricion.Text,
                        fecha = DateTime.Now
                    };
                    var resultado = await App.BaseDatos.guardaAudios(mic);
                    if (resultado != 0)
                    {
                        await DisplayAlert("Aviso", "Audio Guardado!! en\n" + mic.path, "OK");
                        txtdescricion.Text = "";
                        StopRecording();
                    }
                    else
                    {
                        await DisplayAlert("ERROR", "Ocurrio un error al guardar!! en\n" + mic.path, "OK");
                    }
                }
            }
        }

        private async void bntLista_Clicked(object sender, EventArgs e)
        {
            var newpage = new ListAudio();
            newpage.Title = "LISTADO DE AUDIOS";
            await Navigation.PushAsync(newpage);
        }


        public async void sacarUltimoIdAudio()
        {
            List<Audio> lis = await App.BaseDatos.GetListAudios();
            if (lis.Count() > 0)
            {
                Audio ultimoaudio = await App.BaseDatos.GetAudiosporId(lis[lis.Count() - 1].id); 
                ultimoIDAUDIO = ultimoaudio.id += 2;   
            }
        }

        public void StopRecording()
        {
            isTimerRunning = false;
            isTimer2Running = false;
            bntRecord.IsEnabled = true;
            bntRecord.BackgroundColor = Color.FromHex("#7cbb45");
            bntPlay.IsEnabled = true;
            bntPlay.BackgroundColor = Color.FromHex("#7cbb45");
            bntStop.IsEnabled = false;
            bntStop.BackgroundColor = Color.Silver;
            lblSeconds.Text = "00";
            lblMinutes.Text = "00";
            lblinfo.Text = "LISTO GRABAR";
            txtdescricion.IsEnabled = true;
            btnsaveSQLite.IsEnabled = true;
            bntLista.IsEnabled = true;
        }

        private string GetNumbers(String InputString)
        {
            String Result = "";
            string Numbers = "0123456789";
            int i = 0;

            for (i = 0; i < InputString.Length; i++)
            {
                if (Numbers.Contains(InputString.ElementAt(i)))
                {
                    Result += InputString.ElementAt(i);
                }
            }
            return Result;
        }

        public void Finish_Playing(object sender, EventArgs e)
        {
            lblSeconds.Text = "00";
            lblMinutes.Text = "00";
            bntRecord.IsEnabled = true;
            bntRecord.BackgroundColor = Color.FromHex("#7cbb45");
            bntPlay.IsEnabled = true;
            bntPlay.BackgroundColor = Color.FromHex("#7cbb45");
            bntStop.IsEnabled = false;
            bntStop.BackgroundColor = Color.Silver;
        }
    }
}
