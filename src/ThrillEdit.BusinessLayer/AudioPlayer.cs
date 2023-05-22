using NAudio.Utils;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.BusinessLayer
{
    //TODO : Rewrite entire class
    public class AudioPlayer : INotifyPropertyChanged
    {
        private readonly VorbisEdit _vorbisEdit;
        private bool _isPlaying = false;
        private bool _stopPlayer = false;
        private bool _setTime = false;
        private bool _changingTime = false;
        private TimeSpan _timeToAppend;

        private TimeSpan _songProgress;

        public event PropertyChangedEventHandler? PropertyChanged;

        private VorbisData _currentSong = new VorbisData();

        public VorbisData CurrentSong
        {
            get { return _currentSong; }
            set { _currentSong = value; OnPropertyChanged(); }
        }


        public TimeSpan SongProgress
        {
            get { return _songProgress; }
            set 
            {  
                _timeToAppend = value;
                if(!_changingTime)
                {
                    _changingTime = true;
                    Task.Run(() => 
                    {
                        Task.Delay(1000);
                        _setTime = true;
                        _changingTime = false;
                    });
                }
            }
        }

        public AudioPlayer(VorbisEdit vorbisEdit)
        {
            _vorbisEdit = vorbisEdit;
        }

        public void Stop()
        {
            _stopPlayer = true;
        }
        public bool IsPlaying()
        {
            return _isPlaying;
        }
        public void Play(VorbisData vorbisData)
        {
            byte[] songData = _vorbisEdit.GetVorbisBytes(vorbisData);
            if (IsPlaying())
            {
                Stop();
            }
            while (IsPlaying())
            {

            }
            CurrentSong = vorbisData;
            Task.Run(() => PlayOggFromByteArrayAsync(songData));
        }
        private async Task PlayOggFromByteArrayAsync(byte[] oggData)
        {
            using (var memoryStream = new MemoryStream(oggData))
            using (var vorbisStream = new VorbisWaveReader(memoryStream))
            using (var waveOut = new WaveOutEvent())
            {
                waveOut.Init(vorbisStream);
                waveOut.Play();
                _isPlaying = true;
                while (waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        if (_stopPlayer)
                        {
                            waveOut.Stop();
                            CurrentSong = new VorbisData();
                            _stopPlayer = false;
                        }
                        if(_setTime)
                        {
                            vorbisStream.CurrentTime = _timeToAppend;
                            _setTime = false;
                        }
                        else
                        {
                            _songProgress = waveOut.GetPositionTimeSpan();
                            OnPropertyChanged(nameof(SongProgress));
                        }
                    }
                }
                waveOut.Dispose();
                vorbisStream.Dispose();
                memoryStream.Dispose();
                _isPlaying = false;
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
