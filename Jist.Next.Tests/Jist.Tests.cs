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
        jist.LoadTypeScript();

        Assert.NotEqual(JsValue.Undefined, jist.Engine.Global.Get("ts"));
    }
}
