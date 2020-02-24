using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileStorageApp.Dependencies
{
  public interface IFileService
  {
    string GetPathMovies();

    string GetPathPictures();

    bool SaveFile(byte[] data, string filePath);
  }
}
