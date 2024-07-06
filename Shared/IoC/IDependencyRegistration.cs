using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC
{
    public interface IDependencyRegistration
    {
        void Execute(IServiceCollection services, IConfiguration configuration);
    }
}