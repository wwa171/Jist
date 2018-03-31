using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using Jint.Runtime.Interop;
using TShockAPI;
using Jint.CommonJS;

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

            if (PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
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
            Jist.ModuleLoadingEngine.ModuleCache.Clear();
            LoadInternalModules();
            Jist.ModuleLoadingEngine.RunMain(JistRoot);
        }

        protected void LoadInternalModules()
        {
            // Jist.ModuleLoadingEngine.RegisterInternalModule("tshock", new NamespaceReference(Jist.Engine, "TShockAPI"));
            Jist.ModuleLoadingEngine.RegisterInternalModule("tshock", typeof(TShock));
            Jist.ModuleLoadingEngine.RegisterInternalModule("tsplayer", typeof(TSPlayer));
            Jist.ModuleLoadingEngine.RegisterInternalModule("commands", typeof(Commands));

            WriteTypings(new ModuleAttribute("tshock", "Jist.Next.Plugin.Lib.tshock.d.ts"), typeof(TShockAPI.TShock));
            WriteTypings(new ModuleAttribute("otapi", "Jist.Next.Plugin.Lib.otapi.d.ts"), this.GetType());

            var types = from i in Assembly.GetExecutingAssembly().GetTypes()
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
                WriteTypings(module.attribute, module.type);
            }
        }

        public void WriteTypings(ModuleAttribute attribute, Type type)
        {
            var typingsPath = Path.Combine(TypingsRoot, $"{attribute.ModuleId}.d.ts");
            var stream = type.Assembly.GetManifestResourceStream(attribute.TypingsResourceId)
                ?? this.GetType().Assembly.GetManifestResourceStream(attribute.TypingsResourceId);

            if (stream == null)
            {
                stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes($@"
/* GENERATED BY JIST NEXT. DO NOT EDIT */

/*
   This file was generated for the {attribute.ModuleId} module
   that was registered from the {type.Assembly.FullName} assembly.

   The module's author did not provide a typings declaration file for this
   module, so IntelliSense is disabled for it.
*/
declare module '{attribute.ModuleId}' {{ let m: any; export = m; }}"));
            }

            using (var typingsStream = new StreamReader(stream))
            {
                File.WriteAllText(typingsPath, typingsStream.ReadToEnd());
            }
        }

        private void OnGameInitialize(EventArgs args)
        {
        }
    }
}
