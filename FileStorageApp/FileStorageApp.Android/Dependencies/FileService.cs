using System;
using Android.Content;
using Android.Webkit;
using Android.Widget;
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

    /// <summary>
    /// Open image in android with default App (if defined)
    /// see: https://stackoverflow.com/questions/41338477/how-to-write-a-method-to-open-an-image-in-c-sharp-that-will-use-the-default-imag
    /// </summary>
    /// <param name="filePath"></param>
    public void OpenFile(string filePath)
    {
      /*
      string dirPath = Xamarin.Forms.Forms.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).Path;
      var FileName = filePath;
      Java.IO.File file = new Java.IO.File(dirPath, FileName);
      */
      byte[] data = null;
      var file = new Java.IO.File(filePath);

      Debug.WriteLine($"File path: {filePath}");
      Debug.WriteLine($"File exists: {file.Exists()}");
      if (!file.Exists())
      {
        data = System.IO.File.ReadAllBytes(filePath);
        //System.IO.File.WriteAllBytes(filePath, data);
      }

      Device.BeginInvokeOnMainThread(() =>
      {
        //var oDir = Xamarin.Forms.Forms.Context.FilesDir.AbsolutePath;
        Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
        Intent intent = new Intent(Intent.ActionView);
        String mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(MimeTypeMap.GetFileExtensionFromUrl((string)uri).ToLower());
        intent.SetDataAndType(uri, mimeType);

        intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

        try
        {
          Xamarin.Forms.Forms.Context.StartActivity(intent);
        }
        catch (System.Exception ex)
        {
          Toast.MakeText(Xamarin.Forms.Forms.Context, "No Application Available to View this file", ToastLength.Short).Show();
        }
      });
    }
  }
}