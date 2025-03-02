using System;


namespace Modules.General
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class RequiredAbData : Attribute { }
}
