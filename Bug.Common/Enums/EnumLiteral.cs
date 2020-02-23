using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Bug.Common.Enums
{
  public static class EnumLiteral
  {
    public static string GetDescription(this Enum value)
    {
      Type type = value.GetType();
      string? name = Enum.GetName(type, value);
      if (name is object)
      {
        FieldInfo? field = type.GetField(name);
        if (field is object)
        {
          EnumInfoAttribute? attr =
                 Attribute.GetCustomAttribute(field,
                   typeof(EnumInfoAttribute)) as EnumInfoAttribute;
          if (attr is object)
          {
            return attr.Description;
          }
        }
      }
      return string.Empty;
    }

    public static string GetCode(this Enum value)
    {
      Type type = value.GetType();
      string? name = Enum.GetName(type, value);
      if (name is object)
      {
        FieldInfo? field = type.GetField(name);
        if (field is object)
        {
          EnumInfoAttribute? attr =
                 Attribute.GetCustomAttribute(field,
                   typeof(EnumInfoAttribute)) as EnumInfoAttribute;
          if (attr is object)
          {
            return attr.Literal;
          }
        }
      }
      return string.Empty;
    }
  }
}
