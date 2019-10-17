using Common.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace CodeReadApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRReaderPage : ContentPage
    {
        public QRReaderPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Scanner();
        }

        private async void Scanner()
        {
            var scannerPage = new ZXingScannerPage();

            scannerPage.Title = "Lector de QR";
            scannerPage.OnScanResult += (result) =>
            {
                scannerPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    AESController aes = new AESController();

                    if (aes.HasError())
                    {
                        DisplayAlert("Error: ", aes.error, "OK");
                    }
                    else
                    {
                        string textDecrypt = aes.Decrypt(result.Text);
                        DisplayAlert("Valor Obtenido", textDecrypt, "OK");
                    }

                });
            };

            await Navigation.PushAsync(scannerPage);
        }
    }
}
