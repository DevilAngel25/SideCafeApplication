using System;
using System.Collections.Generic;
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

using Bing.Maps;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SidewalkCafe
{
    public sealed partial class InfoBox : UserControl
    {
        public InfoBox()
        {
            this.InitializeComponent();
        }

        //this sets the information to be displayed in the infobox.
        public void SetInfo(string name, string tradeName, string street, string phone, Location location)
        {
            txtName.Text = name;
            txtTradeName.Text = tradeName;
            txtStreetAddress.Text = street;
            txtPhoneNumber.Text = phone;

            string latitude, longitude;
            latitude = location.Latitude.ToString();
            longitude = location.Longitude.ToString();

            txtLat.Text = latitude;
            txtLong.Text = longitude;
        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
