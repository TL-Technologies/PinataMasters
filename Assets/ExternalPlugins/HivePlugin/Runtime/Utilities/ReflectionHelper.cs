using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Modules.Hive.Reflection
{
    public static class ReflectionHelper
    {
        #region Methods

        public static T CreateDelegateToMethod<T>(
            Type objectType,
            string methodName,
            BindingFlags bindingFlags,
            bool throwOnBindFailure,
            object objectInstance = null)
            where T : Delegate
        {
            MethodInfo methodInfo = objectType.GetMethod(methodName, bindingFlags);
            return (T)Delegate.CreateDelegate(typeof(T), objectInstance, methodInfo, throwOnBindFailure);
        }


        public static Delegate CreateDelegateToMethod(
            Type objectType,
            string methodName,
            BindingFlags bindingFlags)
        {
            var methodInfo = objectType.GetMethod(methodName, bindingFlags);
            var methodParameters = methodInfo.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(methodInfo, methodParameters);
            return Expression.Lambda(call, methodParameters).Compile();
        }

        #endregion
        
        
        
        #region Properties
        
        public static T CreateDelegateToPropertyGet<T>(
            Type type,
            object obj,
            string propertyName,
            bool throwOnBindFailure)
            where T : Delegate
        {
            return (T)Delegate.CreateDelegate(
                typeof(T),
                obj,
                GetPropertyInfo(type, obj, propertyName).GetMethod,
                throwOnBindFailure);
        }
        
        
        public static T CreateDelegateToPropertySet<T>(
            Type type,
            object obj,
            string propertyName,
            bool throwOnBindFailure)
            where T : Delegate
        {
            return (T)Delegate.CreateDelegate(
                typeof(T),
                obj,
                GetPropertyInfo(type, obj, propertyName).SetMethod,
                throwOnBindFailure);
        }
        
        
        public static T GetPropertyValue<T>(Type type, object obj, string propertyName)
        {
            return (T)GetPropertyInfo(type, obj, propertyName)?.GetValue(obj, null);
        }
        
        
        public static void SetPropertyValue(Type type, object obj, string propertyName, object propertyValue)
        {
            GetPropertyInfo(type, obj, propertyName)?.SetValue(obj, propertyValue);
        }
        
        
        private static PropertyInfo GetPropertyInfo(Type type, object obj, string propertyName)
        {
            if (type == null)
            {
                return default;
            }
            
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= (obj == null ? BindingFlags.Static : BindingFlags.Instance);
            
            return type.GetProperty(propertyName, bindingFlags);
        }
        
        #endregion
        
        
        
        #region Fields
        
        public static T GetFieldValue<T>(Type type, object obj, string fieldName)
        {
            return (T)GetFieldInfo(type, obj, fieldName)?.GetValue(obj);
        }
        
        
        public static void SetFieldValue(Type type, object obj, string fieldName, object fieldValue)
        {
            GetFieldInfo(type, obj, fieldName)?.SetValue(obj, fieldValue);
        }
        
        
        private static FieldInfo GetFieldInfo(Type type, object obj, string fieldName)
        {
            if (type == null)
            {
                return default;
            }
            
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= (obj == null ? BindingFlags.Static : BindingFlags.Instance);
            
            return type.GetField(fieldName, bindingFlags);
        }
        
        #endregion
    }
}
