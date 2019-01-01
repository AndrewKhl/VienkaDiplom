using Diplom.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
		public List<int> numbersStations = new List<int>();
		public List<int> numbersManagers = new List<int>();
        public int maxStationNumber = 1;
        public int maxManagerNumber = 1;
        public UserControl connector = null;
        public Color currentColor;
        private bool? IsRadioConnection = null;

        private static Uri enableRemove = new Uri(@"pack://application:,,,/Resources/Icons/Removed.png");
        private static Uri disableRemove = new Uri(@"pack://application:,,,/Resources/Icons/DisableRemoved.png");
        private static Uri enableManager = new Uri(@"pack://application:,,,/Resources/Icons/NewManager.png");
        private static Uri disableManager = new Uri(@"pack://application:,,,/Resources/Icons/DisableCreateManager.png");
        private static Uri enableStation = new Uri(@"pack://application:,,,/Resources/Icons/NewStation.png");
        private static Uri disableStation = new Uri(@"pack://application:,,,/Resources/Icons/DisableCreateManager.png");
        private static Uri enableProperties = new Uri(@"pack://application:,,,/Resources/Icons/CustomFile.png");
        private static Uri disableProperties = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShowProperty.png");
        private static Uri enableParameters = new Uri(@"pack://application:,,,/Resources/Icons/Params.png");
        private static Uri disableParameters = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShow.png");
        private static Uri enableDB = new Uri(@"pack://application:,,,/Resources/Icons/DBevent.png");
        private static Uri disableDB = new Uri(@"pack://application:,,,/Resources/Icons/DisableDB.png");

        private IFocusable _focusedControl;
        public IFocusable FocusedControl
        {
            get => _focusedControl;
            set
            {
                if (value != null)
                {
                    btnRemoveMenuItem.IsEnabled = true;
                    btnRemoveMenuItem.Icon = new Image {Source = new BitmapImage(enableRemove) };

                    btnProperties.IsEnabled = true;
                    btnProperties.Icon = new Image { Source = new BitmapImage(enableProperties) };

                    if (value is StationControl)
                    {
                        btnParameters.IsEnabled = true;
                        btnParameters.Icon = new Image { Source = new BitmapImage(enableParameters) };
                    }
                }
                else
                {
                    btnRemoveMenuItem.IsEnabled = false;
                    btnRemoveMenuItem.Icon = new Image { Source = new BitmapImage(disableRemove) };

                    btnProperties.IsEnabled = false;
                    btnProperties.Icon = new Image { Source = new BitmapImage(disableProperties) };

                    btnParameters.IsEnabled = false;
                    btnParameters.Icon = new Image { Source = new BitmapImage(disableParameters) };
                }
                _focusedControl = value;
            }
        }

        public WorkWindow()
        {
            InitializeComponent();
			Stock.workWindow = this;
			EnabledButton(false);
		}

		public void EnabledButton(bool enabled)
		{
			btnCreateManagerFast.IsEnabled = enabled;
			btnCreateManagerMenu.IsEnabled = enabled;
			btnCreateStationFast.IsEnabled = enabled;
			btnCreateStationMenu.IsEnabled = enabled;
			btnRemovedMenu.IsEnabled = enabled;
			btnRemoveMenuItem.IsEnabled = enabled;

			if (enabled)
			{
				btnRemoveMenuItem.Icon = new Image { Source = new BitmapImage(enableRemove) };
				btnRemovedMenu.Icon = new Image { Source = new BitmapImage(enableRemove) };
				btnCreateManagerFast.Icon = new Image { Source = new BitmapImage(enableManager) };
				btnCreateManagerMenu.Icon = new Image { Source = new BitmapImage(enableManager) };
                btnCreateStationFast.Icon = new Image { Source = new BitmapImage(enableStation) };
                btnCreateStationMenu.Icon = new Image { Source = new BitmapImage(enableStation) };
			}
		}

        public void CreateStation(string name, int number = 0)
        {
			numbersStations.Add(number);
            numbersStations.Sort();
            var station = new StationControl(this, name, number, currentColor);
            Canvas.SetLeft(station, 0);
            Canvas.SetTop(station, 0);
            foreach (var control in canvas.Children)
            {
                if (control is IFocusable)
                {
                    IFocusable focusable = control as IFocusable;
                    station.FocusedElement += focusable.UnsetFocusBorder;
                    focusable.FocusedElement += station.UnsetFocusBorder;
                }
            }
            canvas.Children.Add(station);
            station.SetFocusBorder();
        }

        public void CreateManager(string name, int number = 0)
        {
            numbersManagers.Add(number);
            var manager = new ManagerControl(this, name, number, currentColor);
            Canvas.SetLeft(manager, 0);
            Canvas.SetTop(manager, 0);
            foreach (var control in canvas.Children)
            {
                if (control is IFocusable)
                {
                    IFocusable focusable = control as IFocusable;
                    manager.FocusedElement += focusable.UnsetFocusBorder;
                    focusable.FocusedElement += manager.UnsetFocusBorder;
                }
            }
            canvas.Children.Add(manager);
            manager.SetFocusBorder();
        }

        public void SetFocus(IFocusable control)
        {
            FocusedControl = control;
        }

        public void DropFocus()
        {
            FocusedControl?.UnsetFocusBorder();
            FocusedControl = null;
        }

        private void ClearLineControls(ConnectionLine line)
        {
            if (line.firstControl is StationControl && line.secondControl is StationControl)
            {
                var first = line.firstControl as StationControl;
                var second = line.secondControl as StationControl;

                first.stationGauge.Visibility = Visibility.Hidden;
                second.stationGauge.Visibility = Visibility.Hidden;

                first.stationLine = null;
                second.stationLine = null;
            }
            else
            {
                StationControl station;
                ManagerControl manager;
                if (line.firstControl is StationControl && line.secondControl is ManagerControl)
                {
                    station = line.firstControl as StationControl;
                    manager = line.secondControl as ManagerControl;
                }
                else
                {
                    manager = line.firstControl as ManagerControl;
                    station = line.secondControl as StationControl;
                }

                station.managerLine = null;
                manager.line = null;

                if (station.stationLine != null)
                {
                    (station.stationLine.firstControl as StationControl).stationGauge.Visibility = Visibility.Hidden;
                    (station.stationLine.secondControl as StationControl).stationGauge.Visibility = Visibility.Hidden;
                }
            }
        }

        public void RemoveRadioConnection(StationControl station)
        {
            canvas.Children.Remove(station.stationLine.line);
            (station.stationLine.firstControl as StationControl).IsChecked = false;
            (station.stationLine.secondControl as StationControl).IsChecked = false;
            ClearLineControls(station.stationLine);
        }

        public void RemoveElement()
        {
            if (FocusedControl is StationControl)
            {
                var station = FocusedControl as StationControl;
                numbersStations.Remove(station.Data.Number);
                numbersStations.Sort();
                if (station.managerLine != null)
                {
                    canvas.Children.Remove(station.managerLine.line);
                    ClearLineControls(station.managerLine);
                }
                if (station.stationLine != null)
                {
                    canvas.Children.Remove(station.stationLine.line);
                    ClearLineControls(station.stationLine);
                }

                DataNetwork.Stations.Remove(station);
            }
            else if (FocusedControl is ManagerControl)
            {
                var manager = FocusedControl as ManagerControl;
                numbersManagers.Remove(manager.Data.Number);
                if (manager.line != null)
                {
                    canvas.Children.Remove(manager.line.line);
                    ClearLineControls(manager.line);
                }

                DataNetwork.Managers.Remove(manager);
            }

            canvas.Children.Remove(FocusedControl as UserControl);
            foreach (var control in canvas.Children)
            {
                if (control is IFocusable)
                    (control as IFocusable).FocusedElement -= FocusedControl.UnsetFocusBorder;
            }
            FocusedControl = null;
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
			UserControl control;
			if (e.Data.GetDataPresent("Station"))
			{
				control = (StationControl)e.Data.GetData("Station");
			}
			else if (e.Data.GetDataPresent("Manager"))
			{
				control = (ManagerControl)e.Data.GetData("Manager");
			}
			else return;
			double shiftX = (double)e.Data.GetData("shiftX");
			double shiftY = (double)e.Data.GetData("shiftY");
			Canvas.SetLeft(control, e.GetPosition(canvas).X - shiftX);
			Canvas.SetTop(control, e.GetPosition(canvas).Y - shiftY);
            if (control is ManagerControl && (control as ManagerControl).line != null)
                (control as ManagerControl).line.UpdatePosition();
            else if (control is StationControl)
            {
                var station = control as StationControl;
                if (station.managerLine != null)
                    station.managerLine.UpdatePosition();
                if (station.stationLine != null)
                    station.stationLine.UpdatePosition();
            }
			e.Handled = true;
        }

        private void CreateNetwork_Click(object sender, RoutedEventArgs e)
        {
            CreateNetwork();
        }

        private void CreateNetwork()
        {
			if (DataNetwork.IsCreate)
			{
				ShowErrorCreateNetwork();
				return;
			}
			if (numbersStations.Count < Stock.numberLimit)
			{
                ConfigurationNetwork wnd = new ConfigurationNetwork { Owner = this };
                wnd.ShowDialog();
			}
			else
				ShowErrorCountStations();
        }

        public void CreateStation_Click(object sender, RoutedEventArgs e)
        {
			if (numbersStations.Count < Stock.numberLimit)
			{
                ConfigurationStation wnd = new ConfigurationStation { Owner = this };
                wnd.ShowDialog();
                if (DataNetwork.Managers.Count > 0)
                    NetworkMenuItem.Visibility = Visibility.Visible;
			}
			else
				ShowErrorCountStations();
		}

		private void ShowErrorCountStations()
		{
			MessageBox.Show("Максимальное кол-во станций", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void ShowErrorCreateNetwork()
		{
			MessageBox.Show("Максимальное кол-во сетей", "Ошибка",
			MessageBoxButton.OK, MessageBoxImage.Error);
		}

        private void CreateManager_Click(object sender, RoutedEventArgs e)
        {
            if (numbersManagers.Count < 1)
            {
                ConfigurationManager wnd = new ConfigurationManager { Owner = this };
                wnd.ShowDialog();
            }
            else
            {
                MessageBox.Show("В данной версии существует ограничение на количество " +
                    "одновременно существующих менеджеров сети: не более одного", 
                    "Ограничения версии", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DropFocus();
            e.Handled = true;
        }

        private void RemoveElement_Click(object sender, RoutedEventArgs e)
        {
            RemoveElement();
        }

        public void ConnectControls(StationControl station, bool isRadio = true)
        {
            if (connector == null)
            {
                if ((isRadio && station.stationLine == null) || (!isRadio && station.managerLine == null))
                {
                    IsRadioConnection = isRadio;
                    connector = station;
                    if (isRadio)
                    {
                        station.IsChecked = false;
                        StationControl.IsConnecting = true;
                    }
                }
            }
            else
            {
                if (connector != station)
                {
                    if (IsRadioConnection == true)
                    {
                        var savedStation = connector as StationControl;
                        var line = new ConnectionLine(savedStation, station, canvas);
                        savedStation.stationLine = line;
                        station.stationLine = line;
                        if (savedStation.IsConnectedToManager() || station.IsConnectedToManager())
                        {
                            savedStation.stationGauge.Visibility = Visibility.Visible;
                            station.stationGauge.Visibility = Visibility.Visible;
                        }
                        savedStation.IsChecked = true;
                        station.IsChecked = true;
                        StationControl.IsConnecting = false;
                    }
                    else if (IsRadioConnection == false && connector is ManagerControl)
                    {
                        var manager = connector as ManagerControl;
                        var line = new ConnectionLine(manager, station, canvas, true);
                        manager.line = line;
                        station.managerLine = line;
                    }
                }
                connector = null;
                IsRadioConnection = null;
            }
        }

        public void ConnectControls(ManagerControl manager)
        {
            if (connector == null)
            {
                if (manager.line == null)
                {
                    connector = manager;
                    IsRadioConnection = false;
                }
            }
            else
            {
                if (IsRadioConnection == false && connector is StationControl && manager.line == null)
                {
                    var station = connector as StationControl;
                    var line = new ConnectionLine(station, manager, canvas, true);
                    manager.line = line;
                    station.managerLine = line;
                }
                connector = null;
                IsRadioConnection = null;
            }
        }

        private void btnParameters_Click(object sender, RoutedEventArgs e)
        {
            (FocusedControl as StationControl).ShowParametrWindow(sender, e);
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) CreateNetwork();
        }

        private void Routing_Click(object sender, RoutedEventArgs e)
        {
            foreach (StationControl station in DataNetwork.Stations)
            {
                if (station.IsConnectedToStation() && station.IsConnectedToManager())
                {
                    (station.stationLine.firstControl as StationControl).stationGauge.Visibility = Visibility.Visible;
                    (station.stationLine.secondControl as StationControl).stationGauge.Visibility = Visibility.Visible;
                }
            }
        }

        private void EditNetwork_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationNetwork wnd = new ConfigurationNetwork { Owner = this, IsEditing = true };
            wnd.nameNewNetwork.Text = DataNetwork.Name;
            wnd.colorCanvas.SelectedColor = currentColor;
            foreach (string item in wnd.listOfAdress.Items)
            {
                if (item == DataNetwork.Address.ToString())
                {
                    wnd.listOfAdress.SelectedItem = item;
                    break;
                }
            }
            foreach (ComboBoxItem item in wnd.typeNetwork.Items)
            {
                if ((string)item.Content == DataNetwork.Type)
                {
                    wnd.typeNetwork.SelectedItem = item;
                    break;
                }
            }
            wnd.typeNetwork.IsEnabled = false;
            wnd.ShowDialog();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            NetworkMenuItem.Header = $"Сеть {DataNetwork.Name}";
        }

        public void UpdateColors()
        {
            foreach (var child in canvas.Children)
            {
                if (child is StationControl)
                    (child as StationControl).SetColor(currentColor);
                else if (child is ManagerControl)
                    (child as ManagerControl).SetColor(currentColor);
            }
        }
    }
}
