using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrillEdit.ApplicationLayer.ViewModels;
using ThrillEdit.BusinessLayer;

namespace ThrillEdit.ApplicationLayer
{
    public class ViewModelSelector
    {
        private readonly VorbisEdit _vorbisEdit;
        private readonly ProgressBar _progressBar;
        public ViewModelSelector(VorbisEdit vorbisEdit, ProgressBar progressBar)
        {
            _vorbisEdit = vorbisEdit;
            _progressBar = progressBar;
        }

        public ViewModelBase GetViewModel(string filePath)
        {
            if(_vorbisEdit.CheckForVorbisData(filePath, 5242880))
            {
                return new MusicPlayerViewModel(_vorbisEdit, filePath, _progressBar);
            }
            return new UnsupportedViewModel();
        }
    }
}
