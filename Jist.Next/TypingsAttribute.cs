
using System;

namespace Jist.Next
{
    /// <summary>
    /// Indicates to Jist that it should write a type declaration file to aid in intellisense for
    /// whatever module is being written.
    /// </summary>
    public class TypeDeclarationAttribute : Attribute
    {
        public Type ResourceType { get; set; }
        public string ResourceId { get; set; }
        public string TypingsFileName { get; set; }


        public TypeDeclarationAttribute(string resourceId, string typingsFileName)
            : this(null, resourceId, typingsFileName)
        {
        }

        public TypeDeclarationAttribute(Type resourceContainingType, string resourceId, string typingsFileName)
        {
            this.ResourceType = resourceContainingType ?? this.GetType();
            this.ResourceId = resourceId;
            this.TypingsFileName = typingsFileName;
        }
    }
}