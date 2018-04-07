using System;
using System.Collections.Generic;
using Jint.Native;
using Jint.Native.Function;
using TerrariaApi.Server;
using TShockAPI;

namespace Jist.Next.Plugin.Lib
{
    [Module("jist", "Jist.Next.Plugin.Lib.jist.d.ts")]
    public static class Jist
    {
        static readonly ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();


        static Jist()
        {
            JistPlugin.Jist.Engine.Execute(@"
            function inspect(obj, depth) {
                if (!obj) return;

                var cons = obj.constructor
                    , name = cons.name
                    , proto = obj.__proto__
                    , depth = depth || 0
                    , indent = Array(depth + 1).join('  ');

                if (0 == depth) {
                    if ('function' == typeof obj) {
                    name = '[' + name + ': ' + obj.name + ']';
                    } else {
                    name = '[' + name + ']';
                    }
                }

                Console.WriteLine(indent + '\033[33m%s\033[0m', name);
                Object.keys(obj).sort().forEach(function(key){
                    var desc = Object.getOwnPropertyDescriptor(obj, key);
                    if (desc.get) Console.WriteLine(indent + '  \033[90m.{0}\033[0m', key);
                    if (desc.set) Console.WriteLine(indent + '  \033[90m.{0}=\033[0m', key);
                    if ('function' == typeof desc.value) {
                    var str = desc.value.toString();
                    var params = str.match(/^function *\((.*?)\)/)
                        , val = params
                        ? params[1].split(/ *, */).map(function(param){
                            return '\033[0m' + param + '\033[90m';
                        }).join(', ')
                        : '';
                    Console.WriteLine(indent + '  \033[90m.{0}({1})\033[0m', key, val);
                    } else if (undefined !== desc.value) {
                        Console.WriteLine(indent + '  \033[90m.{0} {1}\033[0m', key, desc.value);
                    }
                });

                inspect(proto, ++depth);
            };
");
        }

        static List<Action> unregisters = new List<Action>();

        public static Dictionary<string, Action<JsValue>> hookDictionary = new Dictionary<string, Action<JsValue>>()
        {
            {
                "GameInitialize",
                (cb) =>
                {
                    HookHandler<EventArgs> d = args => cb.Invoke(JsValue.Undefined, new JsValue[] {});
                    ServerApi.Hooks.GameInitialize.Register(Plugin.JistPlugin.Instance, d);
                    unregisters.Add(() => ServerApi.Hooks.GameInitialize.Deregister(Plugin.JistPlugin.Instance, d));
                }
            },
            {
                "GamePostInitialize",
                (cb) =>
                {
                    HookHandler<EventArgs> d = args => cb.Invoke(JsValue.Undefined, new JsValue[] {});
                    ServerApi.Hooks.GamePostInitialize.Register(Plugin.JistPlugin.Instance, d);
                    unregisters.Add(() => ServerApi.Hooks.GamePostInitialize.Deregister(Plugin.JistPlugin.Instance, d));
                }
            },
            {
                "ServerJoin",
                (cb) =>
                {
                    HookHandler<JoinEventArgs> d = args => cb.Invoke(JsValue.Undefined, new JsValue[] { JsValue.FromObject(Plugin.JistPlugin.Jist.Engine, args) });
                    ServerApi.Hooks.ServerJoin.Register(Plugin.JistPlugin.Instance, d);
                    unregisters.Add(() => ServerApi.Hooks.ServerJoin.Deregister(Plugin.JistPlugin.Instance, d));
                }
            },
            {
                "ServerLeave",
                (cb) =>
                {
                    HookHandler<LeaveEventArgs> d = args => cb.Invoke(JsValue.Undefined, new JsValue[] { JsValue.FromObject(Plugin.JistPlugin.Jist.Engine, args) });
                    ServerApi.Hooks.ServerLeave.Register(Plugin.JistPlugin.Instance, d);
                    unregisters.Add(() => ServerApi.Hooks.ServerLeave.Deregister(Plugin.JistPlugin.Instance, d));
                }
            },
            {
                "ServerChat",
                (cb) =>
                {
                    HookHandler<ServerChatEventArgs> d = args =>  cb.Invoke(JsValue.Undefined, new JsValue[] { JsValue.FromObject(Plugin.JistPlugin.Jist.Engine, args) });
                    ServerApi.Hooks.ServerChat.Register(Plugin.JistPlugin.Instance, d);
                    unregisters.Add(() => ServerApi.Hooks.ServerChat.Deregister(Plugin.JistPlugin.Instance, d));
                }
            },
            {
                "NpcSpawn",
                (cb) =>
                {
                    HookHandler<NpcSpawnEventArgs> d = args => cb.Invoke(JsValue.Undefined, new JsValue[] { JsValue.FromObject(Plugin.JistPlugin.Jist.Engine, args) });
                    ServerApi.Hooks.NpcSpawn.Register(Plugin.JistPlugin.Instance, d);
                    unregisters.Add(() => ServerApi.Hooks.NpcSpawn.Deregister(Plugin.JistPlugin.Instance, d));
                }
            },
        };

        internal static void ClearHooks()
        {
            foreach (var clearHandler in unregisters)
            {
                clearHandler();
            }

            unregisters.Clear();
        }

        public static void log(string message, params object[] format)
        {
            Console.WriteLine(message, format);
        }

        public static void on(string hook, JsValue d)
        {
            if (!hookDictionary.ContainsKey(hook))
            {
                throw new Exception($"{hook} is an unknown hook.");
            }

            var entry = hookDictionary[hook];
            entry(d);
        }

        public static void executeCommand(string command, TSPlayer player = null)
        {
            player = player ?? TSPlayer.Server;
            TShockAPI.Commands.HandleCommand(player, command);
        }

        public static int random(int from, int to)
        {
            return threadSafeRandom.Next(from, to);
        }

        public static int randomInclusive(int from, int to)
        {
            return threadSafeRandom.NextInclusive(from, to);
        }
    }
}