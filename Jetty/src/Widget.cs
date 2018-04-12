using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using Jetty.Events;
using Jetty.Exceptions;

namespace Jetty
{
    public abstract class Widget : Image
    {
        private static Action EmptyDelegate = delegate () { };

        public string Title { get; set; }
        public int ActiveInstanceCount { get; private set; } = 0;
        public bool IsDragging {
            get
            {
                return this._lmbDown && (this._dragOffset.X > 0 || this._dragOffset.Y > 0);
            }
        }

        public event EventHandler<WidgetActionEvent> Activated;
        public event EventHandler<WidgetActionEvent> Deactivated;
        public event EventHandler<WidgetActionEvent> DragStart;
        public event EventHandler<WidgetActionEvent> DragEnd;

        private bool _lmbDown;
        private Point _dragOffset;

        public Widget(string title, System.Drawing.Bitmap icon)
        {
            if (title == null)
                throw new InvalidWidgetException("Please give the widget a title!",
                    new NullReferenceException(nameof(title)));

            title = title.Trim();

            if (title.Length < 1)
                throw new InvalidWidgetException("Please give the widget a title!",
                    new ArgumentException("Widget's title has 0 characters.", nameof(title)));

            // TODO: Check image...
            // TODO: Notification bubbles?
            // TODO: Separate theme and settings.
            //       - Each theme should have freely chosen settings.

            this.Title = title;
            this.Source = Utils.Bitmap2BitmapImage(icon);
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Top;

            // Set up the ToolTip for this Widget.
            this.ToolTip = new ToolTip()
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Relative,
                PlacementTarget = this,
                Content = this.Title,
                VerticalOffset = -25,
                Height = 20,
                BorderThickness = new Thickness(0)
            };

            // Set the render quality to be the best available.
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
        }

        #region Basic Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this._lmbDown)
            {
                base.OnMouseMove(e);
                return;
            }
            else if (this._lmbDown)
            {
                // This instance is being moved.
                return;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Another widget is being moved over this one.
                return;
            }
            // TODO: Handle drag...

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this._lmbDown = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!this.IsDragging)
            {
                this._lmbDown = false;
                this.Activate();
            }

            base.OnMouseLeftButtonUp(e);
        }

        #endregion

        public virtual void Activate()
        {
            this.ActiveInstanceCount++;
            this.ForceRedraw();

            this.RaiseActivatedEvent(new WidgetActionEvent(WidgetActionType.Activated));
        }

        public virtual void Deactivate()
        {
            this.ActiveInstanceCount--;
            this.ForceRedraw();

            this.RaiseDeactivatedEvent(new WidgetActionEvent(WidgetActionType.Deactivated));
        }

        public void SetIconSize(double iconSize)
        {
            this.Width = iconSize;
            this.Height = iconSize;
        }

        public void SetMargin(double marginSize)
        {
            this.Margin = new Thickness(marginSize, 0, marginSize, 0);
        }

        #region Raising Events

        protected void RaiseActivatedEvent(WidgetActionEvent e)
        {
            this.Activated?.Invoke(this, e);
        }

        protected void RaiseDeactivatedEvent(WidgetActionEvent e)
        {
            this.Deactivated?.Invoke(this, e);
        }

        #endregion

        protected void ForceRedraw()
        {
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // TODO: More instances
            if (this.ActiveInstanceCount > 0)
            {
                var br = new SolidColorBrush(Color.FromArgb(190, 44, 46, 46)); // TODO: Color setting
                dc.DrawEllipse(br, new Pen(br, 2), new Point(this.ActualWidth / 2, this.ActualHeight + 2), 2.5, 2.5); // TODO: Placement setting
            }
        }
    }
}
