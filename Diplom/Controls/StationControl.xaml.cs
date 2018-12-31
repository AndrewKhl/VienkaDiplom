using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    /// <summary>
    /// Interaction logic for StationControl.xaml
    /// </summary>
    public partial class StationControl : UserControl, IFocusable
    {
        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_relay.png");
        private static Uri enableParameters = new Uri(@"pack://application:,,,/Resources/Icons/Params.png");
        private static Uri disableParameters = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShow.png");
		public DataStation Data;
        public ConnectionLine stationLine;
        public ConnectionLine managerLine;
        public bool IsRightRotation = true;

        private static string[] UpdateMainStationMessages =
        {
            "Идет опрос версии ПО БУКС станции {0}",
            "Идет опрос версии ПО ИБЭП станции {0}"
        };
        private static string[] UpdateAnotherStationMessages =
        {
            "Идет опрос версии ПО БУКС станции {0}",
            "Идет опрос версии ПО ТУТС станции {0}",
            "Идет опрос версии ПО ППУ станции {0}",
            "Идет опрос версии ПО допканалов станции {0}",
            "Идет опрос версии ПО мультиплексоров станции {0}",
            "Идет опрос версии ПО ИБЭП станции {0}"
        };

		public StationControl(WorkWindow window, string name, int number, Color color)
        {
            InitializeComponent();
            (Resources["fontColor"] as SolidColorBrush).Color = color;
            image.Source = new BitmapImage(ImageUri);
            BorderThickness = new Thickness(2);
			Data = new DataStation();

            stationName.Text = $"{name} [{number}]";
			Data.Name = name;
			Data.Number = number;

			DataNetwork.Stations.Add(this);

            this.window = window;

			BitmapImage gauge = new BitmapImage();
			gauge.BeginInit();
			gauge.UriSource = new Uri("pack://application:,,,/Resources/gauge_1_30.png");
			gauge.EndInit();
			stationGauge.Source = gauge;
			stationGauge.Visibility = Visibility.Hidden;

            parameterItem.Icon = new Image { Source = new BitmapImage(disableParameters) };
		}

        public WorkWindow window { get; }

		public event Action FocusedElement;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetFocusBorder();
            window.SetFocus(this);
            e.Handled = true;
        }

        public void SetFocusBorder()
        {
            stationImageBorder.BorderBrush = new SolidColorBrush(Colors.White);
            stationNameBorder.BorderBrush = new SolidColorBrush(Colors.White);
            stationImageBorder.Background.Opacity = 0.5;
            stationNameBorder.Background.Opacity = 0.5;
            FocusedElement?.Invoke();
        }

        public void UnsetFocusBorder()
        {
            stationImageBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            stationNameBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            stationImageBorder.Background.Opacity = 0.2;
            stationNameBorder.Background.Opacity = 0.2;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("Station", this);
                data.SetData("shiftX", e.GetPosition(this).X);
                data.SetData("shiftY", e.GetPosition(this).Y);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

		public void ShowParametrWindow(object sender, RoutedEventArgs e)
		{
			ParamsWindow wnd = new ParamsWindow(this);
			wnd.Owner = Stock.workWindow;
			wnd.Show();
		}

        private void RadioConnect_Click(object sender, RoutedEventArgs e)
        {
			window.ConnectControls(this);
        }

        private void LocalConnect_Click(object sender, RoutedEventArgs e)
        {
			window.ConnectControls(this, false);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (IsConnectedToManager())
            {
                LoadingWindow wnd;
                foreach (string message in UpdateMainStationMessages)
                {
                    wnd = new LoadingWindow(string.Format(message, stationName.Text), 1);
                    wnd.ShowDialog();
                }
                parameterItem.IsEnabled = true;
                parameterItem.Icon = new Image { Source = new BitmapImage(enableParameters) };
            }
            else if (IsConnectedToStation())
            {
                //TODO check what notification given on updating if station connected to another station connected to manager
                StationControl another;
                if (stationLine.firstControl == this)
                    another = stationLine.secondControl as StationControl;
                else another = stationLine.firstControl as StationControl;
                if (another.IsConnectedToManager())
                {
                    LoadingWindow wnd;
                    foreach (string message in UpdateAnotherStationMessages)
                    {
                        wnd = new LoadingWindow(string.Format(message, another.stationName.Text), 1);
                        wnd.ShowDialog();
                    }
                    parameterItem.IsEnabled = true;
                    parameterItem.Icon = new Image { Source = new BitmapImage(enableParameters) };
                }
            }
            else
            {
                MessageBox.Show("Не создано ни одного менеджера для осуществления запроса. " +
                    "Создайте управляющий элемент и соедините его с сетью.", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool IsConnectedToManager() => managerLine != null;
        public bool IsConnectedToStation() => stationLine != null;

        public void RotateRight()
        {
            IsRightRotation = true;
            ScaleTransform transform = new ScaleTransform(1, 1);
            image.RenderTransform = transform;
        }

        public void RotateLeft()
        {
            IsRightRotation = false;
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform transform = new ScaleTransform(-1, 1);
            image.RenderTransform = transform;
        }
    }
}
