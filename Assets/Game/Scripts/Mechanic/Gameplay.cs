using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;

public class Gameplay : MonoBehaviour,IUnityAdsListener
{
    #region UIAnimatonsNames
    public static string NameAnimationLevel = "Level";
    private const string NameAnimationLevelStop = "LevelStop";
    private const string NameAnimationDeadInside = "DeadInside";
    private const string NameAnimationIdleCountWalks = "IdleCountWalks";
    #endregion

    public string leaderBoardPlayers = "CgkIkeeqi8oNEAIQAQ";
    private const string RewardedAdUnitId = "ca-app-pub-1746114312849038/6245084151";
    private const string RewardedPlacementId = "rewardedVideo";
    private const string InsterstitialPlacementId = "video";
    private const string InterstitialAdUnitId = "ca-app-pub-1746114312849038/9801185789";
    #if UNITY_IOS
    private string _gameId = "4095886";
#elif UNITY_ANDROID
    private string _gameId = "4095887";
    #endif

    private Coroutine _skyBoxChanger;
    
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    
    [Header("Music Settings")]
    public AudioSource masterAudio;
    public AudioMixerGroup musicMaster;
    public AudioMixerSnapshot volumeMax;
    public AudioMixerSnapshot volumeZero;
    public AudioMixerSnapshot volumeDead;
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;
    public AudioClip gameMinWalksMusic;
    [Header("Sound Settings")] 
    public AudioClip winLevel;

    public AudioClip minWalks;
    public AudioClip gameOver;
    [Header("Object Settings")]
    public GameObject mainMenu;
    public GameObject gameScreen;
    public GameObject pause;
    public GameObject pauseScreen;
    public GameObject tutorialScreen;
    public GameObject deadScreen;
    [Header("UI Settings")]
    public UnityEngine.UI.Button play;

    public UnityEngine.UI.Button pauseButton;
    [Header("Billy Settings")] 
    public GameObject billy;

    public Billy billyController;
    [Header("Game Settings")] 
    public PlayerStats playerStats;
    public TutorialEnd tutorialEnd;
    public Loader loader;
    public Menu menu;
    public Text resultText;
    public Text playerResultText;
    public Text solutionText;
    public Animator countWalksAnimation;
    public Text countWalksText;
    public Animator currentLevelAnimation;
    public Text currentLevelText;
    [Header("Tip Settings")] 
    public GameObject tipQuadLeftDisplay;
    public GameObject tipQuadRightDisplay;
    public GameObject tipQuadUndoResult;
    public GameObject tipQuadRedoResult;
    public GameObject tipCircleUndoSolution;
    public GameObject tipCircleRedoSolution;
    [Header("DeadScreen Settings")] 
    public GameObject continueGame;

    [Header("Background Settings")] 
    public Material skyBox;

    public Color gameNormal;
    public Color gameDead;
    public Color gameMenu;

    public UnityEngine.UI.Button continueGameButton;
    public Text deadText;
    public Text currentRankInTable;
    public Text rankPlayer;
    public Text timeLeftGameSession;
    public Text levelsComplete;
    public Text levelsCompleteOld;

    public UnityEngine.UI.Button undoSolution;
    public UnityEngine.UI.Button redoSolution;
    public UnityEngine.UI.Button undoResult;
    public UnityEngine.UI.Button redoResult;


    [Header("Game Session")]
    [HideInInspector]public List<int> variablesPlayer = new List<int>();
    [HideInInspector]public int variableIndex = 0;
    [HideInInspector]public int solutionIndex = 0;
    [HideInInspector]public Level currentLevel;
    
    
    private void Start()
    {
        play.onClick.AddListener(delegate { PlayGame(); });
        pauseButton.onClick.AddListener(PauseGame);
        continueGameButton.onClick.AddListener(ShowRewardedVideo);
        Advertisement.AddListener(this);
        Advertisement.Initialize(_gameId,false);
        _rewardedAd = new RewardedAd(RewardedAdUnitId);
        _rewardedAd.OnUserEarnedReward += RewardedAdOnOnUserEarnedReward;
        AdRequest request = new AdRequest.Builder().Build();
        _rewardedAd.LoadAd(request);
        

        _interstitialAd = new InterstitialAd(InterstitialAdUnitId);
        AdRequest adRequest = new  AdRequest.Builder().Build();
        _interstitialAd.LoadAd(adRequest);
    }

