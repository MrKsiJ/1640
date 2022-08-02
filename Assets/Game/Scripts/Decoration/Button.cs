using UnityEngine;
public class Button : MonoBehaviour
{
    private const string PressedNameAnimation = "Pressed";
    private Animator _animation;

    public AudioClip pressedButton;
    // Start is called before the first frame update
    private void Start()
    {
        if(GetComponent<Animator>())
            _animation = GetComponent<Animator>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { Pressed(); });
    }

    public void Pressed()
    {
        if(_animation != null)
         _animation.Play(PressedNameAnimation);
        Media.OnPlayAudio(pressedButton);
    }
}
