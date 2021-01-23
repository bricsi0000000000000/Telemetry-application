using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Models;
using Telemetry_presentation_layer.Errors;

namespace Telemetry_presentation_layer.Menus.Live
{
    public partial class LiveMenu : UserControl
    {
        private static HttpClient client = new HttpClient();
        private readonly BrushConverter brushConverter = new BrushConverter();
        private List<Section> sections = new List<Section>();
        private Section activeSection;

        public LiveMenu()
        {
            InitializeComponent();

            InitilaizeHttpClient();
        }

        private void InitilaizeHttpClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri("http://192.168.1.33:5000")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void RefreshSectionsButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));
        }

        private void RefreshSectionsButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
        }

        private void RefreshSectionsButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary700));
        }

        private void RefreshSectionsButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));

            GetAllSectionsAsync();
        }

        private async void GetAllSectionsAsync()
        {
            try
            {
                var response = await client.GetAsync("/api/Section").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                sections = JsonConvert.DeserializeObject<List<Section>>(resultString);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FillSectionsStackPanel(sections);
                    ChangeSectionColors();
                });
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowErrorMessage("Couldn't connect to the sever");
                });
            }
        }

        private void FillSectionsStackPanel(List<Section> sections)
        {
            SectionsStackPanel.Children.Clear();
            sections.Reverse();
            activeSection = sections[0];
            foreach (var section in sections)
            {
                SectionsStackPanel.Children.Add(new LiveSectionItem(section));
            }
        }

        public void SelectSection(int id)
        {
            if (activeSection.ID != id)
            {
                activeSection = GetSection(id);
                ChangeSectionColors();
            }
        }

        public void ChangeStatus(int id)
        {
            SelectSection(id);

            SectionDialogHost.ShowDialog(
                    new ChangeIsLiveStatusDialogContent(
                        $"You are about to change {activeSection.Name}'s status from " +
                        $"{(activeSection.IsLive ? "live" : "offline")} to {(activeSection.IsLive ? "offline" : "live")}\n" +
                        $"Are you sure about that?"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="change">True if the status should changed, false if not.</param>
        public async void ChangeStatusResult(bool change)
        {
            SectionDialogHost.IsOpen = false;

            var updatedSection = new Section
            {
                ID = activeSection.ID,
                Date = activeSection.Date,
                Name = activeSection.Name,
                IsLive = (change ? !activeSection.IsLive : activeSection.IsLive)
            };
            try
            {
                var response = await client.PutAsJsonAsync($"/api/Section", updatedSection).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        activeSection.IsLive = !activeSection.IsLive;
                        ChangeSectionStatus(activeSection.ID, activeSection.IsLive);
                        AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                        ErrorSnackbar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
                        ErrorSnackbar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
                        ShowErrorMessage($"{activeSection.Name}'s status was modified from {!activeSection.IsLive} to {activeSection.IsLive}");
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                        ErrorSnackbar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
                        ErrorSnackbar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
                        ShowErrorMessage($"Couldn't update {activeSection.Name}'s status from {activeSection.IsLive} to {!activeSection.IsLive}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                    ErrorSnackbar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
                    ErrorSnackbar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
                    ShowErrorMessage($"There was an error updating {activeSection.Name}");
                });
            }
        }

        private void ChangeSectionColors()
        {
            foreach (LiveSectionItem liveSectionItem in SectionsStackPanel.Children)
            {
                liveSectionItem.ChangeColor(liveSectionItem.SectionID == activeSection.ID);
            }
        }

        private void ChangeSectionStatus(int sectionID, bool status)
        {
            foreach (LiveSectionItem liveSectionItem in SectionsStackPanel.Children)
            {
                if (liveSectionItem.SectionID == sectionID)
                {
                    liveSectionItem.ChangeStatus(status);
                }
            }
        }

        private Section GetSection(int id) => sections.Find(x => x.ID == id);

        private void AddLiveSection_Click(object sender, RoutedEventArgs e)
        {
            AddNewSectionStatusGrid.Visibility = Visibility.Visible;

            PostNewSection(AddLiveSectionNameTextBox.Text);
            GetAllSectionsAsync();

            AddLiveSectionNameTextBox.Text = string.Empty;
        }

        private async void PostNewSection(string sectionName)
        {
            var response = await client.PostAsJsonAsync($"/api/Section", new Section { Date = DateTime.Now, Name = sectionName }).ConfigureAwait(false);
            var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string resultString = result.GetAwaiter().GetResult();
            if (resultString.Equals("200"))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                    ErrorSnackbar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
                    ErrorSnackbar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
                    ShowErrorMessage($"{sectionName} was added succesfully");
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                    ErrorSnackbar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
                    ErrorSnackbar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
                    ShowErrorMessage($"Couldn't add {sectionName}");
                });
            }
        }

        private void ShowErrorMessage(string message, double time = 3)
        {
            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        /* public void InitChannels()
         {
             ChannelsStackPanel.Children.Clear();

             foreach (var item in InputFileManager.InputFiles.First().Channels)
             {
                 ChannelsStackPanel.Children.Add(new CheckBox()
                 {
                     Content = item.Name
                 });
             }
         }

         private void SendChannelsBtn_Click(object sender, RoutedEventArgs e)
         {
             var inputFile = InputFileManager.InputFiles.First();
             var selectedChannels = new List<Channel>();

             foreach (CheckBox item in ChannelsStackPanel.Children)
             {
                 if (item.IsChecked == true)
                 {
                     selectedChannels.Add(inputFile.GetChannel(item.Content.ToString()));
                 }
             }

             foreach (var channel in selectedChannels)
             {
                 try
                 {
                     var uri = PostChannel(channel).Result;
                     return;
                 }
                 catch (Exception)
                 {
                     return;
                     throw;
                 }
             }
         }

         private HttpContent ConvertToHttpContent(Channel channel)
         {
             var myContent = JsonConvert.SerializeObject(channel);
             var buffer = Encoding.UTF8.GetBytes(myContent);
             var byteContent = new ByteArrayContent(buffer);
             byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

             return byteContent;
         }

         private async Task<Uri> PostChannel(Channel channel)
         {
             var client = new HttpClient
             {
                 Timeout = TimeSpan.FromMinutes(1),
                 BaseAddress = new Uri(url)
             };
             client.DefaultRequestHeaders.Accept.Clear();
             client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
             var response = await client.PostAsync("/api/Channels", ConvertToHttpContent(channel)).ConfigureAwait(false);
             response.EnsureSuccessStatusCode();
             client.Dispose();
             return response.Headers.Location;
         }

         /*    private async Task<Channel> GetChannel(string name)
             {
                 var result = new Channel(name);

                 var client = new HttpClient
                 {
                     Timeout = TimeSpan.FromMinutes(1),
                     BaseAddress = new Uri("https://localhost:44332/")
                 };
                 client.DefaultRequestHeaders.Accept.Clear();
                 client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                 var response = await client.GetAsync("/api/Channels/?name=" + name).ConfigureAwait(false);
                 if (response.IsSuccessStatusCode)
                 {
                     var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                     result = JsonConvert.DeserializeObject<Channel>(resultString);
                 }
                 client.Dispose();
                 return result;
             }*/










        /*   private void NewLiveSectionButton_Click(object sender, RoutedEventArgs e)
      {
          PostNewSection();
      }

    */

        /*private async Task<Uri> PostNewSectionAsync(string sectionName)
        {
            var response = await client.PostAsJsonAsync("/api/Section", sectionName).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    ResultIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Approve;
                }
                else if (resultString.Equals("500"))
                {
                    ResultIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Denied;
                }
            }

            return response.Headers.Location;
        }*/
    }

}
