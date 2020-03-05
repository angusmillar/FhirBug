using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public abstract class ModelBase
  {    
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
  }
}
