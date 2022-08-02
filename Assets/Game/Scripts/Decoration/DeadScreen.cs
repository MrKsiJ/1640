using UnityEngine;

public class DeadScreen : MonoBehaviour
{
    public AudioClip resultPick;

    public void PickSoundOn()
    {
        Media.OnPlayAudio(resultPick);
    }
}
