using System.Collections.Generic;

namespace Bug.Common.Enums
{
  public abstract class MapBase<InputEnumType, ReturnEnumType> 
    where ReturnEnumType : System.Enum
    where InputEnumType : System.Enum
  {
    protected abstract Dictionary<InputEnumType, ReturnEnumType> Map { get; }     

    public ReturnEnumType Get(InputEnumType value)
    {
      if (Map.ContainsKey(value))
      {
        return Map[value];
      }
      else
      {
        string Message = $"Unable to convert {nameof(value)} of type {value.GetType().Name} enum to the required return type.";
        throw new Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, Message);
      }
    }
  }
}
