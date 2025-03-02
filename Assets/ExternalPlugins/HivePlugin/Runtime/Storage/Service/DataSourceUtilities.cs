using System;


namespace Modules.Hive.Storage
{
    public static class DataSourceUtilities
    {
        public static string GetIdentifierFromType<T>() where T : DataModel
            => typeof(T).Name;

        public static string GetPathFromIdentifier(string identifier)
            => $"{identifier}.dat";


        public static T ConvertToDataModel<T>(object data) where T : DataModel
        {
            if (data == null)
            {
                return null;
            }

            if (data is T dataModel)
            {
                return dataModel;
            }

            throw new InvalidCastException($"The type if data '{data.GetType()}' doesn't match expected type of data model '{typeof(T)}'");
        }
    }
}
