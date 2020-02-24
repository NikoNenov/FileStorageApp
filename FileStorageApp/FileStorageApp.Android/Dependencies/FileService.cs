using System;
using Android.Content;
using FileStorageApp.Dependencies;
using FileStorageApp.Droid.Dependencies;
using Java.IO;
using Xamarin.Forms;
using Debug = System.Diagnostics.Debug;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(FileService))]
namespace FileStorageApp.Droid.Dependencies
{
  public class FileService : IFileService
  {
    public string GetPathMovies() => Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMovies).Path;

    public string GetPathPictures() => Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures).Path;

    public bool SaveFile(byte[] data, string filePath)
    {
      var saveStatus = SaveFileMethodA(data, filePath);


      Debug.WriteLine($"[SaveFile] Save status: {saveStatus}");
      return saveStatus;
    }

    /// <summary>
    /// Android save method
    /// see: https://www.grapecity.com/blogs/how-to-save-an-image-to-a-device-using-xuni-and-xamarin-forms
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private bool SaveFileMethodA(byte[] data, string filePath)
    {
      try
      {
        filePath = Path.Combine(filePath, "Test_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg");

        System.IO.File.WriteAllBytes(filePath, data);
        //mediascan adds the saved image into the gallery  
        var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
        mediaScanIntent.SetData(Uri.FromFile(new File(filePath)));
        Forms.Context.SendBroadcast(mediaScanIntent);
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine($"[SaveFileMethodA] Exception: {e}");
        return false;
      }
    }
  }
}