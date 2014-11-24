using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using Windows.UI.Xaml.Resources;

namespace Tracking
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Stopwatch stopwatch=new Stopwatch();
        public bool start = false;
        public bool tracking = false;
        Geolocator geolocator = null;
        Geopoint geopoint;
        List<DataWork.PointA> track = new List<DataWork.PointA>();

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Map.Width = Window.Current.Bounds.Width;
            Map.Height = Window.Current.Bounds.Width;
            Map.TrafficFlowVisible = false;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

         void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {

            DataWork.PointA pointA = new DataWork.PointA();
            pointA.X = double.Parse(args.Position.Coordinate.Latitude.ToString());
            pointA.Y = double.Parse(args.Position.Coordinate.Longitude.ToString());
            pointA.Z = double.Parse(args.Position.Coordinate.Altitude.ToString());
            track.Add(pointA);
             geopoint=new Geopoint(new BasicGeoposition()
                {
                    Latitude =pointA.X,
                    Longitude = pointA.Y
                });
             if (tracking)
             {
                 try
                 {
                     Map.Center = geopoint;
                     Map.ZoomLevel = 12;
                     geolocator = new Geolocator();
                     tracking = false;
                 }
                 catch (Exception)
                 {
                     
                     throw;
                 }

             }

        }

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    statusT1.Text = "Povolte sledování v nastavení.";
                    break;
                case PositionStatus.Initializing:
                    statusT1.Text = "Čekání na data...";
                    break;
                case PositionStatus.NoData:
                    statusT1.Text = "Data nejsou k dispozici.";
                    break;
                case PositionStatus.Ready:
                    statusT1.Text = "Data k dispozici...";
                    break;
                case PositionStatus.NotAvailable:
                    statusT1.Text = "Připojení není dispozici";
                    break;
                case PositionStatus.NotInitialized:
                    statusT1.Text = "Nepodařilo se získat připojení";
                    break;
            }
        }

       private void start_Click(object sender, RoutedEventArgs e)
       {
           work();
       }

       private void Button_Click(object sender, RoutedEventArgs e)
       {
           geolocator.DesiredAccuracy = PositionAccuracy.High;
           geolocator.MovementThreshold = 100;

           geolocator.StatusChanged += geolocator_StatusChanged;
           geolocator.PositionChanged += geolocator_PositionChanged;
           tracking = true;
       }

        private void work()
        {
            Geopoint topLeft, topRight, botLeft, botRight;

            Map.GetLocationFromOffset(new Point(0, 0), out topLeft);
            Map.GetLocationFromOffset(new Point(0, Map.Height), out botLeft);
            Map.GetLocationFromOffset(new Point(Map.Width, 0), out topRight);
            Map.GetLocationFromOffset(new Point(Map.Width, Map.Height), out botRight);

            double topleft = topLeft.Position.Altitude;
            double topright = topRight.Position.Altitude;
            double botleft = botLeft.Position.Altitude;
            double botright = botRight.Position.Altitude;
            double[] mapData = new double[Settings.TILE_WIDTH * Settings.TILE_HEIGHT / Settings.CHUNKS / Settings.CHUNKS];
            // row column
            int rc = Settings.TILE_WIDTH / Settings.CHUNKS;

            // calc row min to row max, cell min to cell max first and last index
            for (int y = 0; y < rc; ++y)
            {
                mapData[y * rc] = topleft - (topleft - botleft) / (rc - 1) * y;
                mapData[y * rc + rc - 1] = topright - (topright - botright) / (rc - 1) * y;
            }

            for (int x = 0; x < rc; ++x)
            {
                mapData[x] = topleft - (topleft - topright) / (rc - 1) * x;
                mapData[x + (rc - 1) * rc] = botleft - (botleft - botright) / (rc - 1) * x;
            }

            // calc data
            for (int x = 0; x < rc; ++x)
            {
                if (x <= 0 || x >= rc - 1)
                    continue;

                for (int y = 0; y < rc; ++y)
                {
                    if (y <= 0 || y >= rc - 1)
                        continue;

                    double currLeft = mapData[y * rc];
                    double currRight = mapData[y * rc + rc - 1];

                    mapData[x + rc * y] = currLeft - ((currLeft - currRight) / (rc - 1)) * x;
                }
            }


        }

    }
}
