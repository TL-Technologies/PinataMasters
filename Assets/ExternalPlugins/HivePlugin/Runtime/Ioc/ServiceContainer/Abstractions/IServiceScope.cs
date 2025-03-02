using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Defines a mechanism for retrieving a scoped service object.
    /// Be adviced the <see cref="IDisposable.Dispose"/> method ends the scope lifetime.
    /// Once Dispose is called, any scoped services that have been resolved from <see cref="IServiceScope"/> will be disposed too.
    /// </summary>
    public interface IServiceScope : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Gets the <see cref="IServiceContainer"/> used to resolve dependencies from the scope.
        /// </summary>
        /// <returns>A <see cref="IServiceContainer"/> that used to resolve dependencies from the scope.</returns>
        IServiceContainer ServiceContainer { get; }
    }
}
