using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    public partial class ManagerControl : UserControl, IFocusable
    {
        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_manager.png");
		public DataManagers Data;
        public ConnectionLine line;

        public WorkWindow workWindow { get; }
        public event Action FocusedElement;

        public int? Port { get; set; }
        public static int? LastPort { get; set; }

        public ManagerControl(WorkWindow window, string name, int number, Color color)
        {
            InitializeComponent();
            SetColor(color);
            image.Source = new BitmapImage(ImageUri);
            BorderThickness = new Thickness(2);
            Data = new DataManagers
            {
                Name = name,
                Number = number
            };
            SetVisibleName();
            UnsetFocusBorder();

            DataNetwork.Managers.Add(this);

            workWindow = window;
        }

        public void SetVisibleName() => managerName.Text = $"{Data.Name} [{Data.Number}]";

        public void SetColor(Color color) => (Resources["fontColor"] as SolidColorBrush).Color = color;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetFocusBorder();
            workWindow.SetFocus(this);
            e.Handled = true;
        }

        public void SetFocusBorder()
        {
            managerImageBorder.BorderBrush = new SolidColorBrush(Colors.White);
            managerNameBorder.BorderBrush = new SolidColorBrush(Colors.White);
            managerImageBorder.Background.Opacity = 0.5;
            managerNameBorder.Background.Opacity = 0.5;
            FocusedElement?.Invoke();
        }

        public void UnsetFocusBorder()
        {
            managerImageBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            managerNameBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            managerImageBorder.Background.Opacity = 0.3;
            managerNameBorder.Background.Opacity = 0.3;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("Manager", this);
                data.SetData("shiftX", e.GetPosition(this).X);
                data.SetData("shiftY", e.GetPosition(this).Y);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (line == null)
            {
                var item = sender as MenuItem;
                if (item.Name == "Com1MenuItem" || item.Name == "LocalCom1MenuItem")
                    LastPort = 1;
                else if (item.Name == "Com3MenuItem" || item.Name == "LocalCom3MenuItem")
                    LastPort = 3;

                workWindow.ConnectControls(this);
            }
            else
                workWindow.RemoveLocalConnection(this);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            LastPort = null;
            workWindow.CancelConnection();
        }

        private void Context_Click(object sender, MouseButtonEventArgs e)
        {
            string menu_type;
            switch (workWindow.connecting)
            {
                case ConnectingType.Manager: menu_type = "CancelMenu"; break;
                case ConnectingType.StationLocal: menu_type = "LocalMenu"; break;
                case ConnectingType.StationRadio: menu_type = "RadioMenu"; break;
                default: menu_type = line == null ? "MainMenu" : "ConnectedMenu"; break;
            }
            stackPanel.ContextMenu = Resources[menu_type] as ContextMenu;
        }

        private MenuItem GetMenuItem(string name, string menu = "MainMenu")
        {
            var mainMenu = Resources[menu] as ContextMenu;
            foreach (var item in mainMenu.Items)
                if (item is MenuItem && (item as MenuItem).Name == name)
                    return item as MenuItem;
            return null;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            GetMenuItem("NetworkMenuItem").Header = $"Сеть \"{DataNetwork.Name} ({DataNetwork.Type})\"";
            GetMenuItem("ManagerMenuItem").Header = $"Менеджер \"{Data.Name} ({Data.Number})\"";
        }

        public void ManagerProperties_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager wnd = new ConfigurationManager(this) { Owner = workWindow };
            wnd.ShowDialog();
        }

        private void ManagerRemove_Click(object sender, RoutedEventArgs e)
        {
            workWindow.SetFocus(this);
            workWindow.RemoveElement();
        }

        private void NetworkProperties_Click(object sender, RoutedEventArgs e) => workWindow.EditNetwork_Click(sender, e);

        private void NetworkRemove_Click(object sender, RoutedEventArgs e) => workWindow.RemoveNetwork_Click(sender, e);

        private void ContextMenu_Opened_1(object sender, RoutedEventArgs e)
        {
            if (line != null)
            {
                var station = (line.firstControl == this ? line.secondControl : line.firstControl) as StationControl;
                var item = GetMenuItem("LocalConnectedMenuItem", "LocalMenu");
                item.Visibility = Visibility.Visible;
                item.Header = $"Соединение по COM{Port} c \"{station.Data.Name} [{station.Data.Number}]\"";
                item.IsChecked = true;

                GetMenuItem("LocalCom1MenuItem", "LocalMenu").Visibility = Visibility.Collapsed;
                GetMenuItem("LocalCom3MenuItem", "LocalMenu").Visibility = Visibility.Collapsed;
            }
            else
            {
                GetMenuItem("LocalCom1MenuItem", "LocalMenu").Visibility = Visibility.Visible;
                GetMenuItem("LocalCom3MenuItem", "LocalMenu").Visibility = Visibility.Visible;

                GetMenuItem("LocalConnectedMenuItem", "LocalMenu").Visibility = Visibility.Collapsed;
            }
        }

        private void ContextMenu_Opened_2(object sender, RoutedEventArgs e)
        {
            GetMenuItem("NetworkConnectedMenuItem", "ConnectedMenu").Header = $"Сеть \"{DataNetwork.Name} ({DataNetwork.Type})\"";
            GetMenuItem("ManagerConnectedMenuItem", "ConnectedMenu").Header = $"Менеджер \"{Data.Name} ({Data.Number})\"";
            var station = (line.firstControl == this ? line.secondControl : line.firstControl) as StationControl;
            var item = GetMenuItem("ConnectedMenuItem", "ConnectedMenu");
            item.Header = $"Соединение по COM{Port} c \"{station.Data.Name} [{station.Data.Number}]\"";
            item.IsChecked = true;
        }

        public void Route_Click(object sender, RoutedEventArgs e)
        {
            var station = (line.firstControl == this ? line.secondControl : line.firstControl) as StationControl;
            station.Update_Click(sender, e);
        }
    }
}
