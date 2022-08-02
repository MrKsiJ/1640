using System.Collections;
using UnityEngine;
using System.IO;

public class ShareButton : MonoBehaviour
{

    private const string Subject = "1640 Game";
    private const string Message = "Посмотри на мой рекорд! Игры от K.J.Pi Games: vk.com/kjpigames";
    private const string LegendaryMessage = "Я прошёл игру 1640 и получил за это щедрую награду! Посмотри! Игры от K.J.Pi Games: vk.com/kjpigames";

    public UnityEngine.UI.Button share;
    public bool Legendary = false;
    public GameObject deadText;
    private void Start()
    {
        share.onClick.AddListener(ShareButtonClick);
    }

    public void ShareButtonClick()
    {
        if(!Legendary)
            deadText.SetActive(false);
        StartCoroutine(TakeSSAndShare());
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        var ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        var filePath = System.IO.Path.Combine(Application.temporaryCachePath, "ScreenScore.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);
        new NativeShare().AddFile(filePath).SetSubject(Subject).SetText(Message).Share();
    }
}