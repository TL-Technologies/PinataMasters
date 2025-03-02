namespace Modules.Hive.Storage
{
    public abstract class DataModel
    {
        internal IDataSource DataSource { get; set; }


        protected internal virtual void OnAfterLoad() { }


        protected internal virtual void OnBeforeSave() { }


        public bool Save()
        {
            return DataSource?.Save() ?? false;
        }


        public virtual void Close()
        {
            if (DataSource != null)
            {
                DataSource.Close();
            }
        }
    }
}
