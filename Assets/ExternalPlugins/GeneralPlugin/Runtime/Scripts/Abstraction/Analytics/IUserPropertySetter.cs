using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IUserPropertySetter
    {
        void SetUserProperty(string propertyName, string propertyValue);
    }


    public interface ITypedUserPropertySetter
    {
        void SetUserProperty(Type type, string propertyName, string propertyValue);
    }


    public interface IMultipleTypedUserPropertySetter
    {
        void SetUserProperty(Type[] types, string propertyName, string propertyValue);
    }
}