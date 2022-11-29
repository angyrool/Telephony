namespace Telephony.EwsdParser.BusinessLogic;

public class EwsdFileParsingTask
{
    public long Id { get; set; }
    public EwsdFile File { get; set; }
    public EwsdFileParsingTaskStatuses Status { get; set; }
    public string? Message { get; set; }
}