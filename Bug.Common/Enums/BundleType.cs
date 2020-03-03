using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum BundleType
  {
    [EnumInfo("Document", "Document")]
    Document = 0,
    [EnumInfo("Message", "Message")]
    Message = 1,
    [EnumInfo("Transaction", "Transaction")]
    Transaction = 2,
    [EnumInfo("TransactionResponse", "TransactionResponse")]
    TransactionResponse = 3,
    [EnumInfo("Batch", "Batch")]
    Batch = 4,
    [EnumInfo("BatchResponse", "BatchResponse")]
    BatchResponse = 5,
    [EnumInfo("History", "History")]
    History = 6,
    [EnumInfo("Searchset", "Searchset")]
    Searchset = 7,
    [EnumInfo("Collection", "Collection")]
    Collection = 8
  }
}
