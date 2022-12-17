using Microsoft.Extensions.Logging;
using Telephony.EwsdModel.Table;

namespace Telephony.EwsdParser.BusinessLogic;

public class EwsdFileProcessLogic : IEwsdFileProcessLogic
{
    private readonly ILogger<EwsdFileProcessLogic> _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IEwsdFilePackageManager _filePackageManager;
    private readonly IEwsdRecordsManager _recordsManager;
    private readonly IEwsdRecordCreatingLogic _recordCreatingLogic;

    public EwsdFileProcessLogic(ILogger<EwsdFileProcessLogic> logger,
        IFileSystem fileSystem, IEwsdFilePackageManager filePackageManager, IEwsdRecordsManager recordsManager, 
        IEwsdRecordCreatingLogic recordCreatingLogic)
    {
        _logger = logger;
        _fileSystem = fileSystem;
        _filePackageManager = filePackageManager;
        _recordsManager = recordsManager;
        _recordCreatingLogic = recordCreatingLogic;
    }

    public void Run(EwsdFileParsingTask fileParsingTask)
    {
        try
        {
            _logger.LogInformation(@"Обработка файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
            TaskId: {FileTaskId}", fileParsingTask.File.Id, fileParsingTask.File.Name,
                fileParsingTask.File.Path, fileParsingTask.Id);

            if (!_fileSystem.IsFileExists(fileParsingTask.File.Path))
            {
                _logger.LogWarning(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не найден", fileParsingTask.File.Id, fileParsingTask.File.Name,
                    fileParsingTask.File.Path, fileParsingTask.Id);
                fileParsingTask.Status = EwsdFileParsingTaskStatuses.NoFile;
                return;
            }

            var fileBytes = _fileSystem.GetFileBytes(fileParsingTask.File.Path);

            if (fileBytes.Length == 0)
            {
                _logger.LogWarning(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не содержит байтов (пустой)", fileParsingTask.File.Id,
                    fileParsingTask.File.Name, fileParsingTask.File.Path, fileParsingTask.Id);
                fileParsingTask.Status = EwsdFileParsingTaskStatuses.NoBytes;
                return;
            }

            var packageArrays = _filePackageManager.GetPackageArrays(fileBytes);

            if (packageArrays.Length == 0)
            {
                _logger.LogWarning(@"Из байтов файла {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
                TaskId: {FileTaskId} не удалось создать массивы пакетов", fileParsingTask.File.Id,
                    fileParsingTask.File.Name, fileParsingTask.File.Path, fileParsingTask.Id);
                fileParsingTask.Status = EwsdFileParsingTaskStatuses.NoRecords;
                return;
            }

            var records = packageArrays
                .Select(packageArray => _recordCreatingLogic.Create(packageArray));

            _recordsManager.Add(records);

            _logger.LogInformation(@"Файл {FileTaskFileId} '{FileTaskFileName}' ({FileTaskFilePath}) 
            TaskId: {FileTaskId} успешно обработан", fileParsingTask.File.Id, fileParsingTask.File.Name,
                fileParsingTask.File.Path, fileParsingTask.Id);
            fileParsingTask.Status = EwsdFileParsingTaskStatuses.Processed;
        }
        catch (Exception e)
        {
            fileParsingTask.Status = EwsdFileParsingTaskStatuses.Error;
            fileParsingTask.Message = e.Message;
        }
    }
}