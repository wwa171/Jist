using System;

namespace Jist.Next
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// Gets the internal module ID for this module type
        /// </summary>
        public string ModuleId { get; }

        /// <summary>
        /// (optional) Gets the embedded resource ID for the provided typings.
        /// </summary>
        public string TypingsResourceId { get; }

        public ModuleAttribute(string moduleId, string typingsResourceId = null)
        {
            this.ModuleId = moduleId;
            this.TypingsResourceId = typingsResourceId;
        }
    }
}