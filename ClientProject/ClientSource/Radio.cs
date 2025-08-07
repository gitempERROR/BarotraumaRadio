using Barotrauma;
using Barotrauma.Items.Components;
using Barotrauma.Sounds;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace BarotraumaRadio.ClientSource
{
    public class Radio : ItemComponent, IDisposable
    {
        private const int INPUT_COUNT = 2;

        private BufferSound radioSound;

        private SoundChannel? radioChannel;

        private bool radioEnabled = false;

        private readonly string[] radioArray =
        {
            "https://pool.anison.fm/AniSonFM(320)",
            "http://radio.truckers.fm",
            "http://stream.radioparadise.com/flacm",
            "http://ice1.somafm.com/groovesalad-256-mp3",
            "http://kexp-mp3-128.streamguys1.com/kexp128.mp3",
            "http://stream.srg-ssr.ch/m/rsj/mp3_128",
            "http://media-ssl.musicradio.com/ClassicFM",
            "http://sc5.radiocaroline.net:8040/stream"
        };
        private readonly string[] radioNamesArray =
{
            "AniSonFM",
            "truckers.fm",
            "radioparadise.com",
            "somafm",
            "kexp",
            "srg-ssr",
            "ClassicFM",
            "radiocaroline"
        };

        private int radioArrayIndex = 0;

        private readonly int range = 900;

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
                    new Thread(delegate() { Play(radioArray[radioArrayIndex]); }).Start();
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
#if CLIENT
            try
            {
                GUI.AddMessage($"Now playing {radioNamesArray[radioArrayIndex]}", Color.Orange, new Vector2(Item.WorldPositionX, Item.WorldPositionY - 20), Vector2.Zero);
                radioSound = new BufferSound(GameMain.SoundManager, "RadioStream", true, true, stationUrl);
                
                radioChannel = SoundPlayer.PlaySound(sound: radioSound, item.WorldPosition, volume: 0.9f, ignoreMuffling: false, range: range);
                if (radioChannel != null)
                {
                    radioChannel.FilledByNetwork = true;
                    radioChannel.IsStream = true;
                    radioChannel.Looping = true;
                }
            }
            catch (Exception e)
            {
                LuaCsSetup.PrintCsError(e);
            }
#endif
        }

        public void SwitchChannel()
        {
            if (++radioArrayIndex == radioArray.Length)
            {
                radioArrayIndex = 0;
            }
            new Thread(delegate () { RestartRadio(); }).Start();
        }

        public void RestartRadio()
        {
            Thread.Sleep(100);
            if (radioChannel != null)
            {
                RadioEnabled = false;
                Thread.Sleep(100);
                RadioEnabled = true;
            }
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
                    SwitchChannel();
                    break;
                }
            }
        }

        public void Stop()
        {
#if CLIENT
            radioSound.Dispose();
            radioChannel?.Dispose();
#endif
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
