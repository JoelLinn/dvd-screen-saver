using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DvdScreenSaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower().Trim().Substring(0, 2) == "/s") //show
                {
                    //run the screen saver
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    ShowScreensaver();
                    Application.Run();
                }
                else if (args[0].ToLower().Trim().Substring(0, 2) == "/p") //preview
                {
                    //show the screen saver preview
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm(new IntPtr(long.Parse(args[1])))); //args[1] is the handle to the preview window
                }
                else if (args[0].ToLower().Trim().Substring(0, 2) == "/c") //configure
                {
                    //inform the user no options can be set in this screen saver
                    MessageBox.Show("This screensaver has no options that you can set",
                        "DVD Screen Saver",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else //an argument was passed, but it wasn't /s, /p, or /c, so we don't care wtf it was
                {
                    //show the screen saver anyway
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    ShowScreensaver();
                    Application.Run();
                }
            }
            else //no arguments were passed
            {
                //run the screen saver
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                ShowScreensaver();
                Application.Run();
            }
        }

        //will show the screen saver
        static void ShowScreensaver()
        {
            var rnd = new Random();
            //loops through all the computer's screens (monitors)
            foreach (Screen screen in Screen.AllScreens)
            {
                //creates a form just for that screen and passes it the bounds of that screen
                MainForm screensaver = new MainForm(screen.Bounds, rnd);
                screensaver.Show();
            }
        }
    }
}
