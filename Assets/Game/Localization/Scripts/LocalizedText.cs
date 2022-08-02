using UnityEngine.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent((typeof(Text)))]
public class LocalizedText : MonoBehaviour
{
    private Text text;
    private string key;
    void Start()
    {
        Localize();
        LocalizationManager.OnLanguageChange += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        Localize();
    }

    private void Localize(string newKey = null)
    {
        if (text == null)
            Init();

        if (newKey != null)
            key = newKey;

        text.text = LocalizationManager.GetTranslate(key);
    }

    private void Init()
    {
        text = GetComponent<Text>();
        key = text.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
