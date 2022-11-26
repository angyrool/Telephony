using Microsoft.Extensions.Logging;

namespace Telephony.EwsdParser.BusinessLogic;

public class EwsdFileProcessLogic : IEwsdFileProcessLogic
{
    private readonly ILogger<EwsdFileProcessLogic> _logger;
    private readonly IEwsdFileParsingTaskManager _fileParsingTaskManager;
    private readonly IFileSystem _fileSystem;
    private readonly IEwsdFilePackageManager _packageManager;
    private readonly IEwsdRecordsManager _recordsManager;

    public EwsdFileProcessLogic(ILogger<EwsdFileProcessLogic> logger, 
        IEwsdFileParsingTaskManager fileParsingTaskManager, 
        IFileSystem fileSystem, IEwsdFilePackageManager packageManager, IEwsdRecordsManager recordsManager)
    {
        _logger = logger;
        _fileParsingTaskManager = fileParsingTaskManager;
        _fileSystem = fileSystem;
        _packageManager = packageManager;
        _recordsManager = recordsManager;
    }

    public void Run()
    {
        var fileParsingTask = _fileParsingTaskManager.GetNew();
        _logger.LogInformation(@"Обработка файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
            TaskId: {FileTaskId}", fileParsingTask.File.Id, fileParsingTask.File.Name, 
            fileParsingTask.File.Path, fileParsingTask.Id);
        
        _fileParsingTaskManager.SetStatus(fileParsingTask, EwsdFileParsingTaskStatuses.InProcess);

        if (!_fileSystem.IsFileExists(fileParsingTask.File.Path))
        {
            _logger.LogWarning(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не найден", fileParsingTask.File.Id, fileParsingTask.File.Name, 
                fileParsingTask.File.Path, fileParsingTask.Id);
            _fileParsingTaskManager.SetStatus(fileParsingTask, EwsdFileParsingTaskStatuses.NoFile);
            return;
        }
        
        var fileBytes = _fileSystem.GetFileBytes(fileParsingTask.File.Path);
        
        if (fileBytes.Length == 0)
        {
            _logger.LogWarning(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не содержит байтов (пустой)", fileParsingTask.File.Id, 
                fileParsingTask.File.Name, fileParsingTask.File.Path, fileParsingTask.Id);
            _fileParsingTaskManager.SetStatus(fileParsingTask, EwsdFileParsingTaskStatuses.NoBytes);
            return;
        }

        var packageArrays = _packageManager.GetPackageArrays(fileBytes);

        if (packageArrays.Length == 0)
        {
            _logger.LogWarning(@"Из байтов файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не удалось создать массивы пакетов", fileParsingTask.File.Id, 
                fileParsingTask.File.Name, fileParsingTask.File.Path, fileParsingTask.Id);
            _fileParsingTaskManager.SetStatus(fileParsingTask, EwsdFileParsingTaskStatuses.NoPackageRecords);
            return;
        }
        
        var records = packageArrays
            .Select(packageArray => new EwsdRecord(packageArray)) as EwsdRecord[] 
                      ?? Array.Empty<EwsdRecord>();

        if (records.Length == 0)
        {
            _logger.LogWarning(@"Из байтов файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не удалось создать записи", fileParsingTask.File.Id, 
                fileParsingTask.File.Name, fileParsingTask.File.Path, fileParsingTask.Id);
            _fileParsingTaskManager.SetStatus(fileParsingTask, EwsdFileParsingTaskStatuses.NoRecords);
            return;
        }

        _recordsManager.Add(records);
        
        _logger.LogInformation(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
            TaskId: {FileTaskId} успешно обработан", fileParsingTask.File.Id, fileParsingTask.File.Name, 
            fileParsingTask.File.Path, fileParsingTask.Id);
        _fileParsingTaskManager.SetStatus(fileParsingTask, EwsdFileParsingTaskStatuses.Processed);
    }
}