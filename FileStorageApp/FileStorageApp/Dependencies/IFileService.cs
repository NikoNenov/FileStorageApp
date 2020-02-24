namespace FileStorageApp.Dependencies
{
  public interface IFileService
  {
    string GetPathMovies();
    string GetPathPictures();

    bool SaveFile(byte[] data, string filePath);
    void OpenFile(string filePath);
  }
}
