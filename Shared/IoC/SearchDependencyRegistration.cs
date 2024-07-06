using Backend.App.Data.Search;
using Backend.App.Infrastructure.Configuration;
using Meilisearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class SearchDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISearchClient, SearchClient>();
        services.AddSingleton(
            serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<AppSettings>();
                return new MeilisearchClient(
                    settings.SearchBaseUrl,
                    settings.SearchAdminApiKey);
            });
    }
}