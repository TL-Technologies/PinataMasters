using Modules.General.Abstraction;
using UnityEngine;


namespace Modules.Advertising
{
    public class ModuleAttribute : PropertyAttribute
    {
        public AdModule AdModuleType { get; }

        public ModuleAttribute(AdModule adModule)
        {
            AdModuleType = adModule;
        }
    }
}