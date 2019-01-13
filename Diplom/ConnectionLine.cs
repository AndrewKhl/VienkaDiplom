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
		private Canvas canvas;
		public Line line = new Line();
        private bool hasErrors = false;
        public bool HasErrors
        {
            get => hasErrors;
            set
            {
                hasErrors = value;
                if (value)
                    line.Stroke = new SolidColorBrush(Colors.Red);
                else
                    line.Stroke = new SolidColorBrush(Colors.Green);
                if (firstControl is StationControl && secondControl is StationControl)
                {
                    (firstControl as StationControl).UpdateLook();
                    (secondControl as StationControl).UpdateLook();
                }
            }
        }

		public ConnectionLine(UserControl control1, UserControl control2, Canvas canvas)
		{
			firstControl = control1;
			secondControl = control2;
			this.canvas = canvas;
			if (firstControl is ManagerControl || secondControl is ManagerControl)
				line.Stroke = new SolidColorBrush(Colors.Blue);
			else
				line.Stroke = new SolidColorBrush(Colors.Green);

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

        public StationControl GetAnotherStation(StationControl station)
        {
            if (firstControl is StationControl && secondControl is StationControl)
                return (firstControl == station ? secondControl : firstControl) as StationControl;
            else
                return null;
        }

        public StationControl GetConnectedStation(ManagerControl manager)
        {
            if ((firstControl is StationControl && secondControl is ManagerControl)
                || (firstControl is ManagerControl && secondControl is StationControl))
                return (firstControl == manager ? secondControl : firstControl) as StationControl;
            else
                return null;
        }
        
        public ManagerControl GetConnectedManager(StationControl station)
        {
            if ((firstControl is StationControl && secondControl is ManagerControl)
                || (firstControl is ManagerControl && secondControl is StationControl))
                return (firstControl == station ? secondControl : firstControl) as ManagerControl;
            else
                return null;
        }
        
        public void ClearControls()
        {
            if (firstControl is StationControl && secondControl is StationControl)
            {
                (firstControl as StationControl).stationLine = null;
                (secondControl as StationControl).stationLine = null;
            }
            else if (firstControl is StationControl && secondControl is ManagerControl)
            {
                (firstControl as StationControl).managerLine = null;
                (secondControl as ManagerControl).line = null;
            }
            else
            {
                (secondControl as StationControl).managerLine = null;
                (firstControl as ManagerControl).line = null;
            }
        }
    }
}