using System;
using Android.Content;
using FileStorageApp.Dependencies;
using FileStorageApp.Droid.Dependencies;
using Java.IO;
using Xamarin.Forms;
using Environment = Android.OS.Environment;
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
      // see: https://www.grapecity.com/blogs/how-to-save-an-image-to-a-device-using-xuni-and-xamarin-forms
      /*
      var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
      var pictures = dir.AbsolutePath;
      //adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name  
      string name = filePath + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
      filePath = System.IO.Path.Combine(pictures, name);
      */
      try
      {
        System.IO.File.WriteAllBytes(filePath, data);
        //mediascan adds the saved image into the gallery  
        var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
        mediaScanIntent.SetData(Uri.FromFile(new File(filePath)));
        Xamarin.Forms.Forms.Context.SendBroadcast(mediaScanIntent);
      }
      catch (System.Exception e)
      {
        System.Console.WriteLine(e.ToString());
      }

      return true;
    }
  }
}