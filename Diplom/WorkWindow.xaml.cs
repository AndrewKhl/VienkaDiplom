using Diplom.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static Uri disableManager = new Uri(@"pack://application:,,,/Resources/Icons/DisabledCreateManager.png");
        private static Uri enableStation = new Uri(@"pack://application:,,,/Resources/Icons/NewStation.png");
        private static Uri disableStation = new Uri(@"pack://application:,,,/Resources/Icons/DisabledCreateStation.png");
        private static Uri enableProperties = new Uri(@"pack://application:,,,/Resources/Icons/CustomFile.png");
        private static Uri disableProperties = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShowProperty.png");
        private static Uri enableParameters = new Uri(@"pack://application:,,,/Resources/Icons/Params.png");
        private static Uri disableParameters = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShow.png");
        private static Uri enableDB = new Uri(@"pack://application:,,,/Resources/Icons/DBevent.png");
        private static Uri disableDB = new Uri(@"pack://application:,,,/Resources/Icons/DisableDB.png");
        private static Uri enableRoute = new Uri(@"pack://application:,,,/Resources/Icons/Marsh.png");
        private static Uri disableRoute = new Uri(@"pack://application:,,,/Resources/Icons/disableRoute.png");

        private static string DefaultTitle = "Мастер Link 3";

        private bool isChanged = false;
        public bool IsMapChanged {
            get => isChanged;
            private set
            {
                if (value && Title[Title.Length - 1] != '*')
                    Title = Title + '*';
                else if (!value && Title[Title.Length - 1] == '*')
                    Title = Title.Remove(Title.Length - 1);
                isChanged = value;
            }
        }
        public void MapChanged() => IsMapChanged = true;

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

                    btnPropertiesFast.IsEnabled = true;
                    btnPropertiesFast.Icon = new Image { Source = new BitmapImage(enableProperties) };

                    btnProperties.IsEnabled = true;
                    btnProperties.Icon = new Image { Source = new BitmapImage(enableProperties) };

                    if (value is ManagerControl && (value as ManagerControl).line != null)
                    {
                        btnRouting.IsEnabled = true;
                        btnRouting.Icon = new Image { Source = new BitmapImage(enableRoute) };
                    }
                    else if (value is StationControl)
                        ToggleParametersButtons((value as StationControl).IsUpdated);
                }
                else
                {
                    btnRemoveMenuItem.IsEnabled = false;
                    btnRemoveMenuItem.Icon = new Image { Source = new BitmapImage(disableRemove) };

                    btnPropertiesFast.IsEnabled = false;
                    btnPropertiesFast.Icon = new Image { Source = new BitmapImage(disableProperties) };

                    btnProperties.IsEnabled = false;
                    btnProperties.Icon = new Image { Source = new BitmapImage(disableProperties) };

                    btnRouting.IsEnabled = false;
                    btnRouting.Icon = new Image { Source = new BitmapImage(disableRoute) };

                    ToggleParametersButtons(false);
                }
                _focusedControl = value;
            }
        }

        public WorkWindow()
        {
            InitializeComponent();
            Title = DefaultTitle;
			Stock.workWindow = this;
			TogglePanelButtons(false);
		}

		public void TogglePanelButtons(bool isEnabled)
		{
			btnCreateManagerFast.IsEnabled = isEnabled;
			btnCreateManagerMenu.IsEnabled = isEnabled;
			btnCreateStationFast.IsEnabled = isEnabled;
			btnCreateStationMenu.IsEnabled = isEnabled;
			btnRemoveMenuItem.IsEnabled = isEnabled;
			btnRemovedMenu.IsEnabled = isEnabled;

            btnEditNetwork.IsEnabled = isEnabled;

			if (isEnabled)
			{
				btnCreateManagerFast.Icon = new Image { Source = new BitmapImage(enableManager) };
				btnCreateManagerMenu.Icon = new Image { Source = new BitmapImage(enableManager) };
                btnCreateStationFast.Icon = new Image { Source = new BitmapImage(enableStation) };
                btnCreateStationMenu.Icon = new Image { Source = new BitmapImage(enableStation) };
				btnRemoveMenuItem.Icon = new Image { Source = new BitmapImage(enableRemove) };
				btnRemovedMenu.Icon = new Image { Source = new BitmapImage(enableRemove) };
			}
            else
            {
				btnCreateManagerFast.Icon = new Image { Source = new BitmapImage(disableManager) };
				btnCreateManagerMenu.Icon = new Image { Source = new BitmapImage(disableManager) };
                btnCreateStationFast.Icon = new Image { Source = new BitmapImage(disableStation) };
                btnCreateStationMenu.Icon = new Image { Source = new BitmapImage(disableStation) };
				btnRemoveMenuItem.Icon = new Image { Source = new BitmapImage(disableRemove) };
				btnRemovedMenu.Icon = new Image { Source = new BitmapImage(disableRemove) };
            }
		}

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if (MapXmlHandler.ReadLastPath())
                    OpenMap(MapXmlHandler.LastPath);
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                MessageBox.Show("Карта не считана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CreateStation(string name, int number = 0, int top = 0, int left = 0)
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
                    station.FocusedElement += focusable.UpdateLook;
                    focusable.FocusedElement += station.UpdateLook;
                }
            }
            canvas.Children.Add(station);
            station.UpdateLook();

            MapChanged();
        }

        public void CreateManager(string name, int number = 0, int top = 0, int left = 0, bool setFocused = true)
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
                    manager.FocusedElement += focusable.UpdateLook;
                    focusable.FocusedElement += manager.UpdateLook;
                }
            }
            canvas.Children.Add(manager);
            manager.UpdateLook();

            MapChanged();
        }

        public void SetFocus(IFocusable control) => FocusedControl = control;

        public void DropFocus()
        {
            IFocusable control = FocusedControl;
            FocusedControl = null;
            control?.UpdateLook();
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

            if (e.GetPosition(canvas).X - shiftX == Canvas.GetLeft(control)
                && e.GetPosition(canvas).Y - shiftY == Canvas.GetTop(control))
                return;

			Canvas.SetLeft(control, e.GetPosition(canvas).X - shiftX);
			Canvas.SetTop(control, e.GetPosition(canvas).Y - shiftY);
            if (control is ManagerControl && (control as ManagerControl).line != null)
            {
                (control as ManagerControl).line.UpdatePosition();
            }
            else if (control is StationControl)
            {
                var station = control as StationControl;
                if (station.managerLine != null)
                    station.managerLine.UpdatePosition();
                if (station.stationLine != null)
                    station.stationLine.UpdatePosition();
            }
			e.Handled = true;
            MapChanged();
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

                MapChanged();
			}
			else
				ShowErrorCountStations();
        }

        public void CreateStation_Click(object sender, RoutedEventArgs e)
        {
            if (!DataNetwork.IsCreated) return;

            if (DataNetwork.Stations.Count < Stock.numberLimit - 1)
			{
                ConfigurationStation wnd = new ConfigurationStation { Owner = this };
                wnd.ShowDialog();

                MapChanged();
			}
			else
				ShowErrorCountStations();
		}

        private void CreateManager_Click(object sender, RoutedEventArgs e)
        {
            if (!DataNetwork.IsCreated) return;

            if (DataNetwork.Managers.Count < 1)
            {
                ConfigurationManager wnd = new ConfigurationManager { Owner = this };
                wnd.ShowDialog();

                MapChanged();
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
            if (FocusedControl is ManagerControl)
                (FocusedControl as ManagerControl).Route_Click(sender, e);
        }

        public void EditNetwork_Click(object sender, RoutedEventArgs e)
        {
            if (!DataNetwork.IsCreated) return;

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

            MapChanged();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (DataNetwork.IsCreated)
            {
                NetworkMenuItem.Visibility = Visibility.Visible;
                NetworkMenuItem.Header = $"Сеть \"{DataNetwork.Name} ({DataNetwork.Type})\"";
            }
            else
            {
                NetworkMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        public void UpdateColors()
        {
            foreach (var child in canvas.Children)
            {
                if (child is StationControl)
                    //(child as StationControl).SetColor(DataNetwork.CurrentColor);
                    (child as StationControl).CurrentColor = DataNetwork.CurrentColor;
                else if (child is ManagerControl)
                    (child as ManagerControl).SetColor(DataNetwork.CurrentColor);
            }
            MapChanged();
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

                    CancelConnection();
                    MapChanged();
                    break;
                case ConnectingType.Manager:
                    var manager = connector as ManagerControl;
                    line = new ConnectionLine(manager, station, canvas);
                    manager.line = line;
                    station.managerLine = line;
                    station.Data.State = "Включено";
                    manager.Port = ManagerControl.LastPort;

                    CancelConnection();
                    MapChanged();
                    break;
            }
        }

        public void LoadStationConnection(int number1, int number2)
        {
            List<StationControl> stations =
                DataNetwork.Stations.Where(s => s.Data.Number == number1 || s.Data.Number == number2).ToList();

            if (stations.Count != 2) throw new Exception("Unknown error");
            ConnectionLine line = new ConnectionLine(stations[0], stations[1], canvas);
            stations[0].stationLine = line;
            stations[1].stationLine = line;
            line.UpdatePosition();

            MapChanged();
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
                    var line = new ConnectionLine(station, manager, canvas);
                    manager.line = line;
                    station.managerLine = line;
                    manager.Port = ManagerControl.LastPort;

                    CancelConnection();
                    MapChanged();
                    break;
            }
        }
         
        public void CancelConnection()
        {
            connector = null;
            connecting = ConnectingType.None;
        }

        public void RemoveRadioConnection(StationControl station)
        {
            // connection between two stations
            if (station.stationLine != null)
            {
                var another = station.stationLine.GetAnotherStation(station);
                if (station.IsConnectedToManager())
                    another.IsUpdated = false;
                else if (another.IsConnectedToManager())
                    station.IsUpdated = false;
            }
            canvas.Children.Remove(station.stationLine.line);
            station.stationLine.ClearControls();
            MapChanged();
        }

        public void RemoveLocalConnection(ConnectionLine line)
        {
            // connection between station and manager
            var manager = (line.firstControl is ManagerControl ? 
                line.firstControl : line.secondControl) as ManagerControl;
            var station = line.GetConnectedStation(manager);
            if (station.stationLine != null)
            {
                (station.stationLine.firstControl as StationControl).IsUpdated = false;
                (station.stationLine.secondControl as StationControl).IsUpdated = false;
            }
            else
            {
                station.IsUpdated = false;
            }
            manager.Port = null;
            canvas.Children.Remove(line.line);
            manager.line.ClearControls();
            MapChanged();
        }

        public void RemoveElement()
        {
            if (FocusedControl == null) return;

            if (FocusedControl is StationControl)
            {
                var station = FocusedControl as StationControl;

                numbersControls.Remove(station.Data.Number);
                numbersControls.Sort();
                
                if (station.managerLine != null)
                    RemoveLocalConnection(station.managerLine);
                if (station.stationLine != null)
                    RemoveRadioConnection(station);

                DataNetwork.Stations.Remove(station);
            }
            else if (FocusedControl is ManagerControl)
            {
                var manager = FocusedControl as ManagerControl;

                numbersControls.Remove(manager.Data.Number);
                numbersControls.Sort();

                if (manager.line != null)
                    RemoveLocalConnection(manager.line);

                DataNetwork.Managers.Remove(manager);
            }

            canvas.Children.Remove(FocusedControl as UserControl);
            foreach (var control in canvas.Children)
            {
                if (control is IFocusable)
                    (control as IFocusable).FocusedElement -= FocusedControl.UpdateLook;
            }
            FocusedControl = null;
            CancelConnection();
            MapChanged();
        }

        public void RemoveNetwork_Click(object sender, RoutedEventArgs e) => RemoveNetwork();

        private void RemoveNetwork()
        {
            canvas.Children.Clear();
            DataNetwork.Managers.Clear();
            DataNetwork.Stations.Clear();
            ToggleNetwork(false);

            numbersControls.Clear();
            maxNumber = 1;

            FocusedControl = null;
            CancelConnection();
            NetworkMenuItem.Visibility = Visibility.Collapsed;
            MapChanged();
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) => SaveCommand();

        private void SaveCommand()
        {
            if (string.IsNullOrEmpty(MapXmlHandler.LastPath))
                SaveAsCommand();
            else
            {
                try
                {
                    MapXmlHandler.WriteMap(MapXmlHandler.LastPath);
                    MapXmlHandler.WriteLastPath();
                    Title = $"{DefaultTitle} - {MapXmlHandler.LastPath}";
                    IsMapChanged = false;
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
                    MapXmlHandler.WriteLastPath();
                    Title = $"{DefaultTitle} - {MapXmlHandler.LastPath}";
                    IsMapChanged = false;
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
            MapXmlHandler.LastPath = null;
            IsMapChanged = false;
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
                OpenMap(dialog.FileName);
        }

        private void OpenMap(string path)
        {
            try
            {
                MapXmlHandler.ReadMap(path);
                MapXmlHandler.LastPath = path;
                MapXmlHandler.WriteLastPath();
                Title = $"{DefaultTitle} - {path}";
                IsMapChanged = false;
                DropFocus();
            }
            catch (Exception ex)
            {
                ShowMapError(ex.Message);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ClosingMessage() == MessageBoxResult.Yes)
            {
                SavingMap();
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ClosingMessage() == MessageBoxResult.Yes)
            {
                SavingMap();
                Close();
            }
        }

        private MessageBoxResult ClosingMessage() => 
            MessageBox.Show("Вы действительно хотите выйти?", 
                "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

        private void SavingMap()
        {
            if (IsMapChanged)
            {
                var result = MessageBox.Show("Карта была изменена. Сохранить изменения?",
                    "Сохранить изменения?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    SaveCommand();
            }
        }

        private void Properties_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PropertiesWindow wnd = new PropertiesWindow { Owner = this };
            wnd.ShowDialog();
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void Properties_CanExecute(object sender, CanExecuteRoutedEventArgs e) => 
            e.CanExecute = true;

        private void ShowMapError(string message) => MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (FocusedControl != null && e.Key == Key.Delete)
                RemoveElement();
            else if (FocusedControl != null && FocusedControl is StationControl && e.Key == Key.F3 && Keyboard.IsKeyDown(Key.LeftShift))
                (FocusedControl as StationControl).Update_Click(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (canvas == null || ScaleComboBox == null) return;
            string option = (e.AddedItems[0] as ComboBoxItem).Content as string;
            double coeff = int.Parse(option.Remove(option.Length - 1)) / 100.0;
            canvas.LayoutTransform = new ScaleTransform(coeff, coeff);
        }

        public void ToggleNetwork(bool value)
        {
            DataNetwork.IsCreated = value;
            TogglePanelButtons(value);
        }

        private void btnProperties_Click(object sender, RoutedEventArgs e)
        {
            if (FocusedControl == null) return;

            if (FocusedControl is StationControl)
                (FocusedControl as StationControl).StationProperties_Click(sender, e);
            else if (FocusedControl is ManagerControl)
                (FocusedControl as ManagerControl).ManagerProperties_Click(sender, e);
        }

        public void ToggleParametersButtons(bool value)
        {
            btnParameters.IsEnabled = value;
            btnParametersFast.IsEnabled = value;
            
            btnParameters.Icon = new Image { Source = new BitmapImage(value ? enableParameters : disableParameters) };
            btnParametersFast.Icon = new Image { Source = new BitmapImage(value ? enableParameters : disableParameters) };
        }
    }
}
