namespace Telephony.Ewsd.BusinessLogic;

public interface IEwsdFileTaskManager
{
    /// <summary>
    /// Взять новую задачу
    /// </summary>
    /// <returns></returns>
    EwsdFileTask GetNew();

    /// <summary>
    /// Изменить статус задачи
    /// </summary>
    /// <param name="fileTask"></param>
    /// <param name="status"></param>
    void SetStatus(EwsdFileTask fileTask, EwsdFileTaskStatuses status);

    /// <summary>
    /// Есть ли задачи на выполнение
    /// </summary>
    /// <returns>true, если есть</returns>
    bool Any();
}

public enum EwsdFileTaskStatuses
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