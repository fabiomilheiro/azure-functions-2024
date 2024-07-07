using Azf.Shared.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC
{
    public class DependencyRegistrationContext
    {
        public required IConfiguration Configuration { get; set; }

        public Type? DbContext { get; set; }
    }

    public interface IDependencyRegistration
    {
        void Execute(IServiceCollection services, DependencyRegistrationContext context);
    }
}