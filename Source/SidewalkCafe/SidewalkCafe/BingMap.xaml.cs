using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

using Bing.Maps;

namespace SidewalkCafe
{
    public sealed partial class MainPage
    {
        int zoom = 0, minZoom = 0, maxZoom = 20;
        bool pushpinTapped = false;

        RealViewModel viewModel;
        InfoBox info;

        public MainPage()
        {
            //initialize the viewModel.
            this.InitializeComponent();
            viewModel = RealViewModel.GetViewModel();
            this.DataContext = viewModel;

            //add one instance of the info box
            info = new InfoBox();
            bingMap.Children.Add(info);
            info.Visibility = Visibility.Collapsed;

            //set default view location
            bingMap.SetView(new Location(40.7451934814453,-73.9048461914063), 11.0f);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        //when zooming out, set the map to zoom out by 2
        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            zoom = (int)bingMap.ZoomLevel - 2;
            bingMap.SetZoomLevel(zoom < minZoom ? minZoom : zoom);
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            zoom = (int)bingMap.ZoomLevel + 2;
            bingMap.SetZoomLevel(zoom > maxZoom ? maxZoom : zoom);
        }

        //switch through the different types of maps
        private void btnChangeMapType_Click(object sender, RoutedEventArgs e)
        {
            switch (bingMap.MapType)
            {
                case Bing.Maps.MapType.Aerial:      { bingMap.MapType = Bing.Maps.MapType.Birdseye; break; }
                case Bing.Maps.MapType.Birdseye:    { bingMap.MapType = Bing.Maps.MapType.Road;     break; }
                default:                            { bingMap.MapType = Bing.Maps.MapType.Aerial;   break; }
            }
        }

        //show or hide traffic.
        private void btnTrafficView_Click(object sender, RoutedEventArgs e)
        {
            bingMap.ShowTraffic = !bingMap.ShowTraffic;
        }

        //bring up the flyout box to add another cafe to the database.
        private void btnAddCafe_Click(object sender, RoutedEventArgs e)
        {
            // **Ensure the app bar is closed**
            bottomAppBar.Visibility = Visibility.Collapsed;
            // Display the flyout popup
            flyoutBorder.Width = 650;
            flyoutBorder.Height = Window.Current.Bounds.Height;
            flyoutTitle.Text = "Add a new Cafe Location";
            //brush the controls according to the parameters
            flyoutBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
            flyoutHeaderGrid.Background = new SolidColorBrush(Windows.UI.Colors.DarkBlue);
            flyoutBorder.Background = new SolidColorBrush(Windows.UI.Colors.DarkBlue);
           
            flyoutPopup.IsOpen = true;
            // Position the flyout
            flyoutPopup.HorizontalOffset = Window.Current.Bounds.Width - flyoutBorder.Width;
            // Set the focus on the Name string text box
            txtName.Focus(FocusState.Programmatic);
        }

        private void flyoutBackButton_Click(object sender, RoutedEventArgs e)
        {
            flyoutPopup.IsOpen = false;

            // **Make sure to Open the app bar so you can access it again**
            bottomAppBar.Visibility = Visibility.Visible;
        }

        //if the text boxes are not empty save the cafe to the database.
        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Trim().Length != 0 && txtTradeName.Text.Trim().Length != 0 && txtStreetAddress.Text.Trim().Length != 0 && txtPhoneNumber.Text.Trim().Length != 0 && txtLongitude.Text.Trim().Length != 0 && txtLatitude.Text.Trim().Length != 0)
            {
                //create the new cafe.
                //I have structured the Location it like this sinice this is how it is stored in the database.
                Cafe newCafe = new Cafe(txtName.Text, txtTradeName.Text, txtStreetAddress.Text, txtPhoneNumber.Text, "Location(" + txtLatitude.Text + ", " + txtLongitude.Text + ")");
                viewModel.AddNewCafe(newCafe); //add it the the List.
                flyoutPopup.IsOpen = false;

                // **Make sure to Open the app bar so you can access it again**
                bottomAppBar.Visibility = Visibility.Visible;
            }
        }

        private void PushpinTapped(object sender, TappedRoutedEventArgs e)
        {
            // Get the position of the pushpin and open the information box
            pushpinTapped = true;

            Pushpin pin = sender as Pushpin;
            Location location = MapLayer.GetPosition(pin);
            //show the info box
            info.Visibility = Visibility.Visible;

            //fill in all information about the chosen cafe.
            //set name, tradename, street address, phone number and latitude, longitude.
            info.SetInfo(viewModel.GetCafeName(location), viewModel.GetCafeTradeName(location), viewModel.GetCafeStreet(location), viewModel.GetCafePhone(location), location);

            MapLayer.SetPosition(info, location); //set the position of the info box
        }

        private void MapTapped(object sender, TappedRoutedEventArgs e)
        {
            //close the information box if the information box is not open and 
            //if the map is tapped not the push pin.
            if (!pushpinTapped)
            {
                info.Visibility = Visibility.Collapsed;
            }
            pushpinTapped = false;
        }
    }
}
