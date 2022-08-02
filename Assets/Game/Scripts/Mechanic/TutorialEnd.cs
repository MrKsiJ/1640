using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = System.Random;

public class TutorialEnd : MonoBehaviour
{
    #region NamesAnimationsConst
    private const string NameAnimationCountWalksTrue = "CountWalksTrue";
    private const string NameAnimationDeadInside= "DeadInside";
    #endregion

    private readonly string _nameAnimationTutorial = "Tutorial";
    private Coroutine _typeSentence;
    private Coroutine _solution;
    private int _indexDialog = 0;
    private int _generateNumeric = 0;
    private string _countWalksKey = "";
    private string _deadKey = "";
    private string _currentRankPlaceKey = "";
    private string _timeLeftKey = "";
    private string _levelCompleteKey = "";
    private string _backResultMmKey = "";
    private char[] _solutions;
    private bool _solutionsDemo = false;

    [Header("Tutorial Settings")] 
    public UnityEngine.UI.Button buttonNextTutorialDialog;

    public UnityEngine.UI.Button buttonSkipTutorial;
    public UnityEngine.UI.Button buttonCompleteGame;
    public AudioSource masterAudio;
    public PlayerStats playerStats;
    public Gameplay gamePlay;
    public Loader loader;
    public Menu menu;

    public Text tutorialEndText;
    public Text resultText;
    public Text playerResultText;
    public Text solutionText;
    public Text countWalksText;
    public Text deadText;
    public Text currentRankInTable;
    public Text rank;
    public Text timeLeftGameSession;
    public Text levelsComplete;
    public Text levelsCompleteOld;
    [Header("Tip Settings")] 
    public GameObject tipQuadLeftDisplay;
    public GameObject tipQuadRightDisplay;
    public GameObject tipQuadUndoResult;
    public GameObject tipQuadRedoResult;
    public GameObject tipCircleUndoSolution;
    public GameObject tipCircleRedoSolution;
    
    public AudioSource tutorialAudio;
    [Header("Object Settings")] 
    public GameObject gameScreen;

    public GameObject pause;
    public GameObject currentLevel;
    public GameObject mainMenu;
    public GameObject countWalks;
    public GameObject tutorialScreen;
    public GameObject completeGameScreen;
    public GameObject deadScreen;
    [Header("Billy Settings")]
    public GameObject billy;

    public Billy billyController;
    [Header("Animation Settings")] 
    public Animator countWalksAnimator;

    [Header("Audio Settings")] 
    public AudioMixerGroup musicMaster;
    public AudioMixerSnapshot volumeMax;
    public AudioMixerSnapshot volumeZero;
    public AudioClip minCountWalks;
    public AudioClip completeGameScreenSound;

    public AudioClip tutorialMusic;
    public AudioClip endMusic;
    public AudioClip mainMenuMusic;

    public List<string> SentencesTutorial { get; set; }
    public List<string> SentencesEnd { get; set; }
    private void Start()
    {
        NextDialogMechanic();
        EndMechanicButton();
        buttonCompleteGame.onClick.AddListener(delegate { End(); });
        _solutions = new char[4];
        _solutions[0] = '+';
        _solutions[1] = '-';
        _solutions[2] = '*';
        _solutions[3] = '/';
    }

    public void LoaderKeysLanguage()
    {
        _countWalksKey = LocalizationManager.GetTranslate(LocalizationKeys.CountWKey);
        _deadKey = LocalizationKeys.DeadKeys[UnityEngine.Random.Range(0, LocalizationKeys.DeadKeys.Length)];
        _currentRankPlaceKey = LocalizationKeys.ResultMmKey;
        _timeLeftKey = LocalizationKeys.TimeLeftKey;
        _levelCompleteKey = LocalizationKeys.LevelCompleteKey;
        _backResultMmKey = LocalizationKeys.BackResultMmKey;
    }

    public void NextDialogMechanic(int tutorial = 0)
    {
        buttonNextTutorialDialog.onClick.RemoveAllListeners(); 
        buttonNextTutorialDialog.onClick.AddListener(delegate { Dialog(tutorial); });
    }

