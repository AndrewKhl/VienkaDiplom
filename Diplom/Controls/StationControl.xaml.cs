using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    public enum ErrorType
    {
        None,
        Frequency,
        Main,
        Synch
    }

    public partial class StationControl : UserControl, IFocusable, INotifyPropertyChanged
    {
        public static readonly int MinFrequency = 238;
        public static readonly int MaxFrequency = 480;

        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_relay.png");
        private static Uri enableParameters = new Uri(@"pack://application:,,,/Resources/Icons/Params.png");
        private static Uri disableParameters = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShow.png");
        private static Uri gaugeUri = new Uri("pack://application:,,,/Resources/gauge_1_50.png");

        private static Color FocusedColor = Colors.White;
        private static Color UnfocusedColor = Colors.Black;
        private static Color ErrorsColor = Colors.Red;

        public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string prop = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private Color currentColor;
        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                OnPropertyChanged("CurrentColor");
            }
        }

        public WorkWindow workWindow { get; }
		public event Action FocusedElement;

        public DataStation Data;
        public ConnectionLine stationLine;
        public ConnectionLine managerLine;
        public bool IsRightRotation { get; set; } = true;

        private bool isUpdated = false;
        public bool IsUpdated
        {
            get => isUpdated;
            set
            {
                isUpdated = value;
                CheckErrors(value);
                Stock.workWindow.ToggleParametersButtons(value);
            }
        }

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
            DataContext = this;
            InitializeComponent();

            SetBackgroundColor(color);
            image.Source = new BitmapImage(ImageUri);
            Data = new DataStation { Name = name, Number = number };
            SetVisibleName();
            UpdateLook();

			DataNetwork.Stations.Add(this);

            workWindow = window;

			BitmapImage gauge = new BitmapImage();
			gauge.BeginInit();
            gauge.UriSource = gaugeUri;
			gauge.EndInit();
			stationGauge.Source = gauge;
            HideGauge();
		}

        public void SetVisibleName() => stationName.Text = $"{Data.Name} [{Data.Number}]";

        public void SetBackgroundColor(Color color) => (Resources["backgroundColor"] as SolidColorBrush).Color = color;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            workWindow.SetFocus(this);
            UpdateLook();
            e.Handled = true;
        }

        private void SetFocusBorder()
        {
            CurrentColor = stationLine != null && stationLine.HasErrors ? ErrorsColor : FocusedColor;
            stationImageBorder.Background.Opacity = 0.5;
            stationNameBorder.Background.Opacity = 0.5;
            FocusedElement?.Invoke();
        }

        private void UnsetFocusBorder()
        {
            CurrentColor = stationLine != null && stationLine.HasErrors ? ErrorsColor : UnfocusedColor;
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
            ParamsWindow wnd = new ParamsWindow(this) { Owner = Stock.workWindow };
            wnd.ShowDialog();
		}

        private void RadioConnect_Click(object sender, RoutedEventArgs e)
        {
            if (stationLine == null)
                workWindow.ConnectControls(this);
            else
                workWindow.RemoveRadioConnection(this);
        }

        private void LocalConnect_Click(object sender, RoutedEventArgs e)
        {
            if (managerLine == null)
                workWindow.ConnectControls(this, false);
            else
                workWindow.RemoveLocalConnection(managerLine);
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
            GetRadioItem().IsChecked = (stationLine != null);
            var item = GetMenuItem("parameterItem");
            item.IsEnabled = IsUpdated;
            item.Icon = new Image { Source = new BitmapImage(IsUpdated ? enableParameters : disableParameters) };
            GetMenuItem("LocalMenuItem").IsChecked = (managerLine != null);
            GetMenuItem("NetworkMenuItem").Header = $"Сеть \"{DataNetwork.Name} ({DataNetwork.Type})\"";
            GetMenuItem("StationMenuItem").Header = $"Станция \"{Data.Name} ({Data.Number})\"";
        }

        private MenuItem GetMenuItem(string name, string menu = "MainMenu")
        {
            var mainMenu = Resources[menu] as ContextMenu;
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

        public void StationProperties_Click(object sender, RoutedEventArgs e)
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

        private void ContextMenu_Opened_1(object sender, RoutedEventArgs e)
        {
            var item = GetMenuItem("Radio2MenuItem", "RadioMenu");
            item.IsEnabled = (stationLine == null);
            item.IsChecked = (stationLine != null);
        }

        private static bool IsFrequenciesEquals(StationControl station1, StationControl station2) =>
            station1.Data.Period == station2.Data.Period;

        private static bool IsCorrectRegime(StationControl station) =>
           (station.Data.Main == "Ведущая" && station.Data.Synhronization == "Внутренняя")
                || (station.Data.Main == "Ведомая" && station.Data.Synhronization == "РК");

        private static bool IsRegimesDiffers(StationControl station1, StationControl station2) =>
            station1.Data.Main != station2.Data.Main;

        public void CheckErrors(bool isUpdated)
        {
            StationControl another = stationLine?.GetAnotherStation(this);
            //if (isUpdated && (!IsConnectedToManager() && IsConnectedToStation() && another.IsUpdated) || (IsConnectedToManager() && IsConnectedToStation() && another.IsUpdated))
            if (isUpdated && IsConnectedToStation() && another.IsUpdated)
            {
                if (!IsFrequenciesEquals(this, another))
                {
                    Data.errorType = ErrorType.Frequency;
                    another.Data.errorType = ErrorType.Frequency;

                    stationLine.HasErrors = true;
                }
                else if (!IsRegimesDiffers(this, another))
                {
                    Data.errorType = ErrorType.Main;
                    another.Data.errorType = ErrorType.Main;

                    stationLine.HasErrors = true;
                }
                else if (!IsCorrectRegime(this) || !IsCorrectRegime(another))
                {
                    Data.errorType = ErrorType.Synch;
                    another.Data.errorType = ErrorType.Synch;

                    stationLine.HasErrors = true;
                }
                else
                {
                    Data.errorType = ErrorType.None;
                    another.Data.errorType = ErrorType.None;

                    stationLine.HasErrors = false;
                }
            }
            else if (stationLine != null)
                stationLine.HasErrors = false;
            else
                UpdateLook();
        }

        public void UpdateLook()
        {
            if (isUpdated && (IsConnectedToManager() || !stationLine.HasErrors))
                ShowGauge();
            else
                HideGauge();

            if (Stock.workWindow.FocusedControl == this)
                SetFocusBorder();
            else
                UnsetFocusBorder();

            UpdateLayout();
        }

        public void Update_Click(object sender, RoutedEventArgs e)
        {
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
                IsUpdated = true;
            }
            else if (IsConnectedToStation())
            {
                var another = stationLine.GetAnotherStation(this);
                if (another.IsConnectedToManager())
                {
                    if (another.IsUpdated)
                    {
                        LoadingWindow wnd;
                        foreach (string message in UpdateAnotherStationMessages)
                        {
                            try
                            {
                                wnd = new LoadingWindow(string.Format(message, another.stationName.Text), 0.5);
                                wnd.ShowDialog();
                            }
                            catch (Exception) { return; }
                        }
                        IsUpdated = true;
                    }
                    else
                        MessageBox.Show("Для обновления параметров этой станции необходимо " +
                            $"обновить параметры станции \"{another.Data.Name} [{another.Data.Number}]\"", 
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    ShowUpdateError();
            }
            else
                ShowUpdateError();
        }

        private void ShowUpdateError() => 
            MessageBox.Show("Не создано ни одного менеджера для осуществления запроса. " +
                    "Создайте управляющий элемент и соедините его с сетью.", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);

        public void HideGauge() => stationGauge.Visibility = Visibility.Hidden;
        public void ShowGauge() => stationGauge.Visibility = Visibility.Visible;
    }
}
