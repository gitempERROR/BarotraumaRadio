using System.Runtime.CompilerServices;
using Barotrauma;
using BarotraumaRadio.ClientSource;

namespace BarotraumaRadio
{
    public partial class Plugin : IAssemblyPlugin
    {
        public void InitClient()
        {
            InjectComponent<Radio>("Radio");
        }
    }
}
