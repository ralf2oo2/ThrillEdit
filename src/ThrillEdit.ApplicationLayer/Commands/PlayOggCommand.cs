using libZPlay;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ThrillEdit.ApplicationLayer.ViewModels;
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.ApplicationLayer.Commands
{
    class PlayOggCommand : AsyncCommandBase
    {
        private readonly VorbisEdit _vorbisEdit;
        private readonly ZPlay _zPlay;
        public PlayOggCommand(VorbisEdit vorbisEdit, ZPlay zPlay)
        {
            _vorbisEdit = vorbisEdit;
            _zPlay = zPlay;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            VorbisData vorbisData = (VorbisData)parameter;
            byte[] songData = await _vorbisEdit.GetVorbisBytesAsync(vorbisData);
            _zPlay.Close();
            _zPlay.OpenStream(true, true, ref songData, (uint)songData.Length, TStreamFormat.sfOgg);
            _zPlay.StartPlayback();
        }
    }
}
