using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public abstract class BaseIntKey : BaseDateStamp
  {
    public int Id { get; set; }
  }
}
