using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;


public static class AttributeHelper
{
    #region Variables

    static readonly string UserAssembly = Path.Combine("Library", "ScriptAssemblies");

    public class AttributeItem
    {
        public Attribute attribute;
        public MethodInfo methodInfo;
    }

    #endregion


    #region Public methods

    public static List<AttributeHelper.AttributeItem> ExtractMethods<T>(object[] signature)
    {
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        List<AttributeItem> list = new List<AttributeItem>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            PropertyInfo dynamicPropertyInfo = assembly.GetType().GetProperty("IsDynamic");

            if (!(assembly is AssemblyBuilder) &&
                (dynamicPropertyInfo == null || !(bool)dynamicPropertyInfo.GetValue(assembly, null)))
            {
                if (assembly.Location.Contains(UserAssembly))
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        MethodInfo[] methods = type.GetMethods (flags);
                        for (int i = 0; i < methods.GetLength(0); i++) 
                        {
                            MethodInfo methodInfo = methods [i];
    
                            if (Attribute.IsDefined (methodInfo, typeof(T)) && IsValidSignature(methodInfo, signature)) 
                            {
                                object[] customAttributes = methodInfo.GetCustomAttributes (typeof(T), false);
                                if (customAttributes != null && customAttributes.Length > 0)
                                {
                                    Attribute item = ((Attribute)customAttributes[0]);

                                    AttributeItem info = new AttributeItem
                                    {
                                        attribute = item, 
                                        methodInfo = methodInfo
                                    };

                                    list.Add(info);
                                }
                            }
                        }
                    }
                }
            }         
        }

        return list;
    }

    #endregion


    #region Private methods

    static bool IsValidSignature(MethodInfo methodInfo, object[] signature)
    {
        ParameterInfo[] param = methodInfo.GetParameters();

        if (signature != null)
        {
            if (param.Length != signature.Length)
            {
                return false;
            }

            for (int i = 0; i < signature.Length; i++)
            {
                if (!param[i].ParameterType.IsAssignableFrom(signature[i].GetType()))
                {
                    return false;
                }
            }
        }

        return true;
    }

    #endregion
}
