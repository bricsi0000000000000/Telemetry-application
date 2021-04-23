using System.Collections.Generic;
using System.Windows.Controls;
using LocigLayer.Texts;
using LogicLayer.Menus.Settings.Live;

namespace LogicLayer.Menus.Live
{
    public partial class LiveMenu : UserControl
    {
        /// <summary>
        /// Menu items.
        /// </summary>
        private readonly List<TabItem> tabs = new List<TabItem>();

        public LiveMenu()
        {
            InitializeComponent();

            InitializeTabs();
        }

        private void InitializeTabs()
        {
            TabItem liveSettingsTab = MakeTab(TextManager.SettingsMenuName, new LiveSettings(), "liveSettingsTab", selected: true);
            MenuManager.LiveSettings = (LiveSettings)liveSettingsTab.Content;
            AddTab(liveSettingsTab);

            TabItem liveTelemetryTab = MakeTab(TextManager.LiveMenuName, new LiveTelemetry(), "liveTelemetryTab", selected: true);
            MenuManager.LiveTelemetry = (LiveTelemetry)liveTelemetryTab.Content;
            AddTab(liveTelemetryTab);
        }

        private TabItem MakeTab(string header, object content, string name, bool selected = false)
        {
            return new TabItem
            {
                Header = header,
                Content = content,
                Name = name,
                IsSelected = selected
            };
        }

        private void AddTab(TabItem tab)
        {
            tabs.Add(tab);
            TabsTabControl.Items.Add(tab);
        }

        /// <summary>
        /// Finds a <see cref="TabItem"/> in <see cref="tabs"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="TabItem"/>s name.</param>
        /// <returns>A <see cref="TabItem"/> whose name is <paramref name="name"/>.</returns>
        public TabItem GetTab(string name) => tabs.Find(x => x.Header.Equals(name));

     

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
