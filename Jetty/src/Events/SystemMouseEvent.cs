using System.Windows;
using System.Windows.Input;

namespace Jetty.Events
{
    public class SystemMouseEvent : SystemInputEvent
    {
        public Point Position;
        public MouseButton Button;
        public MouseButtonState State;
        public double WheelDelta;
    }
}
