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

		public List<int> numbersStations;
		public List<int> numbersManagers;

        private IFocusable _focusedControl;
        public IFocusable FocusedControl
        {
            get { return _focusedControl; }

            set
            {
                if (value == null)
                {
                    btnRemoveMenuItem.IsEnabled = false;
                }
                else
                {
                    btnRemoveMenuItem.IsEnabled = true;
                }
                _focusedControl = value;
            }
        }

        public WorkWindow()
        {
            InitializeComponent();
			numbersStations = new List<int>();
			numbersManagers = new List<int>();
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

			if (!enabled)
			{

			}
			else
			{
				btnRemoveMenuItem.Icon = new Image {
					Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Removed.png"))
				};

				btnRemovedMenu.Icon = new Image
				{
					Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Removed.png"))
				};

				btnCreateManagerFast.Icon = new Image
				{
					Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/NewManager.png"))
				};

				btnCreateManagerMenu.Icon = new Image
				{
					Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/NewManager.png"))
				};

				btnCreateStationFast.Icon = new Image
				{
					Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/NewStation.png"))
				};

				btnCreateStationMenu.Icon = new Image
				{
					Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/NewStation.png"))
				};
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

        public UserControl connector = null;
        public bool ConnectionAttempt(UserControl control)
        {
			if (connector == null) connector = control;
			else {
				if (connector is ManagerControl && control is ManagerControl) return false;

				SolidColorBrush brush;
				ConnectionLine line;

				if (connector is StationControl && control is StationControl)
				{
					brush = new SolidColorBrush(Colors.Green);

					(connector as StationControl).stationGauge.Visibility = Visibility.Visible;
					(control as StationControl).stationGauge.Visibility = Visibility.Visible;

					line = new ConnectionLine(connector, control, canvas);
				}
				else {
					brush = new SolidColorBrush(Colors.Blue);
					line = new ConnectionLine(connector, control, canvas, true);
				}

				connector = null;
				return true;
            }
            return false;
        }
    }
}
