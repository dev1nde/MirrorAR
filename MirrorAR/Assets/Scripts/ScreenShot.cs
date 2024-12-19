using UnityEngine;
using System.Collections;
using System.IO;
 public class ScreenShot : MonoBehaviour
{
    public string screenshotPath = "Screenshots/";
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
     public ImageFormat imageFormat = ImageFormat.PNG;
          public int jpegQuality = 100;
          public float captureDelay = 0.0f;
     public enum ImageFormat
    {
        PNG,
        JPEG
    }
          public void CaptureScreenshotOnClick()
    {
        StartCoroutine(CaptureScreenshot());
    }
     IEnumerator CaptureScreenshot()
    {
        yield return new WaitForSeconds(captureDelay);
                  string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = "Screenshot_" + timestamp;
         string path = Path.Combine(Application.dataPath, screenshotPath);
        Directory.CreateDirectory(path);
         string fullPath = Path.Combine(path, fileName);
                  Screen.SetResolution(resolutionWidth, resolutionHeight, false);
                  yield return new WaitForEndOfFrame();
         Texture2D screenshot = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        screenshot.Apply();
                  byte[] bytes;
         if (imageFormat == ImageFormat.PNG)
        {
            bytes = screenshot.EncodeToPNG();
        }
        else
        {
            bytes = screenshot.EncodeToJPG(jpegQuality);
        }
         File.WriteAllBytes(fullPath + "." + imageFormat.ToString().ToLower(), bytes);
         Debug.Log("Screenshot saved: " + fullPath + "." + imageFormat.ToString().ToLower());
                  Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }
}
