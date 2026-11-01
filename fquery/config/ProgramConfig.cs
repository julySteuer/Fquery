using fquery.adapter;
using fquery.domain;
using Microsoft.Extensions.DependencyInjection;

namespace fquery.config;

public static class ProgramConfig
{
    public static string RunWithDi(RunConfig runConfig)
    {
        var services = new ServiceCollection();

        services.AddSingleton<EvaluateRunConfig>();
        services.AddSingleton<ParseQuery>();
        services.AddSingleton<EvaluateQuery>();
        
        services.AddSingleton<EvaluateQuery.IPort, EvaluateQueryIoAdapter>();
        services.AddSingleton<ParseQuery.IPort, FqueryQlAntlrAdapter>();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<EvaluateRunConfig>().Execute(runConfig);
    }
}