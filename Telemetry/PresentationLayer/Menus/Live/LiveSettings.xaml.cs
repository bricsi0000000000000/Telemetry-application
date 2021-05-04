using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using LogicLayer.Menus.Live;
using DataLayer.Models;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using LogicLayer.Colors;
using LogicLayer.ValidationRules;
using PresentationLayer.InputFiles;
using LogicLayer.Configurations;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Settings.Live
{
    public partial class LiveSettings : UserControl
    {
        private static HttpClient client = new HttpClient();
        private List<Session> sessions = new List<Session>();
        private Session activeSession;
        private bool sessionSelected = false;

        private readonly FieldsViewModel fieldsViewModel = new FieldsViewModel();

        public LiveSettings()
        {
            InitializeComponent();

            fieldsViewModel.SessionName = "a"; // it is necessarry because if I don't give it any value, the error message will be visible through the cover grid
            fieldsViewModel.SessionDate = "a"; // it is necessarry because if I don't give it any value, the error message will be visible through the cover grid
            DataContext = fieldsViewModel;

            InitilaizeHttpClient();
            UpdateSelectedSessionButtons();
            UpdateCarStatus();
            UpdateConfigurationCard();
        }

        private void InitilaizeHttpClient()
        {
            //  ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri(ConfigurationManager.Address)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region async methods
        private async void GetAllSessionsAsync(int? selectedSessionID = null)
        {
            try
            {
                var response = await client.GetAsync(ConfigurationManager.AllLiveSessionsAPICall).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                sessions = JsonConvert.DeserializeObject<List<Session>>(resultString);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (FillSessionsStackPanel(sessions))
                        {
                            ChangeSessionColors();
                            int selectSessionID;
                            if (selectedSessionID == null || !sessions.FindAll(x => x.ID == (int)selectedSessionID).Any())
                            {
                                selectSessionID = sessions.First().ID;
                            }
                            else
                            {
                                selectSessionID = (int)selectedSessionID;
                            }

                            SelectSession(selectSessionID, firstTime: true);
                        }
                    }
                    catch (Exception)
                    {
                        ShowSnackbarMessage("An error occurred while getting the sessions");
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

        private async void PostNewSessionAsync(string sessionName)
        {
            try
            {
                var response = await client.PostAsJsonAsync(ConfigurationManager.PostNewSessionAPICall, new Session { Date = DateTime.Now, Name = sessionName }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GetAllSessionsAsync();
                        UpdateLoadingGrid(visibility: false);
                    });
                }
                else if (resultCode == (int)HttpStatusCode.Conflict)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage($"There is already a session called {sessionName}");
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("There was a problem with the server");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage($"Can't add {sessionName} because can't connect to the server");
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

                if (activeSession.IsLive) // change to offline
                {
                    response = await client.PutAsJsonAsync(ConfigurationManager.ChangeSessionToOfflineAPICall, activeSession.ID).ConfigureAwait(false);
                }
                else //change to live
                {
                    response = await client.PutAsJsonAsync(ConfigurationManager.ChangeSessionToLiveAPICall, activeSession.ID).ConfigureAwait(false);
                }

                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        activeSession.IsLive = !activeSession.IsLive;
                        MenuManager.LiveTelemetry.ChangeSessionStatusIconState(activeSession.IsLive);
                        ChangeSessionStatus(activeSession.ID, activeSession.IsLive);
                        UpdateLoadingGrid(visibility: false);
                        SelectedSessionStatusIcon.Kind = activeSession.IsLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
                        SelectedSessionStatusIcon.Foreground = activeSession.IsLive ? ColorManager.Secondary900.ConvertBrush() :
                                                                                      ColorManager.Primary900.ConvertBrush();
                    });
                }
                else if (resultCode == (int)HttpStatusCode.Conflict)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage($"Only one live session can be at a time!");
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage($"Couldn't update {activeSession.Name}'s status from " +
                                            $"{(activeSession.IsLive ? "live" : "offline")} to " +
                                            $"{(!activeSession.IsLive ? "live" : "offline")}");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage($"There was an error updating {activeSession.Name}");
                });
            }
        }

        private async void DeleteSessionAsync(int id)
        {
            UpdateLoadingGrid(visibility: true, "Deleting session..");

            try
            {
                var response = await client.DeleteAsync($"{ConfigurationManager.DeleteSessionAPICall}?sessionID={id}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        GetAllSessionsAsync();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("Couldn't delete session");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage("Can't delete session because can't connect to the server");
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">New name</param>
        private async void ChangeSessionNameAsync(int id, string name)
        {
            UpdateLoadingGrid(visibility: true, "Updating session..");

            try
            {
                var response = await client.PutAsJsonAsync(ConfigurationManager.ChangeSessionNameAPICall, new Session() { ID = id, Name = name }).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        GetAllSessionsAsync(selectedSessionID: activeSession.ID);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("Couldn't update session");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage("Can't update session because can't connect to the server");
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">New name</param>
        private async void ChangeSessionDateAsync(int id, DateTime newDate)
        {
            UpdateLoadingGrid(visibility: true, "Updating session..");

            try
            {
                var response = await client.PutAsJsonAsync(ConfigurationManager.ChangeSessionDateAPICall, new Session() { ID = id, Name = "a", Date = newDate }).ConfigureAwait(false); // it doesn't work without Name="a", because name is required in api side
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                int resultCode = int.Parse(result.GetAwaiter().GetResult());
                if (resultCode == (int)HttpStatusCode.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        GetAllSessionsAsync(selectedSessionID: activeSession.ID);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLoadingGrid(visibility: false);
                        ShowSnackbarMessage("Couldn't update session");
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateLoadingGrid(visibility: false);
                    ShowSnackbarMessage("Can't update session because can't connect to the server");
                });
            }
        }

        #endregion

        private bool FillSessionsStackPanel(List<Session> sessions)
        {
            SessionsCoverGridGrid.Visibility = Visibility.Visible;
            SessionDataGridCover.Visibility = Visibility.Visible;
            SessionsStackPanel.Children.Clear();

            if (!sessions.Any())
            {
                MenuManager.LiveTelemetry.IsSelectedSession = false;
                ShowSnackbarMessage("There are no sessions on the server", error: false);
                return false;
            }

            sessions.Reverse();
            activeSession = sessions.First();
            foreach (var session in sessions)
            {
                SessionsStackPanel.Children.Add(new LiveSessionItem(session));
            }

            SessionsCoverGridGrid.Visibility = Visibility.Hidden;
            SessionDataGridCover.Visibility = Visibility.Hidden;

            MenuManager.LiveTelemetry.ResetCharts();

            return true;
        }

        /// <summary>
        /// Calls if a session is selected.
        /// </summary>
        /// <param name="sessionID">Selected sessions ID.</param>
        /// <param name="firstTime">If true, it will update the session in <see cref="LiveTelemetry"/>, no matter what.</param>
        public void SelectSession(int sessionID, bool firstTime = false)
        {
            if (activeSession.ID != sessionID || firstTime)
            {
                activeSession = GetSession(sessionID);
                ChangeSessionColors();
                var channelNames = activeSession.SensorNames.Split(';').ToList();
                MenuManager.LiveTelemetry.UpdateSession(activeSession, channelNames);
                InputFileManager.AddLive(activeSession.ID, activeSession.Name, channelNames);
                UpdateSelectedSessionInfo(channelNames);
                SelectedSessionStatusIcon.Kind = activeSession.IsLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
                SelectedSessionStatusIcon.Foreground = activeSession.IsLive ? ColorManager.Secondary900.ConvertBrush() :
                                                                              ColorManager.Primary900.ConvertBrush();
            }
        }

        private void UpdateSelectedSessionInfo(List<string> channelNames)
        {
            if (activeSession != null)
            {
                SessionDataGridCover.Visibility = Visibility.Hidden;
                sessionSelected = true;

                fieldsViewModel.SessionName = activeSession.Name;
                fieldsViewModel.SessionDate = activeSession.Date.ToString();
                SelectedSessionChannelsStackPanel.Children.Clear();

                ChannelsCoverGrid.Visibility = channelNames.Count == 0 ? Visibility.Visible : Visibility.Hidden;

                SelectedSessionChannelsCountTextBox.Text = $"({channelNames.Count})";

                foreach (var item in channelNames)
                {
                    SelectedSessionChannelsStackPanel.Children.Add(new Label() { Content = item });
                }
            }
            else
            {
                SessionDataGridCover.Visibility = Visibility.Visible;
                sessionSelected = false;
            }

            UpdateSelectedSessionButtons();
            ChangeLoadedPackagesLabel();
        }

        public void ChangeLoadedPackagesLabel(int packagesCount = 0)
        {
            SelectedSessionPackagesCountTextBox.Text = $"Downloaded packages: {packagesCount}";
        }

        private void UpdateSelectedSessionButtons()
        {
            ChangeSessionStatusCardButton.Foreground = sessionSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                                         ColorManager.Secondary400.ConvertBrush();
            DeleteSessionCardButton.Background = sessionSelected ? ColorManager.Primary900.ConvertBrush() :
                                                                   ColorManager.Primary200.ConvertBrush();
        }

        public void ChangeStatus(int id)
        {
            SelectSession(id);

            UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

            var changeLiveStatusWindow = new PopUpWindow($"You are about to change {activeSession.Name}'s status from " +
                                                         $"{(activeSession.IsLive ? "live" : "offline")} to " +
                                                         $"{(activeSession.IsLive ? "offline" : "live")}\n" +
                                                         $"Are you sure about that?",
                                                         PopUpWindow.PopUpType.ChangeLiveStatus);
            changeLiveStatusWindow.ShowDialog();
        }

        private void ChangeSessionColors()
        {
            foreach (LiveSessionItem liveSessionItem in SessionsStackPanel.Children)
            {
                liveSessionItem.ChangeColor(liveSessionItem.SessionID == activeSession.ID);
            }
        }

        private void ChangeSessionStatus(int sessionID, bool status)
        {
            foreach (LiveSessionItem liveSessionItem in SessionsStackPanel.Children)
            {
                if (liveSessionItem.SessionID == sessionID)
                {
                    liveSessionItem.ChangeStatus(status);
                }
            }
        }

        private Session GetSession(int id) => sessions.Find(x => x.ID == id);

        private void AddLiveSession_Click(object sender, RoutedEventArgs e)
        {
            UpdateLoadingGrid(visibility: true, "Adding new session..");

            PostNewSessionAsync(AddLiveSessionNameTextBox.Text);

            AddLiveSessionNameTextBox.Text = string.Empty;
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

        private void RefreshSessionsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSessionsButton.Background = ColorManager.Secondary100.ConvertBrush();
            UpdateLoadingGrid(visibility: true, "Loading sessions..");

            int? selectedSessionID = null;

            if (activeSession != null)
            {
                selectedSessionID = activeSession.ID;
            }

            GetAllSessionsAsync(selectedSessionID: selectedSessionID);
        }

        private void DeleteSessionCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sessionSelected)
            {
                DeleteSessionCardButton.Background = ColorManager.Primary700.ConvertBrush();
            }
        }

        private void DeleteSessionCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sessionSelected)
            {
                DeleteSessionCardButton.Background = ColorManager.Primary800.ConvertBrush();

                UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

                var deleteSessionWindow = new PopUpWindow($"You are about to delete {activeSession.Name}\n" +
                                                          $"Are you sure about that?",
                                                          PopUpWindow.PopUpType.DeleteSession);
                deleteSessionWindow.ShowDialog();
            }
        }

        private void DeleteSessionCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sessionSelected)
            {
                DeleteSessionCardButton.Background = ColorManager.Primary800.ConvertBrush();
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void DeleteSessionCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sessionSelected)
            {
                DeleteSessionCardButton.Background = ColorManager.Primary900.ConvertBrush();
                Mouse.OverrideCursor = null;
            }
        }

        public void DeleteSeciton(bool delete)
        {
            if (delete)
            {
                DeleteSessionAsync(activeSession.ID);
            }
            else
            {
                UpdateLoadingGrid(visibility: false);
            }
        }

        public void ChangeName(string newName)
        {
            ChangeSessionNameAsync(activeSession.ID, newName);
        }

        public void ChangeDate(DateTime newDate)
        {
            ChangeSessionDateAsync(activeSession.ID, newDate);
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
            URLLabel.Content = ConfigurationManager.URL;
            PortLabel.Content = ConfigurationManager.Port.ToString();
        }

        #region cards

        private void ChangeSessionStatusCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sessionSelected)
            {
                ChangeSessionStatusCardButton.Background = ColorManager.Secondary200.ConvertBrush();
            }
        }

        private void ChangeSessionStatusCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sessionSelected)
            {
                ChangeSessionStatusCardButton.Background = ColorManager.Secondary100.ConvertBrush();

                ChangeStatus(activeSession.ID);
            }
        }

        private void ChangeSessionStatusCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sessionSelected)
            {
                ChangeSessionStatusCardButton.Background = ColorManager.Secondary100.ConvertBrush();
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void ChangeSessionStatusCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sessionSelected)
            {
                ChangeSessionStatusCardButton.Background = ColorManager.Secondary50.ConvertBrush();
                Mouse.OverrideCursor = null;
            }
        }


        private void ChangeSessionNameCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSessionNameCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ChangeSessionNameCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSessionNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

            string newName = SelectedSessionNameTextBox.Text;
            if (!newName.Equals(string.Empty) && !newName.Equals(activeSession.Name))
            {
                MenuManager.LiveSettings.ChangeName(newName);
            }
        }

        private void ChangeSessionNameCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSessionNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSessionNameCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSessionNameCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void ChangeSessionDateCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSessionDateCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ChangeSessionDateCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeSessionDateCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            UpdateLoadingGrid(visibility: true, progressBarVisibility: false);

            string newDateText = SelectedSessionDateLabel.Text;
            if (!newDateText.Equals(string.Empty))
            {
                DateTime newDate = Convert.ToDateTime(newDateText);
                if (newDate != activeSession.Date)
                {
                    MenuManager.LiveSettings.ChangeDate(newDate);
                }
            }
        }

        private void ChangeSessionDateCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSessionDateCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSessionDateCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeSessionDateCardButton.Background = ColorManager.Secondary50.ConvertBrush();
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
