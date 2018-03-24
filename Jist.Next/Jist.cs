using System.IO;
using System.IO.Compression;
using Jint;
using Jint.CommonJS;

namespace Jist.Next
{
    public class Jist
    {
        public Engine Engine { get; internal set; } = new Jint.Engine(o =>
        {
        });

        internal ModuleLoadingEngine ModuleLoadingEngine { get; set; }

        public string ScriptsDirectory { get; internal set; }

        public Jist(string scriptsDirectory)
        {
            ModuleLoadingEngine = Engine.CommonJS();
            ScriptsDirectory = scriptsDirectory;

            SetupScriptsDirectory();
            LoadTypeScript();
        }

        public void SetupScriptsDirectory()
        {
            var asm = typeof(Jist).Assembly;
            if (!Directory.Exists(ScriptsDirectory))
            {
                Directory.CreateDirectory(ScriptsDirectory);
            }

            if (!File.Exists(Path.Combine(ScriptsDirectory, "tsconfig.json")))
            {
                using (var sr = new StreamReader(asm.GetManifestResourceStream("Jist.Next.Scripts.tsconfig.json")))
                {
                    File.WriteAllText(Path.Combine(ScriptsDirectory, "tsconfig.json"), sr.ReadToEnd());
                }
            }
        }

        public void LoadTypeScript()
        {
            var asm = typeof(Jist).Assembly;

            using (var compressionStream = new GZipStream(asm.GetManifestResourceStream("Jist.Next.Scripts.typescript.js.gz"), CompressionMode.Decompress))
            using (var sr = new StreamReader(compressionStream))
            {
                Engine.Execute(sr.ReadToEnd());
            }
        }
    }
}