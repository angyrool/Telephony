namespace Telephony.EwsdParser.BusinessLogic;

/// <summary>
/// Ewsd запись
/// </summary>
public class EwsdRecord
{
    /// <summary>
    /// Создание записи из массива пакетов
    /// </summary>
    /// <param name="packages">пакеты</param>
    public EwsdRecord(IEwsdPackage[] packages)
    {
        _packages = packages;
    }

    private readonly IEwsdPackage[] _packages;

    public bool IsValid()
    {
        return _packages.Length > 0;
    }
}