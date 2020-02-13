using System.IO;
using System.IO.Compression;

namespace Bug.Common.Compression
{
  public class GZipper : IGZipper
  {
    public byte[] Compress(byte[] InputBytes)
    {
      using (var outputStream = new MemoryStream())
      {
        using (var gZipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
        {
          gZipStream.Write(InputBytes, 0, InputBytes.Length);
        }
        return outputStream.ToArray();
      }
    }

    public string Decompress(byte[] InputBytes)
    {
      using (MemoryStream MemStream = new MemoryStream(InputBytes))
      {
        using (var Decompress = new GZipStream(MemStream, CompressionMode.Decompress))
        {
          using (var StreamReader = new StreamReader(Decompress))
          {
            return StreamReader.ReadToEnd();
          }
        }
      }
    }
  }
}
