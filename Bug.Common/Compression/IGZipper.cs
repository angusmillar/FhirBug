namespace Bug.Common.Compression
{
  public interface IGZipper
  {
    byte[] Compress(byte[] InputBytes);
    string Decompress(byte[] InputBytes);
  }
}