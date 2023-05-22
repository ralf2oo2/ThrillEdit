using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using NAudio.CoreAudioApi;
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
        private readonly AudioPlayer _audioPlayer;
        public PlayOggCommand(VorbisEdit vorbisEdit, AudioPlayer audioPlayer)
        {
            _vorbisEdit = vorbisEdit;
            _audioPlayer = audioPlayer;
        }

        protected async override Task ExecuteAsync(object parameter)
        {
            VorbisData vorbisData = (VorbisData)parameter;
            _audioPlayer.Play(vorbisData);
        }
    }
}
