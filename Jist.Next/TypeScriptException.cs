using System;

namespace Jist.Next
{
    public class TypeScriptException : Exception
    {
        public int Code { get; internal set; }

        public override string Message { get; }

        public string File { get; internal set; }
        public TypeScriptException(int code, string message, string file)
            : base(message: $"Error TS{code} in file {file}: {message}")
        {
            this.Code = code;
            this.Message = message;
            this.File = file;
        }
    }
}
