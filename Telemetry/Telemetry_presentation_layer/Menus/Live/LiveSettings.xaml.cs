using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_presentation_layer.Menus.Live;
using Telemetry_data_and_logic_layer.Models;
using System.Windows.Controls;
using System.Windows;
using Telemetry_data_and_logic_layer.Texts;

namespace Telemetry_presentation_layer.Menus.Settings.Live
{
    /// <summary>
    /// Interaction logic for LiveSettings.xaml
    /// </summary>
    public partial class LiveSettings : UserControl
    {
        private static HttpClient client = new HttpClient();
        private List<Section> sections = new List<Section>();
        private Section activeSection;

        public LiveSettings()
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
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));
        }

        private void RefreshSectionsButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
        }

        private void RefreshSectionsButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));
        }

        private void RefreshSectionsButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));

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
            if (sections.Count <= 0)
            {
                ShowErrorMessage("There are no sections on the server", error: false);
                return;
            }
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
                ((LiveTelemetry)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.LiveMenuName).Content).UpdateSection(activeSection);
            }
        }

        public void ChangeStatus(int id)
        {
            SelectSection(id);

            var changeLiveStatusWindow = new ChangeLiveStatusWindow($"You are about to change {activeSection.Name}'s status from " +
                                                                    $"{(activeSection.IsLive ? "live" : "offline")} to " +
                                                                    $"{(activeSection.IsLive ? "offline" : "live")}\n" +
                                                                    $"Are you sure about that?");
            changeLiveStatusWindow.ShowDialog();
            /*SectionDialogHost.ShowDialog(
                    new ChangeIsLiveStatusDialogContent(
                        $"You are about to change {activeSection.Name}'s status from " +
                        $"{(activeSection.IsLive ? "live" : "offline")} to {(activeSection.IsLive ? "offline" : "live")}\n" +
                        $"Are you sure about that?"));*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="change">True if the status should changed, false if not.</param>
        public async void ChangeStatusResult(bool change)
        {
            if (!change)
            {
                return;
            }

            var updatedSection = new Section
            {
                ID = activeSection.ID,
                Date = activeSection.Date,
                Name = activeSection.Name,
                IsLive = !activeSection.IsLive
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
                        activeSection.IsLive = updatedSection.IsLive;
                        ChangeSectionStatus(activeSection.ID, activeSection.IsLive);
                        AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                        ShowErrorMessage($"{activeSection.Name}'s status was modified from " +
                                         $"{(!activeSection.IsLive ? "live" : "offline")} to " +
                                         $"{(activeSection.IsLive ? "live" : "offline")}", error: false);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                        ShowErrorMessage($"Couldn't update {activeSection.Name}'s status from " +
                                         $"{(activeSection.IsLive ? "live" : "offline")} to " +
                                         $"{(!activeSection.IsLive ? "live" : "offline")}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
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
            try
            {
                var response = await client.PostAsJsonAsync($"/api/Section", new Section { Date = DateTime.Now, Name = sectionName }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                        ShowErrorMessage($"{sectionName} was added succesfully", error: false);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                        ShowErrorMessage($"Couldn't add {sectionName}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddNewSectionStatusGrid.Visibility = Visibility.Hidden;
                    ShowErrorMessage($"Can't add {sectionName} because can't connect to the server");
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error">If true, it's an error message, if not, it's a regular one.</param>
        /// <param name="time"></param>
        private void ShowErrorMessage(string message, bool error = true, double time = 3)
        {
            ErrorSnackbar.Background = error ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900)) :
                                               new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }
    }
}
