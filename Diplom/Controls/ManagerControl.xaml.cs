using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diplom.Models
{
    /// <summary>
    /// Interaction logic for ManagerControl.xaml
    /// </summary>
    public partial class ManagerControl : UserControl, IFocusable
    {
        private static Uri ImageUri { get; } = new Uri("pack://application:,,,/Resources/Canvas/pdh_manager.png");
		private static int numberManager = 0;
		public DataManagers Data;

        public ManagerControl(WorkWindow window, string name, int number)
        {
            InitializeComponent();
            image.Source = new BitmapImage(ImageUri);
            BorderThickness = new Thickness(2);
			Data = new DataManagers();

			if (name == "")
				managerName.Text = "Безымянный " + (++numberManager).ToString();
			else
				managerName.Text = name;

			Data.Name = managerName.Text;
			Data.Number = number;

			DataNetwork.Managers.Add(this);

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
                data.SetData("Manager", this);
                data.SetData("shiftX", e.GetPosition(this).X);
                data.SetData("shiftY", e.GetPosition(this).Y);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }
    }
}
