using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General
{
    public class RequiredAbDataStorage : ScriptableSingleton<RequiredAbDataStorage>
    {
        #region Fields

        [SerializeField] private List<string> requiredAbDataClasses = new List<string>();

        #endregion

        

        #region Methods

        public void AddTestData(Type t)
        {
            var qualifiedName = t.AssemblyQualifiedName;
            if (!requiredAbDataClasses.Contains(qualifiedName))
            {
                requiredAbDataClasses.Add(qualifiedName);
            }
        }

        
        public void RemoveTestData(Type t)
        {
            var qualifiedName = t.AssemblyQualifiedName;
            if (requiredAbDataClasses.Contains(qualifiedName))
            {
                requiredAbDataClasses.Remove(qualifiedName);
            }
        }

        
        public Type[] GetAllTestDataTypes()
        {
            var types = new List<Type>();

            foreach (var testData in requiredAbDataClasses)
            {
                var type = Type.GetType(testData);
                types.Add(type);
            }
            
            return types.ToArray();
        }

        #endregion
    }
}
