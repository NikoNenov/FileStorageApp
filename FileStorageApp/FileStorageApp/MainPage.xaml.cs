/*
 * Convert: https://forums.xamarin.com/discussion/45925/pcl-xamarin-forms-convert-mediafile-into-byte-array
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FileStorageApp.Dependencies;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace FileStorageApp
{
  // Learn more about making custom code visible in the Xamarin.Forms previewer
  // by visiting https://aka.ms/xamarinforms-previewer
  [DesignTimeVisible(false)]
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();
    }

    private async void DebugButton_OnClicked(object sender, EventArgs e)
    {
      Debug.WriteLine($"GetPathMovies: {DependencyService.Get<IFileService>().GetPathMovies()}");
      Debug.WriteLine($"GetPathPictures: {DependencyService.Get<IFileService>().GetPathPictures()}");

      Debug.WriteLine($"CheckPermission<StoragePermission>: {await CheckPermission<StoragePermission>(Permission.Storage)}");
      Debug.WriteLine($"CheckPermission<PhotosPermission>: {await CheckPermission<PhotosPermission>(Permission.Photos)}");
    }

    private async void TakePhotoButton_OnClicked(object sender, EventArgs e)
    {
      var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
      {
        SaveToAlbum = true
      });

      Debug.WriteLine($"TakePhoto -> AlbumPath: {file?.AlbumPath}");
      Debug.WriteLine($"TakePhoto -> Path: {file?.Path}");
    }
    private async void PickPhotoButton_OnClicked(object sender, EventArgs e)
    {
      var file = await CrossMedia.Current.PickPhotoAsync();

      Debug.WriteLine($"PickPhoto -> AlbumPath: {file?.AlbumPath}");
      Debug.WriteLine($"PickPhoto -> Path: {file?.Path}");


      /*
      [0:] PickPhoto -> AlbumPath: content://com.google.android.apps.photos.contentprovider/-1/1/content%3A%2F%2Fmedia%2Fexternal%2Fimages%2Fmedia%2F27809/ORIGINAL/NONE/image%2Fjpeg/1899413141
      [0:] PickPhoto -> Path: /storage/emulated/0/Android/data/com.companyname.filestorageapp/files/Pictures/temp/DSC_0010_1.JPG
      */
      DependencyService.Get<IFileService>().SaveFile(ToByteArray(file), "/storage/emulated/0/Pictures/Niko_01.JPG");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediaFile"></param>
    /// <returns></returns>
    private byte[] ToByteArray(MediaFile mediaFile)
    {
      using (var stream = new MemoryStream())
      {
        mediaFile.GetStream().CopyTo(stream);
        mediaFile.Dispose();
        return stream.ToArray();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="permission"></param>
    /// <returns></returns>
    private async Task<bool> CheckPermission<T>(Permission permission) where T : BasePermission, new()
    {
      var status = PermissionStatus.Unknown;
      try
      {
        status = await CrossPermissions.Current.CheckPermissionStatusAsync<T>();
        if (status != PermissionStatus.Granted)
        {
          if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
          {
            await DisplayAlert("Need location", "Gunna need that location", "OK");
          }

          status = await CrossPermissions.Current.RequestPermissionAsync<T>();
        }
      }
      catch (Exception ex)
      {
        //Something went wrong
        Debug.WriteLine($"Exception: {ex}");
      }

      Debug.WriteLine($"T: {typeof(T)} Permission status: {status}");
      return status == PermissionStatus.Granted;
    }
  }
}
