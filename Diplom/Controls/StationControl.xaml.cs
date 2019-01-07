using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    public partial class StationControl : UserControl, IFocusable
    {
        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_relay.png");
        private static Uri enableParameters = new Uri(@"pack://application:,,,/Resources/Icons/Params.png");
        private static Uri disableParameters = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShow.png");
        private static Uri gaugeUri = new Uri("pack://application:,,,/Resources/gauge_1_30.png");

        public WorkWindow workWindow { get; }
		public event Action FocusedElement;

		public DataStation Data;
        public ConnectionLine stationLine;
        public ConnectionLine managerLine;
        public bool IsRightRotation = true;

        //private bool isChecked = false;
        //public bool IsChecked
        //{
        //    get => isChecked;
        //    set
        //    {
        //        var item = GetRadioItem();
        //        if (item != null) item.IsChecked = value;
        //        isChecked = value;
        //    }
        //}

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
            SetColor(color);
            image.Source = new BitmapImage(ImageUri);
            BorderThickness = new Thickness(2);
            Data = new DataStation
            {
                Name = name,
                Number = number
            };
            SetVisibleName();

			DataNetwork.Stations.Add(this);

            workWindow = window;

			BitmapImage gauge = new BitmapImage();
			gauge.BeginInit();
            gauge.UriSource = gaugeUri;
			gauge.EndInit();
			stationGauge.Source = gauge;
			stationGauge.Visibility = Visibility.Hidden;
		}

        public void SetVisibleName() => stationName.Text = $"{Data.Name} [{Data.Number}]";

        public void SetColor(Color color) => (Resources["fontColor"] as SolidColorBrush).Color = color;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetFocusBorder();
            workWindow.SetFocus(this);
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
            var item = GetRadioItem();
            if (item != null)
                item.IsChecked = false;

            if (stationLine == null)
                workWindow.ConnectControls(this);
            else
                workWindow.RemoveRadioConnection(this);
        }

        private void LocalConnect_Click(object sender, RoutedEventArgs e)
        {
            if (managerLine == null)
                workWindow.ConnectControls(this, false);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            var item = GetMenuItem("parameterItem");
            if (IsConnectedToManager())
            {
                LoadingWindow wnd;
                foreach (string message in UpdateMainStationMessages)
                {
                    try
                    {
                        wnd = new LoadingWindow(string.Format(message, stationName.Text), 1);
                        wnd.ShowDialog();
                    }
                    catch (Exception) { return; }
                }
                if (item != null)
                {
                    item.IsEnabled = true;
                    item.Icon = new Image { Source = new BitmapImage(enableParameters) };
                }
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
                        try
                        {
                            wnd = new LoadingWindow(string.Format(message, another.stationName.Text), 1);
                            wnd.ShowDialog();
                        }
                        catch (Exception) { return; }
                    }
                    if (item != null)
                    {
                        item.IsEnabled = true;
                        item.Icon = new Image { Source = new BitmapImage(enableParameters) };
                    }
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

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var item = GetRadioItem();
            if (item != null)
                item.IsChecked = (stationLine != null);
                //item.IsChecked = IsChecked;

            (GetMenuItem("NetworkMenuItem") as MenuItem).Header = $"Сеть \"{DataNetwork.Name} ({DataNetwork.Type})\"";
            (GetMenuItem("StationMenuItem") as MenuItem).Header = $"Станция \"{Data.Name} ({Data.Number})\"";
        }

        private MenuItem GetMenuItem(string name)
        {
            var mainMenu = Resources["MainMenu"] as ContextMenu;
            foreach (var item in mainMenu.Items)
                if (item is MenuItem && (item as MenuItem).Name == name)
                    return item as MenuItem;
            return null;
        }

        private MenuItem GetRadioItem() => GetMenuItem("RadioItem");

        private void Cancel_Click(object sender, RoutedEventArgs e) => workWindow.CancelConnection();

        private void Context_Click(object sender, MouseButtonEventArgs e)
        {
            string menu_type;
            switch (workWindow.connecting)
            {
                case ConnectingType.Manager: menu_type = "ManagerMenu"; break;
                case ConnectingType.StationLocal: menu_type = (workWindow.connector != this ? "LocalMenu" : "CancelMenu"); break;
                case ConnectingType.StationRadio: menu_type = (workWindow.connector != this ? "RadioMenu" : "CancelMenu"); break;
                default: menu_type = "MainMenu"; break;
            }
            stackPanel.ContextMenu = Resources[menu_type] as ContextMenu;
        }

        private void StationProperties_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationStation wnd = new ConfigurationStation(this) { Owner = workWindow };
            wnd.ShowDialog();
        }

        private void StationRemove_Click(object sender, RoutedEventArgs e)
        {
            workWindow.SetFocus(this);
            workWindow.RemoveElement();
        }

        private void NetworkProperties_Click(object sender, RoutedEventArgs e) =>
            workWindow.EditNetwork_Click(sender, e);

        private void NetworkRemove_Click(object sender, RoutedEventArgs e) =>
            workWindow.RemoveNetwork_Click(sender, e);
    }
}
