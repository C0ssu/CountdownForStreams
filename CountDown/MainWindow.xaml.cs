using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DesktopWPFAppLowLevelKeyboardHook;

namespace CountDown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LowLevelKeyboardListener _listener; //Keyboard listener

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Variables
        //Formats
        string[] Format = { "SS", "MM:SS", "HH:MM:SS", "DD:HH:MM:SS", "WW:DD:HH:MM:SS", "MM:WW:DD:HH:MM:SS" }; //This is the format shown in the hint text and under the timer text
        int[] FormatInt = { 0, 0, 0, 0, 0, 0 }; //This is the array that shows the value of the time in the text file and in the timer_Countdown
                          // s  m  h  d  w  m
                          // 0  1  2  3  4  5
        string[] inputStr = new string[6];
        public string timeString;
        int SelectedFormat = 2;
        // Bools for timer status
        bool timerActive;
        bool timerSummoned = false;
        bool timeStopped = false;
        // Timer
        DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Send); //Set the timer to highest priority
        //TimePath
        string TimePath = @"txt\Time.txt";
        string MarkupPath = @"txt\Markup.txt";
        string FolderPath = @"txt\";
        //Markup variables
        string markup_Keybind = "Home";
        bool keybindChange;
        #endregion
        #region InputTextHint
        //Colors
        SolidColorBrush col_enabled = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xC5, 0x1F, 0x5D)); //Enabled color
        

        //Code for input text losing and getting focus
        private void InputText_GotFocus(object sender, RoutedEventArgs e)
        {
            //if inputtext gets focus we need to hide the hint text behind it
            this.hintText.Text = "";
        }

        private void InputText_LostFocus(object sender, RoutedEventArgs e)
        {
            //if we lose the focus on input text and input text is empty we simply change the hint text back to the selected format
            if(this.InputText.Text == "")
            {
                UpdateFormat();
            }
            // if the input text is not empty we cant show the hint text and we keep it empty.
            else
            { 
                this.hintText.Text = "";
            }
        }

        #endregion


        #region EventHandlers
        private void ParentGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if anything else is clicked we change the focus of everything to the background
            ParentGrid.Focus();
        }

        private void ParentGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //lets set the timer settings when the app is loaded
            dTimer.Tick += TimerTick;
            dTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void comb_FormatSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.comb_FormatSelection.SelectedItem != null)
            {
                //if the format selection has been changed lets change the selected format variable to the index value of the selected format.
                SelectedFormat = comb_FormatSelection.SelectedIndex;
                //then lets update the format
                UpdateFormat();
            }
        }
        void Start_Click(object sender, RoutedEventArgs e)
        {

            bool InvalidFormat = false;
            //Making start/continue button disabled until timer runs out or stop is pressed.
            this.bt_start.IsEnabled = false;
            this.bt_stop.IsEnabled = true;
            //Making input field read only
            this.InputText.IsReadOnly = true;
            if(!timerActive && !timeStopped)
            {
                //GETTING USER INPUT
                try
                {
                    inputStr = this.InputText.Text.Split(':');
                    int x = SelectedFormat;
                    for (int i = 0; i <= SelectedFormat; i++)
                    {
                        //Changing string input to int and putting it into intarr
                        FormatInt[i] = int.Parse(inputStr[x]);
                        x--;
                    }
                }catch(Exception)
                {
                    MessageBox.Show("Invalid input. Example input: 14:23:12. 14 hours 23 minutes 12 seconds");
                    InvalidFormat = true;
                }
                if(!InvalidFormat) //If the format was good
                {
                    //setup timer text
                    SetTimerText();
                    //start timer
                    TimerSetting(true);
                }
                else
                {
                    //if error has happened with the input revert changes made when start is pressed.
                    this.InputText.Text = "";
                    UpdateFormat();
                    this.bt_start.IsEnabled = true;
                    this.bt_stop.IsEnabled = false;
                    this.InputText.IsReadOnly = false;
                }
            }
            else if(!timerActive && timeStopped)
            {
                timeStopped = false;
                SetTimerText();
                TimerSetting(true);
            }
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            TimerSetting(false);
            this.timeStopped = true; 
            this.bt_start.IsEnabled = true; //Enable start button
            this.bt_stop.IsEnabled = false; //Disable stop button
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            //resetting EVERYTHING TO DEFAULT
            this.InputText.IsReadOnly = false;
            this.bt_stop.IsEnabled = false;
            this.bt_start.IsEnabled = true;
            timerActive = false;
            timeStopped = false;
            this.comb_FormatSelection.SelectedIndex = 2;
            this.InputText.Clear();
            this.ParentGrid.Focus();
            for(int i = 0; i < 6; i++)
            {
                FormatInt[i] = 0;
            }
            UpdateFormat();
            TimerSetting(false);
            SetTimerText();
        }
        #endregion

        #region HelpingMethods
        private void TimerSetting(bool start) //Start or stop timer
        {
            if(start)
            {
                timerActive = true;
                dTimer.Start();
            }
            else if(!start)
            {
                timerActive = false;
                dTimer.Stop();
            }
        }
        

        private void UpdateFormat() //Update the program to the selected time format
        {
            if(this.hintText != null && this.InputText.Text == "") //Show the hint text if the field is empty
            {
                this.hintText.Text = Format[SelectedFormat];
            }
            SetTimerText(); //Update the timer text according to the selected format
            this.Timer_Format.Content = Format[SelectedFormat]; //Sets the format text under the counter
        }

        private void SetTimerText() //Update the timer text. This is called every second in the TimerTick method to update the timer
        {
            this.timer_Countdown.Content = ""; //First clear the timer

            //Updating the timer text with a for loop. Processing the numbers
            for (int i = SelectedFormat; i >= 0; i--) //i = the selected format index position, Update the countdown text from left to right
            {
                //This is such a mess

                //If the value of the number is under 10 we want to add a 0 to it. Reason is we dont want the timer to look like this for example
                //Not adding 0: 13:3:8 <- 13 hours 3 minutes 8 seconds.
                //Adding 0: 13:03:08 <- Instead... we want to add the 0
                //And if this is not the first number we want to add the ":" in there too so we can seperate the numbers
                if (FormatInt[i] < 10 && i != SelectedFormat)
                {
                    this.timer_Countdown.Content += ":" + "0" + FormatInt[i].ToString();
                }
                //If this is the first number we are updating we dont want to add ":" first.
                //If we dont do this the timer text would look like this :HH:MM:SS
                //                                                       ^ This is unnecessary
                else if (FormatInt[i] < 10 && i == SelectedFormat) 
                {
                    this.timer_Countdown.Content += "0" + FormatInt[i].ToString();
                }
                //If the number is the first one we are updating and its over 9 we dont add the 0 first. Just the whole number
                else if (i == SelectedFormat && FormatInt[i] > 9)
                {
                    this.timer_Countdown.Content += FormatInt[i].ToString();
                }
                //And if the number is over 10 and is not the first number just add the whole number and the ":".
                else
                {
                    this.timer_Countdown.Content += ":" + FormatInt[i].ToString();
                }
            }
        }

        //Runs every second when the timer is activated with the start button
        void TimerTick(object sender, EventArgs e)
        {
            //If the timer has ended we want to stop the timer and write to the file that the countdown has ended
            if (FormatInt[0] == 0 && FormatInt[1] == 0 && FormatInt[2] == 0 && FormatInt[3] == 0 && FormatInt[4] == 0 && FormatInt[5] == 0)
            {
                TimerSetting(false);
                if (Directory.Exists(FolderPath))
                {
                    File.WriteAllText(TimePath, "Countdown Ended!");
                }
                else
                {
                    Directory.CreateDirectory(FolderPath);
                    File.WriteAllText(TimePath, "Countdown Ended!");
                }
            }
            //I dont know why my brain tought that this is a good way to do this but please somebody change this to a for loop or something else more "dense"
            if(timerActive)
            {
                FormatInt[0]--; //Decrease second every second... yeah... lol

                if (FormatInt[0] < 0) // if seconds go below 0 change seconds to 59 and decrease 1 minute
                {
                    FormatInt[0] = 59;
                    if (FormatInt[1] >= 0) //If the minutes are bigger or equal to 0 decrease the minutes
                    {
                        FormatInt[1]--;
                    }
                }
                if (FormatInt[1] < 0)//etc...
                {
                    FormatInt[1] = 59;
                    if (FormatInt[2] >= 0)
                    {
                        FormatInt[2]--;
                    }
                }
                if (FormatInt[2] < 0)
                {
                    FormatInt[2] = 23;
                    if (FormatInt[3] >= 0)
                    {
                        FormatInt[3]--;
                    }
                }
                if (FormatInt[3] < 0)
                {
                    FormatInt[3] = 6;
                    if (FormatInt[4] >= 0)
                    {
                        FormatInt[4]--;
                    }
                }
                if (FormatInt[4] < 0)
                {
                    FormatInt[4] = 3;
                    if (FormatInt[5] >= 0)
                    {
                        FormatInt[5]--;
                    }
                }
                if (FormatInt[5] < 0)
                {
                    FormatInt[5] = 0;
                }
                    timeString = this.timer_Countdown.Content.ToString();
                if(Directory.Exists(FolderPath)) //Write to file
                {
                    File.WriteAllText(TimePath, timeString);
                }
                else
                {
                    Directory.CreateDirectory(FolderPath);
                    File.WriteAllText(TimePath, timeString);
                }
                SetTimerText();
            }
        }
        #endregion

        //Credits for the keyboard listener goes to Larry57 on github
        //https://gist.github.com/Larry57/5365740 <- Keyboard hook
        //https://gist.github.com/Larry57 <- Larrys profile

        #region Markup

        private void Window_Loaded(object sender, RoutedEventArgs e) //When windows is loaded activate the keyboard listener. Also dont allow resizing
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
        }

        public void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if(keybindChange) // If the keybindchange button has been activated take the next key as the new keybind and return.
            {
                markup_Keybind = e.KeyPressed.ToString();
                Countdown_Window.IsEnabled = true;
                Keybind.Text = markup_Keybind; //Change the text box
                keybindChange = false;
                return;
            }
            if (e.KeyPressed.ToString() == markup_Keybind && timerActive) //If the home button is pressed markup the input
            {
                if (Directory.Exists(FolderPath) && File.Exists(MarkupPath)) //Add the markups to a file
                {
                    File.AppendAllText(MarkupPath, timeString + Environment.NewLine);
                }
                else if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                    File.AppendAllText(MarkupPath, timeString + Environment.NewLine);
                }
                else if (!File.Exists(MarkupPath))
                {
                    File.AppendAllText(MarkupPath, timeString + Environment.NewLine);
                }
            }
        }

        private void chkbx_enableMarkup_Changed(object sender, RoutedEventArgs e)
        {
            if(chkbx_enableMarkup.IsChecked == true)
            {
                _listener.HookKeyboard(); //Hook the keyboard
                Keybind.IsEnabled = true;
                bt_changeKeybind.IsEnabled = true;
                bt_changeKeybind.Foreground = col_enabled; //Change to the pink enabled color
            }
            else
            {
                _listener.UnHookKeyboard(); //Stop the keyboard listener
                Keybind.IsEnabled = false;
                bt_changeKeybind.IsEnabled = false;
                bt_changeKeybind.Foreground = new SolidColorBrush(Colors.LightGray); //Change the foreground to light gray
            }
        }

        private void bt_changeKeybind_Click(object sender, RoutedEventArgs e)
        {
            keybindChange = true;
            Countdown_Window.IsEnabled = false; //disable the window while waiting for user input
        }
    }
    #endregion
}

