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
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
        //private List<Manager> managers { get; set; } = new List<Manager>();
        //private List<Station> stations { get; set; } = new List<Station>();

        public WorkWindow()
        {
            InitializeComponent();
		}

        private void CreateNetwork_Click(object sender, RoutedEventArgs e)
        {
            var station = new StationControl();
            Canvas.SetLeft(station, 0);
            Canvas.SetTop(station, 0);
            canvas.Children.Add(station);
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
            e.Handled = true;
        }
    }
}
