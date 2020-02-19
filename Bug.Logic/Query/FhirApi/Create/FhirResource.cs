using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.Create
{
  public class FhirResource<TResource>
  {
    public TResource Resource { get; set; } 
  }
}
