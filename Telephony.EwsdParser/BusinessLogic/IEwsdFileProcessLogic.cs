namespace Telephony.EwsdParser.BusinessLogic;

/// <summary>
/// Логика обработки (парсинга) ewsd файла
/// </summary>
public interface IEwsdFileProcessLogic
{
    /// <summary>
    /// Запуск обработки (парсинга) ewsd файла
    /// </summary>
    void Run(EwsdFileParsingTask fileParsingTask);
}