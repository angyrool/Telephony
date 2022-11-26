namespace Telephony.Ewsd.BusinessLogic;

public interface IEwsdFileParsingTaskManager
{
    /// <summary>
    /// Взять новую задачу
    /// </summary>
    /// <returns></returns>
    EwsdFileParsingTask GetNew();

    /// <summary>
    /// Изменить статус задачи
    /// </summary>
    /// <param name="fileParsingTask"></param>
    /// <param name="status"></param>
    void SetStatus(EwsdFileParsingTask fileParsingTask, EwsdFileParsingTaskStatuses status);

    /// <summary>
    /// Есть ли задачи на выполнение
    /// </summary>
    /// <returns>true, если есть</returns>
    bool Any();
}

public enum EwsdFileParsingTaskStatuses
{
    /// <summary>
    /// новая задача
    /// </summary>
    New = 0,
    
    /// <summary>
    /// задача в обработке
    /// </summary>
    InProcess = 1,
    
    /// <summary>
    /// задача успешно выполнена
    /// </summary>
    Processed = 2,
    
    NoFile = 3,
    
    /// <summary>
    /// Нет байтов
    /// </summary>
    NoBytes = 4,
    
    /// <summary>
    /// Нет записей (массива пакетов)
    /// </summary>
    NoPackageRecords = 5,
    
    /// <summary>
    /// Нет записей
    /// </summary>
    NoRecords = 6
}