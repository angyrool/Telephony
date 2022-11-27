using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telephony.EwsdParser.BusinessLogic;

namespace Telephony.EwsdParser.Tests.BusinessLogic;

[TestClass]
public class EwsdFileProcessLogicTests
{
    private readonly ContainerBuilder? _containerBuilder;

    public EwsdFileProcessLogicTests()
    {
        var configurationBuilder = new ConfigurationBuilder();
        var configurationRoot = configurationBuilder.Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IEwsdFileProcessLogic, EwsdFileProcessLogic>();
        services.Configure<EwsdSettings>(configurationRoot.GetSection(nameof(EwsdSettings)));

        _containerBuilder = new ContainerBuilder();
        _containerBuilder.Populate(services);
    }

    [TestMethod]
    public void EwsdFileProcessLogic_Run_Processed_Success_Test()
    {
        var parsingTask = new EwsdFileParsingTask()
        {
            File = new EwsdFile()
            {
                Name = "123",
                Path = "c://ewsd/123"
            },
            Status = EwsdFileParsingTaskStatuses.New
        };
        
        var fileParsingTaskManagerMock = new Mock<IEwsdFileParsingTaskManager>();
        
        fileParsingTaskManagerMock
            .Setup(ewsdFileParsingTaskManager => ewsdFileParsingTaskManager.GetNew())
            .Returns(parsingTask);
        
        fileParsingTaskManagerMock
            .Setup(ewsdFileParsingTaskManager => ewsdFileParsingTaskManager
                .SetStatus(It.IsAny<EwsdFileParsingTask>(), It.IsAny<EwsdFileParsingTaskStatuses>()))
            .Callback((EwsdFileParsingTask task, EwsdFileParsingTaskStatuses status) =>
            {
                task.Status = status;
            });
        
        _containerBuilder?.RegisterInstance(fileParsingTaskManagerMock.Object).As<IEwsdFileParsingTaskManager>();


        var fileSystemMock = new Mock<IFileSystem>();

        fileSystemMock
            .Setup(fileSystem => fileSystem.IsFileExists(It.IsAny<string>()))
            .Returns(true);
        
        fileSystemMock
            .Setup(fileSystem => fileSystem.GetFileBytes(It.IsAny<string>()))
            .Returns(new byte[]{ 1,2,3,4,5 });
        
        _containerBuilder?.RegisterInstance(fileSystemMock.Object).As<IFileSystem>();


        var filePackageManagerMock = new Mock<IEwsdFilePackageManager>();

        filePackageManagerMock
            .Setup(filePackageManager => filePackageManager.GetPackageArrays(It.IsAny<byte[]>()))
            .Returns(new[] {new IEwsdPackage[]{ new EwsdPackage(), new EwsdPackage() }});
        
        _containerBuilder?.RegisterInstance(filePackageManagerMock.Object).As<IEwsdFilePackageManager>();

        
        var savedRecords = new List<EwsdRecord>();
        var recordsManagerMock = new Mock<IEwsdRecordsManager>();
        
        recordsManagerMock
            .Setup(recordsManager => recordsManager.Add(It.IsAny<IEnumerable<EwsdRecord>>()))
            .Callback((IEnumerable<EwsdRecord> records) =>
            {
                savedRecords.AddRange(records);
            });
        
        _containerBuilder?.RegisterInstance(recordsManagerMock.Object).As<IEwsdRecordsManager>();

        
        var container = _containerBuilder?.Build();
        var serviceProvider = new AutofacServiceProvider(container);
        var fileProcessLogic = serviceProvider.GetService<IEwsdFileProcessLogic>();
        
        Assert.IsNotNull(fileProcessLogic);
        
        fileProcessLogic.Run();
        
        Assert.AreEqual(1,savedRecords.Count);
        Assert.AreEqual(EwsdFileParsingTaskStatuses.Processed,parsingTask.Status);
    }
    
    [TestMethod]
    public void EwsdFileProcessLogic_Run_Processed_NoFile_Test()
    {
        var parsingTask = new EwsdFileParsingTask()
        {
            File = new EwsdFile()
            {
                Name = "123",
                Path = "c://ewsd/123"
            },
            Status = EwsdFileParsingTaskStatuses.New
        };
        
        var fileParsingTaskManagerMock = new Mock<IEwsdFileParsingTaskManager>();
        
        fileParsingTaskManagerMock
            .Setup(ewsdFileParsingTaskManager => ewsdFileParsingTaskManager.GetNew())
            .Returns(parsingTask);
        
        fileParsingTaskManagerMock
            .Setup(ewsdFileParsingTaskManager => ewsdFileParsingTaskManager
                .SetStatus(It.IsAny<EwsdFileParsingTask>(), It.IsAny<EwsdFileParsingTaskStatuses>()))
            .Callback((EwsdFileParsingTask task, EwsdFileParsingTaskStatuses status) =>
            {
                task.Status = status;
            });
        
        _containerBuilder?.RegisterInstance(fileParsingTaskManagerMock.Object).As<IEwsdFileParsingTaskManager>();


        var fileSystemMock = new Mock<IFileSystem>();

        fileSystemMock
            .Setup(fileSystem => fileSystem.IsFileExists(It.IsAny<string>()))
            .Returns(false);
        
        fileSystemMock
            .Setup(fileSystem => fileSystem.GetFileBytes(It.IsAny<string>()))
            .Returns(new byte[]{ 1,2,3,4,5 });
        
        _containerBuilder?.RegisterInstance(fileSystemMock.Object).As<IFileSystem>();


        var filePackageManagerMock = new Mock<IEwsdFilePackageManager>();

        filePackageManagerMock
            .Setup(filePackageManager => filePackageManager.GetPackageArrays(It.IsAny<byte[]>()))
            .Returns(new[] {new IEwsdPackage[]{ new EwsdPackage(), new EwsdPackage() }});
        
        _containerBuilder?.RegisterInstance(filePackageManagerMock.Object).As<IEwsdFilePackageManager>();

        
        var savedRecords = new List<EwsdRecord>();
        var recordsManagerMock = new Mock<IEwsdRecordsManager>();
        
        recordsManagerMock
            .Setup(recordsManager => recordsManager.Add(It.IsAny<IEnumerable<EwsdRecord>>()))
            .Callback((IEnumerable<EwsdRecord> records) =>
            {
                savedRecords.AddRange(records);
            });
        
        _containerBuilder?.RegisterInstance(recordsManagerMock.Object).As<IEwsdRecordsManager>();

        
        var container = _containerBuilder?.Build();
        var serviceProvider = new AutofacServiceProvider(container);
        var fileProcessLogic = serviceProvider.GetService<IEwsdFileProcessLogic>();
        
        Assert.IsNotNull(fileProcessLogic);
        
        fileProcessLogic.Run();
        
        Assert.AreEqual(0,savedRecords.Count);
        Assert.AreEqual(EwsdFileParsingTaskStatuses.NoFile,parsingTask.Status);
    }
}