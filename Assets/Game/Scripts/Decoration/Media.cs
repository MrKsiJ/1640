using UnityEngine;

public class Media : MonoBehaviour
{
    public static void OnPlayAudio(AudioClip audio)
    {
        GameObject createSound = new GameObject(audio.name);
        createSound.AddComponent<AudioSource>();
        createSound.GetComponent<AudioSource>().clip = audio;
        createSound.GetComponent<AudioSource>().Play();
        Destroy(createSound,audio.length);
    }
}
