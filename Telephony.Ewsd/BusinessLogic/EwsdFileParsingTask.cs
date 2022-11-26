namespace Telephony.Ewsd.BusinessLogic;

public class EwsdFileParsingTask
{
    public long Id { get; set; }
    public EwsdFile File { get; set; }
    public EwsdFileParsingTaskStatuses Status { get; set; }
}