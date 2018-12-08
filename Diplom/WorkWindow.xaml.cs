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
        public WorkWindow()
        {
            InitializeComponent();
			ImageBrush myBrush = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resource/Background.jpg", UriKind.Absolute))
			};
			Background = myBrush;
		}
    }
}
