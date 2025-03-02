using System;


namespace Modules.Hive.Editor.Pipeline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorPipelineOptionsAttribute : Attribute
    {
        #region Option value struct

        private struct OptionValue<T>
        {
            private T value;

            public bool IsDefined { get; private set; }


            public OptionValue(T defaultValue)
            {
                value = defaultValue;
                IsDefined = false;
            }


            public T Value
            {
                get => value;
                set
                {
                    this.value = value;
                    IsDefined = true;
                }
            }


            public bool Override(OptionValue<T> option)
            {
                if (!option.IsDefined)
                {
                    return false;
                }

                Value = option.Value;
                return true;
            }
        }

        #endregion

        
        private OptionValue<AppHostLayer> appHostLayer = new OptionValue<AppHostLayer>(AppHostLayer.Default);
        private OptionValue<int> priority = new OptionValue<int>(0);


        public AppHostLayer AppHostLayer
        {
            get => appHostLayer.Value;
            set => appHostLayer.Value = value;
        }


        public int Priority
        {
            get => priority.Value;
            set => priority.Value = value;
        }


        internal void Override(EditorPipelineOptionsAttribute options)
        {
            if (options == null)
            {
                return;
            }

            appHostLayer.Override(options.appHostLayer);
            priority.Override(options.priority);
        }
    }
}
