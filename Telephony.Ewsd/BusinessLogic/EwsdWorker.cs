using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telephony.Ewsd.BusinessLogic;

public class EwsdWorker : IHostedService
{
    private readonly IEwsdFileProcessLogic _fileProcessLogic;
    private readonly IEwsdFileParsingTaskManager _fileParsingTaskManager;
    private readonly ILogger<EwsdWorker> _logger;
    private readonly EwsdSettings _settings;

    public EwsdWorker(IEwsdFileProcessLogic fileProcessLogic, IEwsdFileParsingTaskManager fileParsingTaskManager, 
        ILogger<EwsdWorker> logger, IOptions<EwsdSettings> settings)
    {
        _fileProcessLogic = fileProcessLogic;
        _fileParsingTaskManager = fileParsingTaskManager;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ewsd files parsing started");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_fileParsingTaskManager.Any())
            {
                _fileProcessLogic.Run();
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.ParsingDelayInSeconds), cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        _logger.LogInformation("Ewsd files parsing stopped");
    }
}