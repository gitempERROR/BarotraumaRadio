using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Barotrauma;
using Barotrauma.Items.Components;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace BarotraumaRadio
{
    public partial class Plugin : IAssemblyPlugin
    {
        public static Plugin Instance;

        public void Initialize()
        {
            Instance = new Plugin();
#if CLIENT
            LuaCsSetup.PrintCsMessage("init client started");
            InitClient();
#elif SERVER
            LuaCsSetup.PrintCsMessage("init server started");
            InitServer();
#endif
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

        public void InjectComponent<T>(string name) where T : ItemComponent
        {
            Type type = typeof(T);
            string hookName = name.ToLower() + ".spawn";
            GameMain.LuaCs.Hook.Add(hookName, hookName,
                (object[] args) => {
                    Item item = (Item)args[2];
                    try
                    {
                        if (item.GetComponent<T>() == null)
                        {
                            ConstructorInfo constructor;
                            try
                            {
                                if (type != typeof(ItemComponent) && !type.IsSubclassOf(typeof(ItemComponent))) { return null; }
                                constructor = type.GetConstructor(new Type[] { typeof(Item), typeof(ContentXElement) });
                                if (constructor == null)
                                {
                                    DebugConsole.ThrowError(
                                        $"Could not find the constructor of the component \"{name}\" ({item.Prefab.ContentFile.Path})");
                                    return null;
                                }
                            }
                            catch (Exception e)
                            {
                                DebugConsole.ThrowError(
                                    $"Could not find the constructor of the component \"{name}\" ({item.Prefab.ContentFile.Path})", e);
                                return null;
                            }
                            ItemComponent ic = null;
                            try
                            {
                                object[] lobject = [item, new ContentXElement(null, new XElement(XName.Get(name)))];
                                object component = constructor.Invoke(lobject);
                                ic = (ItemComponent)component;
                            }
                            catch (TargetInvocationException e)
                            {
                                DebugConsole.ThrowError($"Error while loading component of the type {type}.", e.InnerException);
                                return null;
                            }
                            item.AddComponent(ic);

                        }
                    }
                    catch (Exception ex)
                    {
                        LuaCsSetup.PrintCsError(ex);
                    }
                    return null;
                });
        }

        public void Dispose()
        {
            // Cleanup your plugin!
            throw new NotImplementedException();
        }
    }
}
