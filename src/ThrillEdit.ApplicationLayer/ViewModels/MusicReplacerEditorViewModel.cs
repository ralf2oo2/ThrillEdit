using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThrillEdit.ApplicationLayer.Commands;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.ApplicationLayer.ViewModels
{
    public class MusicReplacerEditorViewModel : ViewModelBase
    {
		private VorbisData _selectedVorbisData;

		public VorbisData SelectedVorbisData
		{
			get { return _selectedVorbisData; }
			set { _selectedVorbisData = value; OnPropertyChanged(); }
		}

		private VorbisData _replacementVorbisData;

		public VorbisData ReplacementVorbisData
		{
			get { return _replacementVorbisData; }
			set { _replacementVorbisData = value; OnPropertyChanged(); }
		}

		public ICommand ReplaceSongCommand { get; set; }

		public MusicReplacerEditorViewModel(VorbisData selectedVorbisData)
		{
			SelectedVorbisData = selectedVorbisData;
			ReplaceSongCommand = new RelayCommand(ReplaceSong, CanReplaceSong);
		}

		private void ReplaceSong(object p)
		{

		}
        private bool CanReplaceSong(object p)
        {
			return true;
        }
    }
}
