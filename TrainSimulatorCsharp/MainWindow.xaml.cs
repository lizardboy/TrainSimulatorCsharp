using System;
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
///#include math.h



namespace TrainSimulatorCsharp
{
    public partial class MainWindow : Window
    {
        static double tempVelocity;

        //Globals, sounds, and Timers

        static int secondsPerDollar = 240;
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
        DispatcherTimer lightFlasher = new System.Windows.Threading.DispatcherTimer();
        

        static string currentImage = "none";
        static int currentFaderIteration = 0;
        static string gameState = "screenSaver";
        static bool ssFwd = true;
        static bool UIbuttonsClickable = false;
        static string buttonClicked;
        static string selectedSegment = "";
        static string selectedTrack = "";
        static string instructionState ="";
        //Rolling up to Field Vars
        static int iterCounter = 0;
        static double velocityDec = 0;
        static string selectStart1;
        static string selectStart2;
        static string selectStart3;
        static string selectStart4;
        static string selectStart5;
        static string selectLabel1;
        static string selectLabel2;
        static string selectLabel3;
        static string selectLabel4;
        static string selectLabel5;
        
        //Control Stand Input Devices
        private InterfaceKit my16_16_0;
        private InterfaceKit my8_8_8;
        private Analog myAnalogOut;

        //Control stand Input Variables
        static bool engineRunSwitch;
        static bool footSwitch;
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
        
        ///status storage for dispalyed elements and sounds
        static bool bailPlaying = false;
        static bool hornPlaying = false;
        static bool bellPlaying = false;
        static bool brakeSoundPlaying = false;
        static bool sandingPlaying = false;
        static bool DynamicPageRELEASE = false;
        static bool DynamicPageAPPLY = false;
        static bool ReverserPage = false;
        static bool ReverserThrottlePageUP = false;
        static bool ReverserThrottlePageDOWN = false;
        static bool ThrottlePageUP = false;
        static bool ThrottlePageDOWN = false;
        static bool LowFuelWarning = false;
        static bool ebrakeState = false;
        static bool eState;

        //Game Data Variables
       
            // throttle variables
        static double throttle_deadzone = 25;    // acceptance window around throttle notches...roughly half of window 
        static int throttlePosition ;
        static double actual_throttle_1_pos;     // keep track of actual position 1 reading for calibration use
        static double Throttle_Idle_Position = 1; /// sets which throttle position is the idle 
         
        
        // setup throttle sector positions 
        // input actual readings from phidgets app here for all positions 
        static double throttle_position_0 = 654;
        static double throttle_position_1 = 600;
        static double throttle_position_2 = 550;
        static double throttle_position_3 = 493;
        static double throttle_position_4 = 439;
        static double throttle_position_5 = 384;
        static double throttle_position_6 = 335;
        static double throttle_position_7 = 283;
        static double throttle_position_8 = 230;

        // calculate throttle notch positions based on position 1 value 
        static double throttle_offset_0 = throttle_position_0 - throttle_position_1;
        static double throttle_offset_1 = throttle_position_1 - throttle_position_1;
        static double throttle_offset_2 = throttle_position_2 - throttle_position_1;
        static double throttle_offset_3 = throttle_position_3 - throttle_position_1;
        static double throttle_offset_4 = throttle_position_4 - throttle_position_1;
        static double throttle_offset_5 = throttle_position_5 - throttle_position_1;
        static double throttle_offset_6 = throttle_position_6 - throttle_position_1;
        static double throttle_offset_7 = throttle_position_7 - throttle_position_1;
        static double throttle_offset_8 = throttle_position_8 - throttle_position_1;

        static double throttle_calibration_value = throttle_position_1 - throttle_deadzone;  // store what the one position on the throttle is during preflight

        /// velsocity and dynamic variables
        static double velocity = 8;                    // initial velocity
        static double MaxVelocity = 65;                // set maximum train velocity (MPH)
        static double MinVelocity = 1;                 // Set Minimum Train Velocity (MPH)
        static double trainDrag;
        static double locomotiveWeight = 400000;
        static double carWeight = 1500000;
        static double trainBrakeForce;
        static double trainTractiveEffort;
        static double velocityMs;
        static double sinGradient;
        static double gradient;
        static double gradeDeg;
        static double trainAcceleration;
        static double maxTractiveEffort;
        static double maxLocoBrakeForce;
        static double maxCarBrakeForce;
        static double locoBrakeForce;
        static double carBrakeForce;
        static double tractionFactor = 0.12;
        static double tractionFactorSsand = 0.15;
        static double tractionFactorDsand = 0.18;
        static string mainBrakeState;
        static double mainBrakeEffort;
        static double trainBrakeEffort;
        static double indBrakeEffort;
        static double dynBrakeEffort;
        static double maxDynBrakeEffort = 40000;
        static double maxMotorEffort = 52000;
        static double maxMotorAmps = 1150;
        static double maxTractiveForce;
        static double bendDrag = 0;
        // Dynamic Brake variables
        static double dynamic_deadzone = 30;    // acceptance window around dynamic selector notches...roughly half of window _deadzone = 25;    // acceptance window around throttle notches...roughly half of window 
        static double dynamicPosition ;                //
        static double actual_dynamic_1_pos;     // keep track of actual dynamic selector pos 1 value for calibration during preflight
        static double Dynamic_Off_Position = 1;   /// sets which selector position is the off #1 is because the mechanical interlocks wont allow the throttle to move in off arrgh
        static double Dynamic_On_Position = 2;
        // setup dynamic brake sector positions 
        // input actual readings from phidgets app here for all positions 
        static double dynamic_position_0 = 576;
        static double dynamic_position_1 = 506;
        static double dynamic_position_2 = 440;
        static double dynamic_position_3 = 341;
        static double dynamic_position_4 = 284;
       
        // calculate dynamic selector  notch positions based on position 1 value 
        static double dynamic_offset_0 = dynamic_position_0 - dynamic_position_1;
        static double dynamic_offset_1 = dynamic_position_1 - dynamic_position_1;
        static double dynamic_offset_2 = dynamic_position_2 - dynamic_position_1;
        static double dynamic_offset_3 = dynamic_position_3 - dynamic_position_1;
        static double dynamic_offset_4 = dynamic_position_4 - dynamic_position_1;

        static double dynamic_calibration_value = dynamic_position_1 - dynamic_deadzone;  // store what the one position on the throttle is during preflight

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

        ///     / game time left
        static int continueTimeLeft = 0;
        static int timeLeft =0;
        static int descriptionPosition = 0;
        static int scrollPix = 0;
        static int iter =0;
        static int iter1 = 0;
        static int iter2 = 0;
        static int iter3 = 0;
        static int iter4 = 0;
        /// ///////////////////////////////////////////////////////////////////////////////////////
        //Application Startup//////////////////////////////////////////////////////////////////////

        public MainWindow()
        {
            InitializeComponent();
            FadeTheMediaElement(0, 1, splashScreen, 1000);
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startGameTimer();
            outWindow.Show();
            outWindow.CabFootageVideo.Stop();
            outWindow.CabFootageVideo.Opacity = 0;
            FadeTheMediaElement(0, 1, splashScreen, 1000);
           /// EngageScreenSaver();



            ////Mouse.OverrideCursor = Cursors.None;



            WindowState = WindowState.Maximized;
            my16_16_0 = new InterfaceKit();
            my8_8_8 = new InterfaceKit();
            myAnalogOut = new Analog();

            //////////open the interface boards by serial number////////////////////
            my16_16_0.open(468344);      ////revelstoke //(344671);
            my8_8_8.open(451950);        ////revelstoke //(327859);
                                         //// myAnalogOut.open(282774);  ////revelstoke //(282774)

            my16_16_0.waitForAttachment(3000);
            my8_8_8.waitForAttachment(3000);

            ////  myAnalogOut.waitForAttachment(3000);
            ////  myAnalogOut.outputs[0].Enabled = true;
            ////  myAnalogOut.outputs[1].Enabled = true;
            ///   myAnalogOut.outputs[0].Voltage = 0;
            ///    myAnalogOut.outputs[1].Voltage = 0;

            intControlsPosition();            ///  /take initial reading of throttle and dynamic as they are not read until moved otherwise

            my8_8_8.SensorChange += new SensorChangeEventHandler(myPotChanged);
            my8_8_8.InputChange += new InputChangeEventHandler(my8InputChanged);
            my16_16_0.InputChange += new InputChangeEventHandler(my16InputChanged);
            
            this.Focus();
            ////StartupLeverPositions();

            my16_16_0.outputs[6] = true;
            my16_16_0.outputs[7] = true;
            my16_16_0.outputs[8] = true;
            my16_16_0.outputs[9] = true;
            my16_16_0.outputs[10] = false;
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
           
            ///46 seconds 490 x 276 preview videos here
            Stop1Ex.Play();            
            Stop1Ex.Pause();
            Stop2Ex.Play();
            Stop2Ex.Pause();
            Stop3Ex.Play();
            Stop3Ex.Pause();
            Stop4Ex.Play();
            Stop4Ex.Pause();
            Stop5Ex.Play();
            Stop5Ex.Pause();
           

            descriptionScreenFootage.Play();
            descriptionScreenFootage.Pause();

            AudioPlaybackEngine.Instance.PlaySound(engine0);
           //  HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
          //    HwndTarget hwndTarget = hwndSource.CompositionTarget;
           ///   hwndTarget.RenderMode = RenderMode.SoftwareOnly;

        }

//screensaver

