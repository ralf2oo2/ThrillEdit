using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThrillEdit.ApplicationLayer
{
    public class ProgressBar : INotifyPropertyChanged
    {
		private int _progressBarPercentage;

		public int ProgressBarPercentage
		{
			get { return _progressBarPercentage; }
			set { _progressBarPercentage = value; OnPropertyChanged(); }
		}

        private bool _disableWindow;

        public bool DisableWindow
        {
            get { return _disableWindow; }
            set { _disableWindow = value; }
        }


        public void SubscribeToProgress(Progress<(long current, long total)> progress)
        {
            progress.ProgressChanged += ProgressUpdate;
        }

        private void ProgressUpdate(object sender, (long current, long total) progress)
        {
            decimal pr = ((decimal)progress.current / (decimal)progress.total) * 100;
            ProgressBarPercentage = (int)Math.Ceiling(pr);
            Debug.Write("Progress: ");
            Debug.WriteLine(pr);
        }

		public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
