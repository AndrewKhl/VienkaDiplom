using Diplom.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {

		public List<int> numberStations;

        private IFocusable _focusedControl;
        public IFocusable FocusedControl
        {
            get { return _focusedControl; }

            set
            {
                if (value == null)
                {
                    RemoveMenuItem.IsEnabled = false;
                }
                else
                {
                    RemoveMenuItem.IsEnabled = true;
                }
                _focusedControl = value;
            }
        }

        public WorkWindow()
        {
            InitializeComponent();
			numberStations = new List<int>();
			Stock.workWindow = this;
		}

        public void CreateStation(string name = "")
        {
            var station = new StationControl(this, name);
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

        public void CreateManager(string name = "")
        {
            var manager = new ManagerControl(this, name);
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
			if (numberStations.Count < 55)
			{
				ConfigurationNetwork wnd = new ConfigurationNetwork();
				wnd.Owner = this;
				wnd.Show();
			}
			else
				ShowErrorCountStations();
        }

        private void CreateStation_Click(object sender, RoutedEventArgs e)
        {
			if (numberStations.Count < 55)
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