        private void ScreenSaver(object sender, EventArgs e)
        {

            if (ssFwd == true)
            {
                TranslateTheMediaElement(0, 0, -648, -528, 50000, 0.3, 0.3, outWindow.forst1);
                TranslateTheMediaElement(0, 0, 648, -528, 50000, 0.3, 0.3, outWindow.forst2);
                TranslateTheMediaElement(0, 0, -648, 528, 50000, 0.3, 0.3, outWindow.forst3);
                TranslateTheMediaElement(0, 0, -648, 528, 50000, 0.3, 0.3, outWindow.cpsd402);
                ssFwd = false;
            }
            else
            {

                TranslateTheMediaElement(-648, -528, 0, 0, 50000, 0.3, 0.3, outWindow.forst1);
                TranslateTheMediaElement(648, -528, 0, 0, 50000, 0.3, 0.3, outWindow.forst2);
                TranslateTheMediaElement(-648, 528, 0, 0, 50000, 0.3, 0.3, outWindow.forst3);
                TranslateTheMediaElement(-648, 528,0 , 0, 50000, 0.3, 0.3, outWindow.cpsd402);
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
        /// 
        ////        //Track selection screen....what happens after the first track is selected...removing the first screen building the second 


        private void selectTrack1(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("select track 1");

            Uri myUri1 = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\descriptionScreenFootage1.wmv", UriKind.Absolute);
            descriptionScreenFootage.Source=  myUri1; 
            System.Diagnostics.Debug.WriteLine(myUri1);
            Stop1Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\GoldenEx.wmv", UriKind.Absolute);
            Stop2Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\Glenogle.wmv", UriKind.Absolute);
            Stop3Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\PalliserEx.wmv", UriKind.Absolute);
            Stop4Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\LeanchoilEx.wmv", UriKind.Absolute);
            Stop5Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\OttertailEx.wmv", UriKind.Absolute);

            StopLabel1.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("goldenLabel.png"));
            StopLabel2.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("glenogleLabel.png"));
            StopLabel3.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("palliserLabel.png"));
            StopLabel4.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("leanchoilLabel.png"));
            StopLabel5.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("ottertailLabel.png"));
            StopLabel6.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("FieldLabel.png"));


            selectedTrackText.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("track1Text.png"));

            try
            {
                descriptionText.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("ScrollableText1.png"));
            }
            catch { System.Diagnostics.Debug.WriteLine("exception in scrollable text");
            }

            
         
            selectStart1 = "selectGolden";
            selectStart2 = "selectGlenogle";
            selectStart3 = "selectPalliser";
            selectStart4 = "selectLeanchoil";
            selectStart5 = "selectOttertail";
            selectLabel1 = "goldenLabel";
            selectLabel2 = "glenogleLabel";
            selectLabel3 = "palliserLabel";
            selectLabel4 = "leanchoilLabel";
            selectLabel5 = "ottertailLabel";

            trackSelect(sender,e);

}

       


        private void selectTrack2(object sender, MouseButtonEventArgs e)
        {
           
               

            Uri myUri2 = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\descriptionScreenFootage2.wmv", UriKind.Absolute);
            descriptionScreenFootage.Source = myUri2;
            System.Diagnostics.Debug.WriteLine(myUri2);

            Stop1Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\TrailEx.wmv", UriKind.Absolute);
            Stop2Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\BrilliantEx.wmv", UriKind.Absolute);
            Stop3Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\ThrumsEx.wmv", UriKind.Absolute);
            Stop4Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\SSlocanEx.wmv", UriKind.Absolute);
            Stop5Ex.Source = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\GraniteEx.wmv", UriKind.Absolute);

            StopLabel1.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("TrailLabel.png"));
            StopLabel2.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("BrilliantLabel.png"));
            StopLabel3.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("ThrumsLabel.png"));
            StopLabel4.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("SSlocanLabel.png"));
            StopLabel5.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("GraniteLabel.png"));
            StopLabel6.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("NelsonLabel.png"));

            selectedTrackText.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("track2Text.png"));
            try
            {
                descriptionText.Source = (ImageSource)new ImageSourceConverter().ConvertFrom("ScrollableText2.png");


            }



            catch { System.Diagnostics.Debug.WriteLine("exception in scrollable text2"); }

            selectStart1 = "selectTrail";
            selectStart2 = "selectBrilliant";
            selectStart3 = "selectThrums";
            selectStart4 = "selectSSlocan";
            selectStart5 = "selectGranite";
            selectLabel1 = "TrailLabel";
            selectLabel2 = "BrilliantLabel";
            selectLabel3 = "ThrumsLabel";
            selectLabel4 = "SSlocanLabel";
            selectLabel5 = "GraniteLabel";

            trackSelect(sender, e);

           

        }

        private void selectTrack3(object sender, MouseButtonEventArgs e)
        {



            Uri myUri3 = new Uri(@"C:\Users\Dar\Desktop\TrainSimulatorCsharp - Copy\TrainSimulatorCsharp\TrainSimulatorCsharp\bin\Debug\descriptionScreenFootage3.wmv", UriKind.Absolute);
            descriptionScreenFootage.Source = myUri3;


            selectedTrackText.Source = ((ImageSource)new ImageSourceConverter().ConvertFrom("track3Text.png"));
            try
            {
                descriptionText.Source = (ImageSource)new ImageSourceConverter().ConvertFrom("ScrollableText3.png");
            }
            catch { System.Diagnostics.Debug.WriteLine("exception in scrollable text3"); }

            selectStart1 = "selectTrail";
            selectStart2 = "selectBrilliant";
            selectStart3 = "selectThrums";
            selectStart4 = "selectSSlocan";
            selectStart5 = "selectGranite";
            selectLabel1 = "TrailLabel";
            selectLabel2 = "BrilliantLabel";
            selectLabel3 = "ThrumsLabel";
            selectLabel4 = "SSlocanLabel";
            selectLabel5 = "GraniteLabel";

            trackSelect(sender, e);
        }


        private void  trackSelect(object sender, MouseButtonEventArgs e)
        { 
            if (UIbuttonsClickable == true) 
            {
                var mySender = sender;
                unhighlightButton(mySender, e);
                gameState = "trackDescription";
                UIbuttonsClickable = false;
                descriptionTextContainer.Visibility = Visibility.Visible;
              
                /// move out select a track message
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(1146, 0, 0, 0, 400, 1, 0, selectATrackText); }), TimeSpan.FromMilliseconds(0));
                /// move in scrolling description text for golden to field 
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 0, -260, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(1000));
                /// move out golden to field and trail to nelson icons
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(331, 0, 0, 0, 200, 1, 0, goldenToFieldIcon); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(775, 0, 0, 0, 200, 1, 0, TrailToNelsonIcon); }), TimeSpan.FromMilliseconds(100));

                /// move in  example video container text divider and golden to field text
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1183, 0, 300, 0, 1, exampleVideoContainer); }), TimeSpan.FromMilliseconds(500));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1173, 0, 300, 0, 1, selectedTextDivider); }), TimeSpan.FromMilliseconds(700));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1129, 0, 200, 1, 0, selectedTrackText); }), TimeSpan.FromMilliseconds(400));

                ///  TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1165, 0, 300, 0, 1, goldenToFieldDescriptionText); }), TimeSpan.FromMilliseconds(900));
                /// move in back and select labels 
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 201, 0, 300, 0, 1, backLabel); }), TimeSpan.FromMilliseconds(1100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -211, 0, 300, 0, 1, selectLabel); }), TimeSpan.FromMilliseconds(1100));
                // wait a second then activate user interface
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(1000));

                ///display description footage in box
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionScreenFootage.Margin = new Thickness(114, 645, 406, 97); }), TimeSpan.FromMilliseconds(800));
                /// after half a second play that footGE
                System.Diagnostics.Debug.WriteLine("descriptionScreenFootage Play");
               

                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionScreenFootage.Play(); }), TimeSpan.FromMilliseconds(800));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, descriptionScreenFootage, 1000); }), TimeSpan.FromMilliseconds(1100));
               
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
                //Make buttons unclickable while they are sliding into position
                UIbuttonsClickable = false;
                //move elements off screen 
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(201, 0, 0, 0, 300, 1, 0, backLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 1, 0, selectLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 1146, 0, 600, 0, 1, selectATrackText); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1129, 0, 0, 0, 200, 1, 0, selectedTrackText); }), TimeSpan.FromMilliseconds(400));
                ////add the other screeens stuff 
                               
                 // display first icon golden to field
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 331, 0, 200, 0, 1, goldenToFieldIcon); }), TimeSpan.FromMilliseconds(1100));
                //display second icon trail to nelson
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 775, 0, 200, 0, 1, TrailToNelsonIcon); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1183, 0, 0, 0, 300, 1, 0, exampleVideoContainer); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1173, 0, 0, 0, 300, 1, 0, selectedTextDivider); }), TimeSpan.FromMilliseconds(400));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, scrollPix, 0, 0, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionTextContainer.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(500));
                descriptionPosition = 0;

                gameState = "trackSelection";
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(1000));

                FadeTheMediaElement(1, 0, descriptionScreenFootage, 200);
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionScreenFootage.Margin = new Thickness(1269, 1013, -489, -273); }), TimeSpan.FromMilliseconds(1000));

            }
            else if (gameState == "startLocationSelection" && UIbuttonsClickable == true)
            {
                var curLabel = new Image();
                backLabel.Opacity = 1;
                UIbuttonsClickable = false;
                double startLocation = 0;
                if (selectedSegment == selectStart1)
                {
                    startLocation = selectStartBox1.Margin.Left;
                    curLabel = StopLabel1;
                    FadeTheMediaElement(1, 0, Stop1Ex, 250);
                    FadeTheMediaElement(1, 0, Stop1Cover, 100);

                }
                else if (selectedSegment == selectStart2)
                {
                    startLocation = selectStartBox2.Margin.Left;
                    curLabel = StopLabel2;
                    FadeTheMediaElement(1, 0, Stop2Ex, 250);
                    FadeTheMediaElement(1, 0, Stop2Cover, 100);
                }
                else if (selectedSegment == selectStart3)
                {
                    startLocation = selectStartBox3.Margin.Left;
                    curLabel = StopLabel3;
                    FadeTheMediaElement(1, 0, Stop3Ex, 250);
                    FadeTheMediaElement(1, 0, Stop3Cover, 100);
                }
                else if (selectedSegment == selectStart4)
                {
                    startLocation = selectStartBox4.Margin.Left;
                    curLabel = StopLabel4;
                    FadeTheMediaElement(1, 0, Stop4Ex, 250);
                    FadeTheMediaElement(1, 0, Stop4Cover, 100);
                }
                else if (selectedSegment == selectStart5)
                {
                    startLocation = selectStartBox5.Margin.Left;
                    curLabel = StopLabel5;
                    FadeTheMediaElement(1, 0, Stop5Ex, 250);
                    FadeTheMediaElement(1, 0, Stop5Cover, 100);
                }

                startLocation = startLocation - 1348;

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, -12, -1296, 0, 200, 0.5, 0.5, curLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-909, -46, -1183, 0, 300, 0.5, 0.5, exampleVideoContainer); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1173, 0, 300, 0, 1, selectedTextDivider); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 600, 1, 0, locationSelectionBar); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLocationSelectionButtons(Visibility.Hidden); }), TimeSpan.FromMilliseconds(200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(startLocation, 0, -1600, 0, 200, 1, 0, selectionIndicatorBar); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1297, 0, 0, 0, 500, 1, 0, selectStartPoint); }), TimeSpan.FromMilliseconds(800));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-2500, 0, -1183, 0, 500, 0, 1, selectedTrackText); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, StopLabel1); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, StopLabel2); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, StopLabel3); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, StopLabel4); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, StopLabel5); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, 0, 0, 500, 1, 0, StopLabel6); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, descriptionScreenFootage, 1000); }), TimeSpan.FromMilliseconds(1700));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 0, -260, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(1500));

                ///////////////////////////////
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionTextContainer.Visibility = Visibility.Visible; }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 1, 0, startLabel); }), TimeSpan.FromMilliseconds(0));//
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -211, 0, 300, 1, 0, selectLabel); }), TimeSpan.FromMilliseconds(300));

                gameState = "trackDescription";

                TimedAction.ExecuteWithDelay(new Action(delegate {

                    Stop1Ex.Pause();
                    Stop2Ex.Pause();
                    Stop3Ex.Pause();
                    Stop4Ex.Pause();
                    Stop5Ex.Pause();

                    Stop2Ex.Margin = new Thickness(1269, 1013, -489, -273);
                    Stop3Ex.Margin = new Thickness(1269, 1013, -489, -273);
                    Stop4Ex.Margin = new Thickness(1269, 1013, -489, -273);
                    Stop5Ex.Margin = new Thickness(1269, 1013, -489, -273);

                    Stop1Ex.Opacity = 0.01;
                    Stop2Ex.Opacity = 0.01;
                    Stop3Ex.Opacity = 0.01;
                    Stop4Ex.Opacity = 0.01;
                    Stop5Ex.Opacity = 0.01;

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
                /// fade away description screen footage
                FadeTheMediaElement(1, 0, descriptionScreenFootage, 200);
                ///remove select label
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 1, 0, selectLabel); }), TimeSpan.FromMilliseconds(0));
                ///move out start label
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -211, 0, 300, 1, 0, startLabel); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1173, 0, -909, -46, 300, 0.5, 0.5, exampleVideoContainer); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1173, 0, 0, 0, 300, 1, 0, selectedTextDivider); }), TimeSpan.FromMilliseconds(400));
               
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 600, 0, 1, locationSelectionBar); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLocationSelectionButtons(Visibility.Visible); }), TimeSpan.FromMilliseconds(1300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1600, 0, -1296, 0, 500, 0, 1, selectionIndicatorBar); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1297, 0, 500, 0, 1, selectStartPoint); }), TimeSpan.FromMilliseconds(700));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1183, 0, -2500, 0, 500, 1, 0, selectedTrackText); }), TimeSpan.FromMilliseconds(200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, StopLabel1); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, StopLabel2); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, StopLabel3); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, StopLabel4); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, StopLabel5); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, -1296, 0, 500, 0, 1, StopLabel6); }), TimeSpan.FromMilliseconds(1200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -1296, -12, 200, 0.5, 0.5, StopLabel1); }), TimeSpan.FromMilliseconds(1800));
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(2800));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, scrollPix, 0, 0, 400, 0, 1, descriptionText); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { descriptionTextContainer.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(500));
                descriptionPosition = 0;
                scrollPix = -260;

                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, Stop1Cover, 200); }), TimeSpan.FromMilliseconds(1500));

                Stop1Ex.Margin = new Thickness(388, 598, 392, 142);

                TimedAction.ExecuteWithDelay(new Action(delegate {

                    Stop1Ex.Opacity = 0.1;
                    Stop1Ex.Play();
                    Stop2Ex.Play();
                    Stop3Ex.Play();
                    Stop4Ex.Play();
                    Stop5Ex.Play();

                }), TimeSpan.FromMilliseconds(1500));

                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0.1, 1, Stop1Ex, 1000); }), TimeSpan.FromMilliseconds(2000));

                TimedAction.ExecuteWithDelay(new Action(delegate {


                    Stop2Ex.Margin = new Thickness(388, 598, 392, 142);
                    Stop3Ex.Margin = new Thickness(388, 598, 392, 142);
                    Stop4Ex.Margin = new Thickness(388, 598, 392, 142);
                    Stop5Ex.Margin = new Thickness(388, 598, 392, 142);

                }), TimeSpan.FromMilliseconds(3000));

                selectedSegment = selectStart1;
                gameState = "startLocationSelection";


            }
            else if (gameState == "startLocationSelection" && UIbuttonsClickable == true)
            {
                UIbuttonsClickable = false;
                selectLabel.Opacity = 1;
                var curLabel = new Image();
                double startLocation = 0;
                if (selectedSegment == selectStart1)
                {
                    startLocation = selectStartBox1.Margin.Left;
                    curLabel = StopLabel1;
                    FadeTheMediaElement(1, 0, Stop1Ex, 250);
                    FadeTheMediaElement(1, 0, Stop1Cover, 100);

                }
                else if (selectedSegment ==  selectStart2)
                {
                    startLocation = selectStartBox2.Margin.Left;
                    curLabel = StopLabel2;
                    FadeTheMediaElement(1, 0, Stop2Ex, 250);
                    FadeTheMediaElement(1, 0, Stop2Cover, 100);
                }
                else if (selectedSegment == selectStart3)
                {
                    startLocation = selectStartBox3.Margin.Left;
                    curLabel = StopLabel3;
                    FadeTheMediaElement(1, 0, Stop3Ex, 250);
                    FadeTheMediaElement(1, 0, Stop3Cover, 100);
                }
                else if (selectedSegment == selectStart4)
                {
                    startLocation = selectStartBox4.Margin.Left;
                    curLabel = StopLabel4;
                    FadeTheMediaElement(1, 0, Stop4Ex, 250);
                    FadeTheMediaElement(1, 0, Stop4Cover, 100);
                }
                else if (selectedSegment == selectStart5)
                {
                    startLocation = selectStartBox5.Margin.Left;
                    curLabel = StopLabel5;
                    FadeTheMediaElement(1, 0, Stop5Ex, 250);
                    FadeTheMediaElement(1, 0, Stop5Cover, 100);
                }



                startLocation = startLocation - 1348;
                outWindow.CabFootageVideo.Visibility = Visibility.Visible;

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, -12, -1296, 0, 200, 0.5, 0.5, curLabel); }), TimeSpan.FromMilliseconds(0));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-909, -46, -2000, -46, 300, 1, 0, exampleVideoContainer); }), TimeSpan.FromMilliseconds(600));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 600, 1, 0, locationSelectionBar); }), TimeSpan.FromMilliseconds(900));
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLocationSelectionButtons(Visibility.Hidden); }), TimeSpan.FromMilliseconds(200));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(startLocation, 0, -1500, 0, 500, 1, 0, selectionIndicatorBar); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1297, 0, -2600, 0, 500, 1, 0, selectStartPoint); }), TimeSpan.FromMilliseconds(800));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, StopLabel1); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, StopLabel2); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, StopLabel3); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, StopLabel4); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, StopLabel5); }), TimeSpan.FromMilliseconds(300));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -2600, 0, 500, 1, 0, StopLabel6); }), TimeSpan.FromMilliseconds(300));
              

                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(3000));
                
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(201, 0, 0, 0, 300, 0, 1, backLabel); }), TimeSpan.FromMilliseconds(300));

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-211, 0, 0, 0, 300, 0, 1, startLabel); }), TimeSpan.FromMilliseconds(300));//

                TimedAction.ExecuteWithDelay(new Action(delegate
                {

                    Stop1Ex.Pause();
                    Stop2Ex.Pause();
                    Stop3Ex.Pause();
                    Stop4Ex.Pause();
                    Stop5Ex.Pause();
                   
                    descriptionScreenFootage.Pause();

                    Stop2Ex.Margin = new Thickness(1269, 1013, -489, -273);
                    Stop3Ex.Margin = new Thickness(1269, 1013, -489, -273);
                    Stop4Ex.Margin = new Thickness(1269, 1013, -489, -273);
                    Stop5Ex.Margin = new Thickness(1269, 1013, -489, -273);

                    Stop1Ex.Opacity = 0.01;
                    Stop2Ex.Opacity = 0.01;
                    Stop3Ex.Opacity = 0.01;
                    Stop4Ex.Opacity = 0.01;
                    Stop5Ex.Opacity = 0.01;


                }), TimeSpan.FromMilliseconds(300));

                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, -564, 0, 400, 1200, 1, 0, selectionScreenBackground); descriptionScreenFootage.Margin = new Thickness(1269, 1013, -489, -273); }), TimeSpan.FromMilliseconds(1700));
                TimedAction.ExecuteWithDelay(new Action(delegate { selectionScreenBackground.Visibility = Visibility.Hidden; toggleExampleVideoVisibility(Visibility.Hidden); }), TimeSpan.FromMilliseconds(3000));

                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0,false); }), TimeSpan.FromMilliseconds(4000));
            }

        }

        private void segmentSelect(object sender, MouseButtonEventArgs e)
        {
            
            var mySender = sender as Rectangle;
            //// cahnge the names to the stored values for this track
            if(mySender.Name == "selectStartBox1")
            { buttonClicked = selectStart1; }
            if (mySender.Name == "selectStartBox2")
            { buttonClicked = selectStart2; }
            if (mySender.Name == "selectStartBox3")
            { buttonClicked = selectStart3; }
            if (mySender.Name == "selectStartBox4")
            { buttonClicked = selectStart4; }
            if (mySender.Name == "selectStartBox5")
            { buttonClicked = selectStart5; }


            if (buttonClicked != selectedSegment && UIbuttonsClickable == true)
            {
                UIbuttonsClickable = false;
                double startLocation = 0;
                double endLocation = mySender.Margin.Left;
                var curLabel = new Image();
                var newLabel = new Image();
                if (selectedSegment == selectStart1)
                {
                    startLocation = selectStartBox1.Margin.Left;
                    curLabel = StopLabel1;
                    FadeTheMediaElement(1, 0, Stop1Ex, 250);
                    FadeTheMediaElement(1, 0, Stop1Cover, 100);
                }
                else if (selectedSegment == selectStart2)
                {
                    startLocation = selectStartBox2.Margin.Left;
                    curLabel = StopLabel2;
                    FadeTheMediaElement(1, 0, Stop2Ex, 250);
                    FadeTheMediaElement(1, 0, Stop2Cover, 100);
                }
                else if (selectedSegment == selectStart3)
                {
                    startLocation = selectStartBox3.Margin.Left;
                    curLabel = StopLabel3;
                    FadeTheMediaElement(1, 0, Stop3Ex, 250);
                    FadeTheMediaElement(1, 0, Stop3Cover, 100);
                }
                else if (selectedSegment == selectStart4)
                {
                    startLocation = selectStartBox4.Margin.Left;
                    curLabel = StopLabel4;
                    FadeTheMediaElement(1, 0, Stop4Ex, 250);
                    FadeTheMediaElement(1, 0, Stop4Cover, 100);
                }
                else if (selectedSegment == selectStart5)
                {
                    startLocation = selectStartBox5.Margin.Left;
                    curLabel = StopLabel5;
                    FadeTheMediaElement(1, 0, Stop5Ex, 250);
                    FadeTheMediaElement(1, 0, Stop5Cover, 100);
                }



                if (buttonClicked == selectStart1)
                {
                    newLabel = StopLabel1;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, Stop1Ex, 250); FadeTheMediaElement(0, 1, Stop1Cover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (buttonClicked == selectStart2)
                {
                    newLabel = StopLabel2;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, Stop2Ex, 250); FadeTheMediaElement(0, 1, Stop2Cover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (buttonClicked == selectStart3)
                {
                    newLabel = StopLabel3;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, Stop3Ex, 250); FadeTheMediaElement(0, 1, Stop3Cover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (buttonClicked == selectStart4)
                {
                    newLabel = StopLabel4;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, Stop4Ex, 250); FadeTheMediaElement(0, 1, Stop4Cover, 250); }), TimeSpan.FromMilliseconds(250));
                }
                else if (buttonClicked == selectStart5)
                {
                    newLabel = StopLabel5;
                    TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, Stop5Ex, 250); FadeTheMediaElement(0, 1, Stop5Cover, 250); }), TimeSpan.FromMilliseconds(250));
                }


                startLocation = startLocation - 1348;
                endLocation = endLocation - 1348;

                TranslateTheMediaElement(startLocation, 0, endLocation, 0, 500, 0.5, 0.5, selectionIndicatorBar);
                TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true ; }), TimeSpan.FromMilliseconds(500));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, -12, -1296, 0, 200, 0.5, 0.5, curLabel); }), TimeSpan.FromMilliseconds(100));
                TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(-1296, 0, -1296, -12, 200, 0.5, 0.5, newLabel); }), TimeSpan.FromMilliseconds(200));
                selectedSegment = buttonClicked;
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
           //// myVis = Visibility.Visible;
            selectStartBox1.Visibility = myVis;
            selectStartBox2.Visibility = myVis;
            selectStartBox3.Visibility = myVis;
            selectStartBox4.Visibility = myVis;
            selectStartBox5.Visibility = myVis;

            Stop1Cover.Visibility = myVis;
            Stop2Cover.Visibility = myVis;
            Stop3Cover.Visibility = myVis;
            Stop4Cover.Visibility = myVis;
            Stop5Cover.Visibility = myVis;
        }

        private void toggleExampleVideoVisibility(Visibility myVis)
        {
          ///  myVis = Visibility.Visible ;
            Stop1Ex.Visibility = myVis;
            Stop2Ex.Visibility = myVis;
            Stop3Ex.Visibility = myVis;
            Stop4Ex.Visibility = myVis;
            Stop5Ex.Visibility = myVis;
            descriptionScreenFootage.Visibility = myVis;
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
                    FadeTheMediaElement(0, 1, outWindow.forst2, 3000);
                    FadeTheMediaElement(1, 0, outWindow.forst1, 3000);
                }
                else if (currentImage == "forst2")
                {
                    currentImage = "forst3";
                    FadeTheMediaElement(0, 1, outWindow.forst3, 3000);
                    FadeTheMediaElement(1, 0, outWindow.forst2, 3000);
                }
                else if (currentImage == "forst3")
                {
                    currentImage = "cpsd402";
                    FadeTheMediaElement(0, 1, outWindow.cpsd402, 3000);
                    FadeTheMediaElement(1, 0, outWindow.forst3, 3000);
                }

                else if (currentImage == "cpsd402")
                {
                    currentImage = "forst1";
                    FadeTheMediaElement(0, 1, outWindow.forst1, 3000);
                    FadeTheMediaElement(1, 0, outWindow.cpsd402, 3000);
                }

            }
            else if (currentFaderIteration == 0)
            {
                FadeTheMediaElement(0, 1, outWindow.forst1, 3000);
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


                if (currentImage == "cpsd402")
                {
                    FadeTheMediaElement(1, 0, outWindow.cpsd402, 3000);
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
            FadeTheMediaElement(1, 0, splashScreen, 300);

            // display background 
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 0, -564, 800, 0.5, 0.5, selectionScreenBackground); }), TimeSpan.FromMilliseconds(0));
            // display "select a track" text
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 1146, 0, 600, 0, 1, selectATrackText); }), TimeSpan.FromMilliseconds(800));
            // Display first track icon Golden to Field
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 331, 0, 400, 0, 1, goldenToFieldIcon); }), TimeSpan.FromMilliseconds(1200));
            // display  second track icon Trail to Nelson
            TimedAction.ExecuteWithDelay(new Action(delegate { TranslateTheMediaElement(0, 0, 775, 0, 400, 0, 1, TrailToNelsonIcon); }), TimeSpan.FromMilliseconds(1200));
            // Change gamestate variable to track selection
            gameState = "trackSelection";
            // make the buttons clickable
            TimedAction.ExecuteWithDelay(new Action(delegate { UIbuttonsClickable = true; }), TimeSpan.FromMilliseconds(1100));
            System.Diagnostics.Debug.WriteLine("buttons clickable go for it");
        }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        /// animation routines and objexts
        ///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


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
            ////////////////////////////////////////////////////wht is this//////////////////////

            scaleTrans.CenterX = myMediaElement.Width /2;          ////////////////
            scaleTrans.CenterY = myMediaElement.Height / 2;         ////////////////

            myMediaElement.Margin = new Thickness(newMarginX, newMarginY, 0, 0);

            scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, aniX);
            scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, aniY);

        }


        ///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ///preflight checks of control positions
        ///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


        private void preflightChecks(int iter, bool throttlepage)
        {

            //// independant  BRAKE CHECK POSITION

            if (my8_8_8.inputs[1] == false && iter == 0)                /// if independant brake position <>0 
            {
                instructionsLabel.Content = "Release Locomotive Brake!";
                TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, indInstructions);
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(1, false); }), TimeSpan.FromMilliseconds(500));

            }


            else if (my8_8_8.inputs[1] == false && iter == 1)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(1, false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my8_8_8.inputs[1] == true && iter == 1)
            {
                TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, indInstructions);
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, false); }), TimeSpan.FromMilliseconds(500));
            }
            ////main brake position

            else if (my16_16_0.inputs[3] == false && iter == 0)
            {
                instructionsLabel.Content = "Release Main Brake!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, brakeInstructions);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(2, false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my16_16_0.inputs[3] == false && iter == 2)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(2, false); }), TimeSpan.FromMilliseconds(500));
            }


            else if (my16_16_0.inputs[3] == true && iter == 2)
            {
                TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, brakeInstructions);
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, false); }), TimeSpan.FromMilliseconds(500));
            }

            ////////////  ////check throttle position

            else if (throttlePosition != Throttle_Idle_Position && iter == 0)
            {

                if (throttlePosition > Throttle_Idle_Position && ThrottlePageDOWN != true)
                {
                    if (ThrottlePageUP == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsup);
                        ThrottlePageUP = false;
                    }    ///remove up instructions

                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructionsdown);      /// post down instructions  
                    //////newidea
                    ThrottlePageDOWN = true;
                }
                if (throttlePosition < Throttle_Idle_Position && ThrottlePageUP != true)
                {
                    if (ThrottlePageDOWN == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);        /// remove down instructions
                        ThrottlePageDOWN = false;
                    }
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructionsup);        ///post up instructions
                    ThrottlePageUP = true;
                    

                }
                instructionsLabel.Content = "Set Throttle To Idle!";
                ////////////// fade instruction label
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(3, true); }), TimeSpan.FromMilliseconds(500));

                }

                else if (throttlePosition != Throttle_Idle_Position && iter == 3)
                {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(3, true); }), TimeSpan.FromMilliseconds(500));////////
                }


                else if (throttlePosition == Throttle_Idle_Position && iter == 3)
                {
                if (my8_8_8.inputs[1] == false || my16_16_0.inputs[3] == false)
                {

                    ////remove throttle instructions
                    if (ThrottlePageUP == true)
                    { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsup);
                        ThrottlePageUP = false;
                    }
                    if (ThrottlePageDOWN == true)
                    { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);
                        ThrottlePageDOWN = false;
                    }
                                      
                }
                /////////fade instruction
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                if (ThrottlePageUP == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsup);
                    ThrottlePageUP = false;
                }
                if (ThrottlePageDOWN == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);
                    ThrottlePageDOWN = false;
                }
                    FadeTheMediaElement(1, 0, instructionsLabel, 300);
                    TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
            }

            //////////  dynamic selector

            else if (dynamicPosition != Dynamic_Off_Position && iter == 0)
            {
                if (dynamicPosition > Dynamic_Off_Position && DynamicPageAPPLY != true)
                {
                    if (DynamicPageRELEASE == true)
                    { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionrelease);
                        DynamicPageRELEASE = false;

                    }
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, dynamicinstructionapply);
                    DynamicPageAPPLY = true;
                }

                if (dynamicPosition < Dynamic_Off_Position && DynamicPageRELEASE != true)

                {
                    if (DynamicPageAPPLY == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionapply);
                        DynamicPageAPPLY = false;
                    }
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, dynamicinstructionrelease);
                    DynamicPageRELEASE = true;
                }

                instructionsLabel.Content = "Set Dynamic Selector to 1!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(4, true); }), TimeSpan.FromMilliseconds(500));

            }


            else if (dynamicPosition != Dynamic_Off_Position && iter == 4)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(4, true); }), TimeSpan.FromMilliseconds(500));

            }


            else if (dynamicPosition == Dynamic_Off_Position && iter == 4)
            {
                if (my8_8_8.inputs[1] == false || my16_16_0.inputs[3] == false)
                {
                if (DynamicPageRELEASE == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionrelease);
                        DynamicPageRELEASE = false;
                    }
                if (DynamicPageAPPLY == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionapply);
                        DynamicPageAPPLY = false;
                    }
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
                                               
                }

                //if dynamic is correct  remove dynamic instructions
                if (DynamicPageRELEASE == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionrelease); }
                if (DynamicPageAPPLY == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionapply); }
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
                DynamicPageRELEASE = false;
                DynamicPageAPPLY = false;

            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////check reverser

            else if (my16_16_0.inputs[13] == false && iter == 0 && throttlePosition == Throttle_Idle_Position)
            {
                if (ReverserThrottlePageUP == true)
                {
                    ///remove any throttleUP instructions
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsup);
                    instructionsSupLabel.Opacity = 0;   ///remove secondary label just in case

                    ReverserThrottlePageUP = false;
                    ReverserPage = false;
                }
                if (ReverserThrottlePageDOWN == true)
                {
                    ///remove any throttleDOWN instructions
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);
                    instructionsSupLabel.Opacity = 0;   ///remove secondary label just in case

                    ReverserThrottlePageDOWN = false;
                    ReverserPage = false;
                }


                if (ReverserPage == false)
                {
                    ///post reverser instructions
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, reverserinstructions);
                    ReverserPage = true;
                }
                instructionsLabel.Content = "Set Reverser to Forward!";
                FadeTheMediaElement(0, 1, instructionsLabel, 300);
                instructionsSupLabel.Opacity = 0;   ///remove secondary label just in case
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(5, true); }), TimeSpan.FromMilliseconds(500));
            }

            else if (my16_16_0.inputs[13] == false && iter == 5)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(5, true); }), TimeSpan.FromMilliseconds(500));
            }


            ////////////////////////////////////////////////

            else if (my16_16_0.inputs[13] == false && iter == 5)
            {
                if (throttlePosition != Throttle_Idle_Position && throttlePosition > Throttle_Idle_Position && ReverserThrottlePageDOWN == false)
                {
                    if (ReverserPage == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, reverserinstructions);  ///reomove reverser instructions
                        ReverserPage = false;
                    }
                    /////////////if throttle page up is displayed remove it
                    if (ReverserThrottlePageUP == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsup);   ///remove up 
                        ReverserThrottlePageUP = false;
                    }
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructionsdown);      //Post down
                    ReverserThrottlePageDOWN = true;
                }
                if (throttlePosition != Throttle_Idle_Position && throttlePosition < Throttle_Idle_Position && ReverserThrottlePageUP == false)

                {
                    /// if reverser is up remove it
                    if (ReverserPage == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, reverserinstructions);
                        ReverserPage = false;
                    }
                    /////////////if throttle page down is displayed remove it
                    if (ReverserThrottlePageDOWN == true)
                    {
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);   ///remove down 
                        ReverserThrottlePageDOWN = false;
                    }

                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructionsup);  ////post up

                    ReverserThrottlePageUP = true;
                }
                instructionsSupLabel.Content = "(Throttle Must be in Idle Position to move Reverser)";
                instructionsSupLabel.Opacity = 1;
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(6, true); }), TimeSpan.FromMilliseconds(500));
            }

            else if (my16_16_0.inputs[13] == false && iter == 6)
            {
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(6, true); }), TimeSpan.FromMilliseconds(500));
            }

            //////////////////////////////

            ////If reverser is okay
            else if (my16_16_0.inputs[13] == true && iter == 6)
            {
                instructionsSupLabel.Opacity = 0;

                //remove instructions 
                if (ReverserPage == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, reverserinstructions); } //// remove reverser instructions                    
                if (ReverserThrottlePageUP == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsup); }   ////remove throttle up
                if (ReverserThrottlePageDOWN == true)
                { TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown); } //// remove throttle down
               
                FadeTheMediaElement(1, 0, instructionsLabel, 300);
                TimedAction.ExecuteWithDelay(new Action(delegate { preflightChecks(0, true); }), TimeSpan.FromMilliseconds(500));
                ReverserPage = false;
                ReverserThrottlePageUP = false;
                ReverserThrottlePageDOWN = false;
                ReverserThrottlePageDOWN = false;
            }

            else
            {
                if (ReverserPage == true)
                {

                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, reverserinstructions);
                    ReverserPage = false;

                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                // analog inputs throttle and dynamic selector values are claibrated here values are compared to "1" position
                //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                //////calibrate throttle and dynamic brake
                //// Calibrate throttle position "1"

                if (throttle_calibration_value + throttle_deadzone != actual_throttle_1_pos)
                {
                    if (throttle_calibration_value + throttle_deadzone < actual_throttle_1_pos)
                    {
                        throttle_calibration_value = throttle_calibration_value + 1;
                    }
                    else if (throttle_calibration_value + throttle_deadzone > actual_throttle_1_pos)
                    {
                        throttle_calibration_value = throttle_calibration_value - 1;
                    }
                    ThrottleCalibrationpositionlabel.Content = "throttle cal: " + Convert.ToString(throttle_calibration_value);
                }

                //// Calibrate dynamic brake off position "1"

                if (dynamic_calibration_value + dynamic_deadzone != actual_dynamic_1_pos)
                {
                    if (dynamic_calibration_value + dynamic_deadzone < actual_dynamic_1_pos)
                    {
                        dynamic_calibration_value = dynamic_calibration_value + 1;
                    }

                    else if (dynamic_calibration_value + dynamic_deadzone > actual_dynamic_1_pos)
                    {
                        dynamic_calibration_value = dynamic_calibration_value - 1;
                    }


                    DynamicCalibrationpositionlabel.Content = "dynamic cal: " + Convert.ToString(dynamic_calibration_value);
                }
                
                LaunchGame();
                instructionsLabel.Opacity = 0;

               

            }
        }

        //Game Control
        private void LaunchGame()
        {
            TimedAction.ExecuteWithDelay(new Action(delegate { TrainSimulatorLogo.Visibility = Visibility.Visible; TranslateTheMediaElement(0, 500, 0, -200, 500, 0, 1, TrainSimulatorLogo); }), TimeSpan.FromMilliseconds(200));
            TimedAction.ExecuteWithDelay(new Action(delegate { fuelBarBackground.Visibility = Visibility.Visible; TranslateTheMediaElement(0, 500, 0, 0, 500, 0, 1, fuelBarBackground); }), TimeSpan.FromMilliseconds(0));
            TimedAction.ExecuteWithDelay(new Action(delegate { fuelBar.Visibility = Visibility.Visible; updateFuelBar(1500); toggleDisplayLights(false); }), TimeSpan.FromMilliseconds(800));
            TimedAction.ExecuteWithDelay(new Action(delegate { fuelBarMessage.Visibility = Visibility.Visible; }), TimeSpan.FromMilliseconds(800));
            gameState = "inGame";

            if (selectedSegment == "selectGolden")
            {
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(0);
                velocity = 12;
                bendIndex = 0;
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();
            }
            else if (selectedSegment == "selectGlenogle")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(324);
                velocity = 20;
                bendIndex = 18;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();

            }
            else if (selectedSegment == "selectPalliser")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(896);
                velocity = 26;
                bendIndex = 50;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();
            }
            else if (selectedSegment == "selectLeanchoil")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(1408);
                velocity = 28;
                bendIndex = 78;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                //outWindow.CabFootageVideo.Play();
            }
            else if (selectedSegment == "selectOttertail")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(2208);
                velocity = 35;
                bendIndex = 98;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
            }

            else if (selectedSegment == "selectTrail")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(0);
                velocity = 20;
                bendIndex = 18;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();

            }
            else if (selectedSegment == "selectBrilliant")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(324);
                velocity = 20;
                bendIndex = 18;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();

            }

            else if (selectedSegment == "selectThrums")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(896);
                velocity = 26;
                bendIndex = 50;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();
            }
            else if (selectedSegment == "selectSSlocan")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(1408);
                velocity = 26;
                bendIndex = 50;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();
            }

            else if (selectedSegment == "selectGranite")
            {
                outWindow.CabFootageVideo.Play();
                //outWindow.CabFootageVideo.Pause();
                outWindow.CabFootageVideo.Position = TimeSpan.FromSeconds(2206);
                velocity = 26;
                bendIndex = 50;
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(3000));
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0, 1, outWindow.CabFootageVideo, 3000); }), TimeSpan.FromMilliseconds(3000));
                outWindow.CabFootageVideo.Play();
            }


            ///  play engine start up
            /// reset segment selection

            selectedSegment = "";
            AudioPlaybackEngine.Instance.PlaySound(engineStartup);

        }

        private  void EndGame()
        {
            gameTimer.Tick -= new EventHandler(continueCountDown);

            outWindow.CabFootageVideo.SpeedRatio = 1;
            FadeTheMediaElement(0.5, 0, outWindow.CabFootageVideo, 1000);
            TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.CabFootageVideo.Position = new TimeSpan(0); outWindow.CabFootageVideo.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(1200));
            fuelBar.Visibility = Visibility.Hidden;
            fuelBarBackground.Visibility = Visibility.Hidden;
            fuelBarMessage.Visibility = Visibility.Hidden;


            FadeTheMediaElement(1, 0, continueScreenLabel, 500);
            FadeTheMediaElement(1, 0, outWindow.continueMainScreenLabel, 500);
            FadeTheMediaElement(1, 0, continueCountdownLabel, 500);
            FadeTheMediaElement(1, 0, outWindow.continueMainCountdownLabel, 500);

            TimedAction.ExecuteWithDelay(new Action(delegate {
                instructionsLabel.Visibility = Visibility.Hidden;
                instructionsSupLabel.Visibility = Visibility.Hidden;
                throttleInstructionsdown.Visibility = Visibility.Hidden;
                throttleInstructionsup.Visibility = Visibility.Hidden;
                dynamicinstructionapply.Visibility = Visibility.Hidden;
                dynamicinstructionrelease. Visibility = Visibility.Hidden;
                brakeInstructions.Visibility = Visibility.Hidden;
                indInstructions.Visibility = Visibility.Hidden;
                TrainSimulatorLogo.Visibility = Visibility.Hidden; 
               
                toggleExampleVideoVisibility(Visibility.Visible);

                toggleDisplayLights(true); 
                continueScreenLabel.Visibility = Visibility.Hidden;
                outWindow.continueMainScreenLabel.Visibility = Visibility.Hidden;
                continueCountdownLabel.Visibility = Visibility.Hidden;                
                outWindow.continueMainCountdownLabel.Visibility = Visibility.Hidden;

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
       /////         myAnalogOut.outputs[1].Voltage = 0;
       ////         myAnalogOut.outputs[0].Voltage = 0;
            }));
            penaltyBrake = false;
            penaltyBrakeCountDown = 0;

        }

        private void reachedFieldLoop(object sender, EventArgs e)
        {
            if (outWindow.CabFootageVideo.SpeedRatio > 1)
            {
                if (iterCounter < 5)
                {
                    iterCounter += 1;

                }
                else
                {
                    iterCounter = 0;
                    outWindow.CabFootageVideo.SpeedRatio -= 0.1;
                }
            }



            double displayMPH = velocity - (Convert.ToDouble(outWindow.CabFootageVideo.Position.TotalSeconds) - 2750) * velocityDec;
            if (displayMPH >= 0)
            {
                updateSpeedometer(displayMPH);
            }
            else
            {
                updateSpeedometer(0);
            }

            playbackSpeedratioLabel.Content = "Playback Speed: " + outWindow.CabFootageVideo.SpeedRatio;
            videoTrackSpeedLabel.Content = "Video MPH: " + displayMPH;

            if (outWindow.CabFootageVideo.Position >= System.TimeSpan.FromSeconds(2875))
            {
                gameTimer.Tick -= new EventHandler(reachedFieldLoop);
                updateSpeedometer(0);
                FadeTheMediaElement(1, 0, outWindow.CabFootageVideo, 1000);
                if (timeLeft < 120 )
                {
                    timeLeft = 120;
                }
                TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.CabFootageVideo.Position = TimeSpan.FromMilliseconds(0); }), TimeSpan.FromMilliseconds(1000));
                TimedAction.ExecuteWithDelay(new Action(delegate { selectionScreenBackground.Visibility = Visibility.Visible; LoadInButtons(); }), TimeSpan.FromMilliseconds(1000));
                TimedAction.ExecuteWithDelay(new Action(delegate { 
                    TrainSimulatorLogo.Visibility = Visibility.Hidden; 
                    fuelBarBackground.Visibility = Visibility.Hidden; 
                    fuelBar.Visibility = Visibility.Hidden;
                    fuelBarMessage.Visibility = Visibility.Hidden;
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
                TimedAction.ExecuteWithDelay(new Action(delegate { FadeTheMediaElement(0.5, 1, outWindow.CabFootageVideo, 500); }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.CabFootageVideo.Play(); }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameTimer.Tick += new EventHandler(gameLoop); }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameState = "inGame"; continueCountdownLabel.Visibility = Visibility.Hidden; continueScreenLabel.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(2000));
                TimedAction.ExecuteWithDelay(new Action(delegate { gameState = "inGame"; outWindow.continueMainCountdownLabel.Visibility = Visibility.Hidden; outWindow.continueMainScreenLabel.Visibility = Visibility.Hidden; }), TimeSpan.FromMilliseconds(2000));
               
                timeLeft += secondsPerDollar * 2;
                updateFuelBar(1700);
                continueTimeLeft = 0;
                FadeTheMediaElement(1, 0, continueScreenLabel, 500);
                FadeTheMediaElement(1, 0, outWindow.continueMainScreenLabel, 500);
                FadeTheMediaElement(1, 0, continueCountdownLabel, 500);
                FadeTheMediaElement(1, 0, outWindow.continueMainCountdownLabel, 500);

                gameTimer.Tick -= new EventHandler(continueCountDown);
                gameState = "inGame";
                if (LowFuelWarning == true)
                {
                    my16_16_0.outputs[14] = false;
                    LowFuelWarning = false;
                }


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
                if (LowFuelWarning == true)
                {
                    my16_16_0.outputs[14] = false;
                    LowFuelWarning= false;
                }

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

            int bendStateTemp = getBendState(outWindow.CabFootageVideo.Position.TotalSeconds);

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
            if (outWindow.CabFootageVideo.Position >= System.TimeSpan.FromSeconds(2750))
            {
                velocityDec = velocity / 122;
                gameTimer.Tick -= new EventHandler(gameLoop);
                gameTimer.Tick += new EventHandler(reachedFieldLoop);
                //apply the brakes
                
            }
            videoTrackSpeedLabel.Content = "Video MPH: " + getVideoMPH(Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds));
            playbackSpeedratioLabel.Content = "Playback Speed: " + outWindow.CabFootageVideo.SpeedRatio;
            videoPositionLabel.Content = "Video Position: " + Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds);
            gradeLabel.Content = "Uphill Grade: " + getGrade(Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds));
            bendStateLabel.Content = "Bend State: " + bendState;
            PenaltyBrakeLabel.Content = "Penalty Brake: " + penaltyBrake;
            PenaltyCountdownLabel.Content = "Penalty Countdown: " + penaltyBrakeCountDown;
            
        }

        private void updatePlaybackSpeed()
        {
            double newPlaybackSpeed = Math.Round(velocity / getVideoMPH(Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds)), 1);

            if (outWindow.CabFootageVideo.SpeedRatio != newPlaybackSpeed)
            {
                if (newPlaybackSpeed >= 3)
                {
                    outWindow.CabFootageVideo.SpeedRatio = 2.9;
                }
                else if (newPlaybackSpeed < 0)
                {
                    outWindow.CabFootageVideo.SpeedRatio = 0;
                }
                else
                {
                    outWindow.CabFootageVideo.SpeedRatio = newPlaybackSpeed;
                }
                
            }

        }

        private void calculateVelocity()
        {

            
            gradient = getGrade(Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds));  ///get grade multiply out % to get grade in dec
            gradeDeg = Math.Atan(gradient / 100);    ///do inverse tan to get grade in degrees
            sinGradient = Math.Sin(gradeDeg);        /// do sin of gradient 
            velocityMs = velocity / 2.237;

            ///Drag on train friction wind bearing drag etc
            trainDrag = 2000 + (20 * velocityMs) + (3.5 * (velocityMs * velocityMs));
            ///dynamic brake effort
            if (dynamicPosition == Dynamic_On_Position || dynamicPosition == 3 || dynamicPosition == 4 || dynamicPosition == 5 && velocity >= 10)
            { dynBrakeEffort = ((maxDynBrakeEffort / 3) * (dynamicPosition - 1)); }
            if (dynamicPosition != Dynamic_On_Position || dynamicPosition != 3 || dynamicPosition != 4 || dynamicPosition != 5) { dynBrakeEffort = 0; }

            //// maximum braking force avaliable based on avaliable traction 
            if (sandingPlaying == true && sandingToggle == true)
            { 
                maxLocoBrakeForce = (tractionFactorDsand * locomotiveWeight) ;
            }

            else if (sandingPlaying == true && sandingToggle != true)
            {
                maxLocoBrakeForce = (tractionFactorSsand * locomotiveWeight) ;
            }

            else
            {
                maxLocoBrakeForce = tractionFactor * locomotiveWeight ;
            }

            maxCarBrakeForce = tractionFactor * carWeight;
            ////////////////////////////////////////////////////   separate car and locomotive caLCULATIONS//////


            if (bailActivated == true)
            { indBrakeEffort = 0; //if bail held down then no locomotive brakes

            }  


            locoBrakeForce = (maxLocoBrakeForce * indBrakeEffort) + dynBrakeEffort;
            carBrakeForce = (maxCarBrakeForce * mainBrakeEffort);


            trainBrakeForce = locoBrakeForce + carBrakeForce ;
           
           
