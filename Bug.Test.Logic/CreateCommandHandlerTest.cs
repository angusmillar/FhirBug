using Bug.Common.Compression;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Interfaces.Repository;
using Bug.R4Fhir.Serialization;
using Bug.Stu3Fhir.Serialization;
using Moq;
using System;
using Xunit;
using Bug.Common.Enums;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query;
using Bug.Common.FhirTools;

namespace Bug.Test.Logic
{
  
  public class CreateCommandTest
  {
    [Fact]
    public void TestDateTime()
    {
      DateTimeOffset TestDateTime = new DateTimeOffset(2020, 01, 05, 11, 30, 20, new TimeSpan(10, 0, 0));
      DateTimeOffset TestDateTimeZulu = new DateTimeOffset(2020, 01, 05, 01, 30, 20, new TimeSpan(0, 0, 0)).UtcDateTime;

      DateTime Result = TestDateTime.UtcDateTime;
      Assert.Equal(Result, TestDateTimeZulu);
    }



    //[Theory]
    //[InlineData("Dummy Resource", "5", "1", FhirVersion.Stu3)]
    //[InlineData("Dummy Resource", "6", "4", FhirVersion.R4)]
    //public async void CreateCommandHandler(string Resource, string ResourceId, string ResourceVersionId, FhirVersion FhirMajorVersion)
    //{
    //  // Prepare
    //  Mock<IResourceStoreRepository> IResourceStoreRepositoryMock = new Mock<IResourceStoreRepository>();
    //  IResourceStoreRepositoryMock.Setup(x =>
    //  x.Add(It.IsAny<Bug.Logic.DomainModel.ResourceStore>()));

    //  Mock<IStu3SerializationToJsonBytes> IStu3SerializationToJsonBytesMock = new Mock<IStu3SerializationToJsonBytes>();
    //  IStu3SerializationToJsonBytesMock.Setup(x =>
    //  x.SerializeToJsonBytes(It.IsAny<IFhirResourceStu3>(), It.IsAny<SummaryType>())).Returns(new byte[10]);

    //  Mock<IR4SerializationToJsonBytes> IR4SerializationToJsonBytesMock = new Mock<IR4SerializationToJsonBytes>();
    //  IR4SerializationToJsonBytesMock.Setup(x =>
    //  x.SerializeToJsonBytes(It.IsAny<IFhirResourceR4>(), It.IsAny<SummaryType>())).Returns(new byte[10]);

    //  Mock<IGZipper> IGZipperMock = new Mock<IGZipper>();
    //  IGZipperMock.Setup(x =>
    //  x.Compress(It.IsAny<byte[]>()));

    //  // Act
    //  CreateQueryHandler CommandHandler = new CreateQueryHandler(IResourceStoreRepositoryMock.Object, IStu3SerializationToJsonBytesMock.Object, IR4SerializationToJsonBytesMock.Object, IGZipperMock.Object);
    //  CreateQuery Command = new CreateQuery()
    //  {
    //    Resource = Resource,
    //    FhirId = ResourceId,
    //    VersionId = ResourceVersionId,
    //    FhirMajorVersion = FhirMajorVersion,
    //    LastUpdated = new DateTimeOffset(2020, 02, 02, 10, 10, 10, new TimeSpan(10, 0, 0))
    //  };
    //  var Result = await CommandHandler.Handle(Command);

    //  //Assert
    //  Assert.Equal(ResourceId, Result.ResourceId);
    //  Assert.Equal(ResourceVersionId, Result.ResourceVersionId);
    //  Assert.Equal(FhirMajorVersion, Result.FhirMajorVersion);
    //  Assert.IsType<string>(Result.Resource);
    //  Assert.Equal(Resource, (string)Result.Resource);
    //}




    //[Theory]
    //[InlineData("Dummy Resource STU3", FhirMajorVersion.Stu3)]
    //[InlineData("Dummy Resource R4", FhirMajorVersion.R4)]
    //public async void CreateDataCollectionDecorator(string Resource, FhirMajorVersion FhirMajorVersion)
    //{
    //  // Prepare           
    //  Mock<IQueryHandler<CreateQuery, FhirApiResult>> ICommandHandlerMock = new Mock<IQueryHandler<CreateQuery, FhirApiResult>>();
    //  ICommandHandlerMock.Setup(x => x.Handle(It.IsAny<CreateQuery>()));     

