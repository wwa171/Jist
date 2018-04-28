using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Jint;
using Jint.CommonJS;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Interop;

namespace Jist.Next
{
    public class Jist
    {
        public static Dictionary<int, string> transpileCache = new Dictionary<int, string>();

        public Engine Engine { get; internal set; } = new Jint.Engine(o =>
        {
        });

        public ModuleLoadingEngine ModuleLoadingEngine { get; internal set; }

        public string ScriptsDirectory { get; internal set; }

        public Jist(string scriptsDirectory)
        {
            ModuleLoadingEngine = Engine.CommonJS();
            ScriptsDirectory = scriptsDirectory;

            Engine.SetValue("Console", TypeReference.CreateTypeReference(Engine, typeof(Console)));

            SetupScriptsDirectory();
            LoadTypeScript();
        }

        private JsValue TranspileModule(string path, IModule module)
        {
            var ts = Engine.Global.Get("ts").AsObject();
            var transpileModule = ts.Get("transpileModule");
            var asm = typeof(Jist).Assembly;
            var sourceCode = File.ReadAllText(path);
            JsValue exports;

            var transpileConfig = new
            {
                compilerOptions = new
                {
                    target = "es5",
                    strict = true,
                },
                reportDiagnostics = true,
                moduleName = module.Id,
                fileName = Path.GetFileName(path),
            };

            var sourceCodeHash = sourceCode.GetHashCode();

            if (transpileCache.ContainsKey(sourceCodeHash))
            {
                var code = transpileCache[sourceCodeHash];
                exports = (module as Module).Compile(code, path).AsObject();

                module.Exports = exports;

                return exports;
            }

            var compileObject = transpileModule.Invoke(new JsValue[] { sourceCode, JsValue.FromObject(Engine, transpileConfig) }).AsObject();
            /*
                    { outputText: '"use strict";\nexports.__esModule = true;\nexports.str = "test";\n',
                    diagnostics: [],
                    sourceMapText: undefined }
             */
            var diagnostics = compileObject.Get("diagnostics").AsArray();
            var length = diagnostics.GetLength();


            if (length > 0)
            {
                var exceptions = new List<TypeScriptException>((int)length);

                for (var i = 0; i < length; i++)
                {
                    var value = diagnostics.Get(i.ToString()) ?? JsValue.Undefined;

                    if (value.IsUndefined())
                    {
                        continue;
                    }

                    var diagnostic = value.AsObject();
                    var category = diagnostic.Get("category").AsNumber();

                    if (category != 1 /* Error */)
                    {
                        continue;
                    }

                    var file = diagnostic.Get("file").AsObject();
                    var fileName = file.Get("fileName").AsString();
                    var errorCode = diagnostic.Get("code").AsNumber();

                    var messageText = diagnostic.Get("messageText").IsObject()
                        ? diagnostic.Get("messageText").AsObject().Get("messageText").AsString()
                        : diagnostic.Get("messageText").AsString();

                    exceptions.Add(new TypeScriptException((int)errorCode, messageText, fileName));
                }

                throw new AggregateException(exceptions);
            }

            var outputText = compileObject.Get("outputText").AsString();
            transpileCache.Add(sourceCode.GetHashCode(), outputText);

            exports = (module as Module).Compile(outputText, path).AsObject();

            module.Exports = exports;

            return exports;
        }

        public void SetupScriptsDirectory()
        {
            var asm = typeof(Jist).Assembly;
            var typingsDirectory = Path.Combine(ScriptsDirectory, "typings");

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

            if (!Directory.Exists(typingsDirectory))
            {
                Directory.CreateDirectory(typingsDirectory);
                using (var sr = new StreamReader(asm.GetManifestResourceStream("Jist.Next.Scripts.README.txt")))
                {
                    File.WriteAllText(Path.Combine(typingsDirectory, "README.txt"), sr.ReadToEnd());
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

            ModuleLoadingEngine.FileExtensionParsers[".ts"] = TranspileModule;
            ModuleLoadingEngine.FileExtensionParsers[".js"] = TranspileModule;
            ModuleLoadingEngine.FileExtensionParsers["default"] = TranspileModule;
        }
        public string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
}