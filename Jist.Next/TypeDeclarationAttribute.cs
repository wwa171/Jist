
using System;

namespace Jist.Next
{
    /// <summary>
    /// Indicates to Jist that it should write a type declaration file to aid in intellisense for
    /// whatever module is being written.
    /// </summary>
    public class TypeDeclarationAttribute : Attribute
    {
        public string ResourceId { get; set; }
        public string TypingsFileName { get; set; }

        public TypeDeclarationAttribute(string resourceId, string typingsFileName)
        {
            this.ResourceId = resourceId;
            this.TypingsFileName = typingsFileName;
        }
    }
}