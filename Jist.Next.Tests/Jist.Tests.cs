using System;
using System.IO;
using Jint.Native;
using Xunit;

public class JistTests
{
    [Fact(DisplayName = "Load TypeScript should load and execute the TypeScript engine")]
    public void LoadTypeScriptShouldLoadAndExecuteTheTypeScriptEngine()
    {
        var jist = new Jist.Next.Jist(Path.GetTempPath());

        Assert.NotEqual(JsValue.Undefined, jist.Engine.Global.Get("ts"));
    }

    [Fact(DisplayName = "It should transpile a module in typescript")]
    public void ItShouldTranspileAModule()
    {
        Directory.SetCurrentDirectory(Path.GetTempPath());

        File.WriteAllText("index-ts.ts", @"
            export const str: string = ""test"";
        ");

        var jist = new Jist.Next.Jist(Path.GetTempPath());
        var exports = jist.ModuleLoadingEngine.RunMain("./index-ts");

        Assert.Equal("test", exports.AsObject().Get("str").AsString());
    }

    [Fact(DisplayName = "It should throw an error when a module can't be compiled because of a TypeScript error")]
    public void ItShouldThrowAnErrorWhenATypeScriptDiagnosticMessageHappens()
    {
        Directory.SetCurrentDirectory(Path.GetTempPath());

        File.WriteAllText("error-ts.ts", @"
            export function blah
        ");

        var jist = new Jist.Next.Jist(Path.GetTempPath());

        Assert.Throws<Exception>(() => jist.ModuleLoadingEngine.RunMain("./error-ts"));
    }
}
