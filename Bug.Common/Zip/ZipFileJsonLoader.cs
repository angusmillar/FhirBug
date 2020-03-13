using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;

namespace Bug.Common.Zip
{
  public class ZipFileJsonLoader : IZipFileJsonLoader
  {
    public JsonReader Load(byte[] ZipFileBytes, string fileNameRequired)
    {
      Stream FileStream = new MemoryStream(ZipFileBytes);
      try
      {
        using (ZipArchive Archive = new ZipArchive(FileStream))
        {
          foreach (ZipArchiveEntry Entry in Archive.Entries)
          {
            if (Entry.FullName.Equals(fileNameRequired, StringComparison.OrdinalIgnoreCase))
            {
              Stream StreamItem = Entry.Open();
              using (StreamItem)
              {
                try
                {
                  var buffer = new MemoryStream();
                  StreamItem.CopyTo(buffer);
                  buffer.Seek(0, SeekOrigin.Begin);
                  return Hl7.Fhir.Utility.SerializationUtil.JsonReaderFromStream(buffer);
                }
                catch (Exception Exec)
                {
                  throw new Exception($"Exception thrown when de-serializing to json the file named {fileNameRequired}. See inner exception for more info.", Exec);
                }
              }
            }
          }
          throw new Exception($"Unable to locate the file named {fileNameRequired} with in the provided zip file byte array. Check the file is found in the zip file being targeted.");
        }
      }
      catch (Exception Exec)
      {
        string ErrorMessage = $"Exception thrown when attempting to unzip the zip file byte array in order to find the file named : {fileNameRequired}. See inner exception for more info.";
        throw new Exception(ErrorMessage, Exec);
      }
    }
  }
}
