using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;

using Bing.Maps;


// I have used the ViewModel from LinkManager as a template, this has been edited and changed to correctly reperesent my data.

namespace SidewalkCafe
{
    /// <summary>
    /// Class that holds the core information of the Cafes
    /// </summary>

    public class Cafe
    {
        private string _name;
        private string _tradeName;

        private string _streetAddress;
        private string _phoneNumber;

        private string _location;
        private float _longitude;
        private float _latitude;

        public Cafe()
        {
        }

        //used when adding a new cafe to the database
        public Cafe(string name, string tradeName, string streetAddress, string phoneNumber, string location)
        {
            Name = name;
            TradeName = tradeName;
            StreetAddress = streetAddress;
            PhoneNumber = phoneNumber;

            Location = location;
        }

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    //after defining the location the long and lat must be extracted from the string.
                    Longitude = getLongFromLoc(_location);
                    Latitude = getLatFromLoc(_location);
                }
            }
        }

        public float Longitude
        {
            get { return _longitude; }
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                }
            }
        }

        public float Latitude
        {
            get { return _latitude; }
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                }
            }
        }

        public string StreetAddress
        {
            get { return _streetAddress; }
            set
            {
                if (_streetAddress != value)
                {
                    _streetAddress = value;
                }
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }

        public string TradeName
        {
            get { return _tradeName; }
            set
            {
                if (_tradeName != value)
                {
                    _tradeName = value;
                }
            }
        }

        //Encode the data to be sent as a url
        public async Task<string> GetUrlEncodedLinkAsString()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("Location", _location);
            keyValuePairs.Add("StreetAddress", _streetAddress);
            keyValuePairs.Add("PhoneNumber", _phoneNumber);
            keyValuePairs.Add("Name", _name);
            keyValuePairs.Add("TradeName", _tradeName);
            FormUrlEncodedContent content = new FormUrlEncodedContent(keyValuePairs);
            return await content.ReadAsStringAsync();
        }

        //method to get the longitude from the location string
        public float getLongFromLoc(string location)
        {
            float longitude;

            string[] locPartsOne = location.Split('(');
            string[] locPartsTwo = locPartsOne[1].Split(',');
            string[] locPartsThree = locPartsTwo[1].Split(')');
            string[] locPartsFour = locPartsThree[0].Split(' ');

            float.TryParse(locPartsFour[1], out longitude);

            return longitude;
        }

        //method to get the latitude from the location string
        public float getLatFromLoc(string location)
        {
            float latitude;

            string[] locPartsOne = location.Split('(');
            string[] locPartsTwo = locPartsOne[1].Split(',');

            float.TryParse(locPartsTwo[0], out latitude);

            return latitude;
        }
    }

    /// <summary>
    /// The CafeGroup is a collection of Cafes
    /// </summary>

    public class CafeGroup
    {
        ObservableCollection<Cafe> _cafes;

        public CafeGroup()
        {
            Cafes = new ObservableCollection<Cafe>();
        }

        public CafeGroup(ObservableCollection<Cafe> cafes)
        {
            Cafes = cafes;
        }

        //a collection of cafes
        public ObservableCollection<Cafe> Cafes
        {
            get
            {
                return _cafes;
            }
            set
            {
                if (_cafes != value)
                {
                    _cafes = value;
                }
            }
        }
    }

    /// <summary>
    /// The base view model class is inherits CafeGroup as there is no need for groups of cafegroups (there is only one group) 
    /// </summary>

    public class BaseViewModel : CafeGroup
    {
       //There is no need for groups of groups so this only inherits from CafeGroup
    }

    /// <summary>
    /// The view model used at run time
    /// </summary>

    public class RealViewModel : BaseViewModel
    {
        static RealViewModel _viewModel = null;

        /// <summary>
        /// Enum used to indicate the source of the data used to create the view model
        /// </summary>

        private enum ViewModelSource
        {
            None,
            Cloud,
            Local
        }

        /// <summary>
        /// Retrive the instance of the view model
        /// </summary>
        /// <returns>The current view model</returns>

        public static RealViewModel GetViewModel()
        {
            if (_viewModel == null)
            {
                _viewModel = new RealViewModel();
            }
            return _viewModel;
        }

        // At runtime populate the realViewModel.
        private RealViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                PopulateViewModel();
            }
        }

        /// <summary>
        /// Populate the view model. We first attempt to load the data from the web service.  If
        /// that fails, we try to load from our local cache file
        /// </summary>

        private async void PopulateViewModel()
        {
            List<Cafe> cafes = null;
            ViewModelSource viewModelSource = ViewModelSource.None;
            try
            {
                // Get the list of cafes from the web service and deserialise it into a List of Cafes
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("http://sidewalkcafes.azurewebsites.net/php/getcafe.php"); //PHP script which gets all of the cafe data.
                response.EnsureSuccessStatusCode();
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Cafe>));
                    cafes = (List<Cafe>)serializer.Deserialize(stream);
                }
                viewModelSource = ViewModelSource.Cloud;
            }
            catch { } // Just fall through if model not created 
            if (viewModelSource == ViewModelSource.None)
            {
                //Load from a file if it exists.
                cafes = await LoadCafesFromFile();
                viewModelSource = ViewModelSource.Local;
            }

            BuildCafeGroups(cafes); //add all of the cafes to the correct List.

            if (viewModelSource == ViewModelSource.Cloud)
            {
                //Save the List of Cafes to the file.
                SaveCafesToFile();
            }
        }

        /// <summary>
        /// Load the cafes from the local cache file
        /// </summary>
        /// <returns>A list of cafes</returns>

        //Load the cafes from a file, if not successful create a new List of cafe.
        private async Task<List<Cafe>> LoadCafesFromFile()
        {
            List<Cafe> cafes = null;
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("CafesModel");
                using (IInputStream stream = await file.OpenSequentialReadAsync())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Cafe>));
                    cafes = (List<Cafe>)serializer.Deserialize(stream.AsStreamForRead());
                }
            }
            catch
            {
                cafes = new List<Cafe>();
            }
            return cafes;
        }

        /// <summary>
        /// Build the cafe groups in the model
        /// </summary>
        /// <param name="cafes">The source list of cafes</param>

        private void BuildCafeGroups(List<Cafe> cafes)
        {
            foreach (Cafe cafe in cafes)
            {
                AddCafeToModel(cafe);
            }
        }

        /// <summary>
        /// Add a cafe to the view model.
        /// </summary>
        /// <param name="cafe">The cafe to be added</param>

        public void AddCafeToModel(Cafe cafe)
        {
            Cafes.Add(cafe);
        }

        /// <summary>
        /// Add a new cafe to the view model and use the web service to update the database in the cloud.
        /// The local cache file is also updated.
        /// </summary>
        /// <param name="cafe">The cafe to be added</param>

        public void AddNewCafe(Cafe cafe)
        {
            AddCafeToModel(cafe);
            SaveCafe(cafe);
        }

        //add the new cafe to the database
        public async void SaveCafe(Cafe cafe)
        {
            try
            {
                // Encode the components of the link so that they can be attached to the URL
                string encodedLink = await cafe.GetUrlEncodedLinkAsString();
                // Now attempt to add the link to the database
                string url = "http://sidewalkcafes.azurewebsites.net/php/addcafe.php?" + encodedLink; //PHP script to add any new cafes to the database
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                //If the update fails the user should be made aware so they can try again.
               
            }
            //Save to the file after updating te database
            SaveCafesToFile();
        }


        /// <summary>
        /// Save the view model to the local cache file as a list of cafes
        /// </summary>

        //save the cafes to a file
        public async void SaveCafesToFile()
        {
            // Create the list of cafes to be written out.
            List<Cafe> cafes = new List<Cafe>();
            foreach (Cafe cafe in Cafes)
            {
                cafes.Add(cafe);
            }
            
            MemoryStream sessionData = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Cafe>));
            serializer.Serialize(sessionData, cafes);

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("CafesModel", CreationCollisionOption.ReplaceExisting);
            using (Stream fileStream = await file.OpenStreamForWriteAsync())
            {
                sessionData.Seek(0, SeekOrigin.Begin);
                await sessionData.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
        }

        //The next few methods Get information from the Cafes and returns them to the InfoBox.

        //this takes the location of the pushpin and compares that with the location of the cafe (longitude, latitude)
        public string GetCafeStreet(Location location)
        {
            string street = "Unknown"; //default value should the for/if fail.

            //loop thought all cafe (not very efficient, could loop through 1 time or 1000 times before/if it finds a match)
            for(int i = 0; i < Cafes.Count(); i++)
            {
                if (Cafes[i].Latitude == location.Latitude && Cafes[i].Longitude == location.Longitude)
                {
                    //if it finds a match then we have found the coresponding cafe.
                    street = Cafes[i].StreetAddress;
                    //if null then return 'Unknown'.
                    if (street != null) { return street; }
                    else { street = "Unknown"; return street; }
                }
            }
            return street;
        }

        public string GetCafePhone(Location location)
        {
            string phone = "Unknown";

            for (int i = 0; i < Cafes.Count(); i++)
            {
                if (Cafes[i].Latitude == location.Latitude && Cafes[i].Longitude == location.Longitude)
                {
                    phone = Cafes[i].PhoneNumber;
                    if (phone != null) { return phone; }
                    else { phone = "Unknown"; return phone; }
                }
            }
            return phone;
        }

        public string GetCafeName(Location location)
        {
            string name = "Unknown";

            for (int i = 0; i < Cafes.Count(); i++)
            {
                if (Cafes[i].Latitude == location.Latitude && Cafes[i].Longitude == location.Longitude)
                {
                    name = Cafes[i].Name;
                    if (name != null) { return name; }
                    else { name = "Unknown"; return name; }
                }
            }
            return name;
        }

        public string GetCafeTradeName(Location location)
        {
            string tradeName = "Unknown";

            for (int i = 0; i < Cafes.Count(); i++)
            {
                if (Cafes[i].Latitude == location.Latitude && Cafes[i].Longitude == location.Longitude)
                {
                    tradeName = Cafes[i].TradeName;
                    //if null or empty string return 'Unknown'.
                    if (tradeName != null) 
                    { 
                        if (tradeName != "") { return tradeName; }
                        else { tradeName = "Unknown"; return tradeName; }
                    }
                    else { tradeName = "Unknown"; return tradeName; }
                }
            }
            return tradeName;
        }
    }
}
