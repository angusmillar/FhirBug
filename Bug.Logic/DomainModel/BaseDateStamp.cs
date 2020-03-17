using System;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public abstract class BaseDateStamp
  {    
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
  }
}
