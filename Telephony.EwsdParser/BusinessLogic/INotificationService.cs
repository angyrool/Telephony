using Telephony.EwsdModel.Table;

namespace Telephony.EwsdParser.BusinessLogic;

public interface INotificationService
{
    void Create(NotificationTypes notificationType, EwsdFileParsingTask fileParsingTask);
}

public enum NotificationTypes
{
    Success = 1,
    Warning,
    Error,
    Critical
}