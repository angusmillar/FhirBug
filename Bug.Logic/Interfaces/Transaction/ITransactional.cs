using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Interfaces.Transaction
{
  public interface ITransactional
  {
    public bool CommitTransaction { get; }
  }
}
