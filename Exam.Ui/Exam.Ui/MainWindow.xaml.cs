using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net;
using Exam.DataAcces;
using Exam.Models;
using System.ComponentModel;

namespace Exam.Ui
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _incrementNumber = 0;
        private int[] _numbers;
        private object _locker = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RequestData(string url, Action<string> action)
        {
            var client = new WebClient();
            string data = await client.DownloadStringTaskAsync(url);
            action(data);
        }

        private async void DownloadButton(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(urlTextBox.Text) || string.IsNullOrWhiteSpace(fileNameTextBox.Text))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadProgressChanged += Client_DownloadProgressChanged;
                        client.DownloadFileCompleted += Client_DownloadFileCompleted;
                        client.DownloadFileAsync(new Uri(urlTextBox.Text), fileNameTextBox.Text);
                    }

                    DownloadFile downloadFile = new DownloadFile()
                    {
                        FileName = fileNameTextBox.Text,
                        Url = urlTextBox.Text,
                    };

                    using (var context = new DataContext())
                    {
                        context.DownloadFiles.Add(downloadFile);
                        await context.SaveChangesAsync();
                    }

                    MessageBox.Show("Файл успешно загружен");
                }
                else
                {
                    MessageBox.Show("Упс что-то пошло не так!");
                }
            }
            catch (UriFormatException exception)
            {
                MessageBox.Show("Недопустимый URI");
                MessageBox.Show(exception.Message);

                DownloadFile downloadFile = new DownloadFile()
                {
                    FileName = fileNameTextBox.Text,
                    Url = urlTextBox.Text,
                };

                using (var context = new DataContext())
                {
                    context.DownloadFiles.Add(downloadFile);
                    await context.SaveChangesAsync();
                }
            }
            
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            asyncProgresBar.Value = 0;
            if (e.Cancelled)
            {
                MessageBox.Show("The download has been cancelled");
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show("An error ocurred while trying to download file");

                return;
            }

            MessageBox.Show("File succesfully downloaded");
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            asyncProgresBar.Value = e.ProgressPercentage;
            
        }


        private void SuperIncrementNumberClick(object sender, RoutedEventArgs e)
        {
            int count;

            if (int.TryParse(quantityTextBox.Text, out count))
            {
                _numbers = new int[count];
                for (int i = 0; i < count; i++)
                {
                    Thread thread = new Thread(AddNumberIn);
                    thread.Start(i + 1);
                    thread.Join();
                }
            }
            else
            {
                MessageBox.Show("Please норм введи! А!");
            }
        }

        private void AddNumberIn(object number)
        {
            Thread.Sleep(100);
            _numbers[_incrementNumber] = (int)number;
            Interlocked.Increment(ref _incrementNumber);
        }
    }
}
