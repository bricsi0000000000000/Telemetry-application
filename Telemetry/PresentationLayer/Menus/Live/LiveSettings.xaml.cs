using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using PresentationLayer.Menus.Live;
using DataLayer.Models;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using LocigLayer.Colors;
using LogicLayer.Configurations;
using PresentationLayer.Extensions;
using PresentationLayer.ValidationRules;
using LocigLayer.InputFiles;
using DataLayer.InputFiles;

namespace PresentationLayer.Menus.Settings.Live
{
    public partial class LiveSettings : UserControl
    {
        private static HttpClient client = new HttpClient();
        private List<Section> sections = new List<Section>();
        private Section activeSection;
        private bool sectionSelected = false;

        private readonly FieldsViewModel fieldsViewModel = new FieldsViewModel();

        public LiveSettings()
        {
            InitializeComponent();

            fieldsViewModel.SectionName = "a";
            fieldsViewModel.SectionDate = "a";
            DataContext = fieldsViewModel;

            InitilaizeHttpClient();
            UpdateSelectedSectionButtons();
            UpdateCarStatus();
            UpdateConfigurationCard();
        }

        private void InitilaizeHttpClient()
        {
            //  ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri(Configurations.Address)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region async methods
        private async void GetAllSectionsAsync(int? selectedSectionID = null)
        {
            try
            {
                var response = await client.GetAsync(Configurations.AllLiveSectionsAPICall).ConfigureAwait(false);
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
                            int selectSectionID;
                            if (selectedSectionID == null || !sections.FindAll(x => x.ID == (int)selectedSectionID).Any())
                            {
                                selectSectionID = sections.First().ID;
                            }
                            else
                            {
                                selectSectionID = (int)selectedSectionID;
                            }

                            SelectSection(selectSectionID, firstTime: true);
                        }
                    }
                    catch (Exception)
                    {
                        ShowSnackbarMessage("An error occurred while getting the sections");
                    }
                });

            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowSnackbarMessage("Couldn't connect to the sever");
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateLoadingGrid(visibility: false);
            });
        }

        private async void PostNewSectionAsync(string sectionName)
        {
            try
            {
                var response = await client.PostAsJsonAsync(Configurations.PostNewSectionAPICall, new Section { Date = DateTime.Now, Name = sectionName }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GetAllSectionsAsync();
                        UpdateLoadingGrid(visibility: false);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage($"Couldn't add {sectionName}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage($"Can't add {sectionName} because can't connect to the server");
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="change">True if the status should changed, false if not.</param>
        public async void ChangeStatusResultAsync(bool change)
        {
            UpdateLoadingGrid(visibility: false);

            if (!change)
            {
                return;
            }

            try
            {
                HttpResponseMessage response;

                if (activeSection.IsLive) // change to offline
                {
                    response = await client.PutAsJsonAsync(Configurations.ChangeSectionToOfflineAPICall, activeSection.ID).ConfigureAwait(false);
                }
                else //change to live
                {
                    response = await client.PutAsJsonAsync(Configurations.ChangeSectionToLiveAPICall, activeSection.ID).ConfigureAwait(false);
                }

                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        activeSection.IsLive = !activeSection.IsLive;
                        ChangeSectionStatus(activeSection.ID, activeSection.IsLive);
                        UpdateLoadingGrid(visibility: false);
                        SelectedSectionStatusIcon.Kind = activeSection.IsLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
                        SelectedSectionStatusIcon.Foreground = activeSection.IsLive ? ColorManager.Secondary900.ConvertBrush() :
                                                                                      ColorManager.Primary900.ConvertBrush();
                    });
                }
                else if (resultCode == (int)HttpStatusCode.Conflict)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage($"Only one live section can be at a time!");
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage($"Couldn't update {activeSection.Name}'s status from " +
                                            $"{(activeSection.IsLive ? "live" : "offline")} to " +
                                            $"{(!activeSection.IsLive ? "live" : "offline")}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage($"There was an error updating {activeSection.Name}");
                });
            }
        }

        private async void DeleteSectionAsync(int id)
        {
            UpdateLoadingGrid(visibility: true, "Deleting section..");

            try
            {
                var response = await client.DeleteAsync($"{Configurations.DeleteSectionAPICall}?sectionID={id}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        GetAllSectionsAsync();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("Couldn't delete section");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage("Can't delete section because can't connect to the server");
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">New name</param>
        private async void ChangeSectionNameAsync(int id, string name)
        {
            UpdateLoadingGrid(visibility: true, "Updating section..");

            try
            {
                var response = await client.PutAsJsonAsync(Configurations.ChangeSectionNameAPICall, new Section() { ID = id, Name = name }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        GetAllSectionsAsync(selectedSectionID: activeSection.ID);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("Couldn't update section");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage("Can't update section because can't connect to the server");
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">New name</param>
        private async void ChangeSectionDateAsync(int id, DateTime newDate)
        {
            UpdateLoadingGrid(visibility: true, "Updating section..");

            try
            {
                var response = await client.PutAsJsonAsync(Configurations.ChangeSectionDateAPICall, new Section() { ID = id, Name = "a", Date = newDate }).ConfigureAwait(false); // it doesn't work without Name="a", because name is required in api side
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        GetAllSectionsAsync(selectedSectionID: activeSection.ID);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("Couldn't update section");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage("Can't update section because can't connect to the server");
                });
            }
        }

        #endregion

        private bool FillSectionsStackPanel(List<Section> sections)
        {
            SectionsCoverGridGrid.Visibility = Visibility.Visible;
            SectionsStackPanel.Children.Clear();

            if (!sections.Any())
            {
                ShowSnackbarMessage("There are no sections on the server", error: false);
                return false;
            }

            sections.Reverse();
            activeSection = sections.First();
            foreach (var section in sections)
            {
                SectionsStackPanel.Children.Add(new LiveSectionItem(section));
            }

            SectionsCoverGridGrid.Visibility = Visibility.Hidden;

            return true;
        }

        /// <summary>
        /// Calls if a section is selected.
        /// </summary>
        /// <param name="sectionID">Selected sections ID.</param>
        /// <param name="firstTime">If true, it will update the section in <see cref="LiveTelemetry"/>, no matter what.</param>
        public void SelectSection(int sectionID, bool firstTime = false)
        {
            if (activeSection.ID != sectionID || firstTime)
            {
                activeSection = GetSection(sectionID);
                ChangeSectionColors();
                var channelNames = activeSection.SensorNames.Split(';').ToList();
                MenuManager.LiveTelemetry.UpdateSection(activeSection, channelNames);
                InputFileManager.AddLive(activeSection.Name, channelNames);
                UpdateSelectedSectionInfo(channelNames);
                SelectedSectionStatusIcon.Kind = activeSection.IsLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
                SelectedSectionStatusIcon.Foreground = activeSection.IsLive ? ColorManager.Secondary900.ConvertBrush() :
                                                                              ColorManager.Primary900.ConvertBrush();
            }
        }

        private void UpdateSelectedSectionInfo(List<string> channelNames)
        {
            if (activeSection != null)
            {
                SectionDataGridCover.Visibility = Visibility.Hidden;
                sectionSelected = true;

                fieldsViewModel.SectionName = activeSection.Name;
                fieldsViewModel.SectionDate = activeSection.Date.ToString();
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
                SectionDataGridCover.Visibility = Visibility.Visible;
                sectionSelected = false;
            }

            UpdateSelectedSectionButtons();
        }

        private void UpdateSelectedSectionButtons()
        {
            ChangeSectionStatusCardButton.Foreground = sectionSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                                         ColorManager.Secondary400.ConvertBrush();
            DeleteSectionCardButton.Background = sectionSelected ? ColorManager.Primary900.ConvertBrush() :
                                                                   ColorManager.Primary200.ConvertBrush();
        }

        public void ChangeStatus(int id)
        {
            SelectSection(id);

            UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

            var changeLiveStatusWindow = new PopUpWindow($"You are about to change {activeSection.Name}'s status from " +
                                                         $"{(activeSection.IsLive ? "live" : "offline")} to " +
                                                         $"{(activeSection.IsLive ? "offline" : "live")}\n" +
                                                         $"Are you sure about that?",
                                                         PopUpWindow.PopUpType.ChangeLiveStatus);
            changeLiveStatusWindow.ShowDialog();
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
            UpdateLoadingGrid(visibility: true, "Adding new section..");

            PostNewSectionAsync(AddLiveSectionNameTextBox.Text);

            AddLiveSectionNameTextBox.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error">If true, it's an error message, if not, it's a regular one.</param>
        /// <param name="time"></param>
        private void ShowSnackbarMessage(string message, bool error = true, double time = 3)
        {
            ErrorSnackbar.Foreground = error ? ColorManager.Primary500.ConvertBrush() :
                                               ColorManager.Secondary50.ConvertBrush();

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        private void UpdateLoadingGrid(bool visibility, string message = "", bool progressBarVisibility = true, bool cancelButtonVisibility = false)
        {
            LoadingGrid.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            LoadingLabel.Content = message;
            LoadingProgressBar.Visibility = progressBarVisibility ? Visibility.Visible : Visibility.Hidden;
            LoadingCancelButtonCard.Visibility = cancelButtonVisibility ? Visibility.Visible : Visibility.Hidden;
        }

        private void RefreshSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSectionsButton.Background = ColorManager.Secondary100.ConvertBrush();
            UpdateLoadingGrid(visibility: true, "Loading sections..");

            int? selectedSectionID = null;

            if (activeSection != null)
            {
                selectedSectionID = activeSection.ID;
            }

            GetAllSectionsAsync(selectedSectionID: selectedSectionID);
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ColorManager.Primary700.ConvertBrush();
            }
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ColorManager.Primary800.ConvertBrush();

                UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

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
                DeleteSectionCardButton.Background = ColorManager.Primary800.ConvertBrush();
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void DeleteSectionCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                DeleteSectionCardButton.Background = ColorManager.Primary900.ConvertBrush();
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
                UpdateLoadingGrid(visibility: false);
            }
        }

        public void ChangeName(string newName)
        {
            ChangeSectionNameAsync(activeSection.ID, newName);
        }

        public void ChangeDate(DateTime newDate)
        {
            ChangeSectionDateAsync(activeSection.ID, newDate);
        }

        /// <param name="sentTime">Time when the package was sent from the data sender</param>
        /// <param name="arrivedTime">Time when the package was sent from the server</param>
        public void UpdateCarStatus(TimeSpan? sentTime = null, long? arrivedTime = null)
        {
            if (sentTime != null)
            {
                CarToDBConnectionSpeedLabel.Content = $"{sentTime.Value.Milliseconds:f0} ms";
            }
            else
            {
                CarToDBConnectionSpeedLabel.Content = "-";
            }

            if (arrivedTime != null)
            {
                DBToAppConnectionSpeedLabel.Content = $"{(long)arrivedTime:f0} ms";
            }
            else
            {
                DBToAppConnectionSpeedLabel.Content = "-";
            }
        }

        public void UpdateConfigurationCard()
        {
            URLLabel.Content = Configurations.URL;
            PortLabel.Content = Configurations.Port.ToString();
        }

        #region cards

        private void ChangeSectionStatusCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ColorManager.Secondary200.ConvertBrush();
            }
        }

        private void ChangeSectionStatusCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ColorManager.Secondary100.ConvertBrush();

                ChangeStatus(activeSection.ID);
            }
        }

        private void ChangeSectionStatusCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ColorManager.Secondary100.ConvertBrush();
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void ChangeSectionStatusCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sectionSelected)
            {
                ChangeSectionStatusCardButton.Background = ColorManager.Secondary50.ConvertBrush();
                Mouse.OverrideCursor = null;
            }
        }


        private void ChangeSectionNameCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ChangeSectionNameCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

            string newName = SelectedSectionNameTextBox.Text;
            if (!newName.Equals(string.Empty) && !newName.Equals(activeSection.Name))
            {
                MenuManager.LiveSettings.ChangeName(newName);
            }
        }

        private void ChangeSectionNameCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSectionNameCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionNameCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void ChangeSectionDateCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ChangeSectionDateCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

            string newDateText = SelectedSectionDateLabel.Text;
            if (!newDateText.Equals(string.Empty))
            {
                DateTime newDate = Convert.ToDateTime(newDateText);
                if (newDate != activeSection.Date)
                {
                    MenuManager.LiveSettings.ChangeDate(newDate);
                }
            }
        }

        private void ChangeSectionDateCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSectionDateCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSectionDateCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void LoadingCancelButtonCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadingCancelButtonCard.Background = ColorManager.Primary700.ConvertBrush();
        }

        private void LoadingCancelButtonCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadingCancelButtonCard.Background = ColorManager.Primary800.ConvertBrush();
        }

        private void LoadingCancelButtonCard_MouseEnter(object sender, MouseEventArgs e)
        {
            LoadingCancelButtonCard.Background = ColorManager.Primary800.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void LoadingCancelButtonCard_MouseLeave(object sender, MouseEventArgs e)
        {
            LoadingCancelButtonCard.Background = ColorManager.Primary900.ConvertBrush();
            Mouse.OverrideCursor = null;
        }
        #endregion
    }
}
