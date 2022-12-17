using Telephony.EwsdModel.Table;

namespace Telephony.EwsdParser.BusinessLogic;

public interface IEwsdFileParsingTaskManager
{
    /// <summary>
    /// Взять новую задачу
    /// </summary>
    /// <returns></returns>
    EwsdFileParsingTask GetNew();

    /// <summary>
    /// Сохранить задачу
    /// </summary>
    /// <param name="fileParsingTask"></param>
    void Save(EwsdFileParsingTask fileParsingTask);

    /// <summary>
    /// Есть ли задачи на выполнение
    /// </summary>
    /// <returns>true, если есть</returns>
    bool Any();
}