namespace Telephony.EwsdModel.Table;

public class EwsdRecord
{
    public long Id { get; set; }
    public long? ConnectionId { get; set; }
    public string? Cgpn { get; set; }
    public string? Cdpn { get; set; }
    public string? Rn { get; set; }
}