using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel;

namespace DrawingApp.CustomControls
{

    class CustomLine : Canvas, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));

                if (IsSelected)
                {
                    ellipse1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ellipse2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    ellipse1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    ellipse2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        public double X1
        {
            get { return line.X1; }
            set
            {
                line.X1 = value;

                Canvas.SetLeft(ellipse1, X1 - 5);
            }
        }

        public double Y1
        {
            get { return line.Y1; }
            set
            {
                line.Y1 = value;

                Canvas.SetTop(ellipse1, Y1 - 5);
            }
        }

        public double X2
        {
            get { return line.X2; }
            set
            {
                line.X2 = value;

                Canvas.SetLeft(ellipse2, X2 - 5);
            }
        }

        public double Y2
        {
            get { return line.Y2; }
            set
            {
                line.Y2 = value;

                Canvas.SetTop(ellipse2, Y2 - 5);
            }
        }

        private Line line;
        private Ellipse ellipse1, ellipse2;
        public CustomLine(ColorPicker colorpicker)
        {

            line = new Line()
            {
                Stroke = new SolidColorBrush(colorpicker.Color),
                //Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
            };

            ellipse1 = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 1,
                Width = 10,
                Height = 10,
                Visibility = Windows.UI.Xaml.Visibility.Collapsed,
            };

            ellipse2 = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 1,
                Width = 10,
                Height = 10,
                Visibility = Windows.UI.Xaml.Visibility.Collapsed,
            };

            this.Children.Add(line);
            this.Children.Add(ellipse1);
            this.Children.Add(ellipse2);
        }
    }
}
