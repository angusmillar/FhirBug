using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public sealed class EnumInfoAttribute : Attribute
  {
    readonly string literal;
    readonly string description;

    // This is a positional argument
    public EnumInfoAttribute(string literal, string description)
    {
      this.literal = literal;
      this.description = description;
    }

    public EnumInfoAttribute(string literal)
    {
      this.literal = literal;
      this.description = "Enum description not defined";
    }

    public string Literal
    {
      get { return literal; }
    }
    public string Description
    {
      get { return description; }
    }

  }
}
