using Barotrauma;
using Barotrauma.Items.Components;
using System.Globalization;

namespace BarotraumaRadio.ServerSource
{
    public class Radio : Powered
    {
        private const int INPUT_COUNT = 3;

        private bool radioEnabled = false;

        private bool RadioEnabled 
        {
            set 
            {
                if (radioEnabled == value || !HasPower)
                {
                    return;
                }
                radioEnabled = value;
            }
        }

        protected readonly Character[] signalSender;

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
            }
        }
    }
}
