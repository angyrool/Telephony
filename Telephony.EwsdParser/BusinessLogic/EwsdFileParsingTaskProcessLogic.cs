using Microsoft.Extensions.Logging;
using Telephony.EwsdModel.Table;

namespace Telephony.EwsdParser.BusinessLogic;

public class EwsdFileParsingTaskProcessLogic : IEwsdFileParsingTaskProcessLogic
{
    private readonly IEwsdFileProcessLogic _fileProcessLogic;
    private readonly IEwsdFileParsingTaskManager _fileParsingTaskManager;
    private readonly ILogger<EwsdFileParsingTaskProcessLogic> _logger;
    private readonly INotificationService _notificationService;

    public EwsdFileParsingTaskProcessLogic(IEwsdFileProcessLogic fileProcessLogic, 
        IEwsdFileParsingTaskManager fileParsingTaskManager, 
        ILogger<EwsdFileParsingTaskProcessLogic> logger, 
        INotificationService notificationService)
    {
        _fileProcessLogic = fileProcessLogic;
        _fileParsingTaskManager = fileParsingTaskManager;
        _logger = logger;
        _notificationService = notificationService;
    }

    public void Run()
    {
        try
        {
            var fileParsingTask = _fileParsingTaskManager.GetNew();
            fileParsingTask.Status = EwsdFileParsingTaskStatuses.InProcess;
            _fileParsingTaskManager.Save(fileParsingTask);
            
            _fileProcessLogic.Run(fileParsingTask);
            _fileParsingTaskManager.Save(fileParsingTask);

            switch (fileParsingTask.Status)
            {
                case EwsdFileParsingTaskStatuses.New:
                case EwsdFileParsingTaskStatuses.InProcess:
                case EwsdFileParsingTaskStatuses.NoRecords:
                    _notificationService.Create(NotificationTypes.Critical, fileParsingTask);
                    _logger.LogCritical("Файл {FilePath} не обработан. Статус: {Status}. " +
                                        "Id задачи: {FileParsingTaskId}", fileParsingTask.File.Path, 
                        fileParsingTask.Status, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.Processed:
                    _notificationService.Create(NotificationTypes.Success, fileParsingTask);
                    _logger.LogInformation("Файл {FileParsingTaskFilePath} успешно обработан и будет удален. " +
                                           "Id задачи: {FileParsingTaskId}",
                        fileParsingTask.File.Path, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.NoFile:
                    _notificationService.Create(NotificationTypes.Warning, fileParsingTask);
                    _logger.LogWarning("Файл {FilePath} не найден. Id задачи: {Id}", 
                        fileParsingTask.File.Path, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.NoBytes:
                    _notificationService.Create(NotificationTypes.Warning, fileParsingTask);
                    _logger.LogWarning("Файл {FilePath} не содержит байтов (пустой) и будет удален. " +
                                       "Id задачи: {Id}", fileParsingTask.File.Path, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.Error:
                    _notificationService.Create(NotificationTypes.Error, fileParsingTask);
                    _logger.LogError("При обработке файла {FilePath} возникла ошибка: {Message}. " +
                                     "Id задачи: {Id}", 
                        fileParsingTask.File.Path, fileParsingTask.Message, fileParsingTask.Id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            /*switch (fileParsingTask.Status)
            {
                case EwsdFileParsingTaskStatuses.New:
                case EwsdFileParsingTaskStatuses.InProcess:
                case EwsdFileParsingTaskStatuses.NoRecords:
                    _notificationService.Create(NotificationTypes.Critical, $"Файл {fileParsingTask.File.Path} не обработан. " +
                                                                            $"Статус: {fileParsingTask.Status}. Id задачи: {fileParsingTask.Id}.");
                    _logger.LogCritical("Файл {FilePath} не обработан. Статус: {Status}. " +
                                        "Id задачи: {FileParsingTaskId}", fileParsingTask.File.Path, 
                        fileParsingTask.Status, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.Processed:
                    _notificationService.Create(NotificationTypes.Success, $"Файл {fileParsingTask.File.Path} успешно обработан и будет " +
                                                                            $"удален. Id задачи: {fileParsingTask.Id}.");
                    _logger.LogInformation("Файл {FileParsingTaskFilePath} успешно обработан и будет удален. " +
                                           "Id задачи: {FileParsingTaskId}",
                        fileParsingTask.File.Path, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.NoFile:
                    _notificationService.Create(NotificationTypes.Warning, $"Файл {fileParsingTask.File.Path} не найден. Id задачи: {fileParsingTask.Id}.");
                    _logger.LogWarning("Файл {FilePath} не найден. Id задачи: {Id}", 
                        fileParsingTask.File.Path, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.NoBytes:
                    _notificationService.Create(NotificationTypes.Warning, $"Файл {fileParsingTask.File.Path} не содержит байтов " +
                                                                         $"(пустой) и будет удален. Id задачи: {fileParsingTask.Id}.");
                    _logger.LogWarning("Файл {FilePath} не содержит байтов (пустой) и будет удален. " +
                                       "Id задачи: {Id}", fileParsingTask.File.Path, fileParsingTask.Id);
                    break;
                case EwsdFileParsingTaskStatuses.Error:
                    _notificationService.Create(NotificationTypes.Error, $"При обработке файла {fileParsingTask.File.Path} возникла " +
                                                                         $"ошибка: {fileParsingTask.Message}. Id задачи: {fileParsingTask.Id}");
                    _logger.LogError("При обработке файла {FilePath} возникла ошибка: {Message}. " +
                                     "Id задачи: {Id}", 
                        fileParsingTask.File.Path, fileParsingTask.Message, fileParsingTask.Id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }*/
        }
        catch (Exception e)
        {
            _logger.LogError(@"Ошибка при обработке задачи: {Message}", e.Message);
        }
    }
}