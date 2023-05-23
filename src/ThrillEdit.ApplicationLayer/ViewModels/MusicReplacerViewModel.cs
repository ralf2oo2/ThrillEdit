using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ThrillEdit.ApplicationLayer.Commands;
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Models;
using ThrillEdit.BusinessLayer.Extentions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

namespace ThrillEdit.ApplicationLayer.ViewModels
{
    class MusicReplacerViewModel : ViewModelBase
    {
        private readonly VorbisEdit _vorbisEdit;
        private readonly DispatcherTimer _timer;
        private readonly ProgressBar _progressBar;
        private AudioPlayer _audioPlayer;

        private bool _startingSong = false;

        private string _title;
        private double _currentTrackLenght;
        private double _currentTrackPosition;
        private string _playPauseImageSource;
        private float _currentVolume;

        private bool _autoPlay = false;

        private ObservableCollection<VorbisData> _vorbisData;
        private VorbisData _currentlyPlayingVorbis;
        private VorbisData _currentlySelectedVorbis;

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string PlayPauseImageSource
        {
            get { return _playPauseImageSource; }
            set
            {
                if (value == _playPauseImageSource) return;
                _playPauseImageSource = value;
                OnPropertyChanged(nameof(PlayPauseImageSource));
            }
        }

        public float CurrentVolume
        {
            get { return _currentVolume; }
            set
            {
                if (value.Equals(_currentVolume)) return;
                _currentVolume = value;
                OnPropertyChanged(nameof(CurrentVolume));
            }
        }

        public double CurrentTrackLenght
        {
            get { return _currentTrackLenght; }
            set
            {
                if (value.Equals(_currentTrackLenght)) return;
                _currentTrackLenght = value;
                OnPropertyChanged(nameof(CurrentTrackLenght));
            }
        }

        public double CurrentTrackPosition
        {
            get { return _currentTrackPosition; }
            set
            {
                if (value.Equals(_currentTrackPosition)) return;
                _currentTrackPosition = value;
                OnPropertyChanged(nameof(CurrentTrackPosition));
            }
        }

        public VorbisData CurrentlySelectedVorbis
        {
            get { return _currentlySelectedVorbis; }
            set
            {
                if (Equals(value, _currentlySelectedVorbis)) return;
                _currentlySelectedVorbis = value;
                OnPropertyChanged(nameof(CurrentlySelectedVorbis));
            }
        }


        public VorbisData CurrentlyPlayingVorbis
        {
            get { return _currentlyPlayingVorbis; }
            set
            {
                if (Equals(value, _currentlyPlayingVorbis)) return;
                _currentlyPlayingVorbis = value;
                OnPropertyChanged(nameof(CurrentlySelectedVorbis));
            }
        }

        public bool AutoPlay
        {
            get { return _autoPlay; }
            set
            {
                if (Equals(value, _autoPlay)) return;
                _autoPlay = value;
                OnPropertyChanged(nameof(AutoPlay));
            }
        }


        public ObservableCollection<VorbisData> VorbisData
        {
            get { return _vorbisData; }
            set
            {
                if (Equals(value, _vorbisData)) return;
                _vorbisData = value;
                OnPropertyChanged(nameof(VorbisData));
            }
        }

        public ICommand PlayVorbisCommand { get; set; }
        public ICommand RewindToStartCommand { get; set; }
        public ICommand StartPlaybackCommand { get; set; }
        public ICommand StopPlaybackCommand { get; set; }
        public ICommand ForwardToEndCommand { get; set; }

        public ICommand TrackControlMouseDownCommand { get; set; }
        public ICommand TrackControlMouseUpCommand { get; set; }
        public ICommand VolumeControlValueChangedCommand { get; set; }

        private void LoadCommands()
        {
            // Menu commands

            //ExitApplicationCommand = new RelayCommand(ExitApplication, CanExitApplication);

            PlayVorbisCommand = new RelayCommand(PlayVorbis, CanPlayVorbis);

            // Player commands
            RewindToStartCommand = new RelayCommand(RewindToStart, CanRewindToStart);
            StartPlaybackCommand = new RelayCommand(StartPlayback, CanStartPlayback);
            StopPlaybackCommand = new RelayCommand(StopPlayback, CanStopPlayback);
            ForwardToEndCommand = new RelayCommand(ForwardToEnd, CanForwardToEnd);

            // Event commands
            TrackControlMouseDownCommand = new RelayCommand(TrackControlMouseDown, CanTrackControlMouseDown);
            TrackControlMouseUpCommand = new RelayCommand(TrackControlMouseUp, CanTrackControlMouseUp);
            VolumeControlValueChangedCommand = new RelayCommand(VolumeControlValueChanged, CanVolumeControlValueChanged);
        }

        private enum PlaybackState
        {
            Playing, Stopped, Paused
        }

        private PlaybackState _playbackState;