//////////////////////

            if (locoBrakeForce > maxLocoBrakeForce)
            {
                if (iter2 == 0)
                {
                    ///remove logo
                    FadeTheMediaElement(1, 0, TrainSimulatorLogo, 300);                   
                    /// fade  fuel bar background
                    FadeTheMediaElement(1, 0, fuelBarBackground, 500);
                    //// fade fuel bar
                    FadeTheMediaElement(1, 0, fuelBar, 500);
                    FadeTheMediaElement(1, 0, fuelBarMessage, 500);

                    //// activate wheel slippagelight perhaps flashing 
                    my16_16_0.outputs[10] = true;
                    /// display wheel slippage reduce brake setting message
                    instructionsLabel.Content = "Warning Locomotive Slippage!";
                    FadeTheMediaElement(0, 1, instructionsLabel, 500);
                    if (getMainBrakeState() != 0)
                    {
                        instructionsSupLabel.Content = "Release Automatic Brake";
                        FadeTheMediaElement(0, 1, instructionsSupLabel, 500);
                        ///put out mainbrake 
                        TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, brakeInstructions);
                        instructionState = "mainBrake";
                        iter2 = 1;
                    }
                    if (getMainBrakeState() == 0 && getIndependantBrakeState() != 0)
                    {
                        instructionsSupLabel.Content = "Release locomotive Brake";
                        FadeTheMediaElement(0, 1, instructionsSupLabel, 500);
                        /// put out indbrake 
                        TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, indInstructions);
                        instructionState = "indBrake";
                        iter2 = 1;
                    }
                    if (getMainBrakeState() == 0 && getIndependantBrakeState() == 0 && dynamicPosition != Dynamic_Off_Position)
                    {
                        instructionsSupLabel.Content = "Reduce Dynamic Brake";
                        FadeTheMediaElement(0, 1, instructionsSupLabel, 500);
                        /// put out dyn brake label
                        TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructionsdown);      /// post down instructions  
                        instructionState = "dynBrake";
                        iter2 = 1;

                    }
                }
              
            }



