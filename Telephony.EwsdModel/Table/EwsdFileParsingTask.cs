namespace Telephony.EwsdModel.Table;

public class EwsdFileParsingTask
{
    public long Id { get; set; }
    public EwsdFile File { get; set; }
    public EwsdFileParsingTaskStatuses Status { get; set; }
    public string? Message { get; set; }
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