    public void EndMechanicButton(int tutorial = 0)
    {
        buttonSkipTutorial.onClick.RemoveAllListeners();
        buttonSkipTutorial.onClick.AddListener(delegate { End(tutorial ); });
    }

    public void Begin(int tutorial = 0)
    {
        _indexDialog = 0;
        volumeZero.TransitionTo(1f);
        if (tutorial == 0)
        {
            loader.ChangeIconButtons();
            gamePlay.DeadMechanic(tutorialMusic,gamePlay.gameNormal);
        }
        else
        {
            gamePlay.DeadMechanic(endMusic,gamePlay.gameNormal);
        }
        volumeMax.TransitionTo(1f);
        billy.SetActive(true);
        billyController.billyAnimator.enabled = true;
        Dialog(tutorial);
    }

    private void Dialog(int tutorial = 0)
    {
        tutorialAudio.Play();
        billyController.ChangedAnimation(1);
        tutorialEndText.text = "";
        if (tutorial == 0)
        {
            if (_indexDialog < SentencesTutorial.Count)
            {
                if (_typeSentence == null)
                    _typeSentence = StartCoroutine(TypeSentence(SentencesTutorial[_indexDialog]));
                else
                {
                    StopCoroutine(_typeSentence);
                    _typeSentence = StartCoroutine(TypeSentence(SentencesTutorial[_indexDialog]));
                }
            }
            else
            {
                StopCoroutine(_typeSentence);
            }
        
            TutorialMechanic();
        }
        else
        {
            if (_indexDialog < SentencesEnd.Count)
            {
                if (_typeSentence == null)
                    _typeSentence = StartCoroutine(TypeSentence(SentencesEnd[_indexDialog]));
                else
                {
                    StopCoroutine(_typeSentence);
                    _typeSentence = StartCoroutine(TypeSentence(SentencesEnd[_indexDialog]));
                }
            }
            else
            {
                StopCoroutine(_typeSentence);
            }
            
            EndMechanic();
        }
            
        _indexDialog++;
    }
    

    private void TutorialMechanic()
    {
        switch (_indexDialog)
        {
           case 3:
               billy.SetActive(false);
               gameScreen.SetActive(true);
               pause.SetActive(false);
               countWalks.SetActive(false);
               currentLevel.SetActive(false);
               _generateNumeric = UnityEngine.Random.Range(-99, 99);
               resultText.text = _generateNumeric.ToString();
               tipQuadLeftDisplay.SetActive(true);
               break;
           case 4:
               tipQuadLeftDisplay.SetActive(false);
               playerResultText.text =_generateNumeric.ToString();
               tipQuadRightDisplay.SetActive(true);
               break;
           case 5:
               tipQuadRightDisplay.SetActive(false);
               break;
           case 6:
               _solutionsDemo = true;
               _solution = StartCoroutine(SolutionTextChanged());
               break;
           case 7:
               StopCoroutine(_solution);
               _solution = StartCoroutine(SolutionTextChanged(true));
               break;
           case 8:
               tipCircleRedoSolution.SetActive(true);
               tipCircleUndoSolution.SetActive(true);
               break;
           case 9:
               _solutionsDemo = false;
               solutionText.text = "";
               tipCircleRedoSolution.SetActive(false);
               tipCircleUndoSolution.SetActive(false);
               break;
           case 10:
               countWalks.SetActive(true);
               countWalksAnimator.Play(NameAnimationCountWalksTrue);
               _generateNumeric = UnityEngine.Random.Range(1, 9);
               countWalksText.text = _countWalksKey + _generateNumeric;
               break;
           case 11:
               Media.OnPlayAudio(minCountWalks);
               gamePlay.DeadMechanic(tutorialMusic,gamePlay.gameDead,false);
               countWalksAnimator.Play(NameAnimationDeadInside);
               countWalksAnimator.SetBool(_nameAnimationTutorial,true);
               _generateNumeric = 0;
               countWalksText.text = _countWalksKey + _generateNumeric;
               Invoke(nameof(DeadScreenMechanic),1f);
               break;
           case 13:
               deadScreen.SetActive(false);
               gamePlay.DeadMechanic(tutorialMusic,gamePlay.gameNormal,false);
               gameScreen.SetActive(false);
               billy.SetActive(true);
               billyController.ChangedAnimation(1);
               break;
           case 15:
               deadScreen.SetActive(false);
               gameScreen.SetActive(true);
               billy.SetActive(false);
               _generateNumeric = 0;
               resultText.text = _generateNumeric.ToString();
               playerResultText.text = _generateNumeric.ToString();
               countWalksText.text = _countWalksKey + UnityEngine.Random.Range(1,9);
               break;
           case 16:
               deadScreen.SetActive(false);
               tipQuadUndoResult.SetActive(true);
               break;
           case 17:
               deadScreen.SetActive(false);
               tipQuadUndoResult.SetActive(false);
               break;
           case 19:
               deadScreen.SetActive(false);
               tipQuadRedoResult.SetActive(true);
               break;
           case 20:
               deadScreen.SetActive(false);
               tipQuadRedoResult.SetActive(false);
               End();
               break;
        }
    }

