using Diplom.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Diplom
{
    public partial class ParamsWindow : Window
    {
		private StationControl _currentStation;

        public ParamsWindow(StationControl station)
        {
			_currentStation = station;
            InitializeComponent();
			periodStationTextBlock.Text = _currentStation.Data.Period.ToString() + " МГц";
			DataContext = _currentStation.Data;
			StateStation.Visibility = Visibility.Visible;
        }

		public void VisualTree(object sender, Visibility visible)
		{
			if (sender is FrameworkElement obj)
			{
				int count = VisualTreeHelper.GetChildrenCount(obj);

				if (sender as TextBlock != null && (sender as TextBlock).Tag?.ToString() == "changedElement")
					obj.Visibility = visible;

				for (int i = 0; i < count; ++i)
				{
					var item = VisualTreeHelper.GetChild(sender as DependencyObject, i);
					VisualTree(item, visible);
				}
			}
		}

		private void RefreshParams(object sender, RoutedEventArgs e)
		{
			FrameworkElement obj = sender as FrameworkElement;
			ContextMenu menu = obj.Parent as ContextMenu;

			if (_currentStation.Data.State == "включено")
				VisualTree(menu.PlacementTarget, Visibility.Visible);
			else
			{
				MessageBox.Show("Включите станцию", "Ошибка",
				MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (_currentStation.Data.firstRefreshStation == DateTime.MinValue)
			{
				UASexit.Text = "0";
				timeCalculated.Text = "0";
				_currentStation.Data.firstRefreshStation = DateTime.Now;
			}
			else
			{
				TimeSpan period = DateTime.Now - _currentStation.Data.firstRefreshStation;

				UASexit.Text = (period.Hours * 3600 + period.Minutes * 60 + period.Seconds).ToString();
				timeCalculated.Text = (period.Hours * 3600 + period.Minutes * 60 + period.Seconds).ToString();
			}

			MessageBox.Show("Параметры успешно обновлены", "Уведомление",
                MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void SetNewPeriodStation(object sender, RoutedEventArgs e)
		{
            SetPeriodWindow wnd = new SetPeriodWindow(_currentStation) { Owner = this };
            wnd.ShowDialog();
		}

		private void SetNewMainStatusStation(object sender, RoutedEventArgs e)
		{
            SetMainStationWindow wnd = new SetMainStationWindow(_currentStation) { Owner = this };
            wnd.ShowDialog();
		}

		private void SetNewSynhronizationStation(object sender, RoutedEventArgs e)
		{
            SetSynhronizationStation wnd = new SetSynhronizationStation(_currentStation) { Owner = this };
            wnd.ShowDialog();
		}

		private void SetNewStateStation(object sender, RoutedEventArgs e)
		{
            OnOffStation wnd = new OnOffStation(_currentStation) { Owner = this };
            wnd.ShowDialog();
		}
    }
}
