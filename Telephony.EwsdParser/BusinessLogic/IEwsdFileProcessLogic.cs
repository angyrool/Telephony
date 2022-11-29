namespace Telephony.EwsdParser.BusinessLogic;

/// <summary>
/// Логика обработки ewsd файла
/// </summary>
public interface IEwsdFileProcessLogic
{
    /// <summary>
    /// Запуск обработки ewsd файла
    /// </summary>
    void Run(EwsdFileParsingTask fileParsingTask);
}