using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public abstract class IndexBase : BaseIntKey
  {
    public int FkResourceStoreId { get; set; }
    public ResourceStore ResourceStore { get; set; }
    public int FkSearchParameterId { get; set; }
    public SearchParameter SearchParameter { get; set; }
  }
}
