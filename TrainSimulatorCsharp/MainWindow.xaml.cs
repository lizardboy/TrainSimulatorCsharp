﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Phidgets;
using Phidgets.Events;

namespace TrainSimulatorCsharp
{
    public partial class MainWindow : Window
    {

        //Globals, sounds, and Timers

        static int secondsPerDollar = 240;
        static bool demonstrationMode = false;




        static CachedSound engine8 = new CachedSound("Engine8.wav");
        static CachedSound engine7 = new CachedSound("Engine7.wav");
        static CachedSound engine6 = new CachedSound("Engine6.wav");
        static CachedSound engine5 = new CachedSound("Engine5.wav");
        static CachedSound engine4 = new CachedSound("Engine4.wav");
        static CachedSound engine3 = new CachedSound("Engine3.wav");
        static CachedSound engine2 = new CachedSound("Engine2.wav");
        static CachedSound engine1 = new CachedSound("Engine1.wav");
        static CachedSound engine0 = new CachedSound("EngineIdle.wav");
        static CachedSound horn = new CachedSound("Horn.wav");
        static CachedSound bell = new CachedSound("BellLoop.wav");
        static CachedSound ambient = new CachedSound("AmbientAudio.wav");
        static CachedSound engineStartup = new CachedSound("EngineStart.wav");
        static CachedSound bailSound = new CachedSound("BailSound.wav");
        static CachedSound brakeSound = new CachedSound("MainBrakeApplication.wav");

        static CachedSound CornerSqueal0 = new CachedSound("CornerSqueal0.wav");
        static CachedSound CornerSqueal1 = new CachedSound("CornerSqueal1.wav");
        static CachedSound CornerSqueal2 = new CachedSound("CornerSqueal2.wav");
        static CachedSound CornerSqueal3 = new CachedSound("CornerSqueal3.wav");
        static CachedSound CornerSqueal4 = new CachedSound("CornerSqueal4.wav");
        static CachedSound CornerSqueal5 = new CachedSound("CornerSqueal5.wav");
        static CachedSound SandingSound = new CachedSound("SandingSound.wav");
        static CachedSound ambient1 = new CachedSound("AmbientAudio1.wav");
        static CachedSound coinSound = new CachedSound("CoinSound.wav");


        

        static VideoOutputWindow outWindow = new VideoOutputWindow();



        DispatcherTimer gameTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer screenSaver = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer faderTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer lightFlahser = new System.Windows.Threading.DispatcherTimer();


        static string currentImage = "none";
        static int currentFaderIteration = 0;
        static string gameState = "screenSaver";
        static bool ssFwd = true;
        static bool UIbuttonsClickable = false;
        static string selectedSegment = "none";
        static string selectedTrack = "none";
        
        //Rolling up to Field Vars
        static int iterCounter = 0;
        static double velocityDec = 0; 
        


        
        //Control Stand Input Devices
        private InterfaceKit my16_16_0;
        private InterfaceKit my8_8_8;
        private Analog myAnalogOut;


        //Control stand Input Variables
        static bool engineRunSwitch;
        static bool foaSwitch;
        static bool attendSwitch;
        static bool resetSwitch;
        static bool pcsPressed;
        static bool bellPulled;
        static bool sandingPressed;
        static bool sandingToggle;
        static bool hornPulled;
        static bool pedalPressed;
        static bool bailActivated;
        static int indPosition = 0;
        static int brakePosition;

        static bool bailPlaying = false;
        static bool hornPlaying = false;
        static bool bellPlaying = false;
        static bool brakeSoundPlaying = false;
        static bool sandingPlaying = false;


        //Game Data Variables
        static double velocity = 8; 
        static int throttlePosition = 0;
        static int dynamicPosition = 0;


        //Pneumatics Globals
        static double mainBrakePressureIndicated = 90;
        static double indBrakePressureIndicated = 0;
        static double mainBrakePressureRequested = 90;
        static double indBrakePressureRequested = 0;
        static int mainDir = 1; //0 decreasing, 1 steady, 2 ascending
        static int indDir = 1; //0 decreasing, 1 steady, 2 ascending
        static bool penaltyBrake;
        static int penaltyBrakeCountDown;


        
        //Bend Globals
        static int bendState;
        static int bendIndex = 0;
        static int[,] bendList = { 
            { 0, 0 },
            { 100, 2 }, 
            { 115, 0 }, 
            { 135, 3 }, 
            { 154, 0 }, 
            { 162, 2 }, 
            { 169, 0 },
            { 173, 3 }, 
            { 192, 0 },
            { 198, 2 }, 
            { 219, 0 },
            { 223, 2 }, 
            { 247, 0 },
            { 260, 2 }, 
            { 273, 0 },
            { 278, 1 }, 
            { 293, 0 },
            { 308, 2 }, 
            { 317, 0 },
            { 341, 1 }, 
            { 366, 0 },
            { 373, 1 }, 
            { 386, 0 },
            { 417, 2 }, 
            { 433, 0 },
            { 437, 2 }, 
            { 445, 0 },
            { 472, 1 }, 
            { 496, 0 },
            { 565, 2 }, 
            { 581, 0 },
            { 589, 1 }, 
            { 606, 0 },
            { 625, 2 }, 
            { 637, 0 },
            { 677, 1 }, 
            { 692, 0 },
            { 702, 1 }, 
            { 716, 0 },
            { 760, 1 }, 
            { 764, 0 },
            { 781, 2 }, 
            { 789, 0 },
            { 805, 3 }, 
            { 830, 0 },
            { 838, 2 }, 
            { 851, 0 },
            { 856, 1 }, 
            { 866, 0 },
            { 872, 2 }, 
            { 895, 0 },
            { 900, 3 }, 
            { 908, 0 },
            { 925, 2 }, 
            { 943, 0 },
            { 949, 2 }, 
            { 980, 0 },
            { 995, 1 }, 
            { 1015, 0 },
            { 1019, 1 }, 
            { 1028, 0 },
            { 1035, 1 },
            { 1056, 0 },
            { 1095, 1 }, 
            { 1106, 0 },
            { 1128, 1 }, 
            { 1134, 0 },
            { 1146, 1 }, 
            { 1166, 0 },
            { 1174, 1 }, 
            { 1188, 0 },
            { 1193, 1 }, 
            { 1218, 0 },
            { 1243, 2 }, 
            { 1248, 0 },
            { 1265, 1 },
            { 1281, 0 },
            { 1378, 1 },
            { 1407, 0 },
            { 1450, 1 },
            { 1490, 0 },
            { 1520, 2 }, 
            { 1540, 0 },
            { 1779, 1 }, 
            { 1816, 0 },
            { 1999, 1 }, 
            { 2006, 0 },
            { 2022, 1 }, 
            { 2026, 0 },
            { 2034, 1 }, 
            { 2040, 0 },
            { 2092, 1 }, 
            { 2098, 0 },
            { 2119, 2 }, 
            { 2138, 0 },
            { 2144, 3 }, 
            { 2155, 0 },
            { 2173, 2 }, 
            { 2187, 0 },
            { 2217, 2 }, 
            { 2232, 0 },
            { 2238, 2 }, 
            { 2279, 0 },
            { 2288, 2 }, 
            { 2298, 0 },
            { 2308, 1 }, 
            { 2318, 0 },
            { 2360, 3 }, 
            { 2369, 0 },
            { 2380, 1 }, 
            { 2397, 0 },
            { 2420, 1 }, 
            { 2433, 0 },
            { 2439, 2 }, 
            { 2458, 0 },
            { 2465, 3 }, 
            { 2474, 0 },
            { 2492, 2 }, 
            { 2502, 0 },
            { 2509, 1 }, 
            { 2519, 0 },
            { 2520, 2 }, 
            { 2534, 0 },
            { 2540, 1 }, 
            { 2549, 0 },
            { 2555, 1 }, 
            { 2563, 0 },
            { 2573, 2 }, 
            { 2590, 0 },
            { 2601, 1 }, 
            { 2607, 0 },
            { 2647, 1 }, 
            { 2662, 0 },
            { 2678, 2 }, 
            { 2690, 0 },
            { 2764, 1 }, 
            { 2771, 0 }
            };

        static int continueTimeLeft = 0; 
        static int timeLeft = 1500;  ///0;

        static int descriptionPosition = 0;
        static int scrollPix = 0;

        //static CachedSound engineStartup = new CachedSound();

        //Application Startup


        public MainWindow()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {

            
            startGameTimer();
            outWindow.Show();
            outWindow.goldenToFieldVideo.Stop();
            outWindow.goldenToFieldVideo.Opacity = 0;
            //EngageScreenSaver();
           //// Mouse.OverrideCursor = Cursors.None;
            WindowState = WindowState.Maximized;
            my16_16_0 = new InterfaceKit();
            my8_8_8 = new InterfaceKit();
            myAnalogOut = new Analog();

            my16_16_0.open(468344);
            ////my16_16_0.open(344671);
            //// my8_8_8.open(327859);
                 my8_8_8.open(451950);
            //// myAnalogOut.open(282774);

            my16_16_0.waitForAttachment(3000);
            my8_8_8.waitForAttachment(3000);
          ///  myAnalogOut.waitForAttachment(3000);

          ////  myAnalogOut.outputs[0].Enabled = true;
          ////  myAnalogOut.outputs[1].Enabled = true;
        ///   myAnalogOut.outputs[0].Voltage = 0;
        ///    myAnalogOut.outputs[1].Voltage = 0;
            my8_8_8.SensorChange += new SensorChangeEventHandler(myPotChanged);
            my8_8_8.InputChange += new InputChangeEventHandler(my8InputChanged);
            my16_16_0.InputChange += new InputChangeEventHandler(my16InputChanged);
            this.Focus();
            StartupLeverPositions();
            

            my16_16_0.outputs[6] = true;
            my16_16_0.outputs[7] = true;
            my16_16_0.outputs[8] = true;
            my16_16_0.outputs[9] = true;
            my16_16_0.outputs[10] = true;
            my16_16_0.outputs[11] = true;
            my16_16_0.outputs[12] = true;
            my16_16_0.outputs[13] = true;
           my16_16_0.outputs[14] = true;
            my16_16_0.outputs[15] = true;

            descriptionTextContainer.Visibility = Visibility.Hidden;
            toggleLocationSelectionButtons(Visibility.Hidden);
            toggleHUD();

            getMainBrakeState();
            indPosition = 0;

            GoldenEx.Play();
            GoldenEx.Pause();
            GlenogleEx.Play();
            GlenogleEx.Pause();
            PalliserEx.Play();
            PalliserEx.Pause();
            LeanchoilEx.Play();
            LeanchoilEx.Pause();
            OttertailEx.Play();
            OttertailEx.Pause();
            DescriptionScreenFootage.Play();
            DescriptionScreenFootage.Pause();

            //AudioPlaybackEngine.Instance.PlaySound(engine0);

           // HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
          //  HwndTarget hwndTarget = hwndSource.CompositionTarget;
          //  hwndTarget.RenderMode = RenderMode.SoftwareOnly;

        }

