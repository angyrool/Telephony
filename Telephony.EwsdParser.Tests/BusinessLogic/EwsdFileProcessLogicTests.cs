using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telephony.EwsdModel.Table;
using Telephony.EwsdParser.BusinessLogic;

namespace Telephony.EwsdParser.Tests.BusinessLogic;

[TestClass]
public class EwsdFileProcessLogicTests
{
    private readonly ContainerBuilder? _containerBuilder;

    public EwsdFileProcessLogicTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IEwsdFileProcessLogic, EwsdFileProcessLogic>();

        _containerBuilder = new ContainerBuilder();
        _containerBuilder.Populate(services);
    }

    private EwsdFileParsingTask? _fileParsingTask;
    private List<EwsdRecord>? _savedRecords;

    [TestInitialize]
    public void Setup()
    {
        _fileParsingTask = new EwsdFileParsingTask()
        {
            File = new EwsdFile()
            {
                Name = "123",
                Path = "c://ewsd/123"
            },
            Status = EwsdFileParsingTaskStatuses.New
        };

        _savedRecords = new List<EwsdRecord>();
        
        RegisterAllMockObjects();
    }

    private void RegisterAllMockObjects()
    {
        var fileSystemMock = new Mock<IFileSystem>();

        fileSystemMock
            .Setup(fileSystem => fileSystem.IsFileExists(It.IsAny<string>()))
            .Returns(true);

        fileSystemMock
            .Setup(fileSystem => fileSystem.GetFileBytes(It.IsAny<string>()))
            .Returns(new byte[] { 1, 2, 3, 4, 5 });

        _containerBuilder?.RegisterInstance(fileSystemMock.Object).As<IFileSystem>();


        var filePackageManagerMock = new Mock<IEwsdFilePackageManager>();

        filePackageManagerMock
            .Setup(filePackageManager => filePackageManager.GetPackageArrays(It.IsAny<byte[]>()))
            .Returns(new[] { new IEwsdPackage[] { new EwsdPackage(), new EwsdPackage() } });

        _containerBuilder?.RegisterInstance(filePackageManagerMock.Object).As<IEwsdFilePackageManager>();
        
        
        var recordCreatingLogicMock = new Mock<IEwsdRecordCreatingLogic>();

        recordCreatingLogicMock
            .Setup(filePackageManager => filePackageManager.Create(It.IsAny<IEwsdPackage[]>()))
            .Returns(new EwsdRecord());

        _containerBuilder?.RegisterInstance(recordCreatingLogicMock.Object).As<IEwsdRecordCreatingLogic>();


        var recordsManagerMock = new Mock<IEwsdRecordsManager>();

        recordsManagerMock
            .Setup(recordsManager => recordsManager.Add(It.IsAny<IEnumerable<EwsdRecord>>()))
            .Callback((IEnumerable<EwsdRecord> records) => { _savedRecords?.AddRange(records); });

        _containerBuilder?.RegisterInstance(recordsManagerMock.Object).As<IEwsdRecordsManager>();
    }

    [TestMethod]
    public void EwsdFileProcessLogic_Run_Processed_Success_Test()
    {
        var container = _containerBuilder?.Build();
        var serviceProvider = new AutofacServiceProvider(container);
        var fileProcessLogic = serviceProvider.GetService<IEwsdFileProcessLogic>();
        
        Assert.IsNotNull(fileProcessLogic);
        
        fileProcessLogic.Run(_fileParsingTask!);
        
        Assert.AreEqual(1,_savedRecords?.Count);
        Assert.AreEqual(EwsdFileParsingTaskStatuses.Processed,_fileParsingTask?.Status);
    }
    
    [TestMethod]
    public void EwsdFileProcessLogic_Run_Processed_NoFile_Test()
    {
        var fileSystemMock = new Mock<IFileSystem>();

        fileSystemMock
            .Setup(fileSystem => fileSystem.IsFileExists(It.IsAny<string>()))
            .Returns(false);
        
        fileSystemMock
            .Setup(fileSystem => fileSystem.GetFileBytes(It.IsAny<string>()))
            .Returns(new byte[]{ 1,2,3,4,5 });
        
        _containerBuilder?.RegisterInstance(fileSystemMock.Object).As<IFileSystem>();

        
        var container = _containerBuilder?.Build();
        var serviceProvider = new AutofacServiceProvider(container);
        var fileProcessLogic = serviceProvider.GetService<IEwsdFileProcessLogic>();
        
        Assert.IsNotNull(fileProcessLogic);
        
        fileProcessLogic.Run(_fileParsingTask!);
        
        Assert.AreEqual(0,_savedRecords?.Count);
        Assert.AreEqual(EwsdFileParsingTaskStatuses.NoFile, _fileParsingTask?.Status);
    }
    
    [TestMethod]
    public void EwsdFileProcessLogic_Run_Processed_NoBytes_Test()
    {
        var fileSystemMock = new Mock<IFileSystem>();

        fileSystemMock
            .Setup(fileSystem => fileSystem.IsFileExists(It.IsAny<string>()))
            .Returns(true);
        
        fileSystemMock
            .Setup(fileSystem => fileSystem.GetFileBytes(It.IsAny<string>()))
            .Returns(Array.Empty<byte>());
        
        _containerBuilder?.RegisterInstance(fileSystemMock.Object).As<IFileSystem>();


        var container = _containerBuilder?.Build();
        var serviceProvider = new AutofacServiceProvider(container);
        var fileProcessLogic = serviceProvider.GetService<IEwsdFileProcessLogic>();
        
        Assert.IsNotNull(fileProcessLogic);
        
        fileProcessLogic.Run(_fileParsingTask!);
        
        Assert.AreEqual(0,_savedRecords?.Count);
        Assert.AreEqual(EwsdFileParsingTaskStatuses.NoBytes, _fileParsingTask?.Status);
    }
    
    [TestMethod]
    public void EwsdFileProcessLogic_Run_Processed_NoRecords_Test()
    {
        var filePackageManagerMock = new Mock<IEwsdFilePackageManager>();

        filePackageManagerMock
            .Setup(filePackageManager => filePackageManager.GetPackageArrays(It.IsAny<byte[]>()))
            .Returns(Array.Empty<IEwsdPackage[]>());

        _containerBuilder?.RegisterInstance(filePackageManagerMock.Object).As<IEwsdFilePackageManager>();
        
        var container = _containerBuilder?.Build();
        var serviceProvider = new AutofacServiceProvider(container);
        var fileProcessLogic = serviceProvider.GetService<IEwsdFileProcessLogic>();
        
        Assert.IsNotNull(fileProcessLogic);
        
        fileProcessLogic.Run(_fileParsingTask!);
        
        Assert.AreEqual(0,_savedRecords?.Count);
        Assert.AreEqual(EwsdFileParsingTaskStatuses.NoRecords, _fileParsingTask?.Status);
    }
}