    private void RewardedAdOnOnUserEarnedReward(object sender, Reward e)
    {
        RewardedPlayer();
    }

 

    public void PlayGame(bool isChangedKeyBoard = true)
    {
        if (playerStats.IsFirstGameLaunch == 1)
        {
            playerStats.CurrentLevelComplete = File.Exists(menu.PathToSaveLevels) ? playerStats.CurrentLevelStop : 0;
            playerStats.SecondChance = false;
            mainMenu.SetActive(false);
            billy.SetActive(false);
            gameScreen.SetActive(true);
            pause.SetActive(true);
            if (File.Exists(menu.PathToSaveLevels))
            {
                variableIndex = loader.saveData.variableIndex;
                variablesPlayer = loader.saveData.variablesPlayer;
            }
            else
            {
                variableIndex = 0;
                variablesPlayer = new List<int>();
            }
            if(isChangedKeyBoard)
                loader.ChangeIconButtons();
            LoadLevel(true);
            MsgCurrentLevel();
            MsgCountWalks();
            RefreshButtons();
            tipQuadLeftDisplay.SetActive(false);
            tipQuadRightDisplay.SetActive(false);
            tipQuadUndoResult.SetActive(false);
            tipQuadRedoResult.SetActive(false);
            tipCircleUndoSolution.SetActive(false);
            tipCircleRedoSolution.SetActive(false);
            TimeController.instance.BeginTimer();
            if (playerStats.CurrentCountWalks != 1)
                DeadMechanic(gameMusic,gameNormal);
        }
        else
            menu.ShowTutorial();
    }

