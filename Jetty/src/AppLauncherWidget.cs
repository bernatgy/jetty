using System.Drawing;

namespace Jetty
{
    public class AppLauncherWidget : Widget
    {
        public string ExecutablePath { get; set; }

        public AppLauncherWidget(Bitmap icon)
            : base(icon)
        {
        }

        public int Start()
        {
            return 0;
        }
    }
}
