using UnityEngine;
using System.Collections.Generic;
using System.Xml;
public class LocalizationManager : MonoBehaviour
{
    public static int SelectedLanguage {get; private set;}

    public static event LanguageChangeHanlder OnLanguageChange;
    public delegate void LanguageChangeHanlder();
    
    private static Dictionary<string,List<string>> localization;

    [SerializeField] private TextAsset textFile;
    
    private void Awake()
    {
        if (localization == null)
            LoadLocalizeaiton();
    }

    public void SetLanguage(int id)
    {
        SelectedLanguage = id;
        OnLanguageChange?.Invoke();
    }

    private void LoadLocalizeaiton()
    {
        localization = new Dictionary<string, List<string>>();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textFile.text);

        foreach (XmlNode key in (xmlDocument["Keys"].ChildNodes))
        {
            string keyStr = key.Attributes["NameKey"].Value;

            var values = new List<string>();
            foreach (XmlNode translate in key["Translates"].ChildNodes)
            {
                values.Add(translate.InnerText);
                localization[keyStr] = values;
            }
        }
    }

    public static string GetTranslate(string key, int languageId = -1)
    {
        if (languageId == -1)
            languageId = SelectedLanguage;
        if (localization.ContainsKey(key))
            return localization[key][languageId];
        return key;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
