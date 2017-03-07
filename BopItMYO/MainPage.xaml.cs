﻿using System;
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
        IChannel _myoChannel1;
        IHub _myoHub;
        IHub _myoHub1;
        public Random rnd = new Random();
        List<string> Gestures = new List<string>()
            {
                "WaveIn",
                "Fist",
                "WaveOut",
                "FingersSpread",
                "DoubleTap",
        };
        List<string> randomImage = new List<string>()
            {
                "ms-appx:/Assets/wave-left.png",
                "ms-appx:/Assets/make-fist.png",
                "ms-appx:/Assets/wave-right.png",
                "ms-appx:/Assets/spread-fingers.png",
                "ms-appx:/Assets/double-tap.png",
        };

        public MainPage()
        {
            this.InitializeComponent();
            randomGesture();


        }
        
        #region Myo Setup Methods
        private void btnMyo_Click(object sender, RoutedEventArgs e)
        { // communication, device, exceptions, poses
            // create the channel
            _myoChannel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub = MyoSharp.Device.Hub.Create(_myoChannel);
            // create the event handlers for connect and disconnect
            _myoHub.MyoConnected += _myoHub_MyoConnected;
            _myoHub.MyoDisconnected += _myoHub_MyoDisconnected;

            // start listening 
            _myoChannel.StartListening();


            // create the channel
            _myoChannel1 = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub1 = MyoSharp.Device.Hub.Create(_myoChannel1);
            // create the event handlers for connect and disconnect
            _myoHub1.MyoConnected += _myoHub_MyoConnected;
            _myoHub1.MyoDisconnected += _myoHub_MyoDisconnected;

            // start listening 
            _myoChannel1.StartListening();

        }

        private async void _myoHub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = tblUpdates.Text + System.Environment.NewLine +
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
                tblUpdates.Text = "Pose Sequence completed";
            });
        }

        private async void Pose_Triggered(object sender, PoseEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Pose Held: " + e.Pose.ToString();
            });

        }


        private async void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            Pose curr = e.Pose;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                tblUpdates.Text = curr.ToString();
                switch (curr)
                {
                    case Pose.WaveIn:
                        if (tblUpdates.Text==expGetures.Text)
                        {
                            randomGesture();
                        }
                        else if (tblUpdates.Text == "WaveIn" && expGetures.Text != "WaveIn")
                        {
                            wrongImage();
                        }
                        break;
                    case Pose.Fist:
                        if (tblUpdates.Text == expGetures.Text)
                        {
                            randomGesture();
                        }
                        else if (tblUpdates.Text == "Fist" && expGetures.Text != "Fist")
                        {
                            wrongImage();
                        }
                        break;
                    case Pose.WaveOut:
                        if (tblUpdates.Text == expGetures.Text)
                        {
                            randomGesture();
                        }
                        else if (tblUpdates.Text == "WaveOut" && expGetures.Text != "WaveOut")
                        {
                            wrongImage();
                        }
                        break;
                    case Pose.FingersSpread:
                        if (tblUpdates.Text == expGetures.Text)
                        {
                            randomGesture();
                        }else if (tblUpdates.Text == "FingersSpread" && expGetures.Text != "FingersSpread")
                        {
                            wrongImage();
                        }

                        break;
                    case Pose.DoubleTap:
                        if (tblUpdates.Text == expGetures.Text)
                        {
                            randomGesture();
                        }else if (tblUpdates.Text =="DoubleTap" && expGetures.Text != "DoubleTap")
                        {
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

        public void randomGesture()
        {              
            

                int randomNumber = rnd.Next(0, Gestures.Count);
                expGetures.Text = Gestures[randomNumber];

            


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
                case "DoubleTap":
                    ImageBrush brush5 = new ImageBrush();
                    brush5.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/double-tap.png", UriKind.RelativeOrAbsolute));
                    gestureImages.Source = brush5.ImageSource;

                    break;
               
                default:
                    break;
            }
        }
        public async void wrongImage()
        {
            ImageBrush brush6 = new ImageBrush();
            brush6.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/x.png", UriKind.RelativeOrAbsolute));
            gestureImages.Source = brush6.ImageSource;
            await Task.Delay(300);
            int randomNumber = rnd.Next(0, Gestures.Count);
            expGetures.Text = Gestures[randomNumber];
            int randomImages = rnd.Next(0, randomImage.Count);
            ImageBrush brush7 = new ImageBrush();
            brush7.ImageSource = new BitmapImage(new Uri(randomImage[randomImages], UriKind.RelativeOrAbsolute));
            gestureImages.Source = brush7.ImageSource;
           


        }

    }
}
