using Jetty.Events;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Jetty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWnd : Window
    {
        private readonly ILogger _logger;
        private readonly ISysHandler _sysHandler;

        private double IconSize { get { return Properties.Settings.Default.IconSize; } }
        private double IconMargin { get { return Properties.Settings.Default.IconMargin; } }

        public MainWnd()
        {
            InitializeComponent();

            // Dependencies
            this._logger = new VSOutputLogger(LogLevel.Debug);
            this._sysHandler = new Win32SysHandler(this._logger);
            this._sysHandler.AttachMouseDownHook((a) =>
            {
                var b = a as SystemMouseEvent;
                System.Diagnostics.Debug.WriteLine(b.Button.ToString());
            });
            this._sysHandler.AttachMouseUpHook((a) =>
            {
                var b = a as SystemMouseEvent;
                System.Diagnostics.Debug.WriteLine(b.Button.ToString());
            });
            this._sysHandler.AttachMouseScrollHook((a) =>
            {
                var b = a as SystemMouseEvent;
                System.Diagnostics.Debug.WriteLine(b.WheelDelta);
            });
            this._sysHandler.AttachKeyDown((a) =>
            {
                var b = a as SystemKeyboardEvent;
                System.Diagnostics.Debug.WriteLine("MOD : " + b.Mods.ToString());
                System.Diagnostics.Debug.WriteLine($"Button '{b.State.ToString()}' : {b.Key}");
            });

            // We are using a 1px offset just to be safe and precise.
            this.Width = SystemParameters.PrimaryScreenWidth - 1;
            this.UpdateDockSize();
            this.Left = 1;

            var store = new TestAppStore();
            var margin = this.IconMargin / 2;

            // Creating an image to render for each item in the dock.
            foreach (var w in store.GetAll())
            {
                w.SetIconSize(this.IconSize);
                w.SetMargin(margin);

                w.MouseEnter += IconHoverEnter;
                w.MouseLeave += IconHoverLeave;
                this.widgetContainer.Children.Add(w);
            }

            // Re-render the dock when it is needed after a setting changes.
            Properties.Settings.Default.PropertyChanged += SettingChanged;
        }

        private void IconHoverEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var w = sender as Widget;
            //var eff = new System.Windows.Media.Effects.BlurEffect();
            var eff = new Effects.BloomEffect();

            w.Effect = eff;
            //var dAnim = new DoubleAnimation(0, 10, new Duration(TimeSpan.FromSeconds(.15)));
            //eff.BeginAnimation(System.Windows.Media.Effects.BlurEffect.RadiusProperty, dAnim);
            var dAnim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(.15)));
            eff.BeginAnimation(Effects.BloomEffect.BloomIntensityProperty, dAnim);
        }

        private void IconHoverLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var w = sender as Widget;
            if (w.Effect != null)
            {
                var eff = w.Effect as Effects.BloomEffect;
                var dAnim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(.15)));
                eff.BeginAnimation(Effects.BloomEffect.BloomIntensityProperty, dAnim);
            }
        }

        private void SettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IconSize":
                    this.UpdateDockSize();
                    break;
                case "IconMargin":
                    this.UpdateIconMargins();
                    break;
            }
        }

        public void UpdateDockSize()
        {
            // Loop through and change the dimensions of each image.
            foreach (var item in this.widgetContainer.Children)
            {
                try
                {
                    var w = (Widget)item;
                    w.SetIconSize(this.IconSize);
                }
                catch (InvalidCastException) {
                    this._logger.LogWarning("Found dock item is not a widget!");
                }
            }

            // Change the height of the dock itself.
            this.Height = this.IconSize + 10;
            this.Top = SystemParameters.PrimaryScreenHeight - (this.IconSize + 7); // TODO: Setting?
        }

        public void UpdateIconMargins()
        {
            var margin = this.IconMargin / 2;

            // Loop through and change the margins of each image.
            foreach (var item in this.widgetContainer.Children)
            {
                try
                {
                    var w = (Widget)item;
                    w.SetMargin(margin);
                }
                catch (InvalidCastException) {
                    this._logger.LogWarning("Found dock item is not a widget!");
                }
            }
        }

        private void AboutCtx_Click(object sender, RoutedEventArgs e)
        {
            var aboutWnd = new AboutWnd();
            aboutWnd.Show();
        }

        private void SettingsCtx_Click(object sender, RoutedEventArgs e)
        {
            var settingsWnd = new SettingsWnd();
            settingsWnd.Show();
        }

        private void ExitCtx_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
