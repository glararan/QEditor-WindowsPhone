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
           if (!tracking)
           {
               geolocator = new Geolocator();
               geolocator.DesiredAccuracy = PositionAccuracy.High;
               geolocator.MovementThreshold = 100;

               geolocator.StatusChanged += geolocator_StatusChanged;
               geolocator.PositionChanged += geolocator_PositionChanged;

               tracking = true;
               procT.Text = "Zaznamenávání dat..";
               startButton.Content = "Zastavit";
               locateButton.IsEnabled = true;
           }
           else
           {
               geolocator.PositionChanged -= geolocator_PositionChanged;
               geolocator.StatusChanged -= geolocator_StatusChanged;
               geolocator = null;

               tracking = false;
               procT.Text = "Vypnuto";
               startButton.Content = "Spustit";
               locateButton.IsEnabled = false;
           }
       }

       private void Button_Click(object sender, RoutedEventArgs e)
       {
           Map.Center = geopoint;
           Map.ZoomLevel = 12;
           Map.LandmarksVisible = true;
       }

    }
}
