using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barotrauma;
using Barotrauma.Sounds;
using OpenAL;
using static Barotrauma.PetBehavior.ItemProduction;

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
            ALFormat = Al.FormatStereo16;
            bufferPlayer.Play(stationUrl);
        }

        public override int FillStreamBuffer(int samplePos, short[] buffer)
        {
            
            int result = bufferPlayer.ReadPcmData(buffer, 88200 * 4);
            return result;
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
