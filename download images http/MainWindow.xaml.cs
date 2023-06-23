using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;

namespace download_images_http
{
    public partial class MainWindow : Window
    {
        WebClient client = new WebClient();

        const string url = @"https://source.unsplash.com/random/";
        public MainWindow()
        {
            InitializeComponent();

            client.DownloadProgressChanged += Client_DownloadProgressChanged;
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private async void DownloadBtnClick(object sender, RoutedEventArgs e)
        {
            string fullUrl = url + $"{WidthTxtBox.Text}x{HeightTxtBox.Text}/?{CategoryTxtBox.Text}&1.jpg";

            string dest = GenerateFilePath(fullUrl);

            AddToHistoryList(fullUrl);

            try
            {
                await client.DownloadFileTaskAsync(new Uri(fullUrl), dest);
                previewImg.Source = new BitmapImage(new Uri(dest));
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.RequestCanceled)
                {
                    MessageBox.Show("Завантаження відмінено.");
                }
                else
                {
                    // Обробка інших типів помилок
                    MessageBox.Show("Помилка завантаження файлу: " + ex.Message);
                }
            }
        }

        public void AddToHistoryList(string url)
        {
            historyList.Items.Add($"{DateTime.Now.ToShortTimeString()} {Path.GetFileName(url).Remove(0,1)} {HeightTxtBox.Text}x{WidthTxtBox.Text}");
        }

        private string GenerateFilePath(string originalPath)
        {
            // get desktop path
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // generate random name
            string name = Guid.NewGuid().ToString();

            // get original extension
            string extension = Path.GetExtension(originalPath);

            // combine destination path
            return Path.Combine(desktop, name + extension);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            client.CancelAsync();
        }
    }
}
