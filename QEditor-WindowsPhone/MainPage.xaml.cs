using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using System.Net.Http;
using System.Threading.Tasks;
using AsyncOAuth;
using AsyncOAuth.WindowsPhone;
using Windows.UI.Popups;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Threading;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization;

namespace QEditor_WindowsPhone
{
    public sealed partial class MainPage : Page
    {
        float[] mapData = null;

        List<Task> threads = new List<Task>();

        string[] thread_results = null;

        public MainPage()
        {
            this.InitializeComponent();

            Map.Width  = Window.Current.Bounds.Width;
            Map.Height = Window.Current.Bounds.Width;
            Map.TrafficFlowVisible = false;

            Initialization = getTwitter();
        }

        public Task Initialization { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            threads.Clear();

            mapData = null;

            await work();
        }

        private async Task work()
        {
            Geopoint topLeft, topRight, botLeft, botRight;

            Map.GetLocationFromOffset(new Point(0, 0), out topLeft);
            Map.GetLocationFromOffset(new Point(0, Map.Height), out botLeft);
            Map.GetLocationFromOffset(new Point(Map.Width, 0), out topRight);
            Map.GetLocationFromOffset(new Point(Map.Width, Map.Height), out botRight);

            mapData = new float[Settings.TILE_WIDTH * Settings.TILE_HEIGHT];

            double xDiff = Map.Width  / Settings.TILE_WIDTH;
            double yDiff = Map.Height / Settings.TILE_HEIGHT;

            progressBar.Value = 1;

            for(int x = 0; x < 4; x++)
            {
                for(int y = 0; y < 4; y++)
                    threads.Add(workPart(xDiff, yDiff, x * Settings.TILE_WIDTH / 4, y * Settings.TILE_HEIGHT / 4, (x + 1) * Settings.TILE_WIDTH / 4, (y + 1) * Settings.TILE_HEIGHT / 4));
            }

            await Task.WhenAll(threads.ToArray());

            progressBar.Value = 50;

            thread_results = new string[threads.Count()];

            threads.Clear();

            await saveData();
        }

        string googleAPI = "";

        HttpClient client = new HttpClient();

        private async Task workPart(double diffX, double diffY, int startX, int startY, int endX, int endY)
        {
            await Task.Run(() =>
            {
                double xDiff = diffX;
                double yDiff = diffY;

                for(int x = startX; x < endX; x++)
                {
                    for(int y = startY; y < endY; y++)
                    {
                        Geopoint point;

                        Map.GetLocationFromOffset(new Point(xDiff * x, yDiff * y), out point);

                        string url = "https://maps.googleapis.com/maps/api/elevation/json?locations=" + point.Position.Latitude + "," + point.Position.Longitude + "&key=" + googleAPI;

                        var result = await client.GetStringAsync(url);



                        mapData[y * Settings.TILE_WIDTH + x] = (float)point.Position.Altitude;
                    }
                }

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                {
                    progressBar.Value += 49 / threads.Count();
                });
            });
        }

        private void Video_DoubleTapped(object sender, TappedRoutedEventArgs e)
        {
            Image target = (Image)sender;

            var options              = new Windows.System.LauncherOptions();
            options.TreatAsUntrusted = true;

            var page = Windows.System.Launcher.LaunchUriAsync(new Uri(target.DataContext.ToString()), options);
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async Task saveData()
        {
            progressBar.Value = 51;

            for(int x = 0; x < 4; x++)
            {
                for(int y = 0; y < 4; y++)
                    threads.Add(saveDataPart(x + (y * 4), x * Settings.TILE_WIDTH / 4, y * Settings.TILE_HEIGHT / 4, (x + 1) * Settings.TILE_WIDTH / 4, (y + 1) * Settings.TILE_HEIGHT / 4));
            }

            await Task.WhenAll(threads.ToArray());

            progressBar.Value = 95;

            string[] list = new string[threads.Count()];

            StringBuilder paste = new StringBuilder();

            for(int i = 0; i < threads.Count(); i++)
                paste.Append(thread_results[i]);

            string fileName = "test.txt";

            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(paste.ToString().ToCharArray());

            StorageFolder externalDevices = Windows.Storage.KnownFolders.RemovableDevices;

            var card = (await externalDevices.GetFoldersAsync()).FirstOrDefault();

            if(card != null)
            {
                var dataFolder = await card.CreateFolderAsync("QEditor", CreationCollisionOption.OpenIfExists);

                var file = await dataFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using(var s = await file.OpenStreamForWriteAsync())
                {
                    s.Write(fileBytes, 0, fileBytes.Length);
                }

                progressBar.Value = 99;

                await new MessageDialog(String.Format("Soubor je zapsán do {0}", fileName)).ShowAsync();
            }

            progressBar.Value = 0;
        }

        private async Task saveDataPart(int index, int startX, int startY, int endX, int endY)
        {
            await Task.Run(() =>
            {
                StringBuilder _result = new StringBuilder();

                for(int x = startX; x < endX; x++)
                {
                    for(int y = startY; y < endY; y++)
                        _result.Append(x + " " + y + " " + mapData[x + (Settings.TILE_WIDTH * y)].ToString("n5") + "\n");
                }

                thread_results[index] = _result.ToString();

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    progressBar.Value += 42 / threads.Count();
                });
            });
        }

        private void ListTweets_Loaded(object sender, RoutedEventArgs e)
        {
        }

        async Task getTwitter()
        {
            const string consumerKey       = "UdfRBoxSG3AGg4mBfdQ";
            const string consumerSecret    = "jlZRJOzZ4Io1biHq8rqEinDr6CjTRPWoUL793s4I";
            const string accessTokenKey    = "2295754686-bfnEhEE8VUMFdj9AjYpkWsZuTPcYsqKLGYPWsFR";
            const string accessTokenSecret = "XlqwBPE22jayivwgkEx7goRLi2g66PwkqttOlMa0tK3cM";

            AccessToken accessToken = new AccessToken(accessTokenKey, accessTokenSecret);

            var client = new TwitterClient(consumerKey, consumerSecret, accessToken); 
 
            string timeline = await client.GetUserTimeline(5);

            JArray array = JArray.Parse(timeline);

            JToken token = array.First;

            while(token != null)
            {
                string name = JObject.Parse(token.ToString()).Value<JToken>("user").Value<string>("name");
                string text = token.Value<string>("text");
                string date = token.Value<string>("created_at");

                DateTime dTime = DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss +ffff yyyy", CultureInfo.InvariantCulture);

                ListTweets.Items.Add(String.Format("{0}: {1} - {2}", name, text, dTime));

                token = token.Next;
            }
        }
    }

}

