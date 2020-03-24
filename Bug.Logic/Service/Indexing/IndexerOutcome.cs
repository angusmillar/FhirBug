using Bug.Common.Dto.Indexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Indexing
{
  public class IndexerOutcome
  {
    public List<IndexDateTime> DateTimeIndexList { get; set; }
    public List<IndexQuantity> QuantityIndexList { get; set; }
    public List<IndexReference> ReferenceIndexList { get; set; }
    public List<IndexString> StringIndexList { get; set; }
    public List<IndexToken> TokenIndexList { get; set; }
    public List<IndexUri> UriIndexList { get; set; }

    public IndexerOutcome()
    {
      this.DateTimeIndexList = new List<IndexDateTime>();
      this.QuantityIndexList = new List<IndexQuantity>();
      this.ReferenceIndexList = new List<IndexReference>();
      this.StringIndexList = new List<IndexString>();
      this.TokenIndexList = new List<IndexToken>();
      this.UriIndexList = new List<IndexUri>();
    }
  }
}
