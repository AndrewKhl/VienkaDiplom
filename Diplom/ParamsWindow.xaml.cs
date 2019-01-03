using Diplom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для ParamsWindow.xaml
    /// </summary>
    public partial class ParamsWindow : Window
    {
		private StationControl _currentStation;

        public ParamsWindow(StationControl station)
        {
			_currentStation = station;
            InitializeComponent();
			periodStationTextBlock.Text = _currentStation.Data.Period.ToString() + " МГц";
			DataContext = _currentStation.Data;
        }

		private void VisualTree(object sender)
		{
			FrameworkElement obj = sender as FrameworkElement;
			
			if (sender as TextBlock != null)
				obj.Visibility = Visibility.Visible;

			int count = VisualTreeHelper.GetChildrenCount(obj);

			for (int i = 0; i < count; ++i)
			{
				var item = VisualTreeHelper.GetChild(sender as DependencyObject, i);
				VisualTree(item);
			}
		}

		private void RefreshParams(object sender, RoutedEventArgs e)
		{
			FrameworkElement obj = sender as FrameworkElement;
			ContextMenu menu = obj.Parent as ContextMenu;
			VisualTree(menu.PlacementTarget);
			MessageBox.Show("Параметры успешно обновлены", "Уведомление",
			MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void SetNewPeriodStation(object sender, RoutedEventArgs e)
		{
			SetPeriodWindow wnd = new SetPeriodWindow(_currentStation);
			wnd.Owner = this;
			wnd.Show();
		}

		private void SetNewMainStatusStation(object sender, RoutedEventArgs e)
		{
			SetMainStationWindow wnd = new SetMainStationWindow(_currentStation);
			wnd.Owner = this;
			wnd.Show();
		}

		private void SetNewSynhronizationStation(object sender, RoutedEventArgs e)
		{
			SetSynhronizationStation wnd = new SetSynhronizationStation(_currentStation);
			wnd.Owner = this;
			wnd.Show();
		}
	}
}
