using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Windows.UI.Core;
//using BopItMYO.Classes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BopItMYO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IChannel _myoChannel;
        IHub _myoHub;
        public int time = 31;
        public DispatcherTimer Timer;
        public Random rnd = new Random();
        public int score;
        public int lives=3;
        List<string> Gestures = new List<string>()
            {
                "WaveIn",
                "Fist",
                "WaveOut",
                "FingersSpread",
        };
        List<string> randomImage = new List<string>()
            {
                "ms-appx:/Assets/wave-left.png",
                "ms-appx:/Assets/make-fist.png",
                "ms-appx:/Assets/wave-right.png",
                "ms-appx:/Assets/spread-fingers.png",
        };

        public MainPage()
        {

            this.InitializeComponent();
            // creates and starts the timer
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += countdownTimer;
            liveTB.Text = lives.ToString();
            Timer.Start();
          
                MyoSetup();
                gameCountdown();
            

        }
        

        #region Myo Setup Methods
        private void MyoSetup()
        { // communication, device, exceptions, poses
            // create the channel
            _myoChannel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub = MyoSharp.Device.Hub.Create(_myoChannel);
            // create the event handlers for connect and disconnect
            _myoHub.MyoDisconnected -= _myoHub_MyoDisconnected;
            _myoHub.MyoConnected += _myoHub_MyoConnected;
     


            // start listening 
            _myoChannel.StartListening();


        }

        private async void _myoHub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = expGetures.Text + System.Environment.NewLine +
                                    "Myo disconnected";
            });
            _myoHub.MyoConnected -= _myoHub_MyoConnected;
            _myoHub.MyoDisconnected -= _myoHub_MyoDisconnected;
        }

        private async void _myoHub_MyoConnected(object sender, MyoEventArgs e)
        {
            e.Myo.Vibrate(VibrationType.Long);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Myo Connected: " + e.Myo.Handle;
            });
            // add the pose changed event here
            e.Myo.PoseChanged += Myo_PoseChanged;
        


            // unlock the Myo so that it doesn't keep locking between our poses
            e.Myo.Unlock(UnlockType.Hold);

            try
            {
                var sequence = PoseSequence.Create(e.Myo, Pose.FingersSpread, Pose.WaveIn);
                sequence.PoseSequenceCompleted += Sequence_PoseSequenceCompleted;

            }
            catch (Exception myoErr)
            {
                string strMsg = myoErr.Message;
            }

        }
        #endregion
        #region Pose related methods
        

        private async void Sequence_PoseSequenceCompleted(object sender, PoseSequenceEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
         
            });
        }

        private async void Pose_Triggered(object sender, PoseEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
     
            });

        }

        
        private async void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            Pose curr = e.Pose;
         
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {


               
                switch (curr)
                {
                    case Pose.WaveIn:
                        tblUpdates.Text = "WaveIn";
                        if (tblUpdates.Text == expGetures.Text)
                        {
                           
                            score++;
                            await Task.Delay(300);
                            randomGesture();
                        }
                        else if (tblUpdates.Text == "WaveIn" && expGetures.Text != "WaveIn")
                        {
                      
                            await Task.Delay(300);
                            wrongImage();
                        }
                        break;
                    case Pose.Fist:
                        tblUpdates.Text = "Fist";
                        if (tblUpdates.Text == expGetures.Text)
                        {
                          
                            score++;
                            await Task.Delay(300);
                            randomGesture();
                        }
                        else if (tblUpdates.Text == "Fist" && expGetures.Text != "Fist")
                        {
                        
                            await Task.Delay(300);
                            wrongImage();
                        }
                        break;
                    case Pose.WaveOut:
                        tblUpdates.Text = "WaveOut";
                        if (tblUpdates.Text == expGetures.Text)
                        {
                           
                            score++;
                            await Task.Delay(300);
                            randomGesture();

                        }
                        else if (tblUpdates.Text == "WaveOut" && expGetures.Text != "WaveOut")
                        {
                           
                            await Task.Delay(300);
                            wrongImage();
                        }
                        break;
                    case Pose.FingersSpread:
                        tblUpdates.Text = "FingersSpread";
                        if (tblUpdates.Text == expGetures.Text)
                        {
                  
                            score++;
                            await Task.Delay(300);
                            randomGesture();
                        }
                        else if (tblUpdates.Text == "FingersSpread" && expGetures.Text != "FingersSpread")
                        {
            
                            await Task.Delay(300);
                            wrongImage();
                        }

                        break;
                    
                    case Pose.Unknown:
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion

        
        private void randomGesture()
        {         
                int randomNumber = rnd.Next(0, Gestures.Count);
                expGetures.Text = Gestures[randomNumber];
         

            scoreTB.Text = score.ToString();


            switch (Gestures[randomNumber])
            {
                case "WaveIn":

                    
                    ImageBrush brush1 = new ImageBrush();
                    brush1.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/wave-left.png", UriKind.RelativeOrAbsolute));
                    gestureImages.Source = brush1.ImageSource;

                    
                    break;
                case "Fist":
                    ImageBrush brush2 = new ImageBrush();
                    brush2.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/make-fist.png", UriKind.RelativeOrAbsolute));
                    gestureImages.Source = brush2.ImageSource;
                  

                    break;
                case "WaveOut":
                    ImageBrush brush3 = new ImageBrush();
                    brush3.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/wave-right.png", UriKind.RelativeOrAbsolute));
                    gestureImages.Source = brush3.ImageSource;

                    break;
                case "FingersSpread":
                    ImageBrush brush4 = new ImageBrush();
                    brush4.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/spread-fingers.png", UriKind.RelativeOrAbsolute));
                    gestureImages.Source = brush4.ImageSource;

                    break;
               
                default:
                    break;
            }
        }
        private async void countdownTimer(object sender, object e)
        {
            await Task.Delay(3000);
            if (time > 0)
            {
                if (time <= 10)
                {
                    time--;
                    countdown.Text = string.Format("{1}", time / 60, time % 60);
                }
                else
                {
                    time--;
                    countdown.Text = string.Format("{1}", time / 60, time % 60);
                }
            }
            else
            {
                Timer.Stop();
                this.Frame.Navigate(typeof(Leaderboard));


            }


        }

        public async void wrongImage()
        {
           
                ImageBrush brush6 = new ImageBrush();
                brush6.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/x.png", UriKind.RelativeOrAbsolute));
                gestureImages.Source = brush6.ImageSource;
                await Task.Delay(300);
                randomGesture();
                lives--;
                liveTB.Text = lives.ToString();


            if (lives==0)
            {
                ImageBrush brush7 = new ImageBrush();
                brush7.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/x.png", UriKind.RelativeOrAbsolute));
                gestureImages.Source = brush7.ImageSource;
                submitScore();                
            }
            


        }

        private async void gameCountdown()
        {
            await Task.Delay(1000);
            ImageBrush brush3 = new ImageBrush();
            brush3.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/3.png", UriKind.RelativeOrAbsolute));
            gestureImages.Source = brush3.ImageSource;
            await Task.Delay(1000);
            ImageBrush brush4 = new ImageBrush();
            brush4.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/2.png", UriKind.RelativeOrAbsolute));
            gestureImages.Source = brush4.ImageSource;
            await Task.Delay(1000);
            ImageBrush brush5= new ImageBrush();
            brush5.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/1.png", UriKind.RelativeOrAbsolute));
            gestureImages.Source = brush5.ImageSource;
            await Task.Delay(1000);
            randomGesture();

        }
        private async void submitScore()
        {

            
            string uri = App.apiURL+"/test/" + App.user + "/" + score;
            WebRequest wrGETURL = WebRequest.Create(uri);
            wrGETURL.Proxy = null;

            try
            {
                WebResponse response = await wrGETURL.GetResponseAsync();
                Stream dataStream = response.GetResponseStream();
                StreamReader objReader = new StreamReader(dataStream);
                dynamic javaResponse = (objReader.ReadToEnd());
                
                //if response = success return to sign in page.
                if (javaResponse == "success")
                {
                    this.Frame.Navigate(typeof(MainMenu));
                }
                response.Dispose();
                this.Frame.Navigate(typeof(Leaderboard), null);
            }
            catch (WebException )
            {
                //if connection failed, output message to user
                //errorMessage.Visibility = Visibility.Visible;
                //errorMessage.Text = "Failed to connect to server\nPlease check your internet connection";

            }


        }

        private void Buttons()
        {

        }

        private void btnFingers_Click(object sender, RoutedEventArgs e)
        {
            if (expGetures.Text == "FingersSpread")
            {
                score++;
                randomGesture();

            }
            else
            {
                lives--;
                wrongImage();
                  
            }
        }

       
    }
}
