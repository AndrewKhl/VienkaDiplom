using Diplom.Models;
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

		public ConnectionLine(UserControl station1, UserControl station2, Canvas canvas, bool isManager = false)
		{
			firstControl = station1;
			secondControl = station2;
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
            if (firstControl is StationControl && secondControl is StationControl)
            {
                var first = firstControl as StationControl;
                var second = secondControl as StationControl;
                if ((double)first.GetValue(Canvas.LeftProperty) < (double)second.GetValue(Canvas.LeftProperty))
                {
                    first.RotateRight();
                    second.RotateLeft();
                }
                else
                {
                    first.RotateLeft();
                    second.RotateRight();
                }

                int shift = 3;

                var station = firstControl as StationControl;
                line.Y1 = (double)station.GetValue(Canvas.TopProperty) + station.stationNameBorder.ActualHeight
                    + station.indentText.ActualHeight + station.stationImageBorder.ActualHeight / 2;
                if ((station as StationControl).IsRightRotation)
                    line.X1 = (double)station.GetValue(Canvas.LeftProperty) + station.indentLabel.ActualWidth
                        + station.stationImageBorder.ActualWidth - shift;
                else
                    line.X1 = (double)station.GetValue(Canvas.LeftProperty) + station.indentLabel.ActualWidth + shift;


                station = secondControl as StationControl;
                line.Y2 = (double)station.GetValue(Canvas.TopProperty) + station.stationNameBorder.ActualHeight
                    + station.indentText.ActualHeight + station.stationImageBorder.ActualHeight / 2;
                if ((station as StationControl).IsRightRotation)
                    line.X2 = (double)station.GetValue(Canvas.LeftProperty) + station.indentLabel.ActualWidth
                        + station.stationImageBorder.ActualWidth - shift;
                else
                    line.X2 = (double)station.GetValue(Canvas.LeftProperty) + station.indentLabel.ActualWidth + shift;
            }
            else
            {
                line.X1 = (double)firstControl.GetValue(Canvas.LeftProperty) + firstControl.ActualWidth / 2;
                line.Y1 = (double)firstControl.GetValue(Canvas.TopProperty) + firstControl.ActualHeight / 2;
                line.X2 = (double)secondControl.GetValue(Canvas.LeftProperty) + secondControl.ActualWidth / 2;
                line.Y2 = (double)secondControl.GetValue(Canvas.TopProperty) + secondControl.ActualHeight / 2;
            }

        }
    }
}