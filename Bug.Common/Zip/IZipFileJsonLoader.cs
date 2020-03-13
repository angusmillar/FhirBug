using Newtonsoft.Json;

namespace Bug.Common.Zip
{
  public interface IZipFileJsonLoader
  {
    JsonReader Load(byte[] ZipFileBytes, string fileNameRequired);
  }
}