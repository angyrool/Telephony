namespace Telephony.EwsdParser.BusinessLogic;

public interface IEwsdFilePackageManager
{
    /// <summary>
    /// Возвратить массив из массивов IEwsdPackage: <br />
    /// [ <br />
    /// IEwsdPackage[ ], <br />
    /// IEwsdPackage[ ], <br />
    /// ... <br />
    /// ]
    /// </summary>
    /// <param name="fileBytes">байты файла станции ewsd</param>
    /// <returns>массив из массивов IEwsdPackage</returns>
    IEwsdPackage[][] GetPackageArrays(byte[] fileBytes);
}