    private void EndMechanic()
    {
        switch (_indexDialog)
        {
            case 4:
                Social.ReportProgress(GPS.achievement_a_great_strategist_and_mathematician,100.0f, (bool success) => { });
                break;
            case 5:
                Media.OnPlayAudio(completeGameScreenSound);
                completeGameScreen.SetActive(true);
                tutorialScreen.SetActive(false);
                break;
        }
    }

    private void End(int tutorial = 0)
    {
        currentLevel.SetActive(true);
        countWalksAnimator.SetBool(_nameAnimationTutorial,false);
        if (playerStats.IsFirstGameLaunch == 0)
        {
            playerStats.IsFirstGameLaunch = 1;
            playerStats.SavePlayerStats();
            tutorialScreen.SetActive(false);
            billy.SetActive(false);
            gamePlay.PlayGame();
            tutorialAudio.Stop();
        }
        else
        {
            gamePlay.DeadMechanic(mainMenuMusic,gamePlay.gameMenu);
            if (tutorial == 0)
            {
                tutorialScreen.SetActive(false);
                mainMenu.SetActive(true);
                billy.SetActive(true);
                billyController.ChangedAnimation(0);
                menu.ShowFullGameComplete();
                gameScreen.SetActive(false);
                completeGameScreen.SetActive(false);
                tutorialAudio.Stop();
                billyController.StartTypeAnimationChanged();
            }
            else
            {
                _indexDialog = 4;
                EndMechanic();
                _indexDialog = 5;
                EndMechanic();
            }
        }
    }

    private void DeadScreenMechanic()
    {
        deadScreen.SetActive(true);
        TimeController.instance.RandomTimer();
        _generateNumeric = UnityEngine.Random.Range(10, 99);
        deadText.text = LocalizationManager.GetTranslate(_deadKey);
        currentRankInTable.text = LocalizationManager.GetTranslate(_currentRankPlaceKey);
        rank.text = "#" + UnityEngine.Random.Range(1, 9);
        timeLeftGameSession.text = LocalizationManager.GetTranslate(_timeLeftKey) +" " + TimeController.instance.timePlayingGameSession;
        levelsComplete.text = LocalizationManager.GetTranslate(_levelCompleteKey) + " "  + _generateNumeric ;
        levelsCompleteOld.text = LocalizationManager.GetTranslate(_backResultMmKey) + " " + (_generateNumeric - 1);
    }

    private IEnumerator SolutionTextChanged(bool isRandom = false)
    {
        var index = -1;
        while (_solutionsDemo)
        {
            if (index >= _solutions.Length-1)
                index = 0;
            else if(!isRandom)
                index++;
            else
                index = UnityEngine.Random.Range(0, _solutions.Length);
            solutionText.text = _solutions[index].ToString();
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        foreach (var letter in sentence.ToCharArray())
        {
            tutorialEndText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        tutorialAudio.Stop();
        billyController.ChangedAnimation(0);
    }
}
