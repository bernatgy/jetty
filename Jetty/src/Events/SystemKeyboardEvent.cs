using System.Windows.Input;

namespace Jetty.Events
{
    public class SystemKeyboardEvent : SystemInputEvent
    {
        public Key Key;
        public KeyStates State;
    }
}
