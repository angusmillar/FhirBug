using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Dto.Indexing
{
  public class IndexToken : IndexBase
  {
    public IndexToken(int fkSearchParameterId) 
      : base(fkSearchParameterId)
    {
    }

    public string? Code { get; set; }
    public string? System { get; set; }
  }
}
