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
using System.Threading.Tasks;
using Telemetry_presentation_layer.Converters;
using System.Windows.Input;
using System.Diagnostics;
using System.Linq;

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
        private bool sectionSelected = false;

        public LiveSettings()
        {
            InitializeComponent();

            InitilaizeHttpClient();
            UpdateSelectedSectionButtons();
            UpdateCarStatus(new List<TimeSpan>(), -1);
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
                    try
                    {
                        if (FillSectionsStackPanel(sections))
                        {
                            ChangeSectionColors();
                            SelectSection(activeSection.ID, firstTime: true);
                        }
                    }
                    catch (Exception)
                    {
                        ShowErrorMessage("An error occured while getting the sections");
                    }
                });

            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowErrorMessage("Couldn't connect to the sever");
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                SetLoadingGrid(visibility: false);
            });
        }

        private bool FillSectionsStackPanel(List<Section> sections)
        {
            SectionsStackPanel.Children.Clear();
            NoSectionsGrid.Visibility = Visibility.Visible;

            if (sections.Count <= 0)
            {
                ShowErrorMessage("There are no sections on the server", error: false);
                return false;
            }

            NoSectionsGrid.Visibility = Visibility.Hidden;

            sections.Reverse();
            activeSection = sections[0];
            foreach (var section in sections)
            {
                SectionsStackPanel.Children.Add(new LiveSectionItem(section));
            }

            return true;
        }

        /// <summary>
        /// Calls if a section is selected.
        /// </summary>
        /// <param name="id">Selected sections ID.</param>
        /// <param name="firstTime">If true, it will update the section in <see cref="LiveTelemetry"/>, no matter what.</param>
        public void SelectSection(int id, bool firstTime = false)
        {
            if (activeSection.ID != id || firstTime)
            {
                activeSection = GetSection(id);
                ChangeSectionColors();
                var channelNames = GetActiveChannelsAsync(id).Result;
                ((LiveTelemetry)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.LiveMenuName).Content).UpdateSection(activeSection, channelNames);
                UpdateSelectedSectionInfo(channelNames);
                SelectedSectionStatusIcon.Kind = activeSection.IsLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
                SelectedSectionStatusIcon.Foreground = activeSection.IsLive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                                              ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            }
        }

        private void UpdateSelectedSectionInfo(List<string> channelNames)
        {
            if (activeSection != null)
            {
                NoActiveSectionGrid.Visibility = Visibility.Hidden;
                sectionSelected = true;

                SelectedSectionNameTextBox.Text = activeSection.Name;
                SelectedSectionDateLabel.Text = activeSection.Date.ToString();
                SelectedSectionChannelsStackPanel.Children.Clear();

                NoChannelsGrid.Visibility = channelNames.Count == 0 ? Visibility.Visible : Visibility.Hidden;

                SelectedSectionChannelsCountTextBox.Text = $"({channelNames.Count})";

                foreach (var item in channelNames)
                {
                    SelectedSectionChannelsStackPanel.Children.Add(new Label() { Content = item });
                }
            }
            else
            {
                NoActiveSectionGrid.Visibility = Visibility.Visible;
                sectionSelected = false;
            }

            UpdateSelectedSectionButtons();
        }

        private void UpdateSelectedSectionButtons()
        {
            ChangeSectionStatusCardButton.Foreground = sectionSelected ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                                         ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary400);
            DeleteSectionCardButton.Background = sectionSelected ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900) :
                                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary200);
        }

        private async Task<List<string>> GetActiveChannelsAsync(int sectionID)
        {
            var channelNames = new List<string>();

            try
            {
                var response = await client.GetAsync($"/api/Section/channel_names?sectionID={sectionID}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                dynamic data = JsonConvert.DeserializeObject(resultString);
                for (int i = 0; i < data.Count; i++)
                {
                    var a = data[i];
                    channelNames.Add(data[i].ToString());
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowErrorMessage("Couldn't connect to the sever");
                });
            }

            return channelNames;
        }

        public void ChangeStatus(int id)
        {
            SelectSection(id);

            SetLoadingGrid(visibility: true, progressBarVisibility: false);

            var changeLiveStatusWindow = new PopUpWindow($"You are about to change {activeSection.Name}'s status from " +
                                                         $"{(activeSection.IsLive ? "live" : "offline")} to " +
                                                         $"{(activeSection.IsLive ? "offline" : "live")}\n" +
                                                         $"Are you sure about that?",
                                                         PopUpWindow.PopUpType.ChangeLiveStatus);
            changeLiveStatusWindow.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="change">True if the status should changed, false if not.</param>
        public async void ChangeStatusResultAsync(bool change)
        {
            SetLoadingGrid(visibility: false);

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
                        SetLoadingGrid(visibility: false);
                        SelectedSectionStatusIcon.Kind = activeSection.IsLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
                        SelectedSectionStatusIcon.Foreground = activeSection.IsLive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                                                      ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
                    });
                }
                else if (resultString.Equals("409"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        ShowErrorMessage($"Only one live section can be at the time!");
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
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
                    SetLoadingGrid(visibility: false);
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
            SetLoadingGrid(visibility: true, "Adding new section..");

            PostNewSectionAsync(AddLiveSectionNameTextBox.Text);

            AddLiveSectionNameTextBox.Text = string.Empty;
        }

        private async void PostNewSectionAsync(string sectionName)
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
                        GetAllSectionsAsync();
                        SetLoadingGrid(visibility: false);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        ShowErrorMessage($"Couldn't add {sectionName}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetLoadingGrid(visibility: false);
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
            ErrorSnackbar.Foreground = error ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary500)) :
                                               new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        private void SetLoadingGrid(bool visibility, string message = "", bool progressBarVisibility = true, bool cancelButtonVisibility = false)
        {
            LoadingGrid.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            LoadingLabel.Content = message;
            LoadingProgressBar.Visibility = progressBarVisibility ? Visibility.Visible : Visibility.Hidden;
            LoadingCancelButtonCard.Visibility = cancelButtonVisibility ? Visibility.Visible : Visibility.Hidden;
        }

        private void RefreshSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));
            SetLoadingGrid(visibility: true, "Loading sections..");
            GetAllSectionsAsync();
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary700);
            }
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);

                SetLoadingGrid(visibility: true, progressBarVisibility: false);

                var deleteSectionWindow = new PopUpWindow($"You are about to delete {activeSection.Name}\n" +
                                                          $"Are you sure about that?",
                                                          PopUpWindow.PopUpType.DeleteSection);
                deleteSectionWindow.ShowDialog();
            }
        }

        private void DeleteSectionCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void DeleteSectionCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
                Mouse.OverrideCursor = null;
            }
        }

        public void DeleteSeciton(bool delete)
        {
            if (delete)
            {
                DeleteSectionAsync(activeSection.ID);
            }
            else
            {
                SetLoadingGrid(visibility: false);
            }
        }

        private async void DeleteSectionAsync(int id)
        {
            SetLoadingGrid(visibility: true, "Deleting section..");

            try
            {
                var response = await client.DeleteAsync($"/api/Section/{id}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        GetAllSectionsAsync();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        ShowErrorMessage("Couldn't delete section");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetLoadingGrid(visibility: false);
                    ShowErrorMessage("Can't delete section because can't connect to the server");
                });
            }
        }

        public void ChangeName(bool change, string newName = "")
        {
            if (change)
            {
                ChangeSectionNameAsync(activeSection.ID, newName);
            }
            else
            {
                SetLoadingGrid(visibility: false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">New name</param>
        private async void ChangeSectionNameAsync(int id, string name)
        {
            SetLoadingGrid(visibility: true, "Updating section..");

            try
            {
                var response = await client.PutAsJsonAsync("/api/Section/change_name", new Section() { ID = id, Name = name }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        GetAllSectionsAsync();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        ShowErrorMessage("Couldn't update section");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetLoadingGrid(visibility: false);
                    ShowErrorMessage("Can't update section because can't connect to the server");
                });
            }
        }

        public void ChangeDate(bool change, DateTime newDate = new DateTime())
        {
            if (change)
            {
                ChangeSectionDateAsync(activeSection.ID, newDate);
            }
            else
            {
                SetLoadingGrid(visibility: false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">New name</param>
        private async void ChangeSectionDateAsync(int id, DateTime newDate)
        {
            SetLoadingGrid(visibility: true, "Updating section..");

            try
            {
                var response = await client.PutAsJsonAsync("/api/Section/change_date", new Section() { ID = id, Date = newDate }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        GetAllSectionsAsync();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetLoadingGrid(visibility: false);
                        ShowErrorMessage("Couldn't update section");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetLoadingGrid(visibility: false);
                    ShowErrorMessage("Can't update section because can't connect to the server");
                });
            }
        }

        public void UpdateCarStatus(List<TimeSpan> carTimes, long appTimes)
        {
            if (carTimes.Count > 0)
            {
                var timesTicks = new List<long>();
                foreach (var item in carTimes)
                {
                    timesTicks.Add(item.Ticks);
                }
                var time = new TimeSpan((long)timesTicks.Average());
                CarToDBConnectionSpeedLabel.Content = $"{time.Milliseconds:f0} ms";
            }
            else
            {
                CarToDBConnectionSpeedLabel.Content = "-";
            }

            if (appTimes >= 0)
            {
                DBToAppConnectionSpeedLabel.Content = $"{appTimes:f0} ms";
            }
            else
            {
                DBToAppConnectionSpeedLabel.Content = "-";
            }
        }

        #region cards

        private void ChangeSectionStatusCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
            }
        }

        private void ChangeSectionStatusCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

                ChangeStatus(activeSection.ID);
            }
        }

        private void ChangeSectionStatusCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void ChangeSectionStatusCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
                Mouse.OverrideCursor = null;
            }
        }


        private void ChangeSectionNameCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSectionNameCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            SetLoadingGrid(visibility: true, progressBarVisibility: false);

            var changeNameWindow = new PopUpEditWindow("Change name", PopUpEditWindow.EditType.ChangeSectionName);
            changeNameWindow.ShowDialog();
        }

        private void ChangeSectionNameCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSectionNameCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void ChangeSectionDateCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSectionDateCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            SetLoadingGrid(visibility: true, progressBarVisibility: false);

            var changeDateWindow = new PopUpEditDateWindow("Change date");
            changeDateWindow.ShowDialog();
        }

        private void ChangeSectionDateCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSectionDateCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void LoadingCancelButtonCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadingCancelButtonCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary700);
        }

        private void LoadingCancelButtonCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadingCancelButtonCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
        }

        private void LoadingCancelButtonCard_MouseEnter(object sender, MouseEventArgs e)
        {
            LoadingCancelButtonCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void LoadingCancelButtonCard_MouseLeave(object sender, MouseEventArgs e)
        {
            LoadingCancelButtonCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            Mouse.OverrideCursor = null;
        }
        #endregion
    }
}
