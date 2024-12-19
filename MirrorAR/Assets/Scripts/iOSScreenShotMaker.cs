using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static NativeGallery;
using static NativeGallery.MediaSaveCallback;

public class iOSScreenShotMaker : MonoBehaviour
{
    public Button screenshotButton;
    public AudioClip screenshotSound;
    public AudioSource audioSource;

    void Start()
    {
        screenshotButton.onClick.AddListener(SaveScreenshotOnClick);
    }

    public void SaveScreenshotOnClick()
    {
        SetButtonAlpha(0f);
        StartCoroutine(TakeScreenshotAndSave());
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        audioSource.PlayOneShot(screenshotSound);
        yield return new WaitForEndOfFrame();
        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);
        SetButtonAlpha(1f);
        Destroy(ss);
    }
    private void SetButtonAlpha(float alpha)
    {
        Color currentColor = screenshotButton.image.color;
        currentColor.a = alpha;
        screenshotButton.image.color = currentColor;
    }
}
