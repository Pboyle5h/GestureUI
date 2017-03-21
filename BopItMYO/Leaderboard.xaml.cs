using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BopItMYO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Leaderboard : Page
    {
        public class LeaderboardBody
        {
            public string Nickname { get; set; }
            public string score { get; set; }
        }
        public Leaderboard()
        {
            this.InitializeComponent();
            LeadboardTable();
        }
        public async void LeadboardTable()
        {

            string uri = App.apiURL+"/GetScores";
            System.Net.WebRequest wrGETURL = WebRequest.Create(uri);
            wrGETURL.Proxy = null;

            try
            {
                WebResponse response = await wrGETURL.GetResponseAsync();
                Stream dataStream = response.GetResponseStream();
                StreamReader objReader = new StreamReader(dataStream);

                dynamic javaResponse = (objReader.ReadToEnd());
                var list = JsonConvert.DeserializeObject<List<LeaderboardBody>>(javaResponse);

                //loops through the list elements
                foreach (LeaderboardBody rt in list)
                {
                    //if the list element is not equal to null it enters the if statement
                    //This makes sure that only accurate data is shown.

                    if (rt != null)
                    {
                        //appends the rota on to the screen for the employee 
                        leaderboardTB.Text += "Nickname: " + rt.Nickname +
                                         "\r\nScore: " + rt.score;
                        //passes the list into a global list to be transfered on button click
                    }


                }

                response.Dispose();
            }
            catch (WebException ex)
            {
                //if connection failed, output message to user
               // errorMessage.Visibility = Visibility.Visible;
               // errorMessage.Text = "Failed to connect to server\nPlease check your internet connection";

            }
        }
    }
}
