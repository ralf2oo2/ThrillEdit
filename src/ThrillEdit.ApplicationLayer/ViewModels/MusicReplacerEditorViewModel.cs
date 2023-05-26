using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThrillEdit.ApplicationLayer.Commands;
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.ApplicationLayer.ViewModels
{
    public class MusicReplacerEditorViewModel : ViewModelBase
    {
		private readonly VorbisEdit _vorbisEdit;
        private readonly Action _reloadMethod;
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

		private VorbisData _originalVorbisData;

		public VorbisData OriginalVorbisData
		{
			get { return _originalVorbisData; }
			set { _originalVorbisData = value; }
		}


		private string _selectedPath;

		public string SelectedPath
		{
			get { return _selectedPath; }
			set { _selectedPath = value; OnPropertyChanged(); }
		}


		public ICommand ReplaceSongCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }

        public MusicReplacerEditorViewModel(VorbisEdit vorbisEdit, VorbisData selectedVorbisData, VorbisData originalVorbisData, Action reloadMethod)
		{
			_vorbisEdit = vorbisEdit;
			_reloadMethod = reloadMethod;
			SelectedVorbisData = selectedVorbisData;
			OriginalVorbisData = originalVorbisData;
			ReplaceSongCommand = new RelayCommand(ReplaceSong, CanReplaceSong);
			OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
		}

        private void OpenFile(object p)
        {
			var ofd = new OpenFileDialog();
			ofd.Multiselect = false;
			ofd.Filter = "Vorbis files (*.ogg) | *.ogg;";
			var result = ofd.ShowDialog();
			if(result == true)
			{
				SelectedPath = ofd.FileName;
                if (_vorbisEdit.CheckForVorbisData(SelectedPath, 5242880))
                {
                    VorbisData replacementVorbis = _vorbisEdit.ExtractVorbisData(SelectedPath, 5242880)[0];
                    replacementVorbis.SongName = ofd.SafeFileName.Remove(ofd.SafeFileName.Length - 4);
                    ReplacementVorbisData = replacementVorbis;
                }
            }
        }
        private bool CanOpenFile(object p)
        {
            return true;
        }

        private void ReplaceSong(object p)
		{
			DataReplacement dataReplacement = new DataReplacement { OriginalData = SelectedVorbisData, newData = ReplacementVorbisData };
			_vorbisEdit.ReplaceVorbisData(new List<DataReplacement> { dataReplacement }, SelectedVorbisData.Origin);
			_reloadMethod();
		}
        private bool CanReplaceSong(object p)
        {
			if(ReplacementVorbisData != null)
			{
                /*if(OriginalVorbisData.Size != 0)
				{
					if(ReplacementVorbisData.Size <= OriginalVorbisData.Size)
					{
						return true;
					}
				}
				else
				{
                    
                }*/

                if (ReplacementVorbisData.Size <= SelectedVorbisData.Size)

                {
                    return true;
                }
            }
			return false;
        }
    }
}
