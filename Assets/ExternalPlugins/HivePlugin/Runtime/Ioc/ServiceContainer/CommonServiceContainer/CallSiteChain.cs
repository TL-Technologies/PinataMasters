using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Modules.Hive.Ioc
{
    internal class CallSiteChain
    {
        private int order = 0;
        private Dictionary<Type, int> services = new Dictionary<Type, int>();

        
        public void Add(Type implementationType)
        {
            if (services.ContainsKey(implementationType))
            {
                throw new InvalidOperationException(CreateCircularDependencyExceptionMessage(implementationType));
            }

            services.Add(implementationType, order);
            order++;
        }

        
        public bool Remove(Type implementationType)
        {
            return services.Remove(implementationType);
        }
        

        private string CreateCircularDependencyExceptionMessage(Type type)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat(ServiceProviderUtilities.CircularDependencyExceptionMessageFormat, TypeNameHelper.GetTypeDisplayName(type));
            messageBuilder.AppendLine();

            AppendResolutionPath(messageBuilder, type);

            return messageBuilder.ToString();
        }
        

        private void AppendResolutionPath(StringBuilder builder, Type currentlyResolving = null)
        {
            foreach (var pair in services.OrderBy(p => p.Value))
            {
                builder.AppendFormat("{0} ->", TypeNameHelper.GetTypeDisplayName(pair.Key));
                builder.AppendLine();
            }

            builder.Append(TypeNameHelper.GetTypeDisplayName(currentlyResolving));
        }
    }
}
