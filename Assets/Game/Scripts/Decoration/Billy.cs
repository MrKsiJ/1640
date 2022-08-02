using System.Collections;
using UnityEngine;

public class Billy : MonoBehaviour
{
    #region AnimationConstRegion
    private const string NameParametrIdleAnimation = "IdleAnimation";
    private const string NameAnimationTalk = "Talk";
    private const string NameAnimationIdleStop = "IdleEyesStop";
    #endregion

    private Coroutine _typeAnimationChanged;
    [Header("Other Settings")] 
    public Loader loader;
    [Header("Object Settings")] 
    public GameObject mainMenu;

    [Header("Billy Settings")] 
    public Animator billyAnimator;

    public Cube billyCube;

    public void ChangedAnimation(int animationIndex)
    {
        if(_typeAnimationChanged != null)
            StopCoroutine(_typeAnimationChanged);
        switch (animationIndex)
        {
            case 0:
                billyAnimator.Play(NameAnimationIdleStop);
                break;
            case 1:
                billyAnimator.Play(NameAnimationTalk);
                break;
        }
    }

    public void StartTypeAnimationChanged()
    {
        _typeAnimationChanged = StartCoroutine(TypeAnimationChanged());
    }
    
    public void WelcomeBack()
    {
        mainMenu.SetActive(true);
        billyCube.enabled = true;
        billyAnimator.enabled = false;
        StartTypeAnimationChanged();
        StopCoroutine(loader._camChanged);
        StopCoroutine(loader._skyBoxChanger);
    }

    public void NulledIdleAnimation()
    {
        billyAnimator.SetInteger(NameParametrIdleAnimation,0);
    }

    private IEnumerator TypeAnimationChanged()
    {
        var t = Random.Range(1f,2f);
        while (t > 0)
        {
            t -= Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }

        if (t <= 0)
            ChangedAnimationBilly();
    }

    private void ChangedAnimationBilly()
    {
        var index = Random.Range(1, 5);
        billyAnimator.SetInteger(NameParametrIdleAnimation,index);
        StopCoroutine(_typeAnimationChanged);
        _typeAnimationChanged = StartCoroutine(TypeAnimationChanged());
    }
}
