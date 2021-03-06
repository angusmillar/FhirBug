﻿using Bug.Logic.Query;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Delete;
using Bug.Logic.Query.FhirApi.HistoryBase;
using Bug.Logic.Query.FhirApi.HistoryInstance;
using Bug.Logic.Query.FhirApi.HistoryResource;
using Bug.Logic.Query.FhirApi.Read;
using Bug.Logic.Query.FhirApi.Search;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Query.FhirApi.VRead;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirApiQueryHandlerFactory
  {
    IQueryHandler<UpdateQuery, FhirApiTransactionalResult> GetUpdateCommand();
    IQueryHandler<CreateQuery, FhirApiTransactionalResult> GetCreateCommand();
    IQueryHandler<DeleteQuery, FhirApiTransactionalResult> GetDeleteCommand();
    IQueryHandler<ReadQuery, FhirApiResult> GetReadCommand();
    IQueryHandler<VReadQuery, FhirApiResult> GetVReadCommand();
    IQueryHandler<HistoryInstanceQuery, FhirApiResult> GetHistoryInstanceCommand();
    IQueryHandler<HistoryResourceQuery, FhirApiResult> GetHistoryResourceCommand();
    IQueryHandler<HistoryBaseQuery, FhirApiResult> GetHistoryBaseCommand();
    IQueryHandler<SearchQuery, FhirApiResult> GetSearchCommand();


  }
}