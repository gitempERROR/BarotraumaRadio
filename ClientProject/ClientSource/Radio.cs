using Barotrauma;
using Barotrauma.Items.Components;
using Barotrauma.Sounds;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace BarotraumaRadio.ClientSource
{
    public class Radio : ItemComponent
    {
        private const int INPUT_COUNT = 3;

        private BufferSound radioSound;

        private SoundChannel radioChannel;

        private bool radioEnabled = false;

        private string[] radioArray =
        {
            "https://pool.anison.fm/AniSonFM(320)"
        };

        private int radioArrayIndex = 0;

        private bool RadioEnabled 
        {
            set 
            {
                if (radioEnabled == value)
                {
                    return;
                }
                radioEnabled = value;
                if (value)
                {
                    Play(radioArray[radioArrayIndex]);
                }
                else
                {
                    Stop();
                }
            }
        }

        protected readonly Character[] signalSender;

        public void Play(string stationUrl)
        {
            radioSound = new BufferSound(GameMain.SoundManager, "", true, true, stationUrl);
            radioChannel = SoundPlayer.PlaySound(sound: radioSound, new Vector2(GameMain.SoundManager.ListenerPosition.X, GameMain.SoundManager.ListenerPosition.Y), volume: 0.01f, 1, ignoreMuffling: true);
        }

        public void ChangeState(bool active)
        {
            RadioEnabled = active;
        }

        public Radio(Item item, ContentXElement element)
            :base(item, element)
        {
            signalSender = new Character[INPUT_COUNT];
        }

        public override void ReceiveSignal(Signal signal, Connection connection)
        {
            float value;
            float.TryParse(signal.value, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            switch (connection.Name)
            {
                case "set_state":
                {
                    ChangeState(value != 0f);
                    break;
                }
                case "switch_channel":
                {
                    break;
                }
                case "power_in":
                {
                    ChangeState(value != 0f);
                    break;
                }
            }
        }

        public void Stop()
        {
            radioSound.Dispose();
            radioChannel.Dispose();
        }
    }
}
