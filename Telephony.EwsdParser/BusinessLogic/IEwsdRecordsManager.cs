namespace Telephony.EwsdParser.BusinessLogic;

public interface IEwsdRecordsManager
{
    void Add(IEnumerable<EwsdRecord> records);
}