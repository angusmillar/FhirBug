using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public abstract class BaseIntKey : ModelBase
  {
    public int Id { get; set; }
  }
}
