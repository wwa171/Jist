using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using Jint.Runtime.Interop;
using TShockAPI;
using Jint.CommonJS;
using System.Collections.Generic;

namespace Jist.Next.Plugin
{

    [ApiVersion(2, 1)]
    public class JistPlugin : TerrariaPlugin
    {
        public override string Author => "Tyler W";
        public override bool Enabled => true;
        public override Version Version => typeof(JistPlugin).Assembly.GetName().Version;
        public override string Name => "Jist Next";
        public override string Description => "Javascript interpreted scripting for TShock";

        public static JistPlugin Instance { get; internal set; }

        public static Jist Jist { get; set; }

        /// <summary>
        /// Gets the root directory of the scripts folder.
        /// </summary>
        public string JistRoot => Path.Combine(Environment.CurrentDirectory, "jist");

        protected string TypingsRoot => Path.Combine(JistRoot, "typings");

        public JistPlugin(Main game) : base(game)
        {
            Instance = this;
            /* Jist goes last. */
            Order = Int32.MaxValue;

            Jist = new Jist(JistRoot);

            var mainFile = Path.Combine(JistRoot, "index.ts");
            if (!File.Exists(mainFile))
            {
                using (var sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream("Jist.Next.Plugin.Resources.index.ts")))
                {
                    File.WriteAllText(mainFile, sr.ReadToEnd());
                }
            }

            TShockAPI.Commands.ChatCommands.Add(new Command("jist", OnJistCommand, "jist"));
            TShockAPI.Commands.ChatCommands.Add(new Command("jist.reload", OnJistReloadCommand, "jist-reload", "jist-rl"));
            TShockAPI.Commands.ChatCommands.Add(new Command("jist.modules", OnJistModulesCommand, "jist-modules"));
        }

        private void OnJistModulesCommand(CommandArgs args)
        {
            var modules = from i in Jist.ModuleLoadingEngine.ModuleCache
                          let moduleId = i.Key
                          let module = i.Value
                          let isInternal = i.Value is InternalModule
                          orderby isInternal descending
                          select new
                          {
                              moduleId,
                              children = module.Children.Count,
                              isInternal
                          };

            if (PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out var pageNumber))
            {
                PaginationTools.SendPage(args.Player, pageNumber, modules, modules.Count());
            }
        }

        private void OnJistReloadCommand(CommandArgs args)
        {
            Reload();
            args.Player.SendInfoMessage("Jist reloaded.");
        }

        private void OnJistCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage($"Jist v{Version}");
                return;
            }

            var subCommand = "jist-" + args.Parameters[0];
            args.Parameters.RemoveAt(0);
            Commands.HandleCommand(args.Player, $"{Commands.SilentSpecifier}{subCommand} {string.Join(" ", args.Parameters)}");
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnGameInitialize);
            Reload();
        }

        protected void Reload()
        {
            Lib.Jist.ClearHooks();
            Lib.Timers.ClearAllTimers();
            Jist.ModuleLoadingEngine.ModuleCache.Clear();
            LoadInternalModules();
            Jist.ModuleLoadingEngine.RunMain(JistRoot);
        }

        protected void LoadInternalModules()
        {
            // Jist.ModuleLoadingEngine.RegisterInternalModule("tshock", new NamespaceReference(Jist.Engine, "TShockAPI"));
            Jist.ModuleLoadingEngine.RegisterInternalModule("tshock", typeof(TShock));
            Jist.ModuleLoadingEngine.RegisterInternalModule("tsplayer", typeof(TSPlayer));

            WriteTypings("Jist.Next.Plugin.Lib.tshock.d.ts", "tshock.d.ts");
            WriteTypings("Jist.Next.Plugin.Lib.otapi.d.ts", "otapi.d.ts");

            var types = from asm in AppDomain.CurrentDomain.GetAssemblies()
                        from i in asm.GetTypes()
                        let attr = i.GetCustomAttribute(typeof(ModuleAttribute), true) as ModuleAttribute
                        where attr != null
                        select new
                        {
                            type = i,
                            attribute = attr,
                        };

            foreach (var module in types)
            {
                Jist.ModuleLoadingEngine.RegisterInternalModule(module.attribute.ModuleId, module.type);
            }

            foreach (var typingsAttr in GetTypingsAttributes())
            {
                // Console.WriteLine($"write-typings: Intellisense on {typingsAttr.Item2.ResourceId} in ${typingsAttr.Item1.Name}");
                WriteTypings(typingsAttr);
            }
        }

        public void WriteTypings(string resourceId, string fileName)
        {
            WriteTypings((this.GetType(), new TypeDeclarationAttribute(resourceId, fileName)));
        }

        public void WriteTypings(ValueTuple<Type, TypeDeclarationAttribute> attribute)
        {
            if (attribute.Item2 == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var typingsPath = Path.Combine(TypingsRoot, attribute.Item2.TypingsFileName);

            Stream stream = null;

            if (!string.IsNullOrEmpty(attribute.Item2.ResourceId))
            {
                stream = attribute.Item1.Assembly.GetManifestResourceStream(attribute.Item2.ResourceId);
            }

            //             if (stream == null)
            //             {
            //                 stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes($@"
            // /* GENERATED BY JIST NEXT. DO NOT EDIT */

            // /*
            //    This file was generated for a module registered by the
            //    {attribute.ResourceType.Assembly.FullName} assembly.

            //    The module's author did not provide a typings declaration
            //    file for this module, so IntelliSense is disabled for it.
            // */
            // declare module '{attribute.ModuleId}' {{ let m: any; export = m; }}"));
            //             }

            using (var typingsStream = new StreamReader(stream))
            {
                File.WriteAllText(typingsPath, typingsStream.ReadToEnd());
            }
        }

        /// <summary>
        /// Generator which yields all the possible typings attribute instances in the process.
        /// </summary>
        protected IEnumerable<(Type, TypeDeclarationAttribute)> GetTypingsAttributes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                {
                    TypeDeclarationAttribute attr = null;
                    if ((attr = type.GetCustomAttribute(typeof(TypeDeclarationAttribute)) as TypeDeclarationAttribute) != null)
                    {
                        yield return (type, attr);
                    }

                    foreach (var prop in type.GetProperties())
                    {
                        if ((attr = prop.GetCustomAttribute(typeof(TypeDeclarationAttribute)) as TypeDeclarationAttribute) != null)
                        {
                            yield return (type, attr);
                        }
                    }

                    foreach (var field in type.GetFields())
                    {
                        if ((attr = field.GetCustomAttribute(typeof(TypeDeclarationAttribute)) as TypeDeclarationAttribute) != null)
                        {
                            yield return (type, attr);
                        }
                    }
                }
        }

        private void OnGameInitialize(EventArgs args)
        {
        }
    }
}
