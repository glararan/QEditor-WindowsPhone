using System;
using System.Collections.Generic;
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
        public bool start = false;
        public bool tracking = false;
        Geolocator geolocator = null;
        List<DataWork.PointA> track = new List<DataWork.PointA>();

        public MainPage()
        {

            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            DataWork.PointA pointA = new DataWork.PointA();
            pointA.X = double.Parse(args.Position.Coordinate.Latitude.ToString());
            pointA.Y = double.Parse(args.Position.Coordinate.Longitude.ToString());
            pointA.Z = double.Parse(args.Position.Coordinate.Altitude.ToString());
            track.Add(pointA);
        }

        async void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {

            switch(args.Status)
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
            if(!tracking)
            {
                geolocator = new Geolocator();
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.MovementThreshold = 100;

                geolocator.StatusChanged += geolocator_StatusChanged;
                geolocator.PositionChanged += geolocator_PositionChanged;

                tracking = true;
                procT.Text = "Zaznamenávání dat..";
                startButton.Content = "Zastavit";
            }
            else
            {
                geolocator.PositionChanged -= geolocator_PositionChanged;
                geolocator.StatusChanged -= geolocator_StatusChanged;
                geolocator = null;

                tracking = false;
                procT.Text = "Vypnuto";
                startButton.Content = "Spustit";
            }
        }

    }
}