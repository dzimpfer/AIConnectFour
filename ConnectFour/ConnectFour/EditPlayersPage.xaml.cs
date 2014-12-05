using ConnectFour.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ConnectFour
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EditPlayersPage : Page
    {
        public string firstPlayerName = "Player One";
        public string secondPlayerName = "Player Two";
        public Color firstPlayerColor = Colors.Blue;
        public Color secondPlayerColor = Colors.Red;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public EditPlayersPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            textBox1.Text = firstPlayerName;
            //firstPlayerScoreTextBlock.Text = firstPlayerColor.ToString();
            ellipse1.Fill = new SolidColorBrush(firstPlayerColor);

            textBox2.Text = secondPlayerName;
            //secondPlayerScoreTextBlock.Text = secondPlayerColor.ToString();
            ellipse2.Fill = new SolidColorBrush(secondPlayerColor);
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings =
        Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("firstPlayerName"))
                firstPlayerName = roamingSettings.Values["firstPlayerName"].ToString();

            if (roamingSettings.Values.ContainsKey("secondPlayerName"))
                secondPlayerName = roamingSettings.Values["secondPlayerName"].ToString();

            if (roamingSettings.Values.ContainsKey("firstPlayerColor"))
                firstPlayerColor = GetColorFromHexString(roamingSettings.Values["firstPlayerColor"].ToString());

            if (roamingSettings.Values.ContainsKey("secondPlayerColor"))
                secondPlayerColor = GetColorFromHexString(roamingSettings.Values["secondPlayerColor"].ToString());

            textBox1.Text = firstPlayerName;
            //firstPlayerScoreTextBlock.Text = firstPlayerColor.ToString();
            ellipse1.Fill = new SolidColorBrush(firstPlayerColor);

            textBox2.Text = secondPlayerName;
            //secondPlayerScoreTextBlock.Text = secondPlayerColor.ToString();
            ellipse2.Fill = new SolidColorBrush(secondPlayerColor);

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings =
        Windows.Storage.ApplicationData.Current.RoamingSettings;

            if (firstPlayerName != textBox1.Text.Trim() || secondPlayerName != textBox2.Text.Trim())
            {
                roamingSettings.Values["firstPlayerScore"] = 0;
                roamingSettings.Values["secondPlayerScore"] = 0;

                roamingSettings.Values["firstPlayerTurn"] = "true";
                roamingSettings.Values["grid"] = "000000000000000000000000000000000000000000";
            }

            roamingSettings.Values["firstPlayerName"] = textBox1.Text.Trim() == "" ? firstPlayerName : textBox1.Text.Trim();

            roamingSettings.Values["secondPlayerName"] = textBox2.Text.Trim() == "" ? secondPlayerName : textBox2.Text.Trim();

            roamingSettings.Values["firstPlayerColor"] = ((SolidColorBrush)ellipse1.Fill).Color.ToString();

            roamingSettings.Values["secondPlayerColor"] = ((SolidColorBrush)ellipse2.Fill).Color.ToString();
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private Color GetColorFromHexString(string hexValue)
        {
            hexValue = hexValue.Substring(1);
            var a = Convert.ToByte(hexValue.Substring(0, 2), 16);
            var r = Convert.ToByte(hexValue.Substring(2, 2), 16);
            var g = Convert.ToByte(hexValue.Substring(4, 2), 16);
            var b = Convert.ToByte(hexValue.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }

        private void ellipse1_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (colorPanel1.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                colorPanel1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                colorPanel1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ellipse2_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (colorPanel2.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                colorPanel2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                colorPanel2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Ellipse_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse1.Fill = ellipse.Fill;
            firstPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse1.Fill = ellipse.Fill;
            firstPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_2(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse1.Fill = ellipse.Fill;
            firstPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_3(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse1.Fill = ellipse.Fill;
            firstPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_4(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse1.Fill = ellipse.Fill;
            firstPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_5(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse1.Fill = ellipse.Fill;
            firstPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_6(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse2.Fill = ellipse.Fill;
            secondPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_7(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse2.Fill = ellipse.Fill;
            secondPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_8(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse2.Fill = ellipse.Fill;
            secondPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_9(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse2.Fill = ellipse.Fill;
            secondPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_10(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse2.Fill = ellipse.Fill;
            secondPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }

        private void Ellipse_PointerPressed_11(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse2.Fill = ellipse.Fill;
            secondPlayerColor = ((SolidColorBrush)ellipse.Fill).Color;
        }
    }
}
