using System;
using System.Linq;

namespace Modules.General.InitializationQueue
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true)]
    public class InitQueueServiceAttribute : Attribute
    {
        #region Fields

        private readonly Type bindTo;

        private readonly Type[] dependencies = new Type[0];

        private readonly int order;

        #endregion



        #region Properties

        public Type BindTo => bindTo;

        public Type[] Dependencies => dependencies;

        public int Order => order;

        #endregion



        #region Constructors

        public InitQueueServiceAttribute(int order)
            : this(order, bindTo: null) { }


        public InitQueueServiceAttribute(int order, Type bindTo = null, params Type[] dependencies)
        {
            this.order = order;
            this.bindTo = bindTo;
            this.dependencies = dependencies.Where(wh => wh != null).ToArray();
        }

        #endregion
    }
}