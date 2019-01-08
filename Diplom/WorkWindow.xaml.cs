using Diplom.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom
{
    public partial class WorkWindow : Window
    {
		public List<int> numbersControls = new List<int>();
        public int maxNumber = 1;

        public ConnectingType connecting = ConnectingType.None;
        public UserControl connector;

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

        private static string DefaultTitle = "Мастер Link 3";

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
            Title = DefaultTitle;
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

        public void CreateStation(string name, int number = 0, double top = 0, double left = 0)
        {
            numbersControls.Add(number);
            numbersControls.Sort();

            var station = new StationControl(this, name, number, DataNetwork.CurrentColor);
            Canvas.SetLeft(station, left);
            Canvas.SetTop(station, top);
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

        public void CreateManager(string name, int number = 0, double top = 0, double left = 0)
        {
            numbersControls.Add(number);
            numbersControls.Sort();

            var manager = new ManagerControl(this, name, number, DataNetwork.CurrentColor);
            Canvas.SetLeft(manager, left);
            Canvas.SetTop(manager, top);
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

        public void SetFocus(IFocusable control) => FocusedControl = control;

        public void DropFocus()
        {
            FocusedControl?.UnsetFocusBorder();
            FocusedControl = null;
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
			UserControl control;
			if (e.Data.GetDataPresent("Station"))
				control = (StationControl)e.Data.GetData("Station");
			else if (e.Data.GetDataPresent("Manager"))
				control = (ManagerControl)e.Data.GetData("Manager");
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

        private void CreateNetwork_Click(object sender, RoutedEventArgs e) => CreateNetwork();

        private void CreateNetwork()
        {
			if (DataNetwork.IsCreated)
			{
				ShowErrorCreateNetwork();
				return;
			}
			if (DataNetwork.Stations.Count < Stock.numberLimit - 1)
			{
                ConfigurationNetwork wnd = new ConfigurationNetwork { Owner = this };
                wnd.ShowDialog();
			}
			else
				ShowErrorCountStations();
        }

        public void CreateStation_Click(object sender, RoutedEventArgs e)
        {
            if (DataNetwork.Stations.Count < Stock.numberLimit - 1)
			{
                ConfigurationStation wnd = new ConfigurationStation { Owner = this };
                wnd.ShowDialog();
                if (DataNetwork.Managers.Count > 0)
                    NetworkMenuItem.Visibility = Visibility.Visible;
			}
			else
				ShowErrorCountStations();
		}

        private void CreateManager_Click(object sender, RoutedEventArgs e)
        {
            if (DataNetwork.Managers.Count < 1)
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

		private void ShowErrorCountStations() =>
			MessageBox.Show("Максимальное кол-во станций", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

		private void ShowErrorCreateNetwork() =>
			MessageBox.Show("Максимальное кол-во сетей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DropFocus();
            e.Handled = true;
        }

        private void RemoveElement_Click(object sender, RoutedEventArgs e) => RemoveElement();

        private void btnParameters_Click(object sender, RoutedEventArgs e) =>
            (FocusedControl as StationControl).ShowParametrWindow(sender, e);

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                CreateNetwork();
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

        public void EditNetwork_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationNetwork wnd = new ConfigurationNetwork { Owner = this, IsEditing = true };
            wnd.nameNewNetwork.Text = DataNetwork.Name;
            wnd.colorCanvas.SelectedColor = DataNetwork.CurrentColor;
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

        private void ContextMenu_Opened(object sender, RoutedEventArgs e) =>
            NetworkMenuItem.Header = $"Сеть \"{DataNetwork.Name} ({DataNetwork.Type})\"";

        public void UpdateColors()
        {
            foreach (var child in canvas.Children)
            {
                if (child is StationControl)
                    (child as StationControl).SetColor(DataNetwork.CurrentColor);
                else if (child is ManagerControl)
                    (child as ManagerControl).SetColor(DataNetwork.CurrentColor);
            }
        }

        public void ConnectControls(StationControl station, bool isRadio = true)
        {
            ConnectionLine line;
            switch (connecting) {
                case ConnectingType.None:
                    connector = station;
                    connecting = (isRadio ? ConnectingType.StationRadio : ConnectingType.StationLocal);
                    break;
                case ConnectingType.StationRadio:
                    var firstStation = connector as StationControl;
                    line = new ConnectionLine(firstStation, station, canvas);
                    firstStation.stationLine = line;
                    station.stationLine = line;
                    if (firstStation.IsConnectedToManager() || station.IsConnectedToManager())
                    {
                        firstStation.stationGauge.Visibility = Visibility.Visible;
                        station.stationGauge.Visibility = Visibility.Visible;
                    }

                    CancelConnection();
                    break;
                case ConnectingType.Manager:
                    var manager = connector as ManagerControl;
                    line = new ConnectionLine(manager, station, canvas, true);
                    manager.line = line;
                    station.managerLine = line;

                    CancelConnection();
                    break;
            }
        }

        public void LoadStationConnection(int number1, int number2)
        {
            List<StationControl> stations =
                DataNetwork.Stations.Where(
                    s => s.Data.Number == number1 || s.Data.Number == number2
                    ).ToList();

            if (stations.Count != 2) throw new Exception("Unknown error");
            ConnectionLine line = new ConnectionLine(stations[0], stations[1], canvas);
            stations[0].stationLine = line;
            stations[1].stationLine = line;
            line.UpdatePosition();
        }

        public void ConnectControls(ManagerControl manager)
        {
            switch (connecting)
            {
                case ConnectingType.None:
                    connector = manager;
                    connecting = ConnectingType.Manager;
                    break;
                case ConnectingType.StationLocal:
                    var station = connector as StationControl;
                    var line = new ConnectionLine(station, manager, canvas, true);
                    manager.line = line;
                    station.managerLine = line;

                    CancelConnection();
                    break;
            }
        }
         
        public void CancelConnection()
        {
            connector = null;
            connecting = ConnectingType.None;
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
            ClearLineControls(station.stationLine);
        }

        public void RemoveLocalConnection(StationControl station)
        {
            canvas.Children.Remove(station.managerLine.line);
            ClearLineControls(station.managerLine);
        }

        public void RemoveLocalConnection(ManagerControl manager)
        {
            canvas.Children.Remove(manager.line.line);
            ClearLineControls(manager.line);
        }

        public void RemoveElement()
        {
            if (FocusedControl is StationControl)
            {
                var station = FocusedControl as StationControl;

                numbersControls.Remove(station.Data.Number);
                numbersControls.Sort();
                
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

                numbersControls.Remove(manager.Data.Number);
                numbersControls.Sort();

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
            CancelConnection();
        }

        public void RemoveNetwork_Click(object sender, RoutedEventArgs e) => RemoveNetwork();

        private void RemoveNetwork()
        {
            canvas.Children.Clear();
            DataNetwork.Managers.Clear();
            DataNetwork.Stations.Clear();
            DataNetwork.IsCreated = false;

            numbersControls.Clear();
            maxNumber = 1;

            FocusedControl = null;
            CancelConnection();
            NetworkMenuItem.Visibility = Visibility.Collapsed;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MapXmlHandler.LastPath))
                SaveAsCommand();
            else
            {
                try
                {
                    MapXmlHandler.WriteMap(MapXmlHandler.LastPath);
                    Title = $"{DefaultTitle} - {MapXmlHandler.LastPath}";
                }
                catch (Exception ex)
                {
                    ShowMapError(ex.Message);
                }
            }
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) => SaveAsCommand();

        private void SaveAsCommand()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Networks schema|*.xml",
                    Title = "Save As"
                };
                if (!string.IsNullOrEmpty(MapXmlHandler.LastPath))
                    dialog.FileName = MapXmlHandler.LastPath;
                if (dialog.ShowDialog() == true)
                {
                    MapXmlHandler.LastPath = dialog.FileName;
                    MapXmlHandler.WriteMap(dialog.FileName);
                    Title = $"{DefaultTitle} - {MapXmlHandler.LastPath}";
                }
            }
            catch (Exception ex)
            {
                ShowMapError(ex.Message);
            }
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveNetwork();
            Title = DefaultTitle;
        }
        
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveNetwork();
            Title = DefaultTitle;
            var dialog = new OpenFileDialog
            {
                Filter = "Network schema|*.xml",
                Title = "Open"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    MapXmlHandler.ReadMap(dialog.FileName);
                    MapXmlHandler.LastPath = dialog.FileName;
                    Title = $"{DefaultTitle} - {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    ShowMapError(ex.Message);
                }
            }
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void ShowMapError(string message) => MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
