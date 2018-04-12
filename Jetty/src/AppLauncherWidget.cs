using System;
using System.Diagnostics;
using System.Drawing;

namespace Jetty
{
    public class AppLauncherWidget : Widget
    {
        public string ExecutablePath { get; set; }

        public AppLauncherWidget(string title, Bitmap icon, string executablePath)
            : base(title, icon)
        {
            this.ExecutablePath = executablePath;
        }

        public AppLauncherWidget(string title, string executablePath)
            : base(title, null)
        {
            this.ExecutablePath = executablePath;
            // TODO: Determine icon by executable
        }

        public override void Activate()
        {
            this.Start();
            base.Activate();
        }

        public int Start()
        {
            // Use ProcessStartInfo ???
            // Show memory data from this process object in tooltip?
            // Redirect stdout option?
            // Process names for instances?

            //pr.Exited += Proc_Exited;
            var pr = Process.Start(this.ExecutablePath);
            pr.EnableRaisingEvents = true;
            // https://stackoverflow.com/questions/1825105/process-startiexplore-exe-immediately-fires-the-exited-event-after-launch
            pr.Exited += Proc_Exited;

            return 0;
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Deactivate();
            });
        }
    }
}
