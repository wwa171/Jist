using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jist.Next.Plugin.Lib
{
    [Module("timers"), TypeDeclaration("Jist.Next.Plugin.Lib.timers.d.ts", "timers.d.ts")]
    public class Timers
    {
        static Dictionary<Guid, CancellationTokenSource> timerTokens = new Dictionary<Guid, CancellationTokenSource>();

        /// <summary>
        /// Runs the callback after a set delay.
        /// </summary>
        public static string setTimeout(Action callback, int timeout)
        {
            var token = new CancellationTokenSource();
            var key = Guid.NewGuid();

            timerTokens.Add(key, token);
            Task.Delay(timeout).ContinueWith(t =>
            {
                try
                {
                    if (!t.IsCanceled)
                    {
                        callback();
                    }
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    if (timerTokens.ContainsKey(key))
                    {
                        timerTokens.Remove(key);
                    }
                }
            }, token.Token, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);

            return key.ToString();
        }

        internal static async Task setIntervalInternal(Action callback, int interval, CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                callback();
                await Task.Delay(interval, token).ConfigureAwait(false);
            }
        }

        public static string setInterval(Action callback, int interval)
        {
            var token = new CancellationTokenSource();
            var key = Guid.NewGuid();

            timerTokens.Add(key, token);

            setIntervalInternal(callback, interval, token.Token).ContinueWith(t =>
            {
                try
                {
                    if (!t.IsCanceled)
                    {
                        callback();
                    }
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    if (timerTokens.ContainsKey(key))
                    {
                        timerTokens.Remove(key);
                    }
                }
            }, token.Token, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);

            return key.ToString();
        }

        public static void clearTimeout(string id)
        {
            if (Guid.TryParse(id, out var guid) || !timerTokens.ContainsKey(guid))
            {
                throw new Exception($"Timer ID was not found.");
            }

            var ct = timerTokens[guid];

            ct.Cancel();
        }

        internal static void ClearAllTimers()
        {
            foreach (var timer in timerTokens)
            {
                timer.Value.Cancel();
            }
        }
    }
}