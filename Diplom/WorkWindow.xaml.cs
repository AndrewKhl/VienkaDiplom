using Diplom.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
		public List<int> numbersStations = new List<int>();
		public List<int> numbersManagers = new List<int>();
        public List<ConnectionLine> connectionLines = new List<ConnectionLine>();

        private Uri enableRemove = new Uri(@"pack://application:,,,/Resources/Icons/Removed.png");
        private Uri disableRemove = new Uri(@"pack://application:,,,/Resources/Icons/DisableRemoved.png");
        private Uri enableManager = new Uri(@"pack://application:,,,/Resources/Icons/NewManager.png");
        private Uri disableManager = new Uri(@"pack://application:,,,/Resources/Icons/DisableCreateManager.png");
        private Uri enableStation = new Uri(@"pack://application:,,,/Resources/Icons/NewStation.png");
        private Uri disableStation = new Uri(@"pack://application:,,,/Resources/Icons/DisableCreateManager.png");
        private Uri enableProperties = new Uri(@"pack://application:,,,/Resources/Icons/CustomFile.png");
        private Uri disableProperties = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShowProperty.png");
        private Uri enableParameters = new Uri(@"pack://application:,,,/Resources/Icons/Params.png");
        private Uri disableParameters = new Uri(@"pack://application:,,,/Resources/Icons/DisabledShow.png");
        private Uri enableDB = new Uri(@"pack://application:,,,/Resources/Icons/DBevent.png");
        private Uri disableDB = new Uri(@"pack://application:,,,/Resources/Icons/DisableDB.png");

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

        public void CreateStation(string name = "", int number = 0)
        {
            var station = new StationControl(this, name, number);
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

        public void CreateManager(string name = "", int number = 0)
        {
            var manager = new ManagerControl(this, name, number);
            Canvas.SetLeft(manager, 0);
            Canvas.SetTop(manager, 0);
            foreach (Control control in canvas.Children)
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

        public void RemoveElement()
        {
			IConnectable connectable = FocusedControl as IConnectable;

            if (FocusedControl != null)
            {
				foreach (var line in (FocusedControl as IConnectable).connectionLines)
				{
					if (line.firstControl == connectable)
					{
						(line.secondControl as IConnectable).connectionLines.Remove(line);
						if (line.secondControl is StationControl)
							(line.secondControl as StationControl).stationGauge.Visibility = Visibility.Hidden;
					}
					else
					{
						(line.firstControl as IConnectable).connectionLines.Remove(line);
						if (line.firstControl is StationControl)
							(line.firstControl as StationControl).stationGauge.Visibility = Visibility.Hidden;
					}
					canvas.Children.Remove(line.line);
				}
				connectable.connectionLines.Clear();

                canvas.Children.Remove(FocusedControl as UserControl);
                foreach (var control in canvas.Children)
                {
					if (control is IFocusable)
						(control as IFocusable).FocusedElement -= FocusedControl.UnsetFocusBorder;
                }
                FocusedControl = null;
            }
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
			foreach (var line in (control as IConnectable).connectionLines)
			{
				line.UpdatePosition();
			}
			e.Handled = true;
        }

        private void CreateNetwork_Click(object sender, RoutedEventArgs e)
        {
			if (DataNetwork.IsCreate)
			{
				ShowErrorCreateNetwork();
				return;
			}
			if (numbersStations.Count < 55)
			{
				ConfigurationNetwork wnd = new ConfigurationNetwork();
				wnd.Owner = this;
				wnd.Show();
			}
			else
				ShowErrorCountStations();
        }

        public void CreateStation_Click(object sender, RoutedEventArgs e)
        {
			if (numbersStations.Count < 55)
			{
				ConfigurationStation wnd = new ConfigurationStation();
				wnd.Owner = this;
				wnd.Show();
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
            CreateManager();
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

        //public bool ConnectionAttempt(UserControl control)
        //{
			//if (connector == null) connector = control;
			//else {
			//	if (connector is ManagerControl && control is ManagerControl) return false;

			//	SolidColorBrush brush;
			//	ConnectionLine line;

			//	if (connector is StationControl && control is StationControl)
			//	{
			//		brush = new SolidColorBrush(Colors.Green);

			//		(connector as StationControl).stationGauge.Visibility = Visibility.Visible;
			//		(control as StationControl).stationGauge.Visibility = Visibility.Visible;

			//		line = new ConnectionLine(connector, control, canvas);
			//	}
			//	else {
			//		brush = new SolidColorBrush(Colors.Blue);
			//		line = new ConnectionLine(connector, control, canvas, true);
			//	}

			//	connector = null;
			//	return true;
   //         }
   //         return false;
   //     }
        public UserControl connector = null;

        public void ConnectControls(StationControl control)
        {
            if (connector == null)
            {
                if (control.firstLine == null || control.secondLine == null)
                    connector = control;
            }
            else if (connector != control)
            {
                if (connector is StationControl)
                {
                    var first = connector as StationControl;
                    ConnectionLine line;
                    if (first.firstLine == null && control.firstLine == null)
                    {
                        line = new ConnectionLine(first, control, canvas);
                        first.firstLine = line;
                        control.firstLine = line;
                    }
                    else if (first.secondLine == null && control.firstLine == null)
                    {
                        line = new ConnectionLine(first, control, canvas);
                        first.secondLine = line;
                        control.firstLine = line;
                    }
                    else if (first.firstLine == null && control.secondLine == null)
                    {
                        line = new ConnectionLine(first, control, canvas);
                        first.firstLine = line;
                        control.secondLine = line;
                    }
                    else if (first.secondLine == null && control.secondLine == null) 
                    {
                        line = new ConnectionLine(first, control, canvas);
                        first.secondLine = line;
                        control.secondLine = line;
                    }
                }
                else
                {
                    var first = connector as ManagerControl;
                    ConnectionLine line;
                    if (first.line == null)
                    {
                        if (control.firstLine == null)
                        {
                            line = new ConnectionLine(first, control, canvas, true);
                            first.line = line;
                            control.firstLine = line;
                        }
                        else if (control.secondLine == null)
                        {
                            line = new ConnectionLine(first, control, canvas, true);
                            first.line = line;
                            control.secondLine = line;
                        }
                    }
                }
                connector = null;
            }
        }

        public void ConnectControls(ManagerControl control)
        {
            if (connector == null)
            {
                if (control.line == null) connector = control;
            }
            else if (connector != control)
            {
                if (connector is ManagerControl)
                {

                }
                else
                {

                }
            }
        }

        private void btnParameters_Click(object sender, RoutedEventArgs e)
        {
            (FocusedControl as StationControl).ShowParametrWindow(sender, e);
        }
    }
}
