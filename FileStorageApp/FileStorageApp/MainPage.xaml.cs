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

    /// <summary>
    /// Get android movie and picture path
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetPathsButton_OnClicked(object sender, EventArgs e)
    {
      Debug.WriteLine($"GetPathMovies: {DependencyService.Get<IFileService>().GetPathMovies()}");
      Debug.WriteLine($"GetPathPictures: {DependencyService.Get<IFileService>().GetPathPictures()}");
    }

    /// <summary>
    /// Check permissions event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CheckPermission_OnClicked(object sender, EventArgs e)
    {
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

    /// <summary>
    /// Pick and save photo 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void PickAndSavePhotoButton_OnClicked(object sender, EventArgs e)
    {
      var file = await CrossMedia.Current.PickPhotoAsync();
      Debug.WriteLine($"PickPhoto -> AlbumPath: {file?.AlbumPath}");
      Debug.WriteLine($"PickPhoto -> Path: {file?.Path}");

      if(file != null)
      {
        DependencyService.Get<IFileService>().SaveFile(ToByteArray(file), DependencyService.Get<IFileService>().GetPathPictures());
      }
    }

    /// <summary>
    /// Open dummy photo with default App (if defined)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenPhotoButton_OnClicked(object sender, EventArgs e)
    {
      DependencyService.Get<IFileService>().OpenFile(Path.Combine(DependencyService.Get<IFileService>().GetPathPictures(), "Niko_01.JPG"));
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