        public MusicReplacerViewModel(VorbisEdit vorbisEdit, string filePath, ProgressBar progressBar)
        {
            _vorbisEdit = vorbisEdit;
            _progressBar = progressBar;
            LoadCommands();
            VorbisData = _vorbisEdit.ExtractVorbisData(filePath, 5242880);

            _playbackState = PlaybackState.Stopped;
            _timer = new DispatcherTimer();
            _timer.Tick += UpdateProgressBar;
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Start();

            CurrentVolume = 1;
            PlayPauseImageSource = "../Images/play.png";
        }
        public void CloseWindow()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.Dispose();
            }
        }

        private void RewindToStart(object p)
        {
            _audioPlayer.SetPosition(0);
        }

        private bool CanRewindToStart(object p)
        {
            if (_playbackState == PlaybackState.Playing)
            {
                return true;
            }
            return false;
        }

        private void StartPlayback(object p)
        {
            if (CurrentlySelectedVorbis != null)
            {
                if (_playbackState == PlaybackState.Stopped)
                {
                    byte[] songData = _vorbisEdit.GetVorbisBytes(CurrentlySelectedVorbis);
                    _audioPlayer = new AudioPlayer(songData, CurrentVolume);

                    _audioPlayer.PlaybackStopType = AudioPlayer.PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                    _audioPlayer.PlaybackPaused += _audioPlayer_PlaybackPaused;
                    _audioPlayer.PlaybackResumed += _audioPlayer_PlaybackResumed;
                    _audioPlayer.PlaybackStopped += _audioPlayer_PlaybackStopped;
                    CurrentTrackLenght = _audioPlayer.GetLenghtInSeconds();
                    CurrentlyPlayingVorbis = CurrentlySelectedVorbis;
                }

                if (CurrentlySelectedVorbis == CurrentlyPlayingVorbis)
                {
                    _audioPlayer.TogglePlayPause(CurrentVolume);
                }
            }
        }

        private bool CanStartPlayback(object p)
        {
            if (CurrentlySelectedVorbis != null)
            {
                return true;
            }
            return false;
        }

        private void StopPlayback(object p)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.PlaybackStopType = AudioPlayer.PlaybackStopTypes.PlaybackStoppedByUser;
                _audioPlayer.Stop();
            }
        }

        private bool CanStopPlayback(object p)
        {
            if (_playbackState == PlaybackState.Playing || _playbackState == PlaybackState.Paused)
            {
                return true;
            }
            return false;
        }

        private void ForwardToEnd(object p)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.PlaybackStopType = AudioPlayer.PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                _audioPlayer.SetPosition(_audioPlayer.GetLenghtInSeconds());
            }
        }

        private bool CanForwardToEnd(object p)
        {
            if (_playbackState == PlaybackState.Playing)
            {
                return true;
            }
            return false;
        }

        private void TrackControlMouseDown(object p)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.Pause();
            }
        }

        private void TrackControlMouseUp(object p)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.SetPosition(CurrentTrackPosition);
                _audioPlayer.Play(NAudio.Wave.PlaybackState.Paused, CurrentVolume);
            }
        }

        private bool CanTrackControlMouseDown(object p)
        {
            if (_playbackState == PlaybackState.Playing)
            {
                return true;
            }
            return false;
        }

        private bool CanTrackControlMouseUp(object p)
        {
            if (_playbackState == PlaybackState.Paused)
            {
                return true;
            }
            return false;
        }

        private void VolumeControlValueChanged(object p)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.SetVolume(CurrentVolume); // set value of the slider to current volume
            }
        }

        private bool CanVolumeControlValueChanged(object p)
        {
            return true;
        }

        private void PlayVorbis(object p)
        {
            CurrentlySelectedVorbis = (VorbisData)p;
            _startingSong = true;
            Task.Run(() => StartPlayingVorbis());
        }
        private void StartPlayingVorbis()
        {
            if (_playbackState != PlaybackState.Stopped)
            {
                StopPlayback(null);
                while (_playbackState != PlaybackState.Stopped) ;
            }
            StartPlayback(null);
            _startingSong = false;
        }

        private bool CanPlayVorbis(object p)
        {
            return !_startingSong;
        }
        private void _audioPlayer_PlaybackStopped()
        {
            _playbackState = PlaybackState.Stopped;
            PlayPauseImageSource = "../Images/play.png";
            CommandManager.InvalidateRequerySuggested();
            CurrentTrackPosition = 0;

            if (_audioPlayer.PlaybackStopType == AudioPlayer.PlaybackStopTypes.PlaybackStoppedReachingEndOfFile && AutoPlay)
            {
                CurrentlySelectedVorbis = VorbisData.NextItem(CurrentlyPlayingVorbis);
                StartPlayback(null);
            }
        }


        private void _audioPlayer_PlaybackResumed()
        {
            _playbackState = PlaybackState.Playing;
            PlayPauseImageSource = "../Images/pause.png";
        }
        private void UpdateProgressBar(object sender, EventArgs e)
        {
            if(_playbackState == PlaybackState.Playing)
            {
                CurrentTrackPosition = _audioPlayer.GetPositionInSeconds();
            }
        }

        private void _audioPlayer_PlaybackPaused()
        {
            _playbackState = PlaybackState.Paused;
            PlayPauseImageSource = "../Images/play.png";
        }
    }
}
