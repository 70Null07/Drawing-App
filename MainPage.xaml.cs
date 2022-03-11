using DrawingApp.CustomControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace DrawingApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private List<CustomLine> lines;
        private CustomLine newLine = null;
        public MainPage()
        {
            this.InitializeComponent();

            lines = new List<CustomLine>();

            mainCanvas.PointerPressed += OnPointerPressed;
            mainCanvas.PointerReleased += OnPointerReleased;
            mainCanvas.PointerMoved += OnPointerMoved;

        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var ptrPoint = e.GetCurrentPoint(mainCanvas);

            if (ptrPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (ptrPoint.Properties.IsLeftButtonPressed)
                {
                    mainCanvas.CapturePointer(e.Pointer);

                    var x = ptrPoint.Position.X;
                    var y = ptrPoint.Position.Y;

                    newLine = new CustomLine(colorPicker)
                    {
                        X1 = x,
                        Y1 = y,
                        X2 = x,
                        Y2 = y,
                    };

                    lines.Add(newLine);
                    mainCanvas.Children.Add(newLine);
                }
            }
        }
        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var ptrPoint = e.GetCurrentPoint(mainCanvas);
            if (ptrPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (ptrPoint.Properties.IsLeftButtonPressed)
                {
                    if (newLine != null)
                    {
                        newLine.X2 = ptrPoint.Position.X;
                        newLine.Y2 = ptrPoint.Position.Y;
                    }
                }
            }
        }
        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var ptrPoint = e.GetCurrentPoint(mainCanvas);
            if (ptrPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (newLine != null)
                {
                    if (newLine.X1 == newLine.X2 && newLine.Y1 == newLine.Y2)
                    {
                        lines.Remove(newLine);
                        mainCanvas.Children.Remove(newLine);
                        newLine = null;
                        return;
                    }
                }

                if (!lines.Any(l => l.IsSelected))
                {
                    newLine.IsSelected = true;
                    newLine = null;
                }
            }

            mainCanvas.ReleasePointerCaptures();
        }
    }
}
