
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TShockAPI;

namespace Jist.Next.Plugin.Lib 
{
    /// <summary>
    /// Provides JavaScript API for commands with cooldowns.
    /// </summary>
    [Module("cooldown"), TypeDeclaration("Jist.Next.Plugin.Lib.cooldown.d.ts", "cooldown.d.ts")]
    public static class Cooldown {
        static readonly List<(TSPlayer player, string key, DateTime timeout)> cooldownDictionary = new List<(TSPlayer player, string key, DateTime timeout)>();

        static System.Timers.Timer cooldownEvictTimer = new System.Timers.Timer() {
            Enabled = true,
            Interval = 1000,
        };

        static Cooldown()
        {
            cooldownEvictTimer.Elapsed += Evict;
        }

        static void Evict(object sender, ElapsedEventArgs e)
        {
            cooldownDictionary.RemoveAll(i => i.timeout < DateTime.Now);
        }

        public static void setCooldown(string key, TSPlayer player, int durationSeconds)
        {
            var entry = cooldownDictionary.FirstOrDefault(i => i.player == player && i.key == key);
            if (entry.Equals(default(ValueTuple<TSPlayer, string, DateTime>)))
            {
                cooldownDictionary.Add((player, key, DateTime.Now.AddSeconds(durationSeconds)));
            }

            entry.timeout = DateTime.Now.AddSeconds(durationSeconds);
        }
        
        public static void removeCooldown(string key, TSPlayer player)
        {
            setCooldown(key, player, 0);
        }

        public static int checkCooldown(string key, TSPlayer player)
        {
            var entry = cooldownDictionary.FirstOrDefault(i => i.player == player && i.key == key);
            if (entry.Equals(default(ValueTuple<TSPlayer, string, DateTime>)))
            {
                return 0;
            }

            return (int)entry.timeout.Subtract(DateTime.Now).TotalSeconds;
        }

    }
}