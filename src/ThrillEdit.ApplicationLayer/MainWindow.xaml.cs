﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Models;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.ComponentModel;
using ThrillEdit.BusinessLayer.Providers;
using ThrillEdit.ApplicationLayer.ViewModels;

namespace ThrillEdit.ApplicationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ViewModelSelector _viewModelSelector;
        private readonly ItemProvider _itemProvider;
        private readonly ProgressBar _progressBar;
        private readonly ApplicationSettings _applicationSettings;

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; OnPropertyChanged(); }
        }


        private List<Item> _fileTreeItems = new List<Item>();

        public List<Item> FileTreeItems
        {
            get { return _fileTreeItems; }
            set { _fileTreeItems = value; OnPropertyChanged(); }
        }

        private string _currentFile;

        public string CurrentFile
        {
            get { return _currentFile; }
            set { _currentFile = value; OnPropertyChanged(); }
        }

        public ProgressBar ProgressBar => _progressBar;

        public MainWindow(ViewModelSelector viewModelSelector, ItemProvider itemProvider, ProgressBar progressBar, ApplicationSettings applicationSettings) 
        {
            _viewModelSelector = viewModelSelector;
            _itemProvider = itemProvider;
            _progressBar = progressBar;
            _applicationSettings = applicationSettings;
            InitializeComponent();
            DataContext = this;
            Settings settings = _applicationSettings.GetApplicationSettings();
            _fileTreeItems = _itemProvider.GetItems(settings.GameDirectory, new string[] { "zap" });    
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnItemMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (_progressBar.DisableWindow) return;
            Button button = (Button)sender;
            Debug.WriteLine(button.Tag);

            CurrentFile = button.Tag.ToString();
            if(CurrentViewModel != null)
            {
                CurrentViewModel.Cleanup();
            }
            CurrentViewModel = _viewModelSelector.GetViewModel(button.Tag.ToString());
        }
    }
}
