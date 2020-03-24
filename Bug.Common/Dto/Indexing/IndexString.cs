using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Dto.Indexing
{
  public class IndexString : IndexBase
  {
    public IndexString(int fkSearchParameterId, string value) 
      : base(fkSearchParameterId)
    {
      this.String = value;
    }

    public string String { get; set; }
  }
}
