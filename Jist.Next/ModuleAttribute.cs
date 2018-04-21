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

        public ModuleAttribute(string moduleId)
        {
            this.ModuleId = moduleId;
        }
    }
}