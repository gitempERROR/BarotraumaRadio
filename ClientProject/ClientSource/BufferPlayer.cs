using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BarotraumaRadio.ClientSource
{
    public class BufferPlayer : IDisposable
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private static int _bufferSize = 88200 * 4;
        private readonly CircularBuffer _pcmBuffer = new(_bufferSize);
        private bool _disposed;

        public BufferPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            _mediaPlayer.SetAudioFormat("S16N", 44100, 2);
            _mediaPlayer.SetAudioCallbacks(PlayCallback, null, null, null, null);
        }

        public void LogPlayerState()
        {
            Console.WriteLine($"State: {_mediaPlayer.State}");
            Console.WriteLine($"IsPlaying: {_mediaPlayer.IsPlaying}");
            Console.WriteLine($"Media: {_mediaPlayer.Media?.Mrl}");
        }

        private void PlayCallback(IntPtr opaque, IntPtr samples, uint count, long pts)
        {
            int bytes = (int)count * 2 * 2;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(samples, buffer, 0, bytes);
            _pcmBuffer.Write(buffer);
        }

        public int ReadPcmData(short[] outputBuffer, int samplesRequested)
        {
            int bytesRequested = samplesRequested * 2;
            byte[] byteBuffer = new byte[bytesRequested];
            int bytesRead = _pcmBuffer.Read(byteBuffer, 0, bytesRequested);
            Buffer.BlockCopy(byteBuffer, 0, outputBuffer, 0, bytesRead);

            return bytesRead / 2;
        }

        public async void Play(string url)
        {
            string streamUrl = url;
            if (string.IsNullOrEmpty(streamUrl))
            {
                return;
            }

            using Media media = new Media(_libVLC, streamUrl, FromType.FromLocation);
            _mediaPlayer.Play(media);
        }

        public void Stop()
        {
            if (_mediaPlayer == null)
            {
                return;
            }
            _mediaPlayer.Stop();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
            _disposed = true;
        }
    }
}
