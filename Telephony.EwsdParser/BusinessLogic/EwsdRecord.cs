namespace Telephony.EwsdParser.BusinessLogic;

/// <summary>
/// Ewsd запись
/// </summary>
public class EwsdRecord
{
    /// <summary>
    /// Создание записи из массива пакетов
    /// </summary>
    /// <param name="ewsdPackageArray">массив пакетов</param>
    public EwsdRecord(IEwsdPackage[] ewsdPackageArray)
    {
        if (ewsdPackageArray.Length == 0)
        {
            throw new Exception("нет пакетов для создания записи");
        }
    }
}