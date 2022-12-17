using Telephony.EwsdModel.Table;

namespace Telephony.EwsdParser.BusinessLogic;

public interface IEwsdRecordCreatingLogic
{
    EwsdRecord Create(IEwsdPackage[] packages);
}