        private void StartupLeverPositions()
        {
            this.Dispatcher.Invoke(new Action(delegate()
           {

        ///           if (my8_8_8.sensors[6].Value >= 605)
        ///           {
                       throttlePosition = 0;
       ///            }
        ///           else if (my8_8_8.sensors[6].Value >= 563)
         ///          {
       ///                throttlePosition = 1;
        ///           }
      ///             else if (my8_8_8.sensors[6].Value >= 511)
       ///            {
      ///                 throttlePosition = 2;
     ///              }
     ///              else if (my8_8_8.sensors[6].Value >= 458)
     ///              {
      ///                 throttlePosition = 3;
     ///              }
      ///             else if (my8_8_8.sensors[6].Value >= 409)
     ///              {
      ///                 throttlePosition = 4;
     ///              }
     ///              else if (my8_8_8.sensors[6].Value >= 359)
     ///              {
     ///                  throttlePosition = 5;
      ///             }
      ///             else if (my8_8_8.sensors[6].Value >= 313)
      ///             {
     ///                  throttlePosition = 6;
      ///             }
      ///             else if (my8_8_8.sensors[6].Value >= 268)
      ///             {
     ///                  throttlePosition = 7;
     ///              }
    ///               else if (my8_8_8.sensors[6].Value >= 231)
    ///               {
    ///                   throttlePosition = 8;
   ///                }
    ///               else if (my8_8_8.sensors[6].Value <231)
    ///               {
    ///                   throttlePosition = 9;
   ///                }

   ///                if (my8_8_8.sensors[7].Value >= 623)
    ///               {
                       dynamicPosition = 0;
///                    }
///                   else if (my8_8_8.sensors[7].Value >= 547)
 ///                  {
 ///                      dynamicPosition = 1;
 ///                  }
 ///                  else if (my8_8_8.sensors[7].Value >= 469)
 ///                  {
  ///                     dynamicPosition = 2;
 ///                  }
 ///                  else if (my8_8_8.sensors[7].Value >= 393)
 ///                  {
 ///                      dynamicPosition = 3;
 ///                  }
 ///                  else if (my8_8_8.sensors[7].Value < 393)
 ///                  {
 ///                      dynamicPosition = 4;
 ///                  }



  ///                 if (my8_8_8.inputs[1] == true)
  ///                 {
                      indPosition = 0;
 ///                  }
///                   else if (my8_8_8.inputs[2] == true)
 ///                  {
 ///                      indPosition = 1;
 ///                  }
///                   else if (my8_8_8.inputs[3] == true)
 ///                  {
 ///                      indPosition = 2;
 ///                  }
///                   else
 ///                  {
 ///                      indPosition = 3;
 ///                  }
                      outWindow.indPositionLabel.Content = "Ind Position: " + Convert.ToString(indPosition);

                      

                   outWindow.dynamicPositionLabel.Content = "Dynamic Position: " + Convert.ToString(dynamicPosition);
                   outWindow.throttlePositionLabel.Content = "Throttle Position: " + Convert.ToString(throttlePosition);
           }));
                   

        }

        private void ScreenSaver(object sender, EventArgs e)
        {

            if (ssFwd == true)
            {
                TranslateTheMediaElement(0, 0, -648, -528, 50000, 0.3, 0.3, outWindow.forst1);
                TranslateTheMediaElement(0, 0, 648, -528, 50000, 0.3, 0.3, outWindow.forst2);
                TranslateTheMediaElement(0, 0, -648, 528, 50000, 0.3, 0.3, outWindow.forst3);
                ssFwd = false;
            }
            else
            {

                TranslateTheMediaElement(-648, -528, 0, 0, 50000, 0.3, 0.3, outWindow.forst1);
                TranslateTheMediaElement(648, -528, 0, 0, 50000, 0.3, 0.3, outWindow.forst2);
                TranslateTheMediaElement(-648, 528, 0, 0, 50000, 0.3, 0.3, outWindow.forst3);
                ssFwd = true;
            }

        }

        private void EngageScreenSaver()
        {
            screenSaver.Interval = TimeSpan.FromMilliseconds(50000);
            screenSaver.Tick += new EventHandler(ScreenSaver);
            screenSaver.Start();
            ScreenSaver(this, null);

            faderTimer.Interval = TimeSpan.FromMilliseconds(500);
            faderTimer.Tick += new EventHandler(ScreenSaverFader);
            faderTimer.Start();
        }


