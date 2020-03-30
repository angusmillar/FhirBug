using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.Transaction;
using Microsoft.Extensions.Primitives;

namespace Bug.Logic.Query.FhirApi
{
  public class FhirApiTransactionalResult : FhirApiResult, ITransactional
  {
    public FhirApiTransactionalResult(HttpStatusCode HttpStatusCode, FhirVersion FhirVersion, string CorrelationId, bool CommitTransaction = false) 
      : base(HttpStatusCode, FhirVersion, CorrelationId)
    {
      this.CommitTransaction = CommitTransaction;
    }
    public bool CommitTransaction { get; set; }
  }
}
