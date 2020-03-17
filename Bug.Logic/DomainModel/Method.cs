using Bug.Common.Enums;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public class Method : BaseEnumKey<HttpVerb>
  {    
    public string Code { get; set; }
  }
}
