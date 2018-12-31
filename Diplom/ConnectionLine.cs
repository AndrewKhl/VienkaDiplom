using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diplom
{
    public class ConnectionLine
    {
		public UserControl firstControl;
		public UserControl secondControl;
		Canvas canvas;
		SolidColorBrush brush;
		public Line line = new Line();

		public ConnectionLine(UserControl control1, UserControl control2, Canvas canvas, bool isManager = false)
		{
			firstControl = control1;
			secondControl = control2;
			this.canvas = canvas;
			if (isManager)
				brush = new SolidColorBrush(Colors.Blue);
			else
				brush = new SolidColorBrush(Colors.Green);

			line.Stroke = brush;
			line.StrokeThickness = 3;
			Canvas.SetLeft(line, 0);
			Canvas.SetTop(line, 0);
			UpdatePosition();
			canvas.Children.Insert(0, line);
		}

		public void UpdatePosition()
		{
			line.X1 = (double)firstControl.GetValue(Canvas.LeftProperty) + firstControl.ActualWidth / 2;
			line.Y1 = (double)firstControl.GetValue(Canvas.TopProperty) + firstControl.ActualHeight / 2;
			line.X2 = (double)secondControl.GetValue(Canvas.LeftProperty) + secondControl.ActualWidth / 2;
			line.Y2 = (double)secondControl.GetValue(Canvas.TopProperty) + secondControl.ActualHeight / 2;
		}
    }
}