using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Barotrauma;

namespace BarotraumaRadio
{
    public partial class Plugin : IAssemblyPlugin
    {
        public void Initialize()
        {
        }

        public void OnLoadCompleted()
        {
            // After all plugins have loaded
            // Put code that interacts with other plugins here.
        }

        public void PreInitPatching()
        {
            // Not yet supported: Called during the Barotrauma startup phase before vanilla content is loaded.
        }

        public void Dispose()
        {
            // Cleanup your plugin!
            throw new NotImplementedException();
        }
    }
}
