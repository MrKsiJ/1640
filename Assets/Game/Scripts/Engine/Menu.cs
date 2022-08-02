using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    private const string NameLevelsJsonFile = "levels.json";
    [Header("Menu Settings")] 
    public TutorialEnd tutorialStats;

    public Gameplay gamePlay;
    public PlayerStats playerStats;
    public Loader loader;
    public UnityEngine.UI.Button leaderBoard;
    public UnityEngine.UI.Button tutorial;
    public UnityEngine.UI.Button language;

    [Header("Pause Menu Settings")] 
    public UnityEngine.UI.Button resume;

    public UnityEngine.UI.Button restart;
    public UnityEngine.UI.Button achivements;
    public UnityEngine.UI.Button saveAndQuit;

    [Header("Billy Settings")] 
    public GameObject billy;
    public Billy billyController;
    [Header("Object Settings")] 
    public GameObject mainMenu;

    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject fullGameComplete;
    public GameObject tutorialScreen;

    [Header("Audio Settings")] 
    public AudioMixerGroup musicMaster;
    public AudioMixerSnapshot volumeMax;
    public AudioMixerSnapshot volumeZero;
    public AudioSource masterAudio;
    public AudioClip mainMenuMusic;

    public string PathToSaveLevels { get; private set; }
    private void Start()
    {
        leaderBoard.onClick.AddListener(ShowLeaderboard);
        tutorial.onClick.AddListener(ShowTutorial);
        resume.onClick.AddListener(ResumeMechanic);
        restart.onClick.AddListener(RestartMechanic);
        achivements.onClick.AddListener(ShowAchivements);
        saveAndQuit.onClick.AddListener(SaveAndQuitMechanic);
        language.onClick.AddListener(ChangeLanguage);
    }

    public void LoaderPathToSaveLevelsFile()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
                PathToSaveLevels = System.IO.Path.Combine(Application.persistentDataPath, NameLevelsJsonFile);
        #else
                PathToSaveLevels = System.IO.Path.Combine(Application.dataPath,NameLevelsJsonFile);
        #endif
        if (!File.Exists(PathToSaveLevels)) 
            return;
        loader.saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(PathToSaveLevels));
        loader.levels = loader.saveData.levels;
    }

    private void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    private void ShowAchivements()
    {
        Social.ShowAchievementsUI();
    }

    public void ShowTutorial()
    {
        tutorialStats.Begin();
        mainMenu.SetActive(false);
        tutorialScreen.SetActive(true);
    }

    private void ResumeMechanic()
    {
        Time.timeScale = 1f;
        masterAudio.Play();
        pauseScreen.SetActive(false);
        gamePlay.currentLevelAnimation.Play(Gameplay.NameAnimationLevel);
    }

    private void RestartMechanic()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        loader.LoaderRestartLevels();
        gamePlay.PlayGame(false);
    }

    private void SaveAndQuitMechanic()
    {
        if (!File.Exists(PathToSaveLevels))
            File.Create(PathToSaveLevels).Close();
        Time.timeScale = 1f;
        playerStats.CurrentLevelStop = playerStats.CurrentLevelComplete;
        playerStats.SavePlayerStats();
        var indexButtons = loader.saveData.indexButtons;
        loader.saveData = new SaveData 
        {
            levels = loader.levels, 
            variablesPlayer = gamePlay.variablesPlayer,
            variableIndex = gamePlay.variableIndex,
            solutionIndex = gamePlay.solutionIndex,
            currentCountWalks = playerStats.CurrentCountWalks,
            currentPlayerResult = playerStats.CurrentCalculatorResult,
            indexButtons = indexButtons
        };
        var saveData = JsonUtility.ToJson(loader.saveData);
        File.WriteAllText(PathToSaveLevels,saveData);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        mainMenu.SetActive(true);
        billy.SetActive(true);
        billyController.ChangedAnimation(0);
        billyController.StartTypeAnimationChanged();
        gamePlay.DeadMechanic(mainMenuMusic,loader.gameMenu);
    }

    public void ShowFullGameComplete()
    {
        fullGameComplete.SetActive(playerStats.IsGameComplete == 1);
    }
    private void ChangeLanguage()
    {
        playerStats.LanguageId = playerStats.LanguageId == 0 ? 1 : 0;
        loader.LoaderLanguage();
    }
}
