
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jist.Next.Plugin.Lib
{
    [Module("commands"), TypeDeclaration("Jist.Next.Plugin.Lib.commands.d.ts", "commands.d.ts")]
    public class Commands
    {
        static List<TShockAPI.Command> commands = new List<TShockAPI.Command>();

        public static void addCommand(dynamic data, TShockAPI.CommandDelegate callback)
        {
            string commandName = data.name;
            object[] permissions = null;
            bool removeExisting = true;
            int? cooldown = null;

            /*
             * NOTE
             *
             * This is garbage code for handling garbage dynamics.
             */

            try
            {
                permissions = data.permissions;
            }
            catch
            {
                permissions = new object[] {};
            }

            try
            {
                removeExisting = data.removeExisting;
            }
            catch
            {
            }

            try
            {
                cooldown = Convert.ToInt32(data.cooldown);
            }
            catch
            {
            }

            if (removeExisting && removeCommand(commandName) > 0)
            {
                TShockAPI.TShock.Log.Warn($"jist next: warning: Jist command overrides previously added command {TShockAPI.Commands.Specifier}{commandName}");
            }
            
            var command = new TShockAPI.Command(permissions.Select(i => i.ToString()).ToList(), callback, commandName);

            TShockAPI.Commands.ChatCommands.Add(command);
            commands.Add(command);
        }

        public static int removeCommand(string commandName)
        {
            return TShockAPI.Commands.ChatCommands.RemoveAll(i => i.Name == commandName || i.Names.Contains(commandName));
        }

        internal static void ClearCommands()
        {
            foreach (var command in commands)
            {
                TShockAPI.Commands.ChatCommands.Remove(command);
            }

            commands.Clear();
        }
    }
}