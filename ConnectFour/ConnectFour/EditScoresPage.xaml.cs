using ConnectFour.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ConnectFour
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EditScoresPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public string[] topPlayers = { "One", "Two", "Three", "Four", "Five" };
        public int[] topPlayerScores = { 5, 4, 3, 2, 1 };
        public string firstPlayerName = "Player One";
        public string secondPlayerName = "Player Two";
        public int firstPlayerScore = 0;
        public int secondPlayerScore = 0;

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


        public EditScoresPage()
        {
            this.InitializeComponent();



            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            topScorerTextBlock1.Text = topPlayers[0] + ":";
            topScoreTextBlock1.Text = "  " + topPlayerScores[0].ToString();

            topScorerTextBlock2.Text = topPlayers[1] + ":";
            topScoreTextBlock2.Text = "  " + topPlayerScores[1].ToString();

            topScorerTextBlock3.Text = topPlayers[2] + ":";
            topScoreTextBlock3.Text = "  " + topPlayerScores[2].ToString();

            topScorerTextBlock4.Text = topPlayers[3] + ":";
            topScoreTextBlock4.Text = "  " + topPlayerScores[3].ToString();

            topScorerTextBlock5.Text = topPlayers[4] + ":";
            topScoreTextBlock5.Text = "  " + topPlayerScores[4].ToString();

            name1.Text = firstPlayerName + ":  ";
            score1.Text = firstPlayerScore.ToString();

            name2.Text = secondPlayerName + ":  ";
            score2.Text = secondPlayerScore.ToString();
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

            if (roamingSettings.Values.ContainsKey("topPlayers"))
                topPlayers = deserializePlayers(roamingSettings.Values["topPlayers"].ToString());

            if (roamingSettings.Values.ContainsKey("topPlayerScores"))
                topPlayerScores = deserializeScores(roamingSettings.Values["topPlayerScores"].ToString());

            if (roamingSettings.Values.ContainsKey("firstPlayerScore"))
                firstPlayerScore = Convert.ToInt32(roamingSettings.Values["firstPlayerScore"].ToString());

            if (roamingSettings.Values.ContainsKey("secondPlayerScore"))
                secondPlayerScore = Convert.ToInt32(roamingSettings.Values["secondPlayerScore"].ToString());

            topScorerTextBlock1.Text = topPlayers[0] + ":";
            topScoreTextBlock1.Text = "  " + topPlayerScores[0].ToString();

            topScorerTextBlock2.Text = topPlayers[1] + ":";
            topScoreTextBlock2.Text = "  " + topPlayerScores[1].ToString();

            topScorerTextBlock3.Text = topPlayers[2] + ":";
            topScoreTextBlock3.Text = "  " + topPlayerScores[2].ToString();

            topScorerTextBlock4.Text = topPlayers[3] + ":";
            topScoreTextBlock4.Text = "  " + topPlayerScores[3].ToString();

            topScorerTextBlock5.Text = topPlayers[4] + ":";
            topScoreTextBlock5.Text = "  " + topPlayerScores[4].ToString();

            name1.Text = firstPlayerName + ":  ";
            score1.Text = firstPlayerScore.ToString();

            name2.Text = secondPlayerName + ":  ";
            score2.Text = secondPlayerScore.ToString();
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
            roamingSettings.Values["topPlayers"] = serializePlayers(topPlayers);

            roamingSettings.Values["topPlayerScores"] = serializeScores(topPlayerScores);

            roamingSettings.Values["firstPlayerScore"] = firstPlayerScore.ToString();

            roamingSettings.Values["secondPlayerScore"] = secondPlayerScore.ToString();
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

        private string serializeScores(int[] scores)
        {
            string result = "";
            for (int c = 0; c < 5; c++)
            {
                result += scores[c].ToString() + ",";
            }
            return result;
        }

        private string serializePlayers(string[] players)
        {
            string result = "";
            for (int c = 0; c < 5; c++)
            {
                result += players[c] + ",";
            }
            return result;
        }

        private string[] deserializePlayers(string serialization)
        {
            return serialization.Split(',');
        }

        private int[] deserializeScores(string serialization)
        {
            int[] result = { 0, 0, 0, 0, 0 };
            string[] test = serialization.Split(',');
            for (int i = 0; i < 5; i++)
            {
                result[i] = Convert.ToInt32(test[i]);
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            firstPlayerScore = 0;
            secondPlayerScore = 0;

            score1.Text = "0";
            score2.Text = "0";

            Windows.Storage.ApplicationDataContainer roamingSettings =
Windows.Storage.ApplicationData.Current.RoamingSettings;

            roamingSettings.Values["firstPlayerTurn"] = "true";
            roamingSettings.Values["grid"] = "000000000000000000000000000000000000000000";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            topPlayers[0] = "One";
            topPlayers[1] = "Two";
            topPlayers[2] = "Three";
            topPlayers[3] = "Four";
            topPlayers[4] = "Five";
            for(int i = 0; i < 5; i++)
            {
                topPlayerScores[i] = 0;
            }

            topScorerTextBlock1.Text = topPlayers[0] + ":";
            topScoreTextBlock1.Text = "  " + topPlayerScores[0].ToString();

            topScorerTextBlock2.Text = topPlayers[1] + ":";
            topScoreTextBlock2.Text = "  " + topPlayerScores[1].ToString();

            topScorerTextBlock3.Text = topPlayers[2] + ":";
            topScoreTextBlock3.Text = "  " + topPlayerScores[2].ToString();

            topScorerTextBlock4.Text = topPlayers[3] + ":";
            topScoreTextBlock4.Text = "  " + topPlayerScores[3].ToString();

            topScorerTextBlock5.Text = topPlayers[4] + ":";
            topScoreTextBlock5.Text = "  " + topPlayerScores[4].ToString();
        }
    }
}
