using Microsoft.Extensions.Hosting;

namespace Telephony.Ewsd.BusinessLogic;

public class EwsdWorker : IHostedService
{
    private readonly IEwsdFileProcessLogic _fileProcessLogic;
    private readonly IEwsdFileParsingTaskManager _fileParsingTaskManager;

    public EwsdWorker(IEwsdFileProcessLogic fileProcessLogic, IEwsdFileParsingTaskManager fileParsingTaskManager)
    {
        _fileProcessLogic = fileProcessLogic;
        _fileParsingTaskManager = fileParsingTaskManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        while (_fileParsingTaskManager.Any())
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