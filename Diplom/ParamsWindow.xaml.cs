using Diplom.Models;
using System;
using System.ComponentModel;
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
            VisibilityParams();
            PPUFreqNumberTextBlock.Text = _currentStation.Data.Period.ToString() + " МГц";
			DataContext = _currentStation.Data;
			StateStation.Visibility = Visibility.Visible;

            Closing += ParamsWindowClosing;
            Closed += ParamsWindowClosed;

            if (_currentStation.Data.errorType != ErrorType.None)
                HighlightErrors();
        }

        private void VisibilityParams()
        {
            if (_currentStation.Data.firstRefreshStation != DateTime.MinValue)
            {
                Resources["VisibilityParams"] = Visibility.Visible;
            }
        }

        private void HighlightErrors()
        {
            var red = Brushes.Red;

            switch (_currentStation.Data.errorType)
            {
                case ErrorType.Main:
                    PPUControlsTextBlock.Foreground = red;
                    PPUmodeTextBlock.Foreground = red;
                    PPUmodeNumberTextBlock.Foreground = red;
                    PPUtextBlock.Foreground = red;
                    break;

                case ErrorType.Synch:
                    MD1ControlsTextBlock.Foreground = red;
                    MD1SyncTextBlock.Foreground = red;
                    MD1SyncNumberTextBlock.Foreground = red;
                    MD1TextBlock.Foreground = red;
                    break;

                case ErrorType.Frequency:
                    PPUControlsTextBlock.Foreground = red;
                    PPUFreqTextBlock.Foreground = red;
                    PPUFreqNumberTextBlock.Foreground = red;
                    PPUtextBlock.Foreground = red;
                    break;
                
                default:
                    break;
            }
        }

        private void ClearErrors()
        {
            var black = Brushes.Black;

            PPUControlsTextBlock.Foreground = black;
            PPUFreqNumberTextBlock.Foreground = black;
            PPUFreqTextBlock.Foreground = black;
            PPUmodeNumberTextBlock.Foreground = black;
            PPUmodeTextBlock.Foreground = black;
            PPUtextBlock.Foreground = black;
            MD1ControlsTextBlock.Foreground = black;
            MD1SyncNumberTextBlock.Foreground = black;
            MD1SyncTextBlock.Foreground = black;
            MD1TextBlock.Foreground = black;
        }

        private void ParamsWindowClosing(object sender, CancelEventArgs e)
        {
            Owner.Topmost = true;
        }

        private void ParamsWindowClosed(object sender, EventArgs e)
        {
            Owner.Topmost = false;
        }

        private void OpenCloseTree(object item, bool state)
        {
            if (!(item is TreeViewItem node))
                return;
            node.IsExpanded = state;
            foreach (var child in node.Items)
                OpenCloseTree(child, state);
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

		async private void RefreshParams(object sender, RoutedEventArgs e)
		{
			FrameworkElement obj = sender as FrameworkElement;
			ContextMenu menu = obj.Parent as ContextMenu;

            if (_currentStation.Data.State == "включено")
            {
                //OpenCloseTree(menu.PlacementTarget, true);
                VisualTree(menu.PlacementTarget, Visibility.Visible);
            }
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
