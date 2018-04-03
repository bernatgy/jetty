using System.Drawing;
using System.Windows.Controls;

namespace Jetty
{
    public abstract class Widget
    {
        public string Title { get; set; }
        public Bitmap Icon { get; private set; }
        public ContextMenu Menu { get; private set; }
        public int ActiveInstanceCount { get; protected set; } = 0;

        public Widget(string title, Bitmap icon)
        {
            this.Title = title;
            this.Icon = icon;
        }

        public virtual void Activate()
        {
            this.ActiveInstanceCount++;
        }

        public virtual void Deactivate()
        {
            this.ActiveInstanceCount--;
        }

        public virtual double GetWidth()
        {
            return this.Icon.Width;
        }

        public virtual double GetHeight()
        {
            return this.Icon.Height;
        }
    }
}
