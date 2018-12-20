using Diplom.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
            foreach (IFocusable control in canvas.Children)
            {
                station.FocusedElement += control.UnsetFocusBorder;
                control.FocusedElement += station.UnsetFocusBorder;
            }
            canvas.Children.Add(station);
            station.SetFocusBorder();
        }

        public void CreateManager(string name = "", int number = 0)
        {
            var manager = new ManagerControl(this, name, number);
            Canvas.SetLeft(manager, 0);
            Canvas.SetTop(manager, 0);
            foreach (IFocusable control in canvas.Children)
            {
                manager.FocusedElement += control.UnsetFocusBorder;
                control.FocusedElement += manager.UnsetFocusBorder;
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
            if (FocusedControl != null)
            {
                canvas.Children.Remove(FocusedControl as UserControl);
                foreach (IFocusable control in canvas.Children)
                {
                    control.FocusedElement -= FocusedControl.UnsetFocusBorder;
                }
                FocusedControl = null;
            }
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Station"))
            {
                var station = (StationControl)e.Data.GetData("Station");
                double shiftX = (double)e.Data.GetData("shiftX");
                double shiftY = (double)e.Data.GetData("shiftY");
                Canvas.SetLeft(station, e.GetPosition(canvas).X - shiftX);
                Canvas.SetTop(station, e.GetPosition(canvas).Y - shiftY);
            }
            else if (e.Data.GetDataPresent("Manager"))
            {
                var manager = (ManagerControl)e.Data.GetData("Manager");
                double shiftX = (double)e.Data.GetData("shiftX");
                double shiftY = (double)e.Data.GetData("shiftY");
                Canvas.SetLeft(manager, e.GetPosition(canvas).X - shiftX);
                Canvas.SetTop(manager, e.GetPosition(canvas).Y - shiftY);
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
    }
}
