using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wikimedia_commons.Resources;
using System.IO;
using System.Xml.Linq;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

namespace wikimedia_commons
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            GetImage();                       

        }

        void GetImage()
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(new Uri("http://commons.wikimedia.org/w/api.php?action=query&list=random&rnnamespace=6&rnlimit=1&format=xml"));
            
            webRequest.BeginGetResponse(new AsyncCallback(ConnectCallback), webRequest);
            
        }

       
        private void ConnectCallback(IAsyncResult asynchronousResult)
        {

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            
            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                String resultString = streamReader1.ReadToEnd();
                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);
                String file = xdoc.Element("api").Element("query").Element("random").Element("page").Attribute("title").Value;
                file = file.Replace("File:", "");
            
            response.GetResponseStream().Close();
            response.Close();
            string url = "http://en.wikipedia.org/w/api.php?action=query&prop=imageinfo&iiprop=url&meta=siteinfo&siprop=rightsinfo&format=xml&callback=?&titles=Image:" + file;

            HttpWebRequest webRequest = WebRequest.CreateHttp(new Uri(url));
            webRequest.BeginGetResponse(new AsyncCallback(ConnectCallback2), webRequest);

            }
        }

            //her
        private void ConnectCallback2(IAsyncResult asynchronousResult)
        {

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {

                String resultString = streamReader1.ReadToEnd();
                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);
                response.GetResponseStream().Close();
                response.Close();
                String file = xdoc.Element("api").Element("query").Element("pages").Element("page").Element("imageinfo").Element("ii").Attribute("url").Value;
                System.Diagnostics.Debug.WriteLine("file: " + file);
                if (file.ToLower().EndsWith(".svg"))
                {
                    System.Diagnostics.Debug.WriteLine("unsupported file: " + file);
                    return;
                }
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(file));
                webRequest.BeginGetResponse(new AsyncCallback(ConnectCallback3), webRequest);

            }
        }
    

         private void ConnectCallback3(IAsyncResult asynchronousResult)
            {

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            StreamReader streamReader1 = new StreamReader(response.GetResponseStream());

             var s = streamReader1.BaseStream;
    
             Dispatcher.BeginInvoke(() =>
             {
                 var bmp = new System.Windows.Media.Imaging.BitmapImage();
                 bmp.SetSource(s);
                 
                 var imageBrush = new ImageBrush
                 {
                     ImageSource = bmp,
                     //Opacity = 0.5d,
                     Stretch = Stretch.UniformToFill
                 };

                 this.ContentPanel.Background = imageBrush;
                 s.Close();
                 response.GetResponseStream().Close();
                 response.Close();
             });
             


        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetImage();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}