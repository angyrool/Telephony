namespace Telephony.EwsdParser.BusinessLogic;

public interface IEwsdFileParsingTaskManager
{
    /// <summary>
    /// Взять новую задачу
    /// </summary>
    /// <returns></returns>
    EwsdFileParsingTask GetNew();

    /// <summary>
    /// Сохранить задачу
    /// </summary>
    /// <param name="fileParsingTask"></param>
    void Save(EwsdFileParsingTask fileParsingTask);

    /// <summary>
    /// Есть ли задачи на выполнение
    /// </summary>
    /// <returns>true, если есть</returns>
    bool Any();
}

public enum EwsdFileParsingTaskStatuses
{
    /// <summary>
    /// Новая задача
    /// </summary>
    New = 0,
    
    /// <summary>
    /// Задача в обработке
    /// </summary>
    InProcess = 1,
    
    /// <summary>
    /// Задача успешно выполнена
    /// </summary>
    Processed = 2,
    
    /// <summary>
    /// Файла не существует
    /// </summary>
    NoFile = 3,
    
    /// <summary>
    /// Нет байтов
    /// </summary>
    NoBytes = 4,

    /// <summary>
    /// Нет записей
    /// </summary>
    NoRecords = 5,
    
    /// <summary>
    /// Возникла ошибка при обработке файла
    /// </summary>
    Error = 6
}