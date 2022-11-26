using Microsoft.Extensions.Logging;

namespace Telephony.Ewsd.BusinessLogic;

public class EwsdFileProcessLogic : IEwsdFileProcessLogic
{
    private readonly ILogger<EwsdFileProcessLogic> _logger;
    private readonly IEwsdFileTaskManager _fileTaskManager;
    private readonly IFileSystem _fileSystem;
    private readonly IEwsdFilePackageManager _packageManager;
    private readonly IEwsdRecordsManager _recordsManager;

    public EwsdFileProcessLogic(ILogger<EwsdFileProcessLogic> logger, IEwsdFileTaskManager fileTaskManager, 
        IFileSystem fileSystem, IEwsdFilePackageManager packageManager, IEwsdRecordsManager recordsManager)
    {
        _logger = logger;
        _fileTaskManager = fileTaskManager;
        _fileSystem = fileSystem;
        _packageManager = packageManager;
        _recordsManager = recordsManager;
    }

    public void Run()
    {
        var fileTask = _fileTaskManager.GetNew();
        _logger.LogInformation(@"Обработка файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
            TaskId: {FileTaskId}", fileTask.FileId, fileTask.FileName, fileTask.FilePath, fileTask.Id);
        
        _fileTaskManager.SetStatus(fileTask, EwsdFileTaskStatuses.InProcess);

        if (!_fileSystem.IsFileExists(fileTask.FilePath))
        {
            _logger.LogWarning(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не найден", fileTask.FileId, fileTask.FileName, 
                fileTask.FilePath, fileTask.Id);
            _fileTaskManager.SetStatus(fileTask, EwsdFileTaskStatuses.NoFile);
            return;
        }
        
        var fileBytes = _fileSystem.GetFileBytes(fileTask.FilePath);
        
        if (fileBytes.Length == 0)
        {
            _logger.LogWarning(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не содержит байтов (пустой)", fileTask.FileId, fileTask.FileName, 
                fileTask.FilePath, fileTask.Id);
            _fileTaskManager.SetStatus(fileTask, EwsdFileTaskStatuses.NoBytes);
            return;
        }

        var packageArrays = _packageManager.GetPackageArrays(fileBytes);

        if (packageArrays.Length == 0)
        {
            _logger.LogWarning(@"Из байтов файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не удалось создать массивы пакетов", fileTask.FileId, fileTask.FileName, 
                fileTask.FilePath, fileTask.Id);
            _fileTaskManager.SetStatus(fileTask, EwsdFileTaskStatuses.NoPackageRecords);
            return;
        }
        
        var records = packageArrays
            .Select(packageArray => new EwsdRecord(packageArray)) as EwsdRecord[] 
                      ?? Array.Empty<EwsdRecord>();

        if (records.Length == 0)
        {
            _logger.LogWarning(@"Из байтов файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не удалось создать записи", fileTask.FileId, fileTask.FileName, 
                fileTask.FilePath, fileTask.Id);
            _fileTaskManager.SetStatus(fileTask, EwsdFileTaskStatuses.NoRecords);
            return;
        }

        _recordsManager.Add(records);
        
        _logger.LogInformation(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
            TaskId: {FileTaskId} успешно обработан", fileTask.FileId, fileTask.FileName, 
            fileTask.FilePath, fileTask.Id);
        _fileTaskManager.SetStatus(fileTask, EwsdFileTaskStatuses.Processed);
    }
}