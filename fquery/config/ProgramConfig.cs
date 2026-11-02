using fquery.adapter;
using fquery.domain;
using Microsoft.Extensions.DependencyInjection;

namespace fquery.config;

public static class ProgramConfig
{
    public static void RunWithDi(RunConfig runConfig)
    {
        var services = new ServiceCollection();

        services.AddSingleton<EvaluateRunConfig>();
        services.AddSingleton<ParseQuery>();
        services.AddSingleton<EvaluateQuery>();
        services.AddSingleton<GenerateOutput>();
        
        services.AddSingleton<EvaluateQuery.IPort, EvaluateQueryIoAdapter>();
        services.AddSingleton<ParseQuery.IPort, FqueryQlAntlrAdapter>();
        services.AddSingleton<GenerateOutput.IPort, GenerateOutputIoAdapter>();

        var serviceProvider = services.BuildServiceProvider();

         serviceProvider.GetRequiredService<EvaluateRunConfig>().Execute(runConfig);
    }
}