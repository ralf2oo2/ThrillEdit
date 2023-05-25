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
    public class AudioPlayer
    {
        public enum PlaybackStopTypes
        {
            PlaybackStoppedByUser, PlaybackStoppedReachingEndOfFile
        }

        public PlaybackStopTypes PlaybackStopType { get; set; }

        private MemoryStream _memoryStream;
        private VorbisWaveReader _vorbisWaveReader;
        private WaveChannel32 _waveChannel;
        private DirectSoundOut _output;

        public event Action PlaybackResumed;
        public event Action PlaybackStopped;
        public event Action PlaybackPaused;

        public AudioPlayer(byte[] songArray, float volume)
        {

            PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;

            _memoryStream = new MemoryStream(songArray);
            _vorbisWaveReader = new VorbisWaveReader(_memoryStream);


            _output = new DirectSoundOut(200);

            _output.PlaybackStopped += _output_PlaybackStopped;


            _waveChannel = new WaveChannel32(_vorbisWaveReader);
            _waveChannel.PadWithZeroes = false;


            _output.Init(_waveChannel);
        }

        public void Play(PlaybackState playbackState, double currentVolumeLevel)
        {
            if (playbackState == PlaybackState.Stopped || playbackState == PlaybackState.Paused)
            {

                _output.Play();

            }
            _waveChannel.Volume = (float)currentVolumeLevel;

            if (PlaybackResumed != null)
            {

                PlaybackResumed();

            }
        }

        private void _output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Dispose();

            if (PlaybackStopped != null)
            {
                PlaybackStopped();
            }
        }

        public void Stop()
        {
            if (_output != null)
            {
                _output.Stop();
            }
        }

        public void Pause()
        {
            if (_output != null)
            {
                _output.Pause();

                if (PlaybackPaused != null)
                { 
                    PlaybackPaused();
                }
            }
        }

        public void TogglePlayPause(double currentVolumeLevel)
        {
            if (_output != null)
            {
                if (_output.PlaybackState == PlaybackState.Playing)
                {
                    Pause();
                }
                else
                {
                    Play(_output.PlaybackState, currentVolumeLevel);
                }
            }
            else
            {
                Play(PlaybackState.Stopped, currentVolumeLevel);
            }
        }

        public void Dispose()
        {
            if (_output != null)
            {
                if (_output.PlaybackState == PlaybackState.Playing)
                {
                    _output.Stop();
                }
                _output.Dispose();
                _output = null;
            }

            if (_vorbisWaveReader != null)
            {
                _vorbisWaveReader.Dispose();
                _vorbisWaveReader = null;
            }

            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
                _memoryStream = null;
            }
            if(_waveChannel != null)
            {
                _waveChannel.Dispose();
                _waveChannel = null;
            }
        }

        public bool IsDisposed()
        {
            if(_vorbisWaveReader == null && _memoryStream == null && _output == null && _waveChannel == null)
            {
                return true;
            }
            return false;
        }

        public double GetLenghtInSeconds()
        {
            if (_vorbisWaveReader != null)
            {
                return _vorbisWaveReader.TotalTime.TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        public double GetPositionInSeconds()
        {
            if (_vorbisWaveReader != null)
            {
                return _vorbisWaveReader.CurrentTime.TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        public float GetVolume()
        {
            if (_waveChannel != null)
            {
                return _waveChannel.Volume;
            }
            return 1;
        }

        public void SetPosition(double value)
        {
            if (_vorbisWaveReader != null)
            {
                try
                {
                    _vorbisWaveReader.CurrentTime = TimeSpan.FromSeconds(value);
                }
                catch(Exception)
                {

                }
            }
        }

        public void SetVolume(float value)
        {
            if (_waveChannel != null)
            {
                _waveChannel.Volume = value;
            }
        }
    }
}
