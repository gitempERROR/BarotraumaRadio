using Barotrauma;
using Barotrauma.Sounds;
using OpenAL;

namespace BarotraumaRadio.ClientSource
{
    public class BufferSound : Sound
    {

        public override double? DurationSeconds => null;
        private readonly BufferPlayer bufferPlayer;

        public BufferSound(
            SoundManager owner,
            string filename,
            bool stream,
            bool streamsReliably,
            string stationUrl,
            ContentXElement xElement = null,
            bool getFullPath = true) : base(
                owner,
                filename,
                stream,
                streamsReliably,
                xElement)
        {
            bufferPlayer = new();
            SampleRate = 44100;
            ALFormat = Al.FormatMono16;
            bufferPlayer.Play(stationUrl);
        }

        public override int FillStreamBuffer(int samplePos, short[] buffer)
        {
            return bufferPlayer.ReadPcmData(buffer, buffer.Length);
        }

        public override float GetAmplitudeAtPlaybackPos(int playbackPos)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            if (disposed) { return; }

            base.Dispose();
            bufferPlayer.Dispose();
        }
    }
}
