namespace Telephony.Ewsd.BusinessLogic;

public interface IEwsdRecordsManager
{
    void Add(IEnumerable<EwsdRecord> records);
}