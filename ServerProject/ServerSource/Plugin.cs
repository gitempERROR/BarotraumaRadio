using Barotrauma;
using BarotraumaRadio.ServerSource;

namespace BarotraumaRadio
{
    public partial class Plugin : IAssemblyPlugin
    {
        public void InitServer()
        {
            InjectComponent<Radio>("Radio");
        }
    }
}