    //  Mock<Bug.Stu3Fhir.ResourceSupport.IFhirResourceIdSupport> IStu3FhirResourceIdSupportMock = new Mock<Bug.Stu3Fhir.ResourceSupport.IFhirResourceIdSupport>();      
    //  Mock<Bug.R4Fhir.ResourceSupport.IFhirResourceIdSupport> IR4FhirResourceIdSupportMock = new Mock<Bug.R4Fhir.ResourceSupport.IFhirResourceIdSupport>();      
    //  Mock<IFhirResourceIdSupportFactory> IFhirResourceIdSupportFactoryMock = new Mock<IFhirResourceIdSupportFactory>();
    //  IFhirResourceIdSupportFactoryMock.Setup(x => x.GetStu3()).Returns(IStu3FhirResourceIdSupportMock.Object);
    //  IFhirResourceIdSupportFactoryMock.Setup(x => x.GetR4()).Returns(IR4FhirResourceIdSupportMock.Object);

    //  Mock<Bug.Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport> IStu3FhirResourceVersionSupportMock = new Mock<Bug.Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport>();      
    //  Mock<Bug.R4Fhir.ResourceSupport.IFhirResourceVersionSupport> IR4FhirResourceVersionSupportMock = new Mock<Bug.R4Fhir.ResourceSupport.IFhirResourceVersionSupport>();      
    //  Mock<IFhirResourceVersionSupportFactory> IFhirResourceVersionSupportFactoryMock = new Mock<IFhirResourceVersionSupportFactory>();
    //  IFhirResourceVersionSupportFactoryMock.Setup(x => x.GetStu3()).Returns(IStu3FhirResourceVersionSupportMock.Object);
    //  IFhirResourceVersionSupportFactoryMock.Setup(x => x.GetR4()).Returns(IR4FhirResourceVersionSupportMock.Object);

    //  Mock<Bug.Stu3Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport> IStu3FhirResourceLastUpdatedSupportMock = new Mock<Bug.Stu3Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport>();      
    //  Mock<Bug.R4Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport> IR4FhirResourceLastUpdatedSupportMock = new Mock<Bug.R4Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport>();     
    //  Mock<IFhirResourceLastUpdatedSupportFactory> IFhirResourceLastUpdatedSupportFactoryMock = new Mock<IFhirResourceLastUpdatedSupportFactory>();
    //  IFhirResourceLastUpdatedSupportFactoryMock.Setup(x => x.GetStu3()).Returns(IStu3FhirResourceLastUpdatedSupportMock.Object);
    //  IFhirResourceLastUpdatedSupportFactoryMock.Setup(x => x.GetR4()).Returns(IR4FhirResourceLastUpdatedSupportMock.Object);

    //  // Act
    //  var CommandDecorator = new CreateDataCollectionDecorator<CreateQuery, FhirApiResult>(ICommandHandlerMock.Object, IFhirResourceIdSupportFactoryMock.Object, IFhirResourceVersionSupportFactoryMock.Object, IFhirResourceLastUpdatedSupportFactoryMock.Object);

    //  CreateQuery CreateCommand = new CreateQuery()
    //  {       
    //    FhirMajorVersion = FhirMajorVersion,     
    //    Resource = Resource
    //  };
    //  var Result = await CommandDecorator.Handle(CreateCommand);

    //  //Assert
    //  //The key object under test here is the CreateDataCollectionDecorator and we must test the properties it has set on the 
    //  //CreateCommand object that is being passed down through the Decorators to the final command.
    //  //We have no concern about the retuned FhirApiResult here.
    //  Assert.NotNull(CreateCommand.FhirId);
    //  Assert.Equal("1", CreateCommand.VersionId);
    //  Assert.Equal(FhirMajorVersion, CreateCommand.FhirMajorVersion);
    //  Assert.InRange(CreateCommand.LastUpdated, DateTimeOffset.Now.AddSeconds(-20), DateTimeOffset.Now);
    //  Assert.IsType<string>(CreateCommand.Resource);
    //  Assert.Equal(Resource, (string)CreateCommand.Resource);
    //}
  }
}