        //UI Animation
        private void selectTrack(object sender, MouseButtonEventArgs e)
        {
            if (UIbuttonsClickable == true)
            {
                var mySender = sender as Image;
                unhighlightButton(mySender, e);
                gameState = "trackDescription";
                UIbuttonsClickable = false;
                descriptionTextContainer.Visibility = Visibility.Visible;
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(1146, 0, 0, 0, 400, 1, 0, selectATrackText); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 0, -260, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(1000));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(331, 0, 0, 0, 200, 1, 0, goldenToFieldIcon); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1183, 0, 300, 0, 1, exampleVideoContainer); }), TimeSpan.FromMilliseconds(500));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1173, 0, 300, 0, 1, selectedTextDivider); }), TimeSpan.FromMilliseconds(700));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1129, 0, 200, 1, 0, goldenToFieldText); }), TimeSpan.FromMilliseconds(400));

                //TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1165, 0, 300, 0, 1, goldenToFieldDescriptionText); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 201, 0, 300, 0, 1, backLabel); }), TimeSpan.FromMilliseconds(1100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -211, 0, 300, 0, 1, selectLabel); }), TimeSpan.FromMilliseconds(1100));
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(2000));


                TimedAction.ExecuteWithDelay(new Action(delegate { DescriptionScreenFootage.Margin = new Thickness(114, 645, 406, 97); }), TimeSpan.FromMilliseconds(800));
                

                TimedAction.ExecuteWithDelay(new Action(delegate { DescriptionScreenFootage.Play(); }), TimeSpan.FromMilliseconds(800));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, DescriptionScreenFootage, 1000); }), TimeSpan.FromMilliseconds(1100));
                //FadeTheMediaElement( 0, 1, outWindow.testMediaElement, 3000);
                //TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                scrollPix = -260;
            }

        }

        private void scrollDescriptionText(object sender, MouseButtonEventArgs e)
        {
            if (gameState == "trackDescription" && UIbuttonsClickable == true)
            {
                if (descriptionPosition == 0)
                {
                    TranslateTheMediaElement(0, -260, 0, -520, 400, 0, 1, descriptionText);
                    descriptionPosition = 1;
                    scrollPix = -520;
                }
                else if (descriptionPosition == 1)
                {
                    TranslateTheMediaElement(0, -520, 0, -780, 400, 0, 1, descriptionText);
                    descriptionPosition = 2;
                    scrollPix = -780;

                }
                else if (descriptionPosition == 2)
                {
                    TranslateTheMediaElement(0, -780, 0, -1040, 400, 0, 1, descriptionText);
                    scrollPix = -1040;
                    descriptionPosition = 3;

                }
                else
                {
                    TranslateTheMediaElement(0, -1040, 0, -260, 400, 0, 1, descriptionText);
                    descriptionPosition = 0;
                    scrollPix = -260;
                }
                

            }
        }

        private void backSelect(object sender, MouseButtonEventArgs e)
        {
            var mySender = sender as Image;
            if (gameState == "trackDescription" && UIbuttonsClickable == true)
            {
                backLabel.Opacity = 1;
                UIbuttonsClickable = false;

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(201, 0, 0, 0, 300, 1, 0, backLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 1, 0, selectLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 1146, 0, 600, 0, 1, selectATrackText); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1129, 0, 0, 0, 200, 1, 0, goldenToFieldText); }), TimeSpan.FromMilliseconds(400));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 331, 0, 200, 0, 1, goldenToFieldIcon); }), TimeSpan.FromMilliseconds(1100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1183, 0, 0, 0, 300, 1, 0, exampleVideoContainer); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1173, 0, 0, 0, 300, 1, 0, selectedTextDivider); }), TimeSpan.FromMilliseconds(400));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, scrollPix, 0, 0, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionTextContainer.Visibility = Visibility.Hidden;}), TimeSpan.FromMilliseconds(500));
                descriptionPosition = 0;

                gameState = "trackSelection";
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(1100));
                
                FadeTheMediaElement(1, 0, DescriptionScreenFootage, 200);
                TimedAction.ExecuteWithDelay(new Action(delegate { DescriptionScreenFootage.Margin = new Thickness(1269, 1013, -489, -273); }), TimeSpan.FromMilliseconds(1000));

            }
            else if (gameState == "startLocationSelection" && UIbuttonsClickable == true)
            {
                var curLabel = new Image();
                backLabel.Opacity = 1;
                UIbuttonsClickable = false;
                double startLocation = 0;
                if (selectedSegment == selectGolden.Name)
                {
                    startLocation = selectGolden.Margin.Left;
                    curLabel = goldenLabel;
                    FadeTheMediaElement(1, 0, GoldenEx, 250);
                    FadeTheMediaElement(1, 0, goldenCover, 100);

                }
                else if (selectedSegment == selectGlenogle.Name)
                {
                    startLocation = selectGlenogle.Margin.Left;
                    curLabel = glenogleLabel;
                    FadeTheMediaElement(1, 0, GlenogleEx, 250);
                    FadeTheMediaElement(1, 0, glenogleCover, 100);
                }
                else if (selectedSegment == selectPalliser.Name)
                {
                    startLocation = selectPalliser.Margin.Left;
                    curLabel = palliserLabel;
                    FadeTheMediaElement(1, 0, PalliserEx, 250);
                    FadeTheMediaElement(1, 0, palliserCover, 100);
                }
                else if (selectedSegment == selectLeanchoil.Name)
                {
                    startLocation = selectLeanchoil.Margin.Left;
                    curLabel = leanchoilLabel;
                    FadeTheMediaElement(1, 0, LeanchoilEx, 250);
                    FadeTheMediaElement(1, 0, leanchoilCover, 100);
                }
                else if (selectedSegment == selectOttertail.Name)
                {
                    startLocation = selectOttertail.Margin.Left;
                    curLabel = ottertailLabel;
                    FadeTheMediaElement(1, 0, OttertailEx, 250);
                    FadeTheMediaElement(1, 0, ottertailCover, 100);
                }

                startLocation = startLocation - 1348;

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, -12, -1296, 0, 200, 0.5, 0.5, curLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-909, -46, -1183, 0, 300, 0.5, 0.5, exampleVideoContainer); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1173, 0, 300, 0, 1, selectedTextDivider); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 600, 1, 0, locationSelectionBar); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLocationSelectionButtons(Visibility.Hidden); }), TimeSpan.FromMilliseconds(200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(startLocation, 0, -1600, 0, 200, 1, 0, selectionIndicatorBar); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1297, 0, 0, 0, 500, 1, 0, selectStartPoint); }), TimeSpan.FromMilliseconds(800));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-2500, 0, -1183, 0, 500, 0, 1, goldenToFieldText); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, goldenLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, glenogleLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, palliserLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, leanchoilLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, ottertailLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, fieldLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, DescriptionScreenFootage, 1000); }), TimeSpan.FromMilliseconds(1700));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 0, -260, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(1500));
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionTextContainer.Visibility = Visibility.Visible; }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 1, 0, startLabel); }), TimeSpan.FromMilliseconds(0));//
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -211, 0, 300, 1, 0, selectLabel); }), TimeSpan.FromMilliseconds(300));

                gameState = "trackDescription";

                TimedAction.ExecuteWithDelay(new Action(delegate {

                    GoldenEx.Pause();
                    GlenogleEx.Pause();
                    PalliserEx.Pause();
                    LeanchoilEx.Pause();
                    OttertailEx.Pause();

                    GlenogleEx.Margin = new Thickness(1269, 1013, -489, -273);
                    PalliserEx.Margin = new Thickness(1269, 1013, -489, -273);
                    LeanchoilEx.Margin = new Thickness(1269, 1013, -489, -273);
                    OttertailEx.Margin = new Thickness(1269, 1013, -489, -273);

                    GoldenEx.Opacity = 0.01;
                    GlenogleEx.Opacity = 0.01;
                    PalliserEx.Opacity = 0.01;
                    LeanchoilEx.Opacity = 0.01;
                    OttertailEx.Opacity = 0.01;

                }), TimeSpan.FromMilliseconds(300));

                

            }
            
        }

        private void nextSelect(object sender, MouseButtonEventArgs e)
        {
            var mySender = sender as Image;
            if (gameState == "trackDescription" && UIbuttonsClickable == true)
            {
                UIbuttonsClickable = false;
                selectLabel.Opacity = 1;
                FadeTheMediaElement(1, 0, DescriptionScreenFootage, 200);
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 1, 0, selectLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -211, 0, 300, 1, 0, startLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1173, 0, -909, -46, 300, 0.5, 0.5, exampleVideoContainer); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1173, 0, 0, 0, 300, 1, 0, selectedTextDivider); }), TimeSpan.FromMilliseconds(400));
                //TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1165, 0, 0, 0, 300, 1, 0, goldenToFieldDescriptionText); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 600, 0, 1, locationSelectionBar); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLocationSelectionButtons(Visibility.Visible); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1600, 0, -1296, 0, 500, 0, 1, selectionIndicatorBar);}), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1297, 0, 500, 0, 1, selectStartPoint); }), TimeSpan.FromMilliseconds(700));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1183, 0, -2500, 0, 500, 1, 0, goldenToFieldText); }), TimeSpan.FromMilliseconds(200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, goldenLabel); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, glenogleLabel); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, palliserLabel); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, leanchoilLabel); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, ottertailLabel); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, fieldLabel); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -1296, -12, 200, 0.5, 0.5, goldenLabel); }), TimeSpan.FromMilliseconds(1800));
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(2800));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, scrollPix, 0, 0, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionTextContainer.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(500));
                descriptionPosition = 0;
                scrollPix = -260;

                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, goldenCover, 200); }), TimeSpan.FromMilliseconds(1500));

                GoldenEx.Margin = new Thickness(388, 598, 392, 142);

                TimedAction.ExecuteWithDelay(new Action(delegate {

                    GoldenEx.Opacity = 0.1;
                    GoldenEx.Play();
                    GlenogleEx.Play();
                    PalliserEx.Play();
                    LeanchoilEx.Play();
                    OttertailEx.Play();
                
                }), TimeSpan.FromMilliseconds(1500));

                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0.1, 1, GoldenEx, 1000); }), TimeSpan.FromMilliseconds(2000));
                
                TimedAction.ExecuteWithDelay(new Action(delegate {


                    GlenogleEx.Margin = new Thickness(388, 598, 392, 142);
                    PalliserEx.Margin = new Thickness(388, 598, 392, 142);
                    LeanchoilEx.Margin = new Thickness(388, 598, 392, 142);
                    OttertailEx.Margin = new Thickness(388, 598, 392, 142);
                
                }), TimeSpan.FromMilliseconds(3000));

                selectedSegment = "selectGolden";
                gameState = "startLocationSelection";
      
    
            }
            else if (gameState == "startLocationSelection" && UIbuttonsClickable == true)
            {
                UIbuttonsClickable = false;
                selectLabel.Opacity = 1;
                var curLabel = new Image();
                double startLocation = 0;
                if (selectedSegment == selectGolden.Name)
                {
                    startLocation = selectGolden.Margin.Left;
                    curLabel = goldenLabel;
                    FadeTheMediaElement(1, 0, GoldenEx, 250);
                    FadeTheMediaElement(1, 0, goldenCover, 100);

                }
                else if (selectedSegment == selectGlenogle.Name)
                {
                    startLocation = selectGlenogle.Margin.Left;
                    curLabel = glenogleLabel;
                    FadeTheMediaElement(1, 0, GlenogleEx, 250);
                    FadeTheMediaElement(1, 0, glenogleCover, 100);
                }
                else if (selectedSegment == selectPalliser.Name)
                {
                    startLocation = selectPalliser.Margin.Left;
                    curLabel = palliserLabel;
                    FadeTheMediaElement(1, 0, PalliserEx, 250);
                    FadeTheMediaElement(1, 0, palliserCover, 100);
                }
                else if (selectedSegment == selectLeanchoil.Name)
                {
                    startLocation = selectLeanchoil.Margin.Left;
                    curLabel = leanchoilLabel;
                    FadeTheMediaElement(1, 0, LeanchoilEx, 250);
                    FadeTheMediaElement(1, 0, leanchoilCover, 100);
                }
                else if (selectedSegment == selectOttertail.Name)
                {
                    startLocation = selectOttertail.Margin.Left;
                    curLabel = ottertailLabel;
                    FadeTheMediaElement(1, 0, OttertailEx, 250);
                    FadeTheMediaElement(1, 0, ottertailCover, 100);
                }

                startLocation = startLocation - 1348;
                outWindow.goldenToFieldVideo.Visibility = Visibility.Visible;

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, -12, -1296, 0, 200, 0.5, 0.5, curLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-909, -46, -2000, -46, 300, 1, 0, exampleVideoContainer); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 600, 1, 0, locationSelectionBar); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLocationSelectionButtons(Visibility.Hidden); }), TimeSpan.FromMilliseconds(200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(startLocation, 0, -1500, 0, 500, 1, 0, selectionIndicatorBar); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1297, 0, -2600, 0, 500, 1, 0, selectStartPoint); }), TimeSpan.FromMilliseconds(800));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, goldenLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, glenogleLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, palliserLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, leanchoilLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, ottertailLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, fieldLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(3000));

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(201, 0, 0, 0, 300, 0, 1, backLabel); }), TimeSpan.FromMilliseconds(300));

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 0, 1, startLabel); }), TimeSpan.FromMilliseconds(300));//

                TimedAction.ExecuteWithDelay(new Action(delegate
                {

                    GoldenEx.Pause();
                    GlenogleEx.Pause();
                    PalliserEx.Pause();
                    LeanchoilEx.Pause();
                    OttertailEx.Pause();
                    DescriptionScreenFootage.Pause();

                    GlenogleEx.Margin = new Thickness(1269, 1013, -489, -273);
                    PalliserEx.Margin = new Thickness(1269, 1013, -489, -273);
                    LeanchoilEx.Margin = new Thickness(1269, 1013, -489, -273);
                    OttertailEx.Margin = new Thickness(1269, 1013, -489, -273);

                    GoldenEx.Opacity = 0.01;
                    GlenogleEx.Opacity = 0.01;
                    PalliserEx.Opacity = 0.01;
                    LeanchoilEx.Opacity = 0.01;
                    OttertailEx.Opacity = 0.01;

                }), TimeSpan.FromMilliseconds(300));

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, -564, 0, 400, 1200, 1, 0, selectionScreenBackground); DescriptionScreenFootage.Margin = new Thickness(1269, 1013, -489, -273); }), TimeSpan.FromMilliseconds(1700));
                TimedAction.ExecuteWithDelay(new Action(delegate { selectionScreenBackground.Visibility = Visibility.Hidden; toggleExampleVideoVisibility(Visibility.Hidden); }), TimeSpan.FromMilliseconds(3000));

                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0,false); }), TimeSpan.FromMilliseconds(4000));
            }

        }

        private void segmentSelect(object sender, MouseButtonEventArgs e)
        {
            var mySender = sender as Rectangle;
            if (mySender.Name != selectedSegment && UIbuttonsClickable == true)
            {
                UIbuttonsClickable = false;
                double startLocation = 0;
                double endLocation = mySender.Margin.Left;
                var curLabel = new Image();
                var newLabel = new Image();
                if (selectedSegment == selectGolden.Name)
                {
                    startLocation = selectGolden.Margin.Left;
                    curLabel = goldenLabel;
                    FadeTheMediaElement(1, 0, GoldenEx, 250);
                    FadeTheMediaElement(1, 0, goldenCover, 100);
                }
                else if (selectedSegment == selectGlenogle.Name)
                {
                    startLocation = selectGlenogle.Margin.Left;
                    curLabel = glenogleLabel;
                    FadeTheMediaElement(1, 0, GlenogleEx, 250);
                    FadeTheMediaElement(1, 0, glenogleCover, 100);
                }
                else if (selectedSegment == selectPalliser.Name)
                {
                    startLocation = selectPalliser.Margin.Left;
                    curLabel = palliserLabel;
                    FadeTheMediaElement(1, 0, PalliserEx, 250);
                    FadeTheMediaElement(1, 0, palliserCover, 100);
                }
                else if (selectedSegment == selectLeanchoil.Name)
                {
                    startLocation = selectLeanchoil.Margin.Left;
                    curLabel = leanchoilLabel;
                    FadeTheMediaElement(1, 0, LeanchoilEx, 250);
                    FadeTheMediaElement(1, 0, leanchoilCover, 100);
                }
                else if (selectedSegment == selectOttertail.Name)
                {
                    startLocation = selectOttertail.Margin.Left;
                    curLabel = ottertailLabel;
                    FadeTheMediaElement(1, 0, OttertailEx, 250);
                    FadeTheMediaElement(1, 0, ottertailCover, 100);
                }

                if (mySender.Name == selectGolden.Name)
                {
                    newLabel = goldenLabel;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, GoldenEx, 250); FadeTheMediaElement(0, 1, goldenCover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (mySender.Name == selectGlenogle.Name)
                {
                    newLabel = glenogleLabel;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, GlenogleEx, 250); FadeTheMediaElement(0, 1, glenogleCover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (mySender.Name == selectPalliser.Name)
                {
                    newLabel = palliserLabel;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, PalliserEx, 250); FadeTheMediaElement(0, 1, palliserCover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (mySender.Name == selectLeanchoil.Name)
                {
                    newLabel = leanchoilLabel;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, LeanchoilEx, 250); FadeTheMediaElement(0, 1, leanchoilCover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (mySender.Name == selectOttertail.Name)
                {
                    newLabel = ottertailLabel;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, OttertailEx, 250); FadeTheMediaElement(0, 1, ottertailCover, 250); }), TimeSpan.FromMilliseconds(250));
                }


                startLocation = startLocation - 1348;
                endLocation = endLocation - 1348;

                TranslateTheMediaElement(startLocation, 0, endLocation, 0, 500, 0.5, 0.5, selectionIndicatorBar);
                TimedAction.ExecuteWithDelay(new Action(delegate {UIbuttonsClickable = true ; }), TimeSpan.FromMilliseconds(500));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, -12, -1296, 0, 200, 0.5, 0.5, curLabel); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -1296, -12, 200, 0.5, 0.5, newLabel); }), TimeSpan.FromMilliseconds(200));
                selectedSegment = mySender.Name;
            }
        }

        private void ThumbnailMediaEnded(object sender, RoutedEventArgs e)
        {
            var myMedia = sender as MediaElement;
            myMedia.Position = new TimeSpan(0);
            myMedia.Play();
        }

        private void toggleLocationSelectionButtons(Visibility myVis)
        {
            selectGolden.Visibility = myVis;
            selectGlenogle.Visibility = myVis;
            selectPalliser.Visibility = myVis;
            selectLeanchoil.Visibility = myVis;
            selectOttertail.Visibility = myVis;

            goldenCover.Visibility = myVis;
            glenogleCover.Visibility = myVis;
            palliserCover.Visibility = myVis;
            leanchoilCover.Visibility = myVis;
            ottertailCover.Visibility = myVis;
        }

        private void toggleExampleVideoVisibility(Visibility myVis)
        {
            GoldenEx.Visibility = myVis;
            GlenogleEx.Visibility = myVis;
            PalliserEx.Visibility = myVis;
            LeanchoilEx.Visibility = myVis;
            OttertailEx.Visibility = myVis;
            DescriptionScreenFootage.Visibility = myVis;
        }

        private void highlightButton(object sender, MouseButtonEventArgs e)
        {
            if (UIbuttonsClickable == true)
            {
                var mySender = sender as Image;
                mySender.Opacity = 0.5;
            }
        }

        private void unhighlightButton(object sender, MouseEventArgs e)
        {
            if (UIbuttonsClickable == true)
            {

                var mySender = sender as Image;

                mySender.Opacity = 1;
            }
        }

        private void highlightRectangle(object sender, MouseButtonEventArgs e)
        {
            if (UIbuttonsClickable == true)
            {
                var mySender = sender as Rectangle;
                if (mySender.Name == backButton.Name)
                {
                    backLabel.Opacity = 0.5;
                }
                else if (mySender.Name == selectButton.Name)
                {
                    selectLabel.Opacity = 0.5;
                }
            }
        }

        private void unhighlightRectangle(object sender, MouseEventArgs e)
        {
            if (UIbuttonsClickable == true)
            {
                var mySender = sender as Rectangle;
                if (mySender.Name == backButton.Name)
                {
                    backLabel.Opacity = 1;
                }
                else if (mySender.Name == selectButton.Name)
                {
                    selectLabel.Opacity = 1;
                }
            }
        }

        private void ScreenSaverFader(object sender, EventArgs e)
        {
            if (currentFaderIteration == 1 && currentImage != "none")
            {
                if (currentImage == "forst1")
                {
                    currentImage = "forst2";
                    FadeTheMediaElement( 0, 1, outWindow.forst2, 3000);
                    FadeTheMediaElement( 1, 0, outWindow.forst1, 3000);
                }
                else if (currentImage == "forst2")
                {
                    currentImage = "forst3";
                    FadeTheMediaElement( 0, 1, outWindow.forst3, 3000);
                    FadeTheMediaElement( 1, 0, outWindow.forst2, 3000);
                }
                else if (currentImage == "forst3")
                {
                    currentImage = "forst1";
                    FadeTheMediaElement(0, 1, outWindow.forst1, 3000);
                    FadeTheMediaElement(1, 0, outWindow.forst3, 3000);
                }

            }
            else if (currentFaderIteration == 0)
            {
                FadeTheMediaElement( 0, 1, outWindow.forst1, 3000);
            }


            if (currentFaderIteration == 40 && currentImage == "none" && gameState == "screenSaver")
            {
                currentFaderIteration = 0;
                currentImage = "forst1";
            }
            else if (currentFaderIteration == 40 && currentImage != "none" && gameState == "screenSaver")
            {
                currentFaderIteration = 0;
            }
            else if (gameState != "screenSaver" && currentFaderIteration >= 7)
            {
                if (currentImage == "forst1" || currentImage == "none")
                {
                    FadeTheMediaElement(1, 0, outWindow.forst1, 3000);
                }
                if (currentImage == "forst2")
                {
                    FadeTheMediaElement(1, 0, outWindow.forst2, 3000);

                }
                if (currentImage == "forst3")
                {
                    FadeTheMediaElement(1, 0, outWindow.forst3, 3000);
                }
                currentImage = "none";
                faderTimer.Tick -= new EventHandler(ScreenSaverFader);
                screenSaver.Tick -= new EventHandler(ScreenSaver);
                faderTimer.Stop();
                screenSaver.Stop();
                currentFaderIteration = 0;
                return;
            }
            currentFaderIteration += 1;
        }

        private void LoadInButtons()
        {
            //FadeTheMediaElement(1, 0, splashLogo, 300);
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 0, -564, 800, 0.5, 0.5, selectionScreenBackground); }), TimeSpan.FromMilliseconds(0));
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 1146, 0, 600, 0, 1, selectATrackText); }), TimeSpan.FromMilliseconds(800));
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 331, 0, 400, 0, 1, goldenToFieldIcon); }), TimeSpan.FromMilliseconds(1200));
            gameState = "trackSelection";
            TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(1100));
        }

        public static void TranslateTheMediaElement(double startX, double startY, double endX, double endY, int timeSpanMilli, double acceleration, double deceleration, UIElement myMediaElement)
        {
            TimeSpan dur = TimeSpan.FromMilliseconds(timeSpanMilli);
            TransformGroup grp = new TransformGroup();
            TranslateTransform trans = new TranslateTransform();
            Point myStartPoint = new Point(0, 0);
            Point myEndPoint = new Point(0, 0);
            myStartPoint.X = startX;
            myStartPoint.Y = startY;
            myEndPoint.X = endX;
            myEndPoint.Y = endY;
            grp.Children.Add(trans);
           
            myMediaElement.RenderTransform = grp;
            var aniX = new DoubleAnimation
            {
                From = myStartPoint.X,
                To = myEndPoint.X,
                Duration = dur,
                AccelerationRatio = acceleration,
                DecelerationRatio = deceleration,
            };
            var aniY = new DoubleAnimation
            {
                From = myStartPoint.Y,
                To = myEndPoint.Y,
                Duration = dur,
                AccelerationRatio = acceleration,
                DecelerationRatio = deceleration,
                

            };
            trans.BeginAnimation(TranslateTransform.XProperty, aniX);
            trans.BeginAnimation(TranslateTransform.YProperty, aniY);

        }
         
        public static void FadeTheMediaElement(double from, double to, UIElement myMediaElement, int fadeTimeMilli)
            
        {
            TimeSpan dur = TimeSpan.FromMilliseconds(fadeTimeMilli);
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
    
                da.From = from;
                da.To = to;

                da.Duration = dur;
                sb.Children.Add(da);
                Storyboard.SetTargetProperty(da, new PropertyPath(UIElement.OpacityProperty));
                Storyboard.SetTarget(da, myMediaElement);
                sb.Begin();
        }

        public static void AnimateWidthProperty(double from, double to, int fadeTimeMilli, double acceleration, double deceleration, Rectangle myRectangle)
        {
            TimeSpan dur = TimeSpan.FromMilliseconds(fadeTimeMilli);
            DoubleAnimation da = new DoubleAnimation();
            da.FillBehavior = FillBehavior.Stop;
            da.From = from;
            da.To = to;
            da.AccelerationRatio = acceleration;
            da.DecelerationRatio = deceleration;
            da.Duration = dur;
            
            da.Completed += (s, e) =>
            {
                myRectangle.BeginAnimation(Rectangle.WidthProperty, null);
                myRectangle.Width = to;
            };
            
            myRectangle.BeginAnimation(Rectangle.WidthProperty, da);
        }
 
        public static void ResizeTheMediaElement(double startScale, double endScale, int newMarginX, int newMarginY, Rectangle myMediaElement, int timeSpanMilli, double acceleration, double deceleration)
            
        {
            TimeSpan dur = TimeSpan.FromMilliseconds(timeSpanMilli);
            TransformGroup grp = new TransformGroup();
            ScaleTransform scaleTrans = new ScaleTransform();
            Point myStartPoint = new Point(0, 0);
            Point myEndPoint = new Point(0, 0);
            myStartPoint.X = startScale;
            myStartPoint.Y = startScale;
            myEndPoint.X = endScale;
            myEndPoint.Y = endScale;
            grp.Children.Add(scaleTrans);
            myMediaElement.RenderTransform = grp;
            var aniX = new DoubleAnimation
            {
                From = myStartPoint.X,
                To = myEndPoint.X,
                Duration = dur,
                AccelerationRatio = acceleration,
                DecelerationRatio = deceleration,
            };
            var aniY = new DoubleAnimation
            {
                From = myStartPoint.Y,
                To = myEndPoint.Y,
                Duration = dur,
                AccelerationRatio = acceleration,
                DecelerationRatio = deceleration,
            };
            scaleTrans.CenterX = myMediaElement.Width/2;
            scaleTrans.CenterY = myMediaElement.Height/2;

            myMediaElement.Margin = new Thickness(newMarginX,newMarginY,0,0);

            scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, aniX);
            scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, aniY);

        }

        private void preflightChecks( int iter , bool throttlePage)
        {
            if (my8_8_8.inputs[1] == false && iter == 0)
            {
                
                TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, indIstructions);
                instructionsLabel.Content = "Release Locomotive Brake!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(1,false); }), TimeSpan.FromMilliseconds(500));

            }


            else if (my8_8_8.inputs[1] == false && iter == 1)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(1,false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my8_8_8.inputs[1] == true && iter == 1)
            {
                TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, indIstructions);
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0,false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my16_16_0.inputs[3] == false && iter == 0)
            {
                instructionsLabel.Content = "Release Main Brake!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, brakeInstructions);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(2,false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my16_16_0.inputs[3] == false && iter == 2)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(2,false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my16_16_0.inputs[3] == true && iter == 2)
            {
                TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, brakeInstructions);
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, false); }), TimeSpan.FromMilliseconds(500));
            }
            
            
            else if (throttlePosition != 1 && iter == 0)
            {
                if (throttlePage == false)
                {
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructions);
                }
                instructionsLabel.Content = "Set Throttle To Idle!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(3, true); }), TimeSpan.FromMilliseconds(500));
            }


            else if (throttlePosition != 1 && iter == 3)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(3, true); }), TimeSpan.FromMilliseconds(500));
            }


            else if (throttlePosition == 1 && iter == 3)
            {
                if (my8_8_8.inputs[1] == false || my16_16_0.inputs[3] == false)
                {
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructions);
                }
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
            }


            else if (dynamicPosition != 1 && iter == 0)
            {
                if (throttlePage == false)
                {
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructions);
                }
                instructionsLabel.Content = "Set Dynamic Selector to 1!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(4, true); }), TimeSpan.FromMilliseconds(500));
            }


            else if (dynamicPosition != 1 && iter == 4)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(4, true); }), TimeSpan.FromMilliseconds(500));

            }


            else if (dynamicPosition == 1 && iter == 4)
            {
                if (my8_8_8.inputs[1] == false || my16_16_0.inputs[3] == false)
                {
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructions);
                }
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my16_16_0.inputs[13] == false && iter == 0)
            {
                if (throttlePage == false)
                {
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructions);
                }
                instructionsLabel.Content = "Set Reverser to Forward!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(5, true); }), TimeSpan.FromMilliseconds(500));
            }

            else if (my16_16_0.inputs[13] == false && iter == 5)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(5, true); }), TimeSpan.FromMilliseconds(500));
                if (throttlePosition != 1 || dynamicPosition == 0)
                {
                    instructionsSupLabel.Content = "(Throttle Must be in Idle Position)";
                    instructionsSupLabel.Opacity = 1;
                }

            }

            else if (my16_16_0.inputs[13] == true && iter == 5)
            {
                instructionsSupLabel.Opacity = 0;
                if (my8_8_8.inputs[1] == false || my16_16_0.inputs[3] == false)
                {
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructions);
                }
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
            }

            else
            {
                if (throttlePage == true)
                {
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructions);
                }
                LaunchGame();
                instructionsLabel.Opacity = 0;
            }
            

        }




        //Game Control
        private void LaunchGame()
        {
            TimedAction.ExecuteWithDelay(new Action(delegate { TrainSimulatorLogo.Visibility = Visibility.Visible; TranslateTheMediaElement(0, 500, 0, 0, 500, 0, 1, TrainSimulatorLogo); }), TimeSpan.FromMilliseconds(200));
            TimedAction.ExecuteWithDelay(new Action(delegate { fuelBarBackground.Visibility = Visibility.Visible; TranslateTheMediaElement(0, 500, 0, 0, 500, 0, 1, fuelBarBackground); }), TimeSpan.FromMilliseconds(0));
TimedAction.ExecuteWithDelay(new Action(delegate { fuelBar.Visibility = Visibility.Visible; updateFuelBar(1500); toggleDisplayLights(false); }), TimeSpan.FromMilliseconds(800));
            
            gameState = "inGame";

            if (selectedSegment == "selectGolden")
            {
                outWindow.goldenToFieldVideo.Position = TimeSpan.FromSeconds(0);
                velocity = 12;
                bendIndex = 0;
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.goldenToFieldVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                outWindow.goldenToFieldVideo.Play();
            }
            else if (selectedSegment == "selectGlenogle")
            {
                outWindow.goldenToFieldVideo.Play();
                //outWindow.goldenToFieldVideo.Pause();
                outWindow.goldenToFieldVideo.Position = TimeSpan.FromSeconds(324);
                velocity = 20;
                bendIndex = 18;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.goldenToFieldVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.goldenToFieldVideo.Play();

            }
            else if (selectedSegment == "selectPalliser")
            {
                outWindow.goldenToFieldVideo.Play();
                //outWindow.goldenToFieldVideo.Pause();
                outWindow.goldenToFieldVideo.Position = TimeSpan.FromSeconds(896);
                velocity = 26;
                bendIndex = 50;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.goldenToFieldVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.goldenToFieldVideo.Play();
            }
            else if (selectedSegment == "selectLeanchoil")
            {
                outWindow.goldenToFieldVideo.Play();
                //outWindow.goldenToFieldVideo.Pause();
                outWindow.goldenToFieldVideo.Position = TimeSpan.FromSeconds(1408);
                velocity = 28;
                bendIndex = 78;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.goldenToFieldVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                //outWindow.goldenToFieldVideo.Play();
            }
            else if (selectedSegment == "selectOttertail")
            {
                outWindow.goldenToFieldVideo.Play();
                //outWindow.goldenToFieldVideo.Pause();
                outWindow.goldenToFieldVideo.Position = TimeSpan.FromSeconds(2208);
                velocity = 35;
                bendIndex = 98;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.goldenToFieldVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
            }

            

            selectedSegment = "";
            AudioPlaybackEngine.Instance.PlaySound(engineStartup);

        }

        private  void EndGame()
        {
            gameTimer.Tick -= new EventHandler(continueCountDown);

            outWindow.goldenToFieldVideo.SpeedRatio = 1;
            FadeTheMediaElement(0.5, 0, outWindow.goldenToFieldVideo, 1000);
            TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.goldenToFieldVideo.Position = new TimeSpan(0); outWindow.goldenToFieldVideo.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(1200));
            fuelBar.Visibility = Visibility.Hidden;
            FadeTheMediaElement(1, 0, continueScreenLabel, 500);
            FadeTheMediaElement(1, 0, continueCountdownLabel, 500);

            TimedAction.ExecuteWithDelay(new Action(delegate { 
                TrainSimulatorLogo.Visibility = Visibility.Hidden; 
                fuelBarBackground.Visibility = Visibility.Hidden; 
                toggleExampleVideoVisibility(Visibility.Visible);
                toggleDisplayLights(true); 
                continueScreenLabel.Visibility = Visibility.Hidden;
                continueCountdownLabel.Visibility = Visibility.Hidden;
                thankyouScreen.Visibility = Visibility.Visible;
                TranslateTheMediaElement(0, 0, 0, -559, 600, 0.5, 0.5, thankyouScreen);
            }), TimeSpan.FromMilliseconds(600));

            TimedAction.ExecuteWithDelay(new Action(delegate { selectionScreenBackground.Visibility = Visibility.Visible; TranslateTheMediaElement(0, 400, 0, 0, 500, 1, 0, selectionScreenBackground); }), TimeSpan.FromMilliseconds(5000));
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, -559, 0, 0, 1000, 0.5, 0.5, thankyouScreen); }), TimeSpan.FromMilliseconds(5000));
            TimedAction.ExecuteWithDelay(new Action(delegate { thankyouScreen.Visibility = Visibility.Hidden; gameState = "screenSaver"; }), TimeSpan.FromMilliseconds(6200));

            my16_16_0.outputs[2] = true;
            my16_16_0.outputs[5] = false;
            my16_16_0.outputs[0] = true;
            my16_16_0.outputs[1] = false;
            my16_16_0.outputs[3] = false;
            my16_16_0.outputs[4] = false;

            TimedAction.ExecuteWithDelay(new Action(delegate { my16_16_0.outputs[0] = false; my16_16_0.outputs[2] = false; }), TimeSpan.FromMilliseconds(30000));
            this.Dispatcher.Invoke(new Action(delegate()
            {
                myAnalogOut.outputs[1].Voltage = 0;
                myAnalogOut.outputs[0].Voltage = 0;
            }));
            penaltyBrake = false;
            penaltyBrakeCountDown = 0;

        }

        private void reachedFieldLoop(object sender, EventArgs e)
        {
            if (outWindow.goldenToFieldVideo.SpeedRatio > 1)
            {
                if (iterCounter < 5)
                {
                    iterCounter += 1;

                }
                else
                {
                    iterCounter = 0;
                    outWindow.goldenToFieldVideo.SpeedRatio -= 0.1;
                }
            }

            double displayMPH = velocity - (Convert.ToDouble(outWindow.goldenToFieldVideo.Position.TotalSeconds) - 2750) * velocityDec;
            if (displayMPH >= 0)
            {
                updateSpeedometer(displayMPH);
            }
            else
            {
                updateSpeedometer(0);
            }

            outWindow.playbackSpeedratioLabel.Content = "Playback Speed: " + outWindow.goldenToFieldVideo.SpeedRatio;
            outWindow.videoTrackSpeedLabel.Content = "Video MPH: " + displayMPH;

            if (outWindow.goldenToFieldVideo.Position >= System.TimeSpan.FromSeconds(2875))
            {
                gameTimer.Tick -= new EventHandler(reachedFieldLoop);
                updateSpeedometer(0);
                FadeTheMediaElement(1, 0, outWindow.goldenToFieldVideo, 1000);
                if (timeLeft < 120 )
                {
                    timeLeft = 120;
                }
                TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.goldenToFieldVideo.Position = TimeSpan.FromMilliseconds(0); }), TimeSpan.FromMilliseconds(1000));
                TimedAction.ExecuteWithDelay(new Action(delegate { selectionScreenBackground.Visibility = Visibility.Visible; LoadInButtons(); }), TimeSpan.FromMilliseconds(1000));
                TimedAction.ExecuteWithDelay(new Action(delegate { 
                    TrainSimulatorLogo.Visibility = Visibility.Hidden; 
                    fuelBarBackground.Visibility = Visibility.Hidden; 
                    fuelBar.Visibility = Visibility.Hidden; 
                    toggleExampleVideoVisibility(Visibility.Visible); }), TimeSpan.FromMilliseconds(1000));

            }
            
        }

        private void coinDrop(int dollars)
        {

            if (gameState == "screenSaver")
            {
                LoadInButtons();
                timeLeft += secondsPerDollar * 2;
                AudioPlaybackEngine.Instance.PlaySound(coinSound);
            }
            else if (gameState == "continueScreen")
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0.5, 1, outWindow.goldenToFieldVideo, 500); }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.goldenToFieldVideo.Play(); }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameState = "inGame"; continueCountdownLabel.Visibility = Visibility.Hidden; continueScreenLabel.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(2000));
                timeLeft += secondsPerDollar * 2;
                updateFuelBar(1700);
                continueTimeLeft = 0;
                FadeTheMediaElement(1, 0, continueScreenLabel, 500);
                FadeTheMediaElement(1, 0, continueCountdownLabel, 500);

                gameTimer.Tick -= new EventHandler(continueCountDown);
                gameState = "inGame";
                my16_16_0.outputs[14] = false;


            }

            else if (gameState == "endGame")
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { coinDrop(dollars); }), TimeSpan.FromMilliseconds(500));
            }

            else if (gameState == "preflight")
            {
                timeLeft += secondsPerDollar * 2;
            }
            else
            {
                timeLeft += secondsPerDollar * 2;
                updateFuelBar(450);
                my16_16_0.outputs[14] = false;

            }
        }

        private void startGameTimer()
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(500);
            gameTimer.Start(); 
        }

        public void gameLoop(object sender, EventArgs e)
        {

            updateControlStandOutputs();

            int bendStateTemp = getBendState(outWindow.goldenToFieldVideo.Position.TotalSeconds);

            if (bendState == 0 && bendStateTemp != 0)
            {
                if (bendStateTemp == 1 && velocity > 40)
                {
                    AudioPlaybackEngine.Instance.PlaySound(CornerSqueal4);
                }
                else if (bendStateTemp == 2 && velocity > 30)
                {
                    AudioPlaybackEngine.Instance.PlaySound(CornerSqueal4);
                }
                else if (bendStateTemp == 3 && velocity > 20)
                {
                    AudioPlaybackEngine.Instance.PlaySound(CornerSqueal4);
                }
                
            }
            
            bendState = bendStateTemp;
            checkTimeLeft();
            calculateVelocity();
            updatePlaybackSpeed();
            updateFuelBar(0);
            updateSpeedometer(velocity);
            updatePneumatics();
            if (outWindow.goldenToFieldVideo.Position >= System.TimeSpan.FromSeconds(2750))
            {
                velocityDec = velocity / 122;
                gameTimer.Tick -= new EventHandler(gameLoop);
                gameTimer.Tick += new EventHandler(reachedFieldLoop);
                //apply the brakes
                
            }
            outWindow.videoTrackSpeedLabel.Content = "Video MPH: " + getVideoMPH(Convert.ToInt32(outWindow.goldenToFieldVideo.Position.TotalSeconds));
            outWindow.playbackSpeedratioLabel.Content = "Playback Speed: " + outWindow.goldenToFieldVideo.SpeedRatio;
            outWindow.videoPositionLabel.Content = "Video Position: " + Convert.ToInt32(outWindow.goldenToFieldVideo.Position.TotalSeconds);
            outWindow.gradeLabel.Content = "Uphill Grade: " + getGrade(Convert.ToInt32(outWindow.goldenToFieldVideo.Position.TotalSeconds));
            outWindow.bendStateLabel.Content = "Bend State: " + bendState;
            outWindow.PenaltyBrakeLabel.Content = "Penalty Brake: " + penaltyBrake;
            outWindow.PenaltyCountdownLabel.Content = "Penalty Countdown: " + penaltyBrakeCountDown;
            
        }

        private void updatePlaybackSpeed()
        {
            double newPlaybackSpeed = Math.Round(velocity / getVideoMPH(Convert.ToInt32(outWindow.goldenToFieldVideo.Position.TotalSeconds)), 1);

            if (outWindow.goldenToFieldVideo.SpeedRatio != newPlaybackSpeed)
            {
                if (newPlaybackSpeed >= 3)
                {
                    outWindow.goldenToFieldVideo.SpeedRatio = 2.9;
                }
                else if (newPlaybackSpeed < 0)
                {
                    outWindow.goldenToFieldVideo.SpeedRatio = 0;
                }
                else
                {
                    outWindow.goldenToFieldVideo.SpeedRatio = newPlaybackSpeed;
                }
                
            }

        }

        private void calculateVelocity()
        {
            //Throttle, Brake, Independant brake, Dynamic Brake, Grade, Friction, Current Speed, Max speed
            velocity = velocity + (65-velocity)/(65)*(0.09 * (throttlePosition - 1) - (( 0.02 * dynamicPosition) + (0.08 * getIndependantBrakeState()) + (0.2 * getMainBrakeState()) + (0.1 * getGrade(Convert.ToInt32(outWindow.goldenToFieldVideo.Position.TotalSeconds)) + (0.07))));

            if (velocity > 65)
            {
                velocity = 65;
            }
            if (velocity < 8)
            {
                velocity = 8;
            }
        }

        private int getBendState(double videoPosition)
        {
            if (videoPosition >= bendList[bendIndex + 1, 0])
            {
                bendIndex++;
                return bendList[bendIndex, 1];
            }

            return bendList[bendIndex,1];
        }

        private void updateFuelBar(int aniTime)
        {
            if (aniTime == 0)
            {
                if (timeLeft > 2)
                {
                    fuelBar.Width = Convert.ToDouble(timeLeft);
                }
            }

            else
            {
                AnimateWidthProperty(fuelBar.Width, timeLeft, aniTime, 0.5, 0.5, fuelBar);
                //TimedAction.ExecuteWithDelay(new Action(delegate {  }), TimeSpan.FromMilliseconds(aniTime + 50));

            }
        }

        private int getIndependantBrakeState()
        {
            return 0;
        }

        private void updateControlStandOutputs()
        {
            updateSpeedometer(velocity);
            updateAmmeter(velocity * 2 * (throttlePosition));


        }

        private double getVideoMPH(int videoPosition)
        {
            double position = Convert.ToDouble(videoPosition);

            if (position < 319)
            {
                return Math.Round(((position / 319 * 21) + 8),1);
            }

            if (position >= 319 && position < 618)
            {
                return 17.5;
            }
            if (position >= 618 && position < 857)
            {
                return Math.Round((((position-618) / 239 * 6.5) + 17.5),1);
            }
            if (position >= 857 && position < 1016)
            {
                return 23;
            }
            if (position >= 1016 && position < 1247)
            {
                return 25;
            }
            if (position >= 1247 && position < 1962)
            {
                if (position < 1440)
                {
                    return Math.Round((20.8 - ((position-1247)/ 193 * 4.8)),1);
                }
                else
                {
                    return Math.Round((((position - 1440)/ 715 * 11.2) + 16),1);
                }
            }
            if (position >= 1962 && position < 2503)
            {
                if (position < 2091)
                {
                    return 30;
                }
                if (position < 2270 && position >= 2091)
                {
                    return Math.Round((30 - ((position - 2270)/ 179 * 6)),1);
                }
                else
                {
                    return 24;
                }
               
            }
            if (position >= 2503 && position <= 2810)
            {
                if (position < 2584)
                {
                    return 22.4;
                }
                else
                {
                    return Math.Round((22.4 - ((position - 2584) / 307 * 6.4)),1);
                }
            }


            return 1;
        }

        private double getGrade(int videoPosition)
        {
            int position = videoPosition;
            if (position < 116)
            {
                return 0.2;
            }
            else if (position < 260)
            {
                return 0.6;

            }
            else if (position < 319)
            {
                return 1.4;
            }
            else if (position < 570)
            {
                return 2.0;
            }
            else if (position < 618)
            {
                return 0.1;
            }
            else if (position < 857)
            {
                return 1.2;
            }
            else if (position < 1016)
            {
                return 1.7;
            }
            else if (position < 1247)
            {
                return 0.3;
            }
            else if (position < 1769)
            {
                return 1.9;
            }
            else if (position < 2035)
            {
                return -0.4;
            }
            else if (position < 2503)
            {
                return 1;
            }
            else if (position < 2810)
            {
                return 0.6;
            }
            else
            {
                return 0;
            }
        }

        private void checkTimeLeft()
        {
            timeLeft -= 1;
            if (timeLeft < 48)
            {
                my16_16_0.outputs[14] = true;
            }
            if (timeLeft <= 0)
            {
                continueCountdownLabel.Content = "12";
                continueCountdownLabel.Opacity = 0;
                continueScreenLabel.Opacity = 0;
                continueCountdownLabel.Visibility = Visibility.Visible;
                continueScreenLabel.Visibility = Visibility.Visible;
                FadeTheMediaElement(0, 1, continueScreenLabel, 500);
                FadeTheMediaElement(0, 1, continueCountdownLabel, 500);
                gameTimer.Tick -= new EventHandler(gameLoop);
                gameTimer.Tick += new EventHandler(continueCountDown);
                gameState = "continueScreen";
                FadeTheMediaElement(1, 0.5, outWindow.goldenToFieldVideo, 500);
                continueTimeLeft += 24;
                TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.goldenToFieldVideo.Pause(); }), TimeSpan.FromMilliseconds(500));


            }
        }
        
        private void updateSpeedometer(double mph)
        {
            if (mph < 0)
            {
                mph = 0;
            }
            else if (mph > 95)
            {
                mph = 95;
            }

            double outVoltage = mph * 0.0989;
            
            this.Dispatcher.Invoke(new Action(delegate()
            {
                myAnalogOut.outputs[0].Voltage = outVoltage;
            }));
        }

        private void updateAmmeter(double amps)
        {
            
            if (amps < 0)
            {
                amps = 0;
            }
            else if (amps > 1448)
            {
                amps = 1448;
            }

            double outVoltage = amps * 0.006897;

            this.Dispatcher.Invoke(new Action(delegate()
            {
                myAnalogOut.outputs[1].Voltage = outVoltage;
            }));
        }

        private void continueCountDown(object sender, EventArgs e)
        {
            continueCountdownLabel.Content = continueTimeLeft / 2;
            continueTimeLeft -= 1;
            updatePneumatics();
            if (continueTimeLeft == 0)
            {
                EndGame();
                gameState = "endGame";
            }
        }

        private void updatePneumatics()
        {
            if (penaltyBrake == true)
            {
                if (my16_16_0.inputs[0] == true && penaltyBrakeCountDown > 0)
                {
                    penaltyBrakeCountDown -= 1;
                    my16_16_0.outputs[4] = true;
                    my16_16_0.outputs[3] = false;
                    my16_16_0.outputs[5] = false;
                    my16_16_0.outputs[2] = true;
                    my16_16_0.outputs[1] = false;
                    my16_16_0.outputs[4] = false;
                }
                else if (my16_16_0.inputs[0] == true && penaltyBrakeCountDown == 0)
                {
                    penaltyBrake = false;
                    my16_16_0.outputs[13] = false;
                    my16_16_0.outputs[4] = false;
                    my16_16_0.outputs[2] = false;
                }
            }
            else
            {
                if (mainDir == 1 && mainBrakePressureIndicated != mainBrakePressureRequested)
                {
                    if (mainBrakePressureRequested < mainBrakePressureIndicated)
                    {
                        my16_16_0.outputs[5] = true;
                        my16_16_0.outputs[2] = false;
                        mainDir = 0;
                        mainBrakePressureIndicated -= 5;
                        if (indDir == 1 && indBrakePressureIndicated == 0 && mainBrakePressureRequested < 90)
                        {
                            indBrakePressureRequested = 25;
                        }
                    }
                    else
                    {
                        my16_16_0.outputs[2] = true;
                        my16_16_0.outputs[5] = false;
                        mainDir = 2;
                        mainBrakePressureIndicated += 5;
                    }

                }
                else if (mainDir != 1 && mainBrakePressureIndicated != mainBrakePressureRequested)
                {
                    if (mainBrakePressureRequested < mainBrakePressureIndicated)
                    {
                        if (mainDir == 0)
                        {
                            mainBrakePressureIndicated -= 5;
                        }
                        else
                        {
                            mainDir = 0;
                            my16_16_0.outputs[5] = true;
                            my16_16_0.outputs[2] = false;
                            mainBrakePressureIndicated -= 5;
                        }
                    }
                    else
                    {
                        if (mainDir == 2)
                        {
                            mainBrakePressureIndicated += 5;
                        }
                        else
                        {
                            mainDir = 2;
                            my16_16_0.outputs[2] = true;
                            my16_16_0.outputs[5] = false;
                            mainBrakePressureIndicated += 5;
                        }
                    }
                }
                else if (mainBrakePressureIndicated == mainBrakePressureRequested && mainDir != 1)
                {
                    if (mainBrakePressureIndicated != 0 && mainBrakePressureIndicated != 90)
                    {
                        my16_16_0.outputs[2] = false;
                        my16_16_0.outputs[5] = false;
                    }
                    mainDir = 1;
                }
                if (bailActivated == true)
                {
                    if (indBrakePressureIndicated >= 5)
                    {
                        my16_16_0.outputs[3] = false;
                        my16_16_0.outputs[0] = true;
                        indBrakePressureIndicated -= 5;
                        indBrakePressureRequested = indBrakePressureIndicated;
                    }
                    else
                    {
                        my16_16_0.outputs[3] = false;
                        my16_16_0.outputs[0] = true;
                        indDir = 1;
                    }
                }
                else
                {
                    if (indDir == 1 && indBrakePressureIndicated != indBrakePressureRequested)
                    {
                        if (indBrakePressureRequested > indBrakePressureIndicated)
                        {
                            my16_16_0.outputs[3] = true;
                            my16_16_0.outputs[0] = false;
                            indDir = 2;
                            indBrakePressureIndicated += 5;
                        }
                        else
                        {
                            my16_16_0.outputs[3] = false;
                            my16_16_0.outputs[0] = true;
                            indDir = 0;
                            indBrakePressureIndicated -= 5;
                        }
                    }
                    else if (indDir != 1 && indBrakePressureIndicated != indBrakePressureRequested)
                    {
                        if (indBrakePressureRequested > indBrakePressureIndicated)
                        {
                            if (indDir == 2)
                            {
                                indBrakePressureIndicated += 5;
                            }
                            else
                            {
                                my16_16_0.outputs[3] = true;
                                my16_16_0.outputs[0] = false;
                                indBrakePressureIndicated += 5;
                                indDir = 2;
                            }

                        }
                        else
                        {
                            if (indDir == 0)
                            {
                                indBrakePressureIndicated -= 5;
                            }
                            else
                            {
                                indBrakePressureIndicated -= 5;
                                my16_16_0.outputs[0] = false;
                                my16_16_0.outputs[3] = true;
                                indDir = 0;
                            }
                        }
                    }
                    else if (indDir != 1 && indBrakePressureIndicated == indBrakePressureRequested)
                    {
                        if (indBrakePressureRequested != 0 && indBrakePressureRequested != 90)
                        {

                            my16_16_0.outputs[3] = false;
                            my16_16_0.outputs[0] = false;


                        }
                        indDir = 1;
                    }
                }

                outWindow.MainBrakePressureIndicatedLabel.Content = "Main Pressure Ind: " + mainBrakePressureIndicated;
                outWindow.MainBrakePressureRequested.Content = "Main Pressure Req: " + mainBrakePressureRequested;
                outWindow.IndBrakePressureIndicated.Content = "Ind Pressure Ind: " + indBrakePressureIndicated;
                outWindow.IndBrakePressureRequested.Content = "Ind Pressure Req: " + indBrakePressureRequested;

            }
        }


        //Audio
        
        class AudioPlaybackEngine : IDisposable
        //Engine Tone Sound Player
        {
            private readonly IWavePlayer outputDevice;
            private readonly MixingSampleProvider mixer;

            public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
            {
                outputDevice = new WaveOutEvent();
                mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
                mixer.ReadFully = true;
                outputDevice.Init(mixer);
                outputDevice.Play();
            }

            public void PlaySound(string fileName)
            {
                var input = new AudioFileReader(fileName);
                AddMixerInput(new AutoDisposeFileReader(input));
            }

            private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
            {
                if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
                {
                    return input;
                }
                if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
                {
                    return new MonoToStereoSampleProvider(input);
                }
                throw new NotImplementedException("Not yet implemented this channel count conversion");
            }

            public void PlaySound(CachedSound sound)
            {
                AddMixerInput(new CachedSoundSampleProvider(sound));
            }

            private void AddMixerInput(ISampleProvider input)
            {
                mixer.AddMixerInput(ConvertToRightChannelCount(input));
            }

            public void Dispose()
            {
                outputDevice.Dispose();
            }

            public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(48000, 2);
        }

        class AutoDisposeFileReader : ISampleProvider
        //Disposes Engine Tone Sound Player when not active
        {
            private readonly AudioFileReader reader;
            private bool isDisposed;
            public AutoDisposeFileReader(AudioFileReader reader)
            {
                this.reader = reader;
                this.WaveFormat = reader.WaveFormat;
            }

            public int Read(float[] buffer, int offset, int count)
            {
                if (isDisposed)
                    return 0;
                int read = reader.Read(buffer, offset, count);
                if (read == 0)
                {
                    reader.Dispose();
                    isDisposed = true;
                }
                return read;
            }

            public WaveFormat WaveFormat { get; private set; }
        }

        class CachedSound
        //Moves engine sound files to memory for disc read elimination
        {
            public float[] AudioData { get; private set; } 
            public WaveFormat WaveFormat { get; private set; }
            public String Name { get; set; }
            public CachedSound(string audioFileName)
            {
                using (var audioFileReader = new AudioFileReader(audioFileName))
                {
                    WaveFormat = audioFileReader.WaveFormat;
                    var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                    var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                    int samplesRead;
                    while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        wholeFile.AddRange(readBuffer.Take(samplesRead));
                    }
                    AudioData = wholeFile.ToArray();
                    Name = audioFileName;
                }
                

            }
        }

        class CachedSoundSampleProvider : ISampleProvider
        //Provides datastream to Engine Tone Sound Player
        //Also contains logic for next audio audio clip
        {
            private readonly CachedSound cachedSound;
            private long position;


            public CachedSoundSampleProvider(CachedSound cachedSound)
            {
                this.cachedSound = cachedSound;
            }

            public int Read(float[] buffer, int offset, int count) 
            {
                var availableSamples = cachedSound.AudioData.Length - position;
                var samplesToCopy = Math.Min(availableSamples, count);
                Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;
                if (availableSamples > samplesToCopy && availableSamples < samplesToCopy * 2) // triggers on last packet sent
                {
                    //next audio clip logic here//
                    //AudioPlaybackEngine.Instance.PlaySound(engine0);
                    if ((cachedSound.Name == "EngineIdle.wav" || cachedSound.Name == "Engine1.wav" || cachedSound.Name == "Engine2.wav" || cachedSound.Name == "Engine3.wav" || cachedSound.Name == "Engine4.wav" || cachedSound.Name == "Engine5.wav" || cachedSound.Name == "Engine6.wav" || cachedSound.Name == "Engine7.wav" || cachedSound.Name == "Engine8.wav") && (gameState == "inGame" || gameState == "continueScreen"))
                    {
                        if (throttlePosition == 0 || throttlePosition == 1)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine0);
                        }
                        else if (throttlePosition == 2)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine1);
                        }
                        else if (throttlePosition == 3)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine2);
                        }
                        else if (throttlePosition == 4)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine3);
                        }
                        else if (throttlePosition == 5)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine4);
                        }
                        else if (throttlePosition == 6)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine5);
                        }
                        else if (throttlePosition == 7)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine6);
                        }
                        else if (throttlePosition == 8)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine7);
                        }
                        else if (throttlePosition == 9)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(engine8);
                        }
                    }

                    else if (cachedSound.Name == "Horn.wav")
                    {
                        if (hornPulled == true)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(horn); 
                        }
                        else
                        {
                            hornPlaying = false;
                        }
                         
                    }

                    else if ((cachedSound.Name == "AmbientAudio.wav" || cachedSound.Name == "AmbientAudio1.wav" )&& (gameState == "inGame" || gameState == "continueScreen"))
                    {
                        if (timeLeft > 48)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(ambient);
                        }
                        else 
                        {
                            AudioPlaybackEngine.Instance.PlaySound(ambient1);
                        }
                        
                    }
                    else if (cachedSound.Name == "BellLoop.wav")
                    {
                        if (bellPulled == true)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(bell);
                        }
                        else
                        {
                            bellPlaying = false;
                        }
                    }
                    else if ((cachedSound.Name == "CornerSqueal0.wav" || cachedSound.Name == "CornerSqueal1.wav" || cachedSound.Name == "CornerSqueal2.wav" || cachedSound.Name == "CornerSqueal3.wav" || cachedSound.Name == "CornerSqueal4.wav" || cachedSound.Name == "CornerSqueal5.wav") && bendState != 0 && bendState != 3 && gameState == "inGame")
                    {
                        var r = new Random();
                        var s = r.Next(6);
                        if (s == 0)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(CornerSqueal0);
                        }
                        else if (s == 1)
                        {
                            if (bendState > 1)
                            {
                                AudioPlaybackEngine.Instance.PlaySound(CornerSqueal3);
                            }
                            else
                            {
                                AudioPlaybackEngine.Instance.PlaySound(CornerSqueal1);
                            }

                        }
                        else if (s == 2)
                        {
                            if (bendState > 1)
                            {
                                AudioPlaybackEngine.Instance.PlaySound(CornerSqueal5);
                            }
                            else
                            {
                                AudioPlaybackEngine.Instance.PlaySound(CornerSqueal2);
                            }
                        }
                        else if (s == 3)
                        {

                            AudioPlaybackEngine.Instance.PlaySound(CornerSqueal3);
                        }
                        else if (s == 4)
                        {
                            if (bendState > 1)
                            {
                                AudioPlaybackEngine.Instance.PlaySound(CornerSqueal0);
                            }
                            else
                            {
                                AudioPlaybackEngine.Instance.PlaySound(CornerSqueal4);
                            }
                        }
                        else if (s == 5)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(CornerSqueal5);
                        }
                    }


                    else if ((cachedSound.Name == "CornerSqueal0.wav" || cachedSound.Name == "CornerSqueal1.wav" || cachedSound.Name == "CornerSqueal2.wav" || cachedSound.Name == "CornerSqueal3.wav" || cachedSound.Name == "CornerSqueal4.wav" || cachedSound.Name == "CornerSqueal5.wav") && bendState == 3 && gameState == "inGame")
                    {
                        AudioPlaybackEngine.Instance.PlaySound(CornerSqueal5);
                    }


                    else if (cachedSound.Name == "EngineStart.wav")
                    {
                        AudioPlaybackEngine.Instance.PlaySound(ambient);
                        AudioPlaybackEngine.Instance.PlaySound(engine0);
                    }
                    else if (cachedSound.Name == "BailSound.wav")
                    {
                        bailPlaying = false;
                    }
                    else if (cachedSound.Name == "MainBrakeApplication.wav")
                    {
                        brakeSoundPlaying = false;
                    }
                    else if (cachedSound.Name == "SandingSound.wav")
                    {
                        if (sandingPressed == true || sandingToggle == true)
                        {
                            AudioPlaybackEngine.Instance.PlaySound(SandingSound);
                        }
                        else
                        {
                            sandingPlaying = false;
                        }
                    }

                }
                return (int)samplesToCopy;
                 
            }

            public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
        }



        //Misc Functions

       public static class TimedAction
        {
            public static void ExecuteWithDelay(Action action, TimeSpan delay)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = delay;
                timer.Tag = action;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }

            static void timer_Tick(object sender, EventArgs e)
            {
                DispatcherTimer timer = (DispatcherTimer)sender;
                Action action = (Action)timer.Tag;

                action.Invoke();
                timer.Stop();
            }
        }        

       private void applicationReset()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

       private void my8InputChanged(object sender, InputChangeEventArgs e)
       {
           this.Dispatcher.Invoke(new Action(delegate()
               {
                   if (e.Value == true && (e.Index == 1 || e.Index == 2 || e.Index == 3 || e.Index == 4))
                   {
                       indPosition = e.Index - 1;
                       outWindow.indPositionLabel.Content = "Ind Position: " + Convert.ToString(indPosition);
                       if (indPosition == 0)
                       {
                           if (getMainBrakeState() == 0)
                           {
                               indBrakePressureRequested = 0;
                           }
                       }
                       else if (indPosition == 1)
                       {
                           indBrakePressureRequested = 30;
                       }
                       else if (indPosition == 2)
                       {
                           indBrakePressureRequested = 60;
                       }
                       else
                       {
                           indBrakePressureRequested = 90;
                       }
                   }
                   if (e.Index == 5)
                   {
                       bailActivated = e.Value;
                       outWindow.bailStateLabel.Content = "Bail State: " + Convert.ToString(bailActivated);
                       
                       if (bailActivated == false)
                       {
                           if (indPosition == 0)
                           {
                               if (getMainBrakeState() == 0)
                               {
                                   indBrakePressureRequested = 0;
                               }
                           }
                           else if (indPosition == 1)
                           {
                               indBrakePressureRequested = 30;
                           }
                           else if (indPosition == 2)
                           {
                               indBrakePressureRequested = 60;
                           }
                           else
                           {
                               indBrakePressureRequested = 90;
                           }
                       }
                       else if(bailPlaying == false && indBrakePressureIndicated > 0 && gameState == "inGame")
                       {
                           AudioPlaybackEngine.Instance.PlaySound(bailSound);
                           bailPlaying = true;
                       }
                   }
                   if (e.Index == 0)
                   {
                       pedalPressed = e.Value;
                       outWindow.pedalPressedLabel.Content = "Pedal Pressed: " + Convert.ToString(pedalPressed);
                   }
                   if (e.Value == true && (e.Index == 6 || e.Index == 7))
                   {
                       coinDrop(e.Index - 5);
                   }
               }));
       }

       private void my16InputChanged(object sender, InputChangeEventArgs e)
       {
           this.Dispatcher.Invoke(new Action(delegate()
           {
               if (e.Index == 0 || e.Index == 9 || e.Index == 2 || e.Index == 3 || e.Index == 4)
               {
                   brakePosition = getMainBrakeState();
                   if (gameState == "inGame" && brakeSoundPlaying == false)
                   {
                       AudioPlaybackEngine.Instance.PlaySound(brakeSound);
                       brakeSoundPlaying = true;
                   }

               }
               if (e.Index == 1)
               {
                   attendSwitch = !e.Value;
                   outWindow.attendantCallPressedLabel.Content = "Attenant Pressed: " + Convert.ToString(attendSwitch);
               }
               if (e.Index == 10)
               {
                   resetSwitch = !e.Value;
                   outWindow.resetButtonPressedLabel.Content = "Reset Pressed: " + Convert.ToString(resetSwitch);
               }
               if (e.Index == 6)
               {
                   hornPulled = e.Value;
                   outWindow.hornPulleLabel.Content = "Horn Pulled: " + Convert.ToString(hornPulled);
                   if (e.Value == true && gameState == "inGame" && hornPlaying == false)
                   {
                    AudioPlaybackEngine.Instance.PlaySound(horn);
                    hornPlaying = true;
                   }
                   

               }
               if (e.Index == 5)
               {
                   sandingToggle = e.Value;
                   outWindow.sandigLeverStateLabel.Content = "SandingL State: " + Convert.ToString(sandingToggle);
                   if (gameState == "inGame")
                   {

                   
                   if (e.Value == false && sandingPressed == false)
                   {
                       my16_16_0.outputs[12] = false;
                   }
                   else if (e.Value == true)
                   {
                       my16_16_0.outputs[12] = true;
                       AudioPlaybackEngine.Instance.PlaySound(SandingSound);
                       sandingPlaying = true;
                   }

                   }
               }
               if (e.Index == 8)
               {
                   sandingPressed = e.Value;
                   outWindow.sandingButtonPressedLabel.Content = "SandingB Pressed: " + Convert.ToString(sandingPressed);

                   if(gameState == "inGame")
                   {
                        if (e.Value == false && sandingToggle == false)
                        {
                             my16_16_0.outputs[12] = false;
                        }
                        else if (e.Value == true)
                        {
                                my16_16_0.outputs[12] = true;
                                AudioPlaybackEngine.Instance.PlaySound(SandingSound);
                                sandingPlaying = true;

                        }
                   }



               }
               if (e.Index == 11)
               {
                   bellPulled = e.Value;
                   outWindow.bellPulledLabel.Content = "Bell Pulled: " + Convert.ToString(bellPulled);
                   if (e.Value == true && gameState == "inGame" && bellPlaying == false)
                   {
                        AudioPlaybackEngine.Instance.PlaySound(bell);
                        bellPlaying = true;
                   }
                   
               }
               if (e.Index == 15)
               {
                   pcsPressed = e.Value;
                   outWindow.pcsPushed.Content = "PCS Pressed: " + Convert.ToString(pcsPressed);
                   if (e.Value == true && my16_16_0.inputs[1] == false && my16_16_0.inputs[10] == false)
                   {
                       toggleHUD();
                   }

               }
               if (e.Index == 13)
               {
                   foaSwitch = e.Value;
               }
               if (e.Index == 14)
               {
                   engineRunSwitch = e.Value;
                   outWindow.engineRunButtonStateLabel.Content = "Engine Run State: " + Convert.ToString(engineRunSwitch);
               }
           }));
       }

       private void myPotChanged(object sender, SensorChangeEventArgs e) 
       {
           this.Dispatcher.Invoke(new Action(delegate()
           {
               if (e.Index == 6)
               {
                   if (e.Value >= 605)
                   {
                       throttlePosition = 0;
                   }
                   else if (e.Value >= 563)
                   {
                       throttlePosition = 1;
                   }
                   else if (e.Value >= 511)
                   {
                       throttlePosition = 2;
                   }
                   else if (e.Value >= 458)
                   {
                       throttlePosition = 3;
                   }
                   else if (e.Value >= 409)
                   {
                       throttlePosition = 4;
                   }
                   else if (e.Value >= 359)
                   {
                       throttlePosition = 5;
                   }
                   else if (e.Value >= 313)
                   {
                       throttlePosition = 6;
                   }
                   else if (e.Value >= 268)
                   {
                       throttlePosition = 7;
                   }
                   else if (e.Value >= 231)
                   {
                       throttlePosition = 8;
                   }
                   else
                   {
                       throttlePosition = 9;
                   }
                   outWindow.throttlePositionLabel.Content = "Throttle Position: " + Convert.ToString(throttlePosition);
               }

               if (e.Index == 7)
               {
                   if (e.Value >= 623)
                   {
                       dynamicPosition = 0;
                   }
                   else if (e.Value >= 547)
                   {
                       dynamicPosition = 1;
                   }
                   else if (e.Value >= 469)
                   {
                       dynamicPosition = 2;
                   }
                   else if (e.Value >= 393)
                   {
                       dynamicPosition = 3;
                   }
                   else
                   {
                       dynamicPosition = 4;
                   }
                   outWindow.dynamicPositionLabel.Content = "Dynamic Position: " + Convert.ToString(dynamicPosition);

               }

           }));
       }

       private int getMainBrakeState()
       {
            int mainBrakePosition;
    ///        if (my16_16_0.inputs[0] == false)
         ///   {
                if (gameState == "inGame" || gameState == "continueScreen")
                {
                    
                    penaltyBrake = true;
                    penaltyBrakeCountDown = 30;
              ///      my16_16_0.outputs[1] = true;
              ///      my16_16_0.outputs[0] = true;
              ///      my16_16_0.outputs[5] = true;
              ///      my16_16_0.outputs[13] = true;
               }
              ///  mainBrakePosition = 5;

        ///    }
         ///   else
          ///  {
           ///     if (my16_16_0.inputs[3] == true)
              {
                    mainBrakePosition = 0;
                    mainBrakePressureRequested = 90;
                }
          ///      else
           ///     {
               ///     if (my16_16_0.inputs[4] == true)
               ///     {
               ///         mainBrakePosition = 1;
               ///         mainBrakePressureRequested = 70;
              ///      }
              ///      else
              ///      {
              ///          if (my16_16_0.inputs[2] == true)
             ///           {
             ///               mainBrakePosition = 2;
             ///              mainBrakePressureRequested = 50;
             ///           }
             ///           else
              ///          {
             ///               if (my16_16_0.inputs[9] == true)
              ///              {
             ///                   mainBrakePosition = 3;
             ///                   mainBrakePressureRequested = 30;
             ///               }
             ///               else
             ///               {
             ///                   mainBrakePosition = 4;
             ///                   mainBrakePressureRequested = 0;
             ///               }
            ///            }
                 ///   }
            ///    }

                

          /// }
            
            outWindow.brakePositionLabel.Content = "Brake Position: " + Convert.ToString(mainBrakePosition);
            return mainBrakePosition;

       }

       private void toggleDisplayLights(bool visisbility)
       {
           my16_16_0.outputs[10] = visisbility;
           my16_16_0.outputs[11] = visisbility;
           my16_16_0.outputs[12] = visisbility;
           my16_16_0.outputs[13] = visisbility;
           my16_16_0.outputs[14] = visisbility;
           my16_16_0.outputs[15] = visisbility;
       }

       private void toggleHUD()
       {
           if (outWindow.inputDataDisplayGrid.Visibility == Visibility.Visible)
           {
               outWindow.inputDataDisplayGrid.Visibility = Visibility.Hidden;
               outWindow.outputDataDisplayGrid.Visibility = Visibility.Hidden;
           }
           else
           {
               outWindow.inputDataDisplayGrid.Visibility = Visibility.Visible;
               outWindow.outputDataDisplayGrid.Visibility = Visibility.Visible;
           }
       }
    }


} 




//Hotkeys
      //  private void UserControlLoaded(object sender, RoutedEventArgs e)
      //      //temp keyboard control
      //  {
      //      var window = Window.GetWindow(this);
      //      window.KeyDown += HandleKeyPress;
      //  }

      //  private void HandleKeyPress(object sender, KeyEventArgs e)
      //      //temp keyboard bindings
      //  {
      //      if (e.Key == Key.Space)
       //     {
      //          playsoundEffect("bell");
      //      }
      //      if (e.Key == Key.Escape)
      //      {

      //      }
     //       if (e.Key == Key.P)
     //       {

     //       }
     //       if (e.Key == Key.O)
     //       {

     //       }
     //       if (e.Key == Key.Up)
    //        {
    //            outWindow.testMediaElement.SpeedRatio += 0.1;
    //        }
    //        if (e.Key == Key.Down)
    //        {
   //             outWindow.testMediaElement.SpeedRatio -= 0.1;
    //        }
   //         if (e.Key == Key.Delete)
    //        {
   //             applicationReset();
   //         }
    //        if (e.Key == Key.C)
    //        {
    //            coinDrop();
    //        }

    //    }

        //Screen Saver