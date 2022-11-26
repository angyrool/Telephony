using Microsoft.Extensions.Hosting;

namespace Telephony.Ewsd.BusinessLogic;

public class EwsdWorker : IHostedService
{
    private readonly IEwsdFileProcessLogic _fileProcessLogic;
    private readonly IEwsdFileTaskManager _fileTaskManager;

    public EwsdWorker(IEwsdFileProcessLogic fileProcessLogic, IEwsdFileTaskManager fileTaskManager)
    {
        _fileProcessLogic = fileProcessLogic;
        _fileTaskManager = fileTaskManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        while (_fileTaskManager.Any())
        {
            _fileProcessLogic.Run();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        Console.WriteLine("EwsdWorker stopped");
    }
}