    private void PauseGame()
    {
        masterAudio.Pause();
        currentLevelAnimation.Play(NameAnimationLevelStop);
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void RefreshButtons()
    {
        undoSolution.interactable = solutionIndex != 0;
        redoSolution.interactable = solutionIndex < currentLevel.LevelSolutions.Length-1;
        undoResult.interactable = variableIndex != 0;
        redoResult.interactable = variableIndex < variablesPlayer.Count-1;
    }

    public void ButtonMechanic(int numeric)
    {
        if (playerStats.CurrentCountWalks > 0)
        {
            var aciton = solutionText.text[0];
            switch (aciton)
            {
                case '+':
                    playerStats.CurrentCalculatorResult += numeric;
                    break;
                case '-':
                    playerStats.CurrentCalculatorResult -= numeric;
                    break;
                case '*':
                    playerStats.CurrentCalculatorResult *= numeric;
                    break;
                case '/':
                    playerStats.CurrentCalculatorResult /= numeric;
                    break;
            }

            playerResultText.text = playerStats.CurrentCalculatorResult.ToString();
            VariableMechanic();
            CheckResultLevel();
        }
    }

    private void CheckResultLevel(bool adRewarded = false)
    {
        if (playerStats.CurrentCalculatorResult == currentLevel.Result)
        {
            if((playerStats.CurrentLevelComplete+1) % 5 == 0 && !adRewarded)
                ShowIntersitialAd();
            if(!adRewarded)
                AchivementUnlocker();
            Media.OnPlayAudio(winLevel);
            TimeController.instance.EndTimer();
            if (playerStats.CurrentLevelComplete != Loader.maxLevelsGame-1)
            {
                playerStats.CurrentLevelComplete++;
                LoadLevel();
                MsgCurrentLevel();
                MsgCountWalks(adRewarded);
                TimeController.instance.BeginTimer(false);
            }
            else
            {
                gameScreen.SetActive(false);
                tutorialScreen.SetActive(true);
                playerStats.IsGameComplete = 1;
                playerStats.SavePlayerStats();
                tutorialEnd.NextDialogMechanic(1);
                tutorialEnd.EndMechanicButton(1);
                tutorialEnd.Begin(1);
            }
        }
        else
        {
            playerStats.CurrentCountWalks--;
            MsgCountWalks();
            if (playerStats.CurrentCountWalks <= 0)
            {
                TimeController.instance.EndTimer();
                volumeDead.TransitionTo(1f);
                ShowIntersitialAd();
                Invoke(nameof(DeadScreen),1f);
                deadText.gameObject.SetActive(true);
                var deadKey = LocalizationKeys.DeadKeys[UnityEngine.Random.Range(0, LocalizationKeys.DeadKeys.Length)];
                deadText.text = LocalizationManager.GetTranslate(deadKey);
                deadScreen.SetActive(true);
                continueGame.SetActive(Application.internetReachability != NetworkReachability.NotReachable && !playerStats.SecondChance);
            }
        }
    }

    private void ShowIntersitialAd()
    {
        if(_interstitialAd.IsLoaded())
            _interstitialAd.Show();
        else if(Advertisement.IsReady())
            Advertisement.Show();
    }

    private void ShowRewardedVideo()
    {
        if(_rewardedAd.IsLoaded())
            _rewardedAd.Show();
        else if(Advertisement.IsReady(RewardedPlacementId))
            Advertisement.Show(RewardedPlacementId);
    }
    
    private void RewardedPlayer()
    {
        deadScreen.SetActive(false);
        playerStats.CurrentCalculatorResult = currentLevel.Result;
        playerStats.SecondChance = true;
        volumeMax.TransitionTo(1f);
        CheckResultLevel(true);
        
    }

    private void AchivementUnlocker()
    {
        switch (playerStats.CurrentLevelComplete)
        {
            case 5:
                Social.ReportProgress(GPS.achievement_beginner_mathematician,100.0f, (bool success) => { });
                Social.ReportProgress(GPS.achievement_beginner_megamind,100.0f,(bool success) => { });
                break;
            case 10:
                Social.ReportProgress(GPS.achievement_a_fan_of_solving_problems,100.0f,(bool success) => { });
                break;
            case 15:
                Social.ReportProgress(GPS.achievement_brainstorming_session,100.0f,(bool success) => { });
                break;
            case 25:
                Social.ReportProgress(GPS.achievement_horizon_discoverer,100.0f,(bool success) => { });
                break;
            case 30:
                Social.ReportProgress(GPS.achievement_progressor,100.0f,(bool success) => { });
                break;
            case 50:
                Social.ReportProgress(GPS.achievement_this_is_far_from_the_limit,100.0f,(bool success) => { });
                break;
            case 75:
                Social.ReportProgress(GPS.achievement_not_a_step_back,100.0f,(bool success) => { });
                break;
            case 90:
                Social.ReportProgress(GPS.achievement_light_at_the_end_of_the_tunnel,100.0f,(bool success) => { });
                break;
            case 99:
                Social.ReportProgress(GPS.achievement_you_can_do_more,100.0f,(bool success) => { });
                break;
            case 100:
                Social.ReportProgress(GPS.achievement_goal_achieved,100.0f,(bool success) => { });
                break;
        }

        switch (rankPlayer.text)
        {
            case "#1":
                Social.ReportProgress(GPS.achievement_platinum_brain,100.0f,(bool success) => { });
                break;
            case "#2":
                Social.ReportProgress(GPS.achievement_steel_brain,100.0f,(bool success) => { });
                break;
            case "#3":
                Social.ReportProgress(GPS.achievement_strong_brain,100.0f,(bool success) => { });
                break;
        }

        if(TimeController.instance.elapsedTime <= 5f)
            Social.ReportProgress(GPS.achievement_speedrun,100.0f,(bool success) => { });
        else if(TimeController.instance.elapsedTime <= 2.5f)
            Social.ReportProgress(GPS.achievement_be_like_a_leaf,100.0f,(bool success) => { });
        
        if (playerStats.CurrentLevelComplete > playerStats.OldTopResult)
            Social.ReportProgress(GPS.achievement_participant,100.0f,(bool success) => { });
        
    }

    private void DeadScreen()
    {
        Media.OnPlayAudio(gameOver);
        LoadRankPlayer();
        var currentRankPlaceKey = LocalizationKeys.ResultMmKey;
        var timeLeftKey = LocalizationKeys.TimeLeftKey;
        var levelCompleteKey = LocalizationKeys.LevelCompleteKey;
        var backResultMmKey = LocalizationKeys.BackResultMmKey;
        
        currentRankInTable.text = LocalizationManager.GetTranslate(currentRankPlaceKey);
        timeLeftGameSession.text = LocalizationManager.GetTranslate(timeLeftKey) +" " + TimeController.instance.timePlayingGameSession;
        levelsComplete.text = LocalizationManager.GetTranslate(levelCompleteKey) + " " + playerStats.CurrentLevelComplete;
        levelsCompleteOld.text = LocalizationManager.GetTranslate(backResultMmKey) + " "+ playerStats.OldTopResult;
        
        PlayerStatsUpdate();

    }

    private void PlayerStatsUpdate()
    {
        if (playerStats.CurrentLevelComplete > playerStats.OldTopResult)
        {
            Social.ReportScore(playerStats.CurrentLevelComplete,leaderBoardPlayers, (bool success) => { });
            playerStats.CurrentTopResult = playerStats.CurrentLevelComplete;
            playerStats.OldTopResult = playerStats.CurrentLevelComplete;
            playerStats.CurrentLevelStop = 0;
            if(File.Exists(menu.PathToSaveLevels))
                File.Delete(menu.PathToSaveLevels);
            playerStats.SavePlayerStats();
            AchivementUnlocker();
        }
        playerStats.CurrentCalculatorResult = 0;
    }

    private void LoadRankPlayer()
    {
        var scores = loader.scores;
        var isDenided = true;
        foreach (var score in scores)
        {
            if (score.userID == Social.localUser.id)
            {
                isDenided = false;
                currentRankInTable.gameObject.SetActive(true);
                rankPlayer.gameObject.SetActive(true);
                rankPlayer.text = "#" + score.rank;
                if(score.rank >= 1 && score.rank <=3)
                    AchivementUnlocker();
                break;
            }
            else
            {
                isDenided = false;
                currentRankInTable.gameObject.SetActive(false);
                rankPlayer.gameObject.SetActive(false);
            }
        }
        if (isDenided)
        {
            currentRankInTable.gameObject.SetActive(false);
            rankPlayer.gameObject.SetActive(false);
        }
    }

    public void ResumeMechanic()
    {
        DeadMechanic(mainMenuMusic,gameMenu);
        loader.LoaderRestartLevels();
        deadScreen.SetActive(false);
        gameScreen.SetActive(false);
        mainMenu.SetActive(true);
        billy.SetActive(true);
        billyController.ChangedAnimation(0);
        billyController.StartTypeAnimationChanged();
    }
    

    private void VariableMechanic()
    {
        variablesPlayer.Add(playerStats.CurrentCalculatorResult);
        variableIndex = variablesPlayer.Count-1; 
        RefreshButtons();
    }

    private void LoadLevel(bool isLoad = false)
    {
        currentLevel = loader.levels[playerStats.CurrentLevelComplete];
        if (File.Exists(menu.PathToSaveLevels))
        {
            solutionIndex = loader.saveData.solutionIndex;
            playerStats.CurrentCalculatorResult = loader.saveData.currentPlayerResult;
            playerStats.CurrentCountWalks = loader.saveData.currentCountWalks;
        }
        else
        {
            solutionIndex = 0;
            playerStats.CurrentCalculatorResult = currentLevel.CaclulatorResultStart;
            playerStats.CurrentCountWalks = currentLevel.CountWalks;
        }
        resultText.text = currentLevel.Result.ToString();
        playerResultText.text = playerStats.CurrentCalculatorResult.ToString();
        solutionText.text = currentLevel.LevelSolutions[solutionIndex].ToString();
        if(!isLoad || !File.Exists(menu.PathToSaveLevels))
            VariableMechanic();
        RefreshButtons();
    }
    
    public void UndoMechanic(int buttonWork)
    {
        if (playerStats.CurrentCountWalks > 0)
        {
            switch (buttonWork)
            {
                case 0:
                    solutionIndex--;
                    solutionText.text = currentLevel.LevelSolutions[solutionIndex].ToString();
                    RefreshButtons();
                    break;
                case 1:
                    variableIndex--;
                    playerResultText.text = variablesPlayer[variableIndex].ToString();
                    playerStats.CurrentCalculatorResult = variablesPlayer[variableIndex];
                    RefreshButtons();
                    break;
            }
        }
    }

    public void RedoMechanic(int buttonWork)
    {
        if (playerStats.CurrentCountWalks > 0)
        {
            switch (buttonWork)
            {
                case 0:
                    solutionIndex++;
                    solutionText.text = currentLevel.LevelSolutions[solutionIndex].ToString();
                    RefreshButtons();
                    break;
                case 1:
                    variableIndex++;
                    playerResultText.text = variablesPlayer[variableIndex].ToString();
                    playerStats.CurrentCalculatorResult = variablesPlayer[variableIndex];
                    RefreshButtons();
                    break;
            }
        }

    }

    private void MsgCountWalks(bool adRewarded = false)
    {
        var countWalksKey = LocalizationManager.GetTranslate(LocalizationKeys.CountWKey);
        countWalksText.text = countWalksKey + playerStats.CurrentCountWalks;
        if (playerStats.CurrentCountWalks == 1)
        {
            Media.OnPlayAudio(minWalks);
            DeadMechanic(gameMinWalksMusic,gameDead,true,NameAnimationDeadInside);
        }
        else if(masterAudio.clip.name == gameMinWalksMusic.name || adRewarded)
            DeadMechanic(gameMusic,gameNormal);
    }

    public void DeadMechanic(AudioClip music,Color skyBoxColor,bool isMusicChanged = true,string nameAnimationPlayed = NameAnimationIdleCountWalks)
    {
        if (isMusicChanged)
        {
            volumeZero.TransitionTo(1f);
            masterAudio.clip = music;
            masterAudio.Play();
            volumeMax.TransitionTo(1f);
        }
        countWalksAnimation.Play(nameAnimationPlayed,-1,0f);
        if(_skyBoxChanger == null)
            _skyBoxChanger = StartCoroutine(FadeToColor(skyBoxColor, 15f));
        else
        {
            StopCoroutine(_skyBoxChanger);
            _skyBoxChanger = StartCoroutine(FadeToColor(skyBoxColor, 15f));
        }
    }
    private void MsgCurrentLevel()
    {
        var levelKey = LocalizationManager.GetTranslate(LocalizationKeys.LevelKey);
        currentLevelAnimation.Play(NameAnimationLevel,-1,0f);
        currentLevelText.text = levelKey + (playerStats.CurrentLevelComplete + 1).ToString();
    }

    public IEnumerator FadeToColor(Color color,float fadingSpeed)
    {
        float currTime = 0f;
        do {
            skyBox.SetColor(("_Color1"),Color.Lerp (skyBox.GetColor("_Color1"), color, currTime/fadingSpeed));
            currTime += Time.deltaTime;
            yield return null;
        } while (currTime<=fadingSpeed);
    }


    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {
        
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId != RewardedPlacementId) 
            return;
        switch (showResult)
        {
            case ShowResult.Finished:
                RewardedPlayer();
                break;
        }
    }
}
