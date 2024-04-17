using System;

namespace GoogleApis.GenerativeLanguage
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter,
        Inherited = false, AllowMultiple = false)]
    public sealed class FunctionCallAttribute : Attribute
    {
        public readonly string description;

        public FunctionCallAttribute(string description)
        {
            this.description = description;
        }
    }
}