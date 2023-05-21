using libZPlay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThrillEdit.ApplicationLayer.Commands;
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.ApplicationLayer.ViewModels
{
    class MusicReplacerViewModel : ViewModelBase
    {
        private readonly VorbisEdit _vorbisEdit;
        private readonly ZPlay _zPlay;

        private List<VorbisData> _vorbisData;

        public List<VorbisData> VorbisData
        {
            get { return _vorbisData; }
            set { _vorbisData = value; OnPropertyChanged(); }
        }

        public ICommand PlayOggCommand { get; set; }

        public MusicReplacerViewModel(VorbisEdit vorbisEdit, ZPlay zPlay, string filePath)
        {
            _vorbisEdit = vorbisEdit;
            _zPlay = zPlay;
            PlayOggCommand = new PlayOggCommand(_vorbisEdit, _zPlay);
            VorbisData = _vorbisEdit.ExtractVorbisData(filePath, 5242880);
        }
    }
}
