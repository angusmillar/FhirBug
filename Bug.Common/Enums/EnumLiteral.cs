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
      string name = Enum.GetName(type, value);
      if (name != null)
      {
        FieldInfo field = type.GetField(name);
        if (field != null)
        {
          EnumInfoAttribute attr =
                 Attribute.GetCustomAttribute(field,
                   typeof(EnumInfoAttribute)) as EnumInfoAttribute;
          if (attr != null)
          {
            return attr.Description;
          }
        }
      }
      return null;
    }

    public static string GetLiteral(this Enum value)
    {
      Type type = value.GetType();
      string name = Enum.GetName(type, value);
      if (name != null)
      {
        FieldInfo field = type.GetField(name);
        if (field != null)
        {
          EnumInfoAttribute attr =
                 Attribute.GetCustomAttribute(field,
                   typeof(EnumInfoAttribute)) as EnumInfoAttribute;
          if (attr != null)
          {
            return attr.Literal;
          }
        }
      }
      return null;
    }
  }
}
