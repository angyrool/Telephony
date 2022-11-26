namespace Telephony.Ewsd.BusinessLogic;

public interface IFileSystem
{
    byte[] GetFileBytes(string path);
    bool IsFileExists(string fileTaskFilePath);
}