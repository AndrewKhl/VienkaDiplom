using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    /// <summary>
    /// Interaction logic for StationControl.xaml
    /// </summary>
    public partial class StationControl : UserControl, IFocusable
    {
        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_relay.png");
		private static int numberStation = 0;
		public string NameStation { get; set; }


        public StationControl(WorkWindow window, string name)
        {
            InitializeComponent();
            image.Source = new BitmapImage(ImageUri);
            BorderThickness = new Thickness(2);

			if (name == "")
				NameStation = "Безымянная " + (++numberStation).ToString();
			else
				NameStation = name;
			stationName.Text = NameStation;

            this.window = window;
        }

        public WorkWindow window { get; }
        public event Action FocusedElement;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetFocusBorder();
            window.SetFocus(this);
            e.Handled = true;
        }

        public void SetFocusBorder()
        {
            BorderBrush = new SolidColorBrush(Colors.White);
            FocusedElement?.Invoke();
        }

        public void UnsetFocusBorder()
        {
            BorderBrush = new SolidColorBrush(Colors.Black);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("Station", this);
                data.SetData("shiftX", e.GetPosition(this).X);
                data.SetData("shiftY", e.GetPosition(this).Y);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

		private void ShowParametrWindow(object sender, RoutedEventArgs e)
		{
			ParamsWindow wnd = new ParamsWindow();
			wnd.Owner = Stock.workWindow;
			wnd.Show();
		}
	}
}
