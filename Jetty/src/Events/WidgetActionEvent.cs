using System;

namespace Jetty.Events
{
    public class WidgetActionEvent : EventArgs
    {
        public readonly WidgetActionType ActionType;

        public WidgetActionEvent(WidgetActionType actionType)
        {
            this.ActionType = actionType;
        }
    }
}
