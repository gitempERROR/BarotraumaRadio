using Barotrauma;
using LibVLCSharp.Shared;
using System.Reflection;
using System.Runtime.InteropServices;

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
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string vlcPath = Path.Combine(modPath, "../../../Content/libvlc/win-x64");

            // Проверка существования пути
            if (!Directory.Exists(vlcPath))
            {
                DebugConsole.Log($"ОШИБКА: Путь не найден: {vlcPath}");
            }
            else
            {
                DebugConsole.Log($"VLC путь: {vlcPath}");

                // Проверка наличия ключевых файлов
                DebugConsole.Log($"libvlc.dll exists: {File.Exists(Path.Combine(vlcPath, "libvlc.dll"))}");
                DebugConsole.Log($"plugins exists: {Directory.Exists(Path.Combine(vlcPath, "plugins"))}");

                // Инициализация
                Core.Initialize(vlcPath);
            }
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            _mediaPlayer.SetAudioFormat("S16N", 44100, 1);
            _mediaPlayer.SetAudioCallbacks(PlayCallback, null, null, null, null);
        }

        public void LogPlayerState()
        {
            DebugConsole.Log($"State: {_mediaPlayer.State}");
            DebugConsole.Log($"IsPlaying: {_mediaPlayer.IsPlaying}");
            DebugConsole.Log($"Media: {_mediaPlayer.Media?.Mrl}");
        }

        private void PlayCallback(IntPtr opaque, IntPtr samples, uint count, long pts)
        {
            int bytes = (int)count * 2;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(samples, buffer, 0, bytes);
            _pcmBuffer.Write(buffer);
        }

        public int ReadPcmData(short[] outputBuffer, int maxSamples)
        {
            int bytesRequested = maxSamples * 2; // 2 байта на сэмпл (16 бит)
            byte[] byteBuffer = new byte[bytesRequested];
            int bytesRead = _pcmBuffer.Read(byteBuffer, 0, bytesRequested);

            Buffer.BlockCopy(byteBuffer, 0, outputBuffer, 0, bytesRead);
            return bytesRead / 2; // Возвращаем количество прочитанных сэмплов
        }

        public async void Play(string url)
        {
            using Media media = new Media(_libVLC, url, FromType.FromLocation);

            // Добавьте параметры буферизации
            media.AddOption(":network-caching=500"); // 500 мс буферизации
            media.AddOption(":live-caching=500");

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
