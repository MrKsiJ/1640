using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public static readonly int maxLevelsGame = 999;
    private const int SolutionButtonId = 0;
    private const int ResultButtonId = 1;
    private string _loadingKey = "";
    private bool gpsConnection = false; 
    private Coroutine _loading;
    [Header("LeaderBoard Settings")] 
    public IScore[] scores;
    [Header("Animation Coroutine Settings")]
    public Coroutine _camChanged;
    public Coroutine _skyBoxChanger;
    private Coroutine _gpsConnection;
    private Coroutine _leaderBoardConnection;
    [Header("Background Settings")]
    public Color gameMenu;
    private bool _isLoading = false;
    [Header("Loading Settings")]
    public Text loadingText;
    [Header("Game Settings")]
    public PlayerStats playerScript;
    public SaveData saveData;
    public Menu menu;
    public Gameplay gamePlay;
    public TutorialEnd tutorial;
    [Header("Object Settings")]
    public GameObject billy;
    public GameObject loadMenu;
    public List<Level> levels = new List<Level>(maxLevelsGame);
    

    [Header("Buttons Settings")]
    public UnityEngine.UI.Button undoSolution;
    public UnityEngine.UI.Button redoSolution;
    public UnityEngine.UI.Button undoResult;
    public UnityEngine.UI.Button redoResult;
    public UnityEngine.UI.Button resume;
    public UnityEngine.UI.Button[] buttons;
    public Image[] iconsButtons;
    public Sprite[] rndIconButtons;

    [Header("Language Settings")] 
    public LocalizationManager localizationManager;
    public Image language;
    public Sprite[] languageSprites;

    [Header("Camera Settings")] 
    public Camera mainCamera;
    private void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        playerScript.LoadPlayerStats();
        _loadingKey = LocalizationKeys.Loading_Key;;
        _isLoading = true;
        LoaderLanguage();
        menu.LoaderPathToSaveLevelsFile();
        _loading = StartCoroutine(LoadingText());
        Invoke(nameof(LoadingGame),6f);
    }
    public void LoadingGame()
    {
        LoaderTutorialEndDialogs();
        LoaderLevels(playerScript.CurrentLevelComplete);
        LoaderGameButtons();
        LoaderGooglePlayGames();
        billy.SetActive(true);
        _camChanged = StartCoroutine(SizeOrthographicChanged(5f));
        _skyBoxChanger = StartCoroutine(gamePlay.FadeToColor(gameMenu,15f));
        menu.ShowFullGameComplete();
        loadMenu.SetActive(false);
        _isLoading = false;
        StopCoroutine(_loading);
    }

    public void LoaderRestartLevels()
    {
        LoaderLevels(playerScript.CurrentLevelComplete+1);
    }

    private IEnumerator SizeOrthographicChanged(float newSize)
    {
        while (mainCamera.orthographicSize > newSize)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newSize, 2f * Time.deltaTime);
            
            yield return new WaitForSeconds(0.01f);
        }
    }
    private IEnumerator LoadingText()
    {
        var countKama = 0;
        while (_isLoading)
        {
            if (countKama < 3)
            {
                loadingText.text += '.';
                countKama++;
            }
            else
            {
                var startIndex = (loadingText.text.Length) - countKama;
                var clearLoading = loadingText.text.Remove(startIndex,countKama);
                loadingText.text = clearLoading;
                countKama = 0;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    

    private void LoaderLevels(int levelComplete = 0)
    {
        if (File.Exists(menu.PathToSaveLevels) && levelComplete == 0) 
            return;
        Path.RestartValues();
        var loadIndex = levelComplete;
        if (loadIndex == 0)
            loadIndex = maxLevelsGame;

        for (var i = 0; i < loadIndex; i++)
        {
            var level = Path.CreateLevel(i+1);
            if (i >= 0 && i < levels.Count)
                levels[i] = level;
            else
                levels.Add(level);
        }
    }

    private void LoaderTutorialEndDialogs()
    {
        tutorial.SentencesTutorial = new List<string>();
        tutorial.SentencesEnd = new List<string>();
        foreach (var tutorialDialog in LocalizationKeys.TutorialDialog)
        {
            var dialog = LocalizationManager.GetTranslate(tutorialDialog);
            tutorial.SentencesTutorial.Add(dialog);
        }

        foreach (var endGameDialog in LocalizationKeys.CompleteGameDialog)
        {
            var dialog = LocalizationManager.GetTranslate(endGameDialog);
            tutorial.SentencesEnd.Add(dialog);
        }

        tutorial.LoaderKeysLanguage();
    }
    
    private void LoaderGameButtons()
    {
        undoSolution.onClick.AddListener(delegate { gamePlay.UndoMechanic(SolutionButtonId); });
        redoSolution.onClick.AddListener(delegate { gamePlay.RedoMechanic(SolutionButtonId); });
        undoResult.onClick.AddListener(delegate { gamePlay.UndoMechanic(ResultButtonId); });
        redoResult.onClick.AddListener(delegate { gamePlay.RedoMechanic(ResultButtonId); });
        resume.onClick.AddListener(delegate { gamePlay.ResumeMechanic(); });
        foreach (var button in buttons)
        {
            var buttonText = button.transform.GetChild(0).GetComponent<Text>();
            var numeric = int.Parse(buttonText.text);
            button.onClick.AddListener(delegate { gamePlay.ButtonMechanic(numeric); });
        }
    }

    public void ChangeIconButtons()
    {
        var indexButtons = 0;
        if(!File.Exists(menu.PathToSaveLevels))
            saveData.indexButtons = new List<int>();
        foreach (var image in iconsButtons)
        {
            var index = 0;
            if(File.Exists(menu.PathToSaveLevels))
            {
                index = saveData.indexButtons[indexButtons];
                image.sprite = rndIconButtons[index];
                indexButtons++;
            }
            else
            {
                index = UnityEngine.Random.Range(0, rndIconButtons.Length);
                saveData.indexButtons.Add(index);
                image.sprite = rndIconButtons[index];
            }
        }
    }

    public void LoaderLanguage()
    {
        language.sprite = languageSprites[playerScript.LanguageId];
        loadingText.text = LocalizationManager.GetTranslate(_loadingKey);
        localizationManager.SetLanguage(playerScript.LanguageId);
        LoaderTutorialEndDialogs();
    }

    private void LoaderGooglePlayGames()
    {
        _gpsConnection = StartCoroutine(ConnectionGPS());
        _leaderBoardConnection = StartCoroutine(ConnectionLeaderboard());
    }

    private IEnumerator ConnectionGPS()
    {
        gpsConnection = false;
        while (!gpsConnection)
        {
            Social.localUser.Authenticate(success =>
            {
                gpsConnection = success;
                menu.leaderBoard.interactable = success;
            });
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator ConnectionLeaderboard()
    {
        while (true)
        {
            if (gpsConnection)
            {
                Social.LoadScores(gamePlay.leaderBoardPlayers, (score) =>
                {
                    scores = score;
                });
            }
            yield return  new WaitForSeconds(0.25f);
        }
    }
}