////////////////////////////////////////////////////////////
            else if (locoBrakeForce < maxLocoBrakeForce)
            {
                if (iter2 == 1)
                {
                    ///replace logo               
                    FadeTheMediaElement(0, 1, TrainSimulatorLogo, 1000);
                    ///replace fuel bar background
                    FadeTheMediaElement(0, 1, fuelBarBackground, 500);
                    ///replace fuel bar
                    FadeTheMediaElement(0, 1, fuelBar, 500);
                    FadeTheMediaElement(0, 1, fuelBarMessage, 500);
                    ///remove instruction labels
                    FadeTheMediaElement(1, 0, instructionsLabel, 500);
                    FadeTheMediaElement(1, 0, instructionsSupLabel, 500);

                    /// remove instruction graphics
                    if (instructionState == "mainBrake")
                    {
                        ///remove mainBrake instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 0, 1, brakeInstructions);
                    }
                    if (instructionState == "indBrake")
                    {
                        /// remove indBrake Instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 0, 1, indInstructions);
                    }
                    if (instructionState == "dynBrake")
                    {  ///remove DynBrake instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);

                    }
                    instructionState = "";
                    //// turnoff wheel slippage light
                    my16_16_0.outputs[10] = false;


                        iter2 = 2;                   
                }

                if(iter2 == 2)
                {
                    TimedAction.ExecuteWithDelay(new Action(delegate { iter2 = 3; }), TimeSpan.FromMilliseconds(500));
                }
                if(iter2 == 3)
                {
                    iter2 = 0;
                }
            }

            ////////////////////////////////////////////////////////////

            //////////////////////

            if (carBrakeForce > maxCarBrakeForce)
            {
                if (iter2 == 0 && instructionState =="")
                {
                    ///remove logo
                    FadeTheMediaElement(1, 0, TrainSimulatorLogo, 300);
                    /// remove fuel bar background                   
                    FadeTheMediaElement(1, 0, fuelBarBackground, 500);
                    //// fade fuel bar
                    FadeTheMediaElement(1, 0, fuelBar, 500);
                    FadeTheMediaElement(1, 0, fuelBarMessage, 500);
                    /// display wheel slippage reduce brake setting message
                    instructionsLabel.Content = "Warning Car Slippage!";
                    FadeTheMediaElement(0, 1, instructionsLabel, 500);
                    if (getMainBrakeState() != 0)
                    {
                        instructionsSupLabel.Content = "Release Automatic Brake";
                        FadeTheMediaElement(0, 1, instructionsSupLabel, 500);
                        ///put out mainbrake 
                        TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, brakeInstructions);
                        instructionState = "mainBrake";
                        iter2 = 1;
                    }
                    if (getMainBrakeState() == 0 && getIndependantBrakeState() != 0)
                    {
                        instructionsSupLabel.Content = "Release locomotive Brake";
                        FadeTheMediaElement(0, 1, instructionsSupLabel, 500);
                        /// put out indbrake 
                        TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, indInstructions);
                        instructionState = "indBrake";
                        iter2 = 1;
                    }
                   
                }

            }



            ////////////////////////////////////////////////////////////
            else if (carBrakeForce < maxCarBrakeForce)
            {
                if (iter2 == 1)
                {
                    ///replace logo               
                    FadeTheMediaElement(0, 1, TrainSimulatorLogo, 1000);
                    /// replace fuel bar background                   
                    FadeTheMediaElement(0, 1, fuelBarBackground, 500);
                    //// replace  fuel bar
                    FadeTheMediaElement(0, 1, fuelBar, 500);
                    FadeTheMediaElement(0, 1, fuelBarMessage, 500);
                    ///remove instruction labels
                    FadeTheMediaElement(1, 0, instructionsLabel, 500);
                    FadeTheMediaElement(1, 0, instructionsSupLabel, 500);

                    /// remove instruction graphics
                    if (instructionState == "mainBrake")
                    {
                        ///remove mainBrake instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 0, 1, brakeInstructions);
                    }
                    if (instructionState == "indBrake")
                    {
                        /// remove indBrake Instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 0, 1, indInstructions);
                    }
                    if (instructionState == "dynBrake")
                    {  ///remove DynBrake instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, throttleInstructionsdown);
                    }
                    instructionState = "";
                  
                    iter2 = 2;
                }

                if (iter2 == 2)
                {
                    TimedAction.ExecuteWithDelay(new Action(delegate { iter2 = 3; }), TimeSpan.FromMilliseconds(500));
                }
                if (iter2 == 3)
                {
                    iter2 = 0;
                }
            }

            ////////////////////////////////////////////////////////////

            /// maximum effort from the motors at a given speed hence maximum amps so set max motor amps to this 
            if (velocityMs > 0 && velocityMs < 4.2)
            {
                maxTractiveEffort = maxMotorEffort;
            }
            else if (velocityMs > 4.2 && velocityMs < 24.9)
            {
                maxTractiveEffort = (maxMotorEffort +6100) - (1440 * velocityMs);
            }
            else if (velocityMs > 24.9 && velocityMs < 45)
            {
                maxTractiveEffort = (maxMotorEffort-16700) - (525 * velocityMs);
            }

            trainTractiveEffort = ((maxTractiveEffort / 8) * throttlePosition-1);

            if (dynamicPosition ==  Dynamic_On_Position || dynamicPosition == 3 || dynamicPosition == 4 || dynamicPosition == 5)

            {

               if (iter4 == 0 && instructionState ==""){
                    ///remove logo
                    FadeTheMediaElement(1, 0, TrainSimulatorLogo, 300);
                    /// fade  fuel bar background
                    FadeTheMediaElement(1, 0, fuelBarBackground, 500);
                    //// fade fuel bar
                    FadeTheMediaElement(1, 0, fuelBar, 500);
                    FadeTheMediaElement(1, 0, fuelBarMessage, 500);
                    //// post dynamic brake active instructions
                    instructionsLabel.Content = "Dynamic Brake active";
                    FadeTheMediaElement(0, 1, instructionsLabel, 500);
                    /// put out dyn brake label
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, dynamicinstructionrelease);      /// post down instructions  
                    instructionState = "dynBrake";
                    iter4 = 1;
                        }
                if (iter4 == 1)
                { }
            }

            if (dynamicPosition != Dynamic_On_Position || dynamicPosition != 3 || dynamicPosition != 4 || dynamicPosition != 5 && iter4 ==1 )
            {
                ///replace logo               
                FadeTheMediaElement(0, 1, TrainSimulatorLogo, 1000);
                /// replace fuel bar background                   
                FadeTheMediaElement(0, 1, fuelBarBackground, 500);
                //// replace  fuel bar
                FadeTheMediaElement(0, 1, fuelBar, 500);
                FadeTheMediaElement(0, 1, fuelBarMessage, 500);
                ///remove instruction labels
                FadeTheMediaElement(1, 0, instructionsLabel, 500);
               
                if (instructionState == "dynBrake")
                {  ///remove DynBrake instructions
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionrelease);
                    instructionState = "";
                }
               
                iter4 = 0;

            }


            //// maximum tractive force avaliable based on avaliable traction 
            if (sandingPlaying == true && sandingToggle == true)
            {
                maxTractiveForce = tractionFactorDsand * locomotiveWeight;
            }

            else if (sandingPlaying == true && sandingToggle != true)
            {
                maxTractiveForce = tractionFactorSsand * locomotiveWeight;
            }

            else
            {
                maxTractiveForce = tractionFactor * locomotiveWeight;
            }











            if (trainTractiveEffort > maxTractiveForce)    ///same traction value so used braking calc
            {
                if (iter3 == 0)
                {

                    if (instructionState == "dynBrake")
                    {  ///remove DynBrake instructions
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 1, 0, dynamicinstructionrelease);
                        instructionState = "";
                    }



                    ///turnon wheel slippage light 
                    my16_16_0.outputs[10] = true;
                    /// remove logo
                    FadeTheMediaElement(1, 0, TrainSimulatorLogo, 300);
                 //// fade fuel bar background
                FadeTheMediaElement(1, 0, fuelBarBackground, 500);
                //// fade fuel bar
                FadeTheMediaElement(1, 0, fuelBar, 500);
                FadeTheMediaElement(1, 0, fuelBarMessage, 500);



                /// bring out throttle DOWN graphic
                TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, throttleInstructionsdown);      //Post down
                /// display wheel slippage message
                instructionsLabel.Content = "Warning Locomotive Slippage!";
                FadeTheMediaElement(0, 1, instructionsLabel, 500);
                /// display reduce throttle setting message
                instructionsSupLabel.Content = "Reduce Throttle Setting";
                FadeTheMediaElement(0, 1, instructionsSupLabel, 500);
                iter3 = 1;
                    instructionState = "throttle";
                }
            }

            else if (trainTractiveEffort <= maxTractiveForce)
            {
                if (iter3 == 1)
                {
                    ///turnoff wheel slippage light
                    my16_16_0.outputs[10] = false;
                    /// replace logo
                    FadeTheMediaElement(0, 1, TrainSimulatorLogo, 1000);
                    //// replace fuel bar background
                    FadeTheMediaElement(0, 1, fuelBarBackground, 500);
                    //// replace fuel bar 
                    FadeTheMediaElement(0, 1, fuelBar, 500);
                    FadeTheMediaElement(0, 1, fuelBarMessage, 500);

                    /// remove wheel slippage message
                    if (instructionState == "throttle") { 
                    TranslateTheMediaElement(-1196, 0, 0, 0, 500, 0, 1, throttleInstructionsdown);      //remove down
                     }
                    FadeTheMediaElement(1, 0, instructionsLabel, 500);
                    /// remove reduce throttle setting message
                    FadeTheMediaElement(1, 0, instructionsSupLabel, 500);
                    iter3 = 2;
                }
                if (iter3 == 2 )
                {
                   
                    TimedAction.ExecuteWithDelay(new Action(delegate { iter3 = 3; }), TimeSpan.FromMilliseconds(500));                   
                }
                if(iter3 == 3)
                {
                    iter3 = 0;
                }
              
            }

            /////////////// limit forces to maximums

            if ( trainBrakeForce > maxLocoBrakeForce) 
            {  trainBrakeForce = maxLocoBrakeForce;
                }
            if (trainTractiveEffort > maxTractiveForce)
            { trainTractiveEffort = maxTractiveForce;
                }
            ////////////////////////////////take bend state and converrt to corner deg multiply by 0.8 pounds per ton 
            bendDrag = ((getBendState(outWindow.CabFootageVideo.Position.TotalSeconds)) * 4)*(0.8*(carWeight/2000));

            trainAcceleration = (((trainTractiveEffort - trainDrag) - ((locomotiveWeight + carWeight) * sinGradient) - trainBrakeForce - bendDrag))/100000;
        
                velocity = velocity + trainAcceleration;

           ///      tempVelocity = (MaxVelocity - velocity) / (MaxVelocity) * (0.09 * (throttlePosition - 1) - ((0.02 * dynamicPosition) + (0.08 * getIndependantBrakeState()) + (0.2 * getMainBrakeState()) + (0.1 * getGrade(Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds)) + (0.07))));

                    System.Diagnostics.Debug.WriteLine("trainAcceleration");
                    System.Diagnostics.Debug.WriteLine(trainAcceleration);
                    System.Diagnostics.Debug.WriteLine(bendDrag);
                    System.Diagnostics.Debug.WriteLine(dynBrakeEffort);

            /// max calculated speed  = (trainTractiveEffort - trainDrag) / (trainWeight * sinGradient) = 0 ////...no acceleration
            /// 

            /// old method
             
            //Throttle, Brake, Independant brake, Dynamic Brake, Grade, Friction, Current Speed, Max speed
            /// velocity = velocity + (MaxVelocity - velocity) / (MaxVelocity) * (0.09 * (throttlePosition - 1) - ((0.02 * dynamicPosition) + (0.08 * getIndependantBrakeState()) + (0.2 * getMainBrakeState()) + (0.1 * getGrade(Convert.ToInt32(outWindow.CabFootageVideo.Position.TotalSeconds)) + (0.07))));

            if (velocity > MaxVelocity)
                {
                    velocity = MaxVelocity;
                }
                if (velocity < MinVelocity)
                {
                    velocity = MinVelocity;
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
            updateAmmeter((trainTractiveEffort / maxMotorEffort) * maxMotorAmps);             ////(velocity * 2 * (throttlePosition));


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
                TimedAction.ExecuteWithDelay(new Action(delegate { toggleLowFuel(); }), TimeSpan.FromMilliseconds(1000));               
            }
            if (timeLeft <= 0)
            {
                continueCountdownLabel.Content = "13";
                outWindow.continueMainCountdownLabel.Content = "13";
                continueCountdownLabel.Opacity = 0;
                outWindow.continueMainCountdownLabel.Opacity = 0;                
                continueScreenLabel.Opacity = 0;
                outWindow.continueMainScreenLabel.Opacity = 0;
                continueCountdownLabel.Visibility = Visibility.Visible;
                outWindow.continueMainCountdownLabel.Visibility = Visibility.Visible;
                continueScreenLabel.Visibility = Visibility.Visible;
                outWindow.continueMainScreenLabel.Visibility = Visibility.Visible;                                                               
                FadeTheMediaElement(0, 1, continueScreenLabel, 500);
                FadeTheMediaElement(0, 1, outWindow.continueMainScreenLabel, 500);
                FadeTheMediaElement(0, 1, continueCountdownLabel, 500);
                FadeTheMediaElement(0, 1, outWindow.continueMainCountdownLabel, 500);
                gameTimer.Tick -= new EventHandler(gameLoop);
                gameTimer.Tick += new EventHandler(continueCountDown);
                gameState = "continueScreen";
                FadeTheMediaElement(1, 0.5, outWindow.CabFootageVideo, 500);
                continueTimeLeft += 24;
                TimedAction.ExecuteWithDelay(new Action(delegate { outWindow.CabFootageVideo.Pause(); }), TimeSpan.FromMilliseconds(500));


            }
        }


        private void toggleLowFuel ()
        {
            if (my16_16_0.outputs[14] == true)
            {
                my16_16_0.outputs[14] = false;
                LowFuelWarning = false;
            }
            else
            {
                my16_16_0.outputs[14] = true;
                LowFuelWarning = true;
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
            ////    myAnalogOut.outputs[0].Voltage = outVoltage;
            }));
        }

        private void updateAmmeter(double amps)
        {
            
            if (amps < 0)
            {
                amps = 0;
            }
            else if (amps > maxMotorAmps)
            {
                amps = maxMotorAmps;
            }

            if (dynamicPosition == Dynamic_On_Position || dynamicPosition == 3 || dynamicPosition == 4 || dynamicPosition == 5)
            {
                amps = amps / 2;
            }

            double outVoltage = amps * 0.006897;

            this.Dispatcher.Invoke(new Action(delegate()
            {
    ////            myAnalogOut.outputs[1].Voltage = outVoltage;
            }));
        }

        private void continueCountDown(object sender, EventArgs e)
        {
            continueCountdownLabel.Content = continueTimeLeft / 2;
            outWindow.continueMainCountdownLabel.Content = continueTimeLeft / 2;
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
                    /// my16_16_0.outputs[3] = false;
                    indBrakePressureRequested = 90;
                    indBrakeEffort = 1.0;
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
                    indBrakePressureRequested = 0;
                    indBrakeEffort = 0;
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
                        //if (indDir == 1 && indBrakePressureIndicated == 0 && mainBrakePressureRequested < 90)
                        //{
                        //    indBrakePressureRequested = 25;
                        //}
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

                MainBrakePressureIndicatedLabel.Content = "Main Pressure Ind: " + mainBrakePressureIndicated;
                MainBrakePressureRequested.Content = "Main Pressure Req: " + mainBrakePressureRequested;
                IndBrakePressureIndicated.Content = "Ind Pressure Ind: " + indBrakePressureIndicated;
                IndBrakePressureRequested.Content = "Ind Pressure Req: " + indBrakePressureRequested;

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

                    else if ((cachedSound.Name == "AmbientAudio.wav" || cachedSound.Name == "AmbientAudio1.wav") && (gameState == "inGame" || gameState == "continueScreen"))
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
                        if (sandingPressed == true )/// || sandingToggle == true)
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
          
         /////   /// //////////////////////////////////////////////////////////////////////////////
           

       private void my8InputChanged(object sender, InputChangeEventArgs e)
       {
           this.Dispatcher.Invoke(new Action(delegate()
               {
                   if (e.Value == true && (e.Index == 1 || e.Index == 2 || e.Index == 3 || e.Index == 4))
                   {
                       indPosition = e.Index - 1;
                       indPositionLabel.Content = "Ind Position: " + Convert.ToString(indPosition);

                       if (getMainBrakeState() == 0)
                       {
                           if (indPosition == 0)
                           {


                               indBrakePressureRequested = 0;
                               indBrakeEffort = 0.0;
                           }

                           else if (indPosition == 1)
                           {
                               indBrakePressureRequested = 25;
                               indBrakeEffort = 0.30;
                           }
                           else if (indPosition == 2)
                           {
                               indBrakePressureRequested = 55;
                               indBrakeEffort = 0.70;
                           }
                           else
                           {
                               indBrakePressureRequested = 80;
                               indBrakeEffort = .94;
                           }
                       }


                       if (getMainBrakeState() == 1)
                       {



                           if (indPosition == 1)
                           {
                               indBrakePressureRequested = 25;
                               indBrakeEffort = 0.30;
                           }
                           else if (indPosition == 2)
                           {
                               indBrakePressureRequested = 55;
                               indBrakeEffort = 0.70;
                           }
                           else
                           {
                               indBrakePressureRequested = 80;
                               indBrakeEffort = .94;
                           }
                       }

                       if (getMainBrakeState() == 2)
                       {
                            if (indPosition == 1)
                           {
                               indBrakePressureRequested = 25;
                               indBrakeEffort = 0.30;
                           }
                           else if (indPosition == 2)
                           {
                               indBrakePressureRequested = 55;
                               indBrakeEffort = 0.70;
                           }
                           else
                           {
                               indBrakePressureRequested = 80;
                               indBrakeEffort = .94;
                           }
                       }
                   
                   }
                   if (e.Index == 5)
                   {
                       bailActivated = e.Value;
                       bailStateLabel.Content = "Bail State: " + Convert.ToString(bailActivated);
                       
                       if (bailActivated == false)
                       {
                                                      
                                   indBrakePressureRequested = 0;                         
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
                       pedalPressedLabel.Content = "Pedal Pressed: " + Convert.ToString(pedalPressed);
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
                   attendantCallPressedLabel.Content = "Attenant Pressed: " + Convert.ToString(attendSwitch);
               }
               if (e.Index == 10)
               {
                   resetSwitch = !e.Value;
                   resetButtonPressedLabel.Content = "Reset Pressed: " + Convert.ToString(resetSwitch);
               }
               if (e.Index == 6)
               {
                   hornPulled = e.Value;
                   hornPulleLabel.Content = "Horn Pulled: " + Convert.ToString(hornPulled);
                   if (e.Value == true && gameState == "inGame" && hornPlaying == false)
                   {
                    AudioPlaybackEngine.Instance.PlaySound(horn);
                    hornPlaying = true;
                   }
                   

               }
               if (e.Index == 5)
               {
                   sandingToggle = e.Value;
                   sandigLeverStateLabel.Content = "SandingL State: " + Convert.ToString(sandingToggle);
                  
               }
               if (e.Index == 8)
               {
                   sandingPressed = e.Value;
                   sandingButtonPressedLabel.Content = "SandingB Pressed: " + Convert.ToString(sandingPressed);

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
                   bellPulledLabel.Content = "Bell Pulled: " + Convert.ToString(bellPulled);
                   if (e.Value == true && gameState == "inGame" && bellPlaying == false)
                   {
                        AudioPlaybackEngine.Instance.PlaySound(bell);
                        bellPlaying = true;
                   }
                   
               }
               if (e.Index == 15)
               {
                   pcsPressed = e.Value;
                   pcsPushed.Content = "PCS Pressed: " + Convert.ToString(pcsPressed);
                   if (e.Value == true && my16_16_0.inputs[1] == false && my16_16_0.inputs[10] == false)
                   {
                       toggleHUD();
                   }

               }
               if (e.Index == 13)
               {
                   footSwitch = e.Value;
               }
               if (e.Index == 14)
               {
                   engineRunSwitch = e.Value;

                ///   if (engineRunSwitch= true) { playsound.engineStart};

                   engineRunButtonStateLabel.Content = "Engine Run State: " + Convert.ToString(engineRunSwitch);
               }
           }));
       }


        


        private void myPotChanged(object sender, SensorChangeEventArgs e) 
       {
           this.Dispatcher.Invoke(new Action(delegate()
           {
               if (e.Index == 6)
               {
                   if (e.Value >= throttle_calibration_value + throttle_offset_0)              ///       ///605
                   {
                       throttlePosition = 0;        
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_1)         ///           ///563
                   {
                       throttlePosition = 1;

                       actual_throttle_1_pos = e.Value;             // keep track of this value for 
                                                                    //calibrating the throttle during preflight

                   }
                   else if (e.Value >= throttle_calibration_value +throttle_offset_2)          ///          ///511
                   {
                       throttlePosition = 2;
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_3)          ///         ///458
                   {
                       throttlePosition = 3;
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_4)          ///         ///409
                   {
                       throttlePosition = 4;
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_5)          ///         ///359
                   {
                       throttlePosition = 5;
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_6)          ///         ///313
                   {
                       throttlePosition = 6;
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_7)          ///         ///268
                   {
                       throttlePosition = 7;
                   }
                   else if (e.Value >= throttle_calibration_value + throttle_offset_8)          ///         ///231
                   {
                       throttlePosition = 8;
                   }
                   else
                   {
                       throttlePosition = 9;
                   }
                   throttlePositionLabel.Content = "Throttle Position: " + Convert.ToString(throttlePosition);
                  
               }

               if (e.Index == 7)
               {
                   if (e.Value >= dynamic_calibration_value + dynamic_offset_0)             //623)
                   {
                       dynamicPosition = 0;
                      

                   }
                   else if (e.Value >= dynamic_calibration_value + dynamic_offset_1)        // 547)
                   {
                       dynamicPosition = 1;
                       actual_dynamic_1_pos = e.Value;             // keep track of this value for 
                                                                 //calibrating the dynamic selector during preflight
                   }
                   else if (e.Value >= dynamic_calibration_value + dynamic_offset_2)        //469)
                   {
                       dynamicPosition = 2;
                   
                   }
                   else if (e.Value >= dynamic_calibration_value + dynamic_offset_3)        //393)
                   {
                       dynamicPosition = 3;
                       
                   }
                   else if (e.Value >= dynamic_calibration_value + dynamic_offset_4)
                   {
                       dynamicPosition = 4;
                       
                   }
                   dynamicPositionLabel.Content = "Dynamic Position: " + Convert.ToString(dynamicPosition);

               }

           }));
       }

       private int getMainBrakeState()
       {
            int mainBrakePosition;
            if (my16_16_0.inputs[0] == false)
            {
                if (gameState == "inGame" || gameState == "continueScreen")
                {
                    ///put release mainbrake picture up and display emergency brakes activated message
                    emergencyBrakeMessage(true);
                    ///activate penalty braking
                    penaltyBrake = true;
                    penaltyBrakeCountDown = 8;
                    my16_16_0.outputs[1] = true;
                    my16_16_0.outputs[0] = true;
                    my16_16_0.outputs[5] = true;
                    my16_16_0.outputs[13] = true;
                    mainBrakeEffort = 1.0;   ///100%           
                  
                   
                    ebrakeState = true;
                    
                }
                    mainBrakePosition = 5;

             }

//////////////////////////////////
                else
                {
                if (my16_16_0.inputs[3] == true)
                {

                    mainBrakeState = "release";
                    mainBrakePosition = 0;
                    mainBrakePressureRequested = 90;

                    mainBrakeEffort = 0;                    ///0%
                                
                    
                    if (ebrakeState == true)
                    {
                        ebrakeState = false;
                        emergencyBrakeMessage(false);                                           ///do not have an opinion of ind effort if the main brake is released
                       
                    }
                }
                // }

                else
                {
                    if (my16_16_0.inputs[4] == true)
                    {
                        mainBrakeState = "minimum";
                        mainBrakePosition = 1;
                        mainBrakePressureRequested = 85;
                        mainBrakeEffort = 0.25;                 ///25%
                        indBrakePressureRequested = 15;
                        indBrakeEffort = 0.25;
                    }
                    //  }
                    else
                    {
                        if (my16_16_0.inputs[2] == true)
                        {
                            mainBrakeState = "minService";
                            mainBrakePosition = 2;
                            mainBrakePressureRequested = 80;
                            mainBrakeEffort = 0.50;                  ///50%
                            indBrakePressureRequested = 30;
                            indBrakeEffort = .40;
                        }
                        //  }
                        else
                        {
                            if (my16_16_0.inputs[9] == true)
                            {
                                mainBrakeState = "maxService";
                                mainBrakePosition = 3;
                                mainBrakePressureRequested = 65;
                                mainBrakeEffort = 0.90;                 ///90%
                                indBrakePressureRequested = 80;
                                indBrakeEffort = .90;
                            }
                            //  }
                            else
                            {
                                mainBrakeState = "supression";
                                mainBrakePosition = 4;
                                mainBrakePressureRequested = 65;
                                mainBrakeEffort = 0.91;                 ///91%
                              
                                indBrakeEffort = 0.91;

                              ///  if (ebrakeState == true)
                              //  {

                              ///      ebrakeState = false;                 /// add this to have ebrake cancel during supression 
                               ///     emergencyBrakeMessage(false);
                                    

                              ///  }

                             }
                        }
                    }
                }          
                




           }

            brakePositionLabel.Content = "Brake Position: " + Convert.ToString(mainBrakePosition);
            return mainBrakePosition;

       }

       private void toggleDisplayLights(bool visisbility)
       {
         ///  my16_16_0.outputs[10] = visisbility;
           my16_16_0.outputs[11] = visisbility;
           my16_16_0.outputs[12] = visisbility;
           my16_16_0.outputs[13] = visisbility;
           my16_16_0.outputs[14] = visisbility;
           my16_16_0.outputs[15] = visisbility;
       }

       private void toggleHUD()
       {
           if (inputDataDisplayGrid.Visibility == Visibility.Visible)
           {
               inputDataDisplayGrid.Visibility = Visibility.Hidden;
               outputDataDisplayGrid.Visibility = Visibility.Hidden;
           }
           else
           {
               inputDataDisplayGrid.Visibility = Visibility.Visible;
               outputDataDisplayGrid.Visibility = Visibility.Visible;
           }
       }

        private void emergencyBrakeMessage(bool eState)
        {

            if (eState == true)
            {

                if (iter1 == 0)
                {

                    FadeTheMediaElement(1, 0, TrainSimulatorLogo, 200);
                    //// turn on Pcs light
                    ///my16_16_0.outputs[] = true
                    //// display release main brake message
                    instructionsLabel.Content = "Emergency Braking Activated!";
                    FadeTheMediaElement(0, 1, instructionsLabel, 300);
                    instructionsSupLabel.Content = "Release Main Brake!";
                    FadeTheMediaElement(0, 1, instructionsSupLabel, 300);
                    TranslateTheMediaElement(0, 0, -1196, 0, 500, 0, 1, brakeInstructions);
                    TimedAction.ExecuteWithDelay(new Action(delegate { iter1 = 2; }), TimeSpan.FromMilliseconds(500));
                    /// remove fuel bar too
                    
                    FadeTheMediaElement(1, 0, fuelBarBackground, 300);
                    FadeTheMediaElement(1, 0, fuelBar, 300);
                    FadeTheMediaElement(1, 0, fuelBarMessage, 300);
                    mainBrakePressureRequested = 0;
                    indBrakePressureRequested = 90;
                    iter1 = 2;
                }

                if (iter1 == 2)

                { /// mext time through 


                    TimedAction.ExecuteWithDelay(new Action(delegate { iter1 = 3; }), TimeSpan.FromMilliseconds(200));

                }
            }

                if (eState == false)
                {
                    if (iter1 == 3)
                    {
                        /// turn off Pcs light
                        ///my16_16_0.outputs[] = true
                        ////replace logo
                        FadeTheMediaElement(0, 1, TrainSimulatorLogo, 200);
                        /// remove release main brake message
                        TranslateTheMediaElement(-1196, 0, 0, 0, 500, 0, 1, brakeInstructions);

                        /// remove the ebrake text                    
                        FadeTheMediaElement(1, 0, instructionsSupLabel, 300);
                        FadeTheMediaElement(1, 0, instructionsLabel, 300);
                        FadeTheMediaElement(1, 0, TrainSimulatorLogo, 200);
                    /// replace fuel bar
                        FadeTheMediaElement(0, 1, fuelBarBackground, 300);
                        FadeTheMediaElement(0, 1, fuelBar, 300);
                        FadeTheMediaElement(0, 1, fuelBarMessage, 300);
                        mainBrakePressureRequested = 90;
                        indBrakePressureRequested = 0;

                    iter1 = 0;
                    }
                }
                
        }




        private void intControlsPosition()  ///check the two analoge inputs once to establish their initial value as the events oriented loop cant do that

        {
            int val = my8_8_8.sensors[6].Value;

            if (val >= throttle_calibration_value + throttle_offset_0)              ///         ///605
            {
                throttlePosition = 0;
            }
            else if (val >= throttle_calibration_value + throttle_offset_1)         ///          ///563
            {
                throttlePosition = 1;


            }
            else if (val >= throttle_calibration_value + throttle_offset_2)          ///         ///511
            {
                throttlePosition = 2;
            }
            else if (val >= throttle_calibration_value + throttle_offset_3)          ///         ///458
            {
                throttlePosition = 3;
            }
            else if (val >= throttle_calibration_value + throttle_offset_4)          ///         ///409
            {
                throttlePosition = 4;
            }
            else if (val >= throttle_calibration_value + throttle_offset_5)          ///         ///359
            {
                throttlePosition = 5;
            }
            else if (val >= throttle_calibration_value + throttle_offset_6)          ///         ///313
            {
                throttlePosition = 6;
            }
            else if (val >= throttle_calibration_value + throttle_offset_7)          ///         ///268
            {
                throttlePosition = 7;
            }
            else if (val >= throttle_calibration_value + throttle_offset_8)          ///         ///231
            {
                throttlePosition = 8;
            }
            throttlePositionLabel.Content = "Throttle Position: " + Convert.ToString(throttlePosition);

            val = my8_8_8.sensors[7].Value;

            if (val >= dynamic_calibration_value + dynamic_offset_0)             //623)
            {
                dynamicPosition = 0;
            }
            else if (val >= dynamic_calibration_value + dynamic_offset_1)        // 547)
            {
                dynamicPosition = 1;

            }
            else if (val >= dynamic_calibration_value + dynamic_offset_2)        //469)
            {
                dynamicPosition = 2;
            }
            else if (val >= dynamic_calibration_value + dynamic_offset_3)        //393)
            {
                dynamicPosition = 3;
            }
            else if (val >= dynamic_calibration_value + dynamic_offset_4)
            {
                dynamicPosition = 4;
            }

            dynamicPositionLabel.Content = "Dynamic Position: " + Convert.ToString(dynamicPosition);
        }  
                
        
            
        //////////////////////////////////////////////////////////////////////////





    }

        
}


