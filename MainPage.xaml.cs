using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace DrawingApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool isLineSelected, isRectangleSelected, isEllipseSelected;
        private List<Line> lines;
        private List<Windows.UI.Xaml.Shapes.Rectangle> rectangles;
        private List<Ellipse> ellipses;

        private Line newLine = null;

        public MainPage()
        {
            this.InitializeComponent();

            lines = new List<Line>();
            rectangles = new List<Windows.UI.Xaml.Shapes.Rectangle>();
            ellipses = new List<Ellipse>();

            isLineSelected = true;

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

                    newLine = new Line()
                    {
                        Stroke = new SolidColorBrush(colorPicker.Color),
                        //Stroke = LinearGradBrush,
                        StrokeThickness = slider.Value,
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

            }

            mainCanvas.ReleasePointerCaptures();
        }

        private void newItem_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.Children.Clear();
        }

        private async void openItem_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            openPicker.ViewMode = PickerViewMode.Thumbnail;

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            openPicker.CommitButtonText = "Открыть";

            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpg");

            var file = await openPicker.PickSingleFileAsync();

            Image img = new Image();

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.DecodePixelHeight = (int) mainCanvas.Height;
            bitmapImage.DecodePixelWidth = (int) mainCanvas.Width;

            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(fileStream);
            }

            img.Source = bitmapImage;
            img.Stretch = Stretch.Fill;
            mainCanvas.Children.Add(img);
        }

        private async void saveItem_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(mainCanvas);

            Image RenderedImage = new Image();
            RenderedImage.Source = renderTargetBitmap;

            var pixels = await renderTargetBitmap.GetPixelsAsync();
            byte[] bytes = pixels.ToArray();

            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            savePicker.FileTypeChoices.Add("png, jpg", new List<string>() { ".png", ".png" });

            savePicker.SuggestedFileName = "New Image";

            var file = await savePicker.PickSaveFileAsync();

            using (IRandomAccessStream saveStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, saveStream);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)mainCanvas.ActualWidth,
                    (uint)mainCanvas.ActualHeight,
                    96, 96, bytes);

                await encoder.FlushAsync();
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBox.SelectedIndex)
            {
                case 0: 
                    { 
                        isLineSelected = true;
                        isEllipseSelected = false;
                        isRectangleSelected = false;
                    }
                    break;
                case 1: 
                    { 
                        isRectangleSelected = true;
                        isLineSelected = false;
                        isEllipseSelected = false;
                    }
                    break;
                case 2: 
                    {   
                        isEllipseSelected = true;
                        isLineSelected = false;
                        isRectangleSelected = false;
                    }
                    break;
                default: break;
            }
        }
    }
}
