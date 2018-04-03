using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Jetty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWnd : Window
    {
        private double IconSize { get { return Properties.Settings.Default.IconSize; } }
        private double IconMargin { get { return Properties.Settings.Default.IconMargin; } }

        public MainWnd()
        {
            InitializeComponent();

            // We are using a 1px offset just to be safe and precise.
            this.Width = SystemParameters.PrimaryScreenWidth - 1;
            this.Height = this.IconSize;
            this.Top = SystemParameters.PrimaryScreenHeight - (this.IconSize + 1);
            this.Left = 1;

            var store = new TestAppStore();
            var margin = this.IconMargin / 2;

            //ToolTipService.

            // Creating an image to render for each item in the dock.
            foreach (var w in store.GetAll())
            {
                var image = new Image()
                {
                    ContextMenu = w.Menu,
                    Source = Utils.Bitmap2BitmapImage(w.Icon),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Height = this.IconSize,
                    Width = this.IconSize,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(margin, 0, margin, 0)
                };

                var tt = new ToolTip()
                {
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Relative,
                    PlacementTarget = image,
                    Content = w.Title,
                    VerticalOffset = -25,
                    Height = 20,
                    BorderThickness = new Thickness(0)
                };

                image.ToolTip = tt;

                // Set the render quality to be the best available.
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);

                image.MouseEnter += IconHoverEnter;
                image.MouseLeave += IconHoverLeave;
                image.MouseLeftButtonDown += IconMouseDown;
                image.MouseLeftButtonUp += IconMouseUp;
                this.widgetContainer.Children.Add(image);
            }

            // Re-render the dock when it is needed after a setting changes.
            Properties.Settings.Default.PropertyChanged += SettingChanged;
        }

        private void IconMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IconMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IconHoverEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var image = sender as Image;
            //var eff = new System.Windows.Media.Effects.BlurEffect();
            //eff.Radius = 10;
            var eff = new Effects.BloomEffect();

            image.Effect = eff;
            var dAnim = new DoubleAnimation(0, 2, new Duration(TimeSpan.FromSeconds(.15)));
            eff.BeginAnimation(Effects.BloomEffect.BloomIntensityProperty, dAnim);
        }

        private void IconHoverLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var image = sender as Image;
            if (image.Effect != null)
            {
                var eff = image.Effect as Effects.BloomEffect;
                var dAnim = new DoubleAnimation(2, 0, new Duration(TimeSpan.FromSeconds(.15)));
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
                    var imageItem = item as Image;
                    imageItem.Width = this.IconSize;
                    imageItem.Height = this.IconSize;
                }
                catch (Exception) { /* TODO: Log. */ }
            }

            // Change the height of the dock itself.
            this.Height = this.IconSize;
            this.Top = SystemParameters.PrimaryScreenHeight - (this.IconSize + 1);
        }

        public void UpdateIconMargins()
        {
            var margin = this.IconMargin / 2;

            // Loop through and change the margins of each image.
            foreach (var item in this.widgetContainer.Children)
            {
                try
                {
                    var imageItem = item as Image;
                    imageItem.Margin = new Thickness(margin, 0, margin, 0);
                }
                catch (Exception) { /* TODO: Log. */ }
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
    }
}
