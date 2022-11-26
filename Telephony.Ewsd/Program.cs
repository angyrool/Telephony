using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telephony.Ewsd;
using Telephony.Ewsd.BusinessLogic;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<EwsdWorker>();
        services.AddSingleton<IEwsdFileProcessLogic, EwsdFileProcessLogic>();
        services.Configure<EwsdSettings>(hostContext.Configuration.GetSection(nameof(EwsdSettings)));
    })
    .Build();

host.Run();