using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IEventLogger
    {
        void SendEvent(string eventName, Dictionary<string, string> parameters = null);
    }


    public interface ITypedEventLogger
    {
        void SendEvent(Type type, string eventName, Dictionary<string, string> parameters = null);
    }


    public interface IMultipleTypedEventLogger
    {
        void SendEvent(Type[] types, string eventName, Dictionary<string, string> parameters = null);
    }
}
