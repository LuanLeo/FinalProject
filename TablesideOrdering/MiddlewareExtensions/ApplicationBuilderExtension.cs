using TablesideOrdering.SubscribeTableDependencies;

namespace TablesideOrdering.MiddlewareExtensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseOrderTableDependency(this IApplicationBuilder applicationBuilder)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<SubscribeOrderTableDependency>();
            service.SubscribeTableDependency();
        }
    }
}
