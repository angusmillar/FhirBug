using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Attributes
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public sealed class TransactionalAttribute : Attribute
  {
  }
}
