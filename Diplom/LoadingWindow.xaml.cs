using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Diplom
{
    public partial class LoadingWindow : Window
    {
        private double seconds;
        public LoadingWindow(string name, double seconds)
        {
            InitializeComponent();

            loadingName.Text = name;
            this.seconds = seconds;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker { WorkerReportsProgress = true };
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep((int)(seconds*10));
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            loadingBar.Value = e.ProgressPercentage;
        }

        private void loadingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loadingBar.Value >= 100)
                Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            throw new Exception();
        }
    }
}
