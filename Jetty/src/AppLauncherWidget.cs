using System.Drawing;

namespace Jetty
{
    public class AppLauncherWidget : Widget
    {
        public string ExecutablePath { get; set; }

        public AppLauncherWidget(string title, Bitmap icon)
            : base(title, icon)
        {
        }

        public int Start()
        {
            return 0;
        }
    }
}
