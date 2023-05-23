using System;
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
        private readonly VorbisEdit _vorbisEdit;
        private readonly ItemProvider _itemProvider;

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


        public MainWindow(VorbisEdit vorbisEdit, ItemProvider itemProvider) 
        {
            _vorbisEdit = vorbisEdit;
            _itemProvider = itemProvider;
            InitializeComponent();
            DataContext = this;

            _fileTreeItems = _itemProvider.GetItems(@"D:\\SteamLibrary\\steamapps\\common\\Thrillville Off the Rails", new string[] { "zap"});    
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnItemMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            // TODO: Check which view to show
            Button button = (Button)sender;
            Debug.WriteLine(button.Tag);

            CurrentViewModel = new MusicReplacerViewModel(_vorbisEdit, button.Tag.ToString());
        }
    }
}
