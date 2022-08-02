using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private const string SaveKeyTopResult = "TOP";
    private const string SaveKeyOldTopResult = "OLDTOP";
    private const string SaveKeyGameComplete = "Complete";
    private const string SaveKeyCurrentLevelStop = "Level";
    private const string SaveKeyFirstGameLauncher = "Tutorial";
    private const string SaveKeyLanguageId = "Language";

    public int IsFirstGameLaunch { get; set; }
    
    public int IsGameComplete { get; set; }
    
    public int CurrentLevelStop { get; set; }
    
    public int CurrentLevelComplete { get; set; }
    
    public int CurrentCalculatorResult { get; set; }
    
    public int CurrentCountWalks { get; set; }
    
    public int CurrentTopResult { get; set; }
    
    public int OldTopResult { get; set; }
    
    public bool SecondChance { get; set; }
    
    public int LanguageId {get; set;}

    public void LoadPlayerStats()
    {
        if (PlayerPrefs.HasKey(SaveKeyTopResult))
            CurrentTopResult = PlayerPrefs.GetInt(SaveKeyTopResult);
        if (PlayerPrefs.HasKey(SaveKeyOldTopResult))
            OldTopResult = PlayerPrefs.GetInt(SaveKeyOldTopResult);
        if (PlayerPrefs.HasKey(SaveKeyFirstGameLauncher))
            IsFirstGameLaunch = PlayerPrefs.GetInt(SaveKeyFirstGameLauncher);
        if(PlayerPrefs.HasKey(SaveKeyLanguageId))
            LanguageId = PlayerPrefs.GetInt(SaveKeyLanguageId);
        if (PlayerPrefs.HasKey(SaveKeyGameComplete))
            IsGameComplete = PlayerPrefs.GetInt(SaveKeyGameComplete);
        if (PlayerPrefs.HasKey(SaveKeyCurrentLevelStop))
            CurrentLevelStop = PlayerPrefs.GetInt(SaveKeyCurrentLevelStop);
    }

    public void SavePlayerStats()
    {
        PlayerPrefs.SetInt(SaveKeyTopResult,CurrentTopResult);
        PlayerPrefs.SetInt(SaveKeyOldTopResult,OldTopResult);
        PlayerPrefs.SetInt(SaveKeyFirstGameLauncher,IsFirstGameLaunch);
        PlayerPrefs.SetInt(SaveKeyLanguageId,LanguageId);
        PlayerPrefs.SetInt(SaveKeyGameComplete,IsGameComplete);
        PlayerPrefs.SetInt(SaveKeyCurrentLevelStop,CurrentLevelStop);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SavePlayerStats();;
    }
}
