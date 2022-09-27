using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Honeti
{
    /// <summary>
    /// I18N Language code enum.
    /// </summary>
    public enum LanguageCode
    {
        /// <summary>
        /// English
        /// </summary>
        EN = 0,
        /// <summary>
        /// Polish
        /// </summary>
        PL = 1,
        /// <summary>
        /// German
        /// </summary>
        DE = 2,
        /// <summary>
        /// French
        /// </summary>
        FR = 3,
        /// <summary>
        /// Spanish
        /// </summary>
        ES = 4,
        /// <summary>
        /// Japanese
        /// </summary>
        JP = 5,
        /// <summary>
        /// Korean
        /// </summary>
        KO = 6,
        /// <summary>
        /// Chinse Simplified
        /// </summary>
        ZH = 7,
        /// <summary>
        /// Malaysia
        /// </summary>
        MY = 8
    }

    /// <summary>
    /// Internationalization component.
    /// Use getValue() to translate text.
    /// Use setLanguage() to change current application language.
    /// All translations are in _langs variable.
    /// </summary>
    public class I18N : MonoBehaviour
    {
        #region STATIC

        /// <summary>
        /// Default language.
        /// </summary>
        private static LanguageCode _defaultLang = LanguageCode.EN;
        private static I18N _instance = null;

        /// <summary>
        /// I18N components instance.
        /// </summary>
        public static I18N instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<I18N>();
                    _instance.init();
                }
                return _instance;
            }
        }

        #endregion

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if(_instance != null)
                return;
            _instance = this;
            _instance.init();
        }

        #region EVENTS

        public delegate void LanguageChange(LanguageCode newLanguage);
        public static event LanguageChange OnLanguageChanged;

        public delegate void FontChange(I18NFonts newFont);
        public static event FontChange OnFontChanged;

        #endregion

        #region CONST

        private const string GAME_LANG = "game_language";

        #endregion

        #region PRIVATE VARS

        /// <summary>
        /// Language table.
        /// </summary>
        private Hashtable _langs;
        /// <summary>
        /// Current game language. Using getValue() will translate to that language.
        /// </summary>
        [SerializeField]
        private LanguageCode _gameLang = LanguageCode.EN;
        /// <summary>
        /// Returned text when there is no translation.
        /// </summary>
        private string _noTranslationText = "Translation missing for {0}";
        /// <summary>
        /// Language file asset.
        /// Expected file is tsv format (tab-separated values) 
        /// but saved as cvs (Unity does not accept tsv files as TextAssets).
        /// 
        /// Structure:
        /// langKey EN  PL  [other langs]
        /// ^exampleKey translationEN1  translationPL1  [other langs]
        /// ...
        /// 
        /// LangKey column has to be the first column.
        /// 
        /// Language key has to start with '^' character.
        /// If line does not start with '^' it is not parsed.
        /// 
        /// Translation value can contain:
        /// - extra parameters: "{0} {1}"
        /// - new line: "\n"
        /// </summary>
        [SerializeField]
        private TextAsset _languageFile;
        /// <summary>
        /// List of available languages, parsed from language file.
        /// </summary>
        private List<LanguageCode> _availableLangs;
        /// <summary>
        /// When true, I18NText controls will change font for different languages.
        /// Fonts will be selected from _langFonts list. When there is no custom
        /// font set fot language, I18N controls will use default font.
        /// </summary>
        [SerializeField]
        private bool _useCustomFonts = false;
        /// <summary>
        /// Current custom font.
        /// </summary>
        private I18NFonts _currentCustomFont;
        /// <summary>
        /// Custom fonts list for different languages.
        /// </summary>
        [SerializeField]
        private List<I18NFonts> _langFonts;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Current game language
        /// </summary>
        public LanguageCode gameLang
        {
            get
            {
                return _gameLang;
            }
        }

        /// <summary>
        /// True, when I18N is using custom fonts
        /// </summary>
        public bool useCustomFonts
        {
            get
            {
                return _useCustomFonts;
            }
        }

        /// <summary>
        /// Return current custom font or null, when I18N 
        /// is not currently using custom fonts.
        /// </summary>
        public I18NFonts customFont
        {
            get
            {
                if (_useCustomFonts)
                    return _currentCustomFont;
                return null;
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void MergeMoreData(string text)
        {
            Hashtable table = _parseLanguage(text);
            foreach (DictionaryEntry entry in table)
            {
                if (!_langs.ContainsKey(entry.Key))
                    continue;
                Hashtable keys = entry.Value as Hashtable;
                if(keys == null)
                    continue;
                Hashtable keysBase = _langs[entry.Key] as Hashtable;
                if (keysBase == null)
                {
                    keysBase = new Hashtable();
                    _langs.Add(entry.Key, keysBase);
                }
                foreach (DictionaryEntry key in keys)
                {
                    keysBase[key.Key] = key.Value;
                }
            }
        }

        public bool HasKey(string key)
        {
            Hashtable lang = _langs[_gameLang] as Hashtable;
            if (lang == null)
                return false;
            return lang.ContainsKey(key);
        }

        public int CountKey(string key)
        {
            int rs = 0;
            Hashtable lang = _langs[_gameLang] as Hashtable;
            if (lang != null)
            {
                foreach(string keyData in lang.Keys)
                {
                    if(keyData.Contains(key)) rs++;
                }
            }
            return rs;
        }

        /// <summary>
        /// Change current language.
        /// Set default language if not initialized or recognized.
        /// </summary>
        /// <param name="langCode">Language code</param>
        public void setLanguage(string langCode)
        {
            LanguageCode langEnum;
            if (!Enum.TryParse(langCode, out langEnum))
                langEnum = _defaultLang;
            setLanguage(langEnum);
        }

        /// <summary>
        /// Change current language.
        /// Set default if language not initialized or recognized.
        /// </summary>
        /// <param name="langCode">Language code</param>
        public void setLanguage(LanguageCode langCode)
        {
            if (_langs.ContainsKey(langCode))
            {
                _gameLang = langCode;
            }
            else
            {
                _gameLang = _defaultLang;
                Debug.LogError(string.Format("Language {0} not recognized! Using default language.", langCode));
            }

            PlayerPrefs.SetString(GAME_LANG, _gameLang.ToString());

            if (OnLanguageChanged != null)
                OnLanguageChanged(_gameLang);

            if (_useCustomFonts)
            {
                I18NFonts newFont = null;
                _currentCustomFont = null;
                if (_langFonts != null && _langFonts.Count > 0)
                {
                    foreach (I18NFonts f in _langFonts)
                    {
                        if (f.lang == _gameLang)
                        {
                            newFont = f;
                            _currentCustomFont = f;
                            break;
                        }
                    }
                }

                if (OnFontChanged != null)
                    OnFontChanged(newFont);
            }
            else
            {
                _currentCustomFont = null;
            }
        }

        /// <summary>
        /// Get key value in current language.
        /// </summary>
        /// <param name="key">Translation key. String should start with '^' character</param>
        /// <returns>Translation value</returns>
        public string getValue(string key)
        {
            return getValue(key, null);
        }

        /// <summary>
        /// Get key value in current language with additional params. 
        /// Currently not working.
        /// </summary>
        /// <param name="key">Translation key. String should start with '^' character and can contain params ex. {0} {1}...</param>
        /// <param name="parameters">Additional parameters.</param>
        /// <returns>Translation value</returns>
        public string getValue(string key, params string[] parameters)
        {
            if (string.IsNullOrEmpty(key))
                return "";
            Hashtable lang = _langs[_gameLang] as Hashtable;
            string val = lang[key] as String;

            if (val == null || val.Length == 0)
            {
                if (key == "")
                    return "";
                return string.Format(_noTranslationText, key);
            }

            if (parameters != null && parameters.Length > 0)
            {
                return string.Format(val.Replace("\\n", Environment.NewLine), parameters);
            }
            return val.Replace("\\n", Environment.NewLine);
            
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Initialize component
        /// </summary>
        void init()
        {
            DontDestroyOnLoad(_instance.gameObject);
            _availableLangs = new List<LanguageCode>();

            _langs = _parseLanguage(_languageFile.text);

            string lang = "EN";//null;
            /*if (!PlayerPrefs.HasKey(GAME_LANG))
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Polish:
                        lang = "PL";
                        break;
                    case SystemLanguage.English:
                        lang = "EN";
                        break;
                    case SystemLanguage.German:
                        lang = "DE";
                        break;
                    case SystemLanguage.French:
                        lang = "FR";
                        break;
                    case SystemLanguage.Spanish:
                        lang = "ES";
                        break;
                }
            }
            else
            {
                lang = PlayerPrefs.GetString(GAME_LANG);
            }*/
            try
            {
                setLanguage(lang);
            }
            catch
            {
                setLanguage(_defaultLang);
            }

        }

        /// <summary>
        /// Parse language file
        /// </summary>
        /// <param name="lang">Language file asset</param>
        /// <returns>Parsed language hashtable.</returns>
        private Hashtable _parseLanguage(string lang)
        {
            Hashtable table = new Hashtable();
            foreach (var langCode in Enum.GetValues(typeof(LanguageCode)))
            {
                table[langCode] = new Hashtable();
            }

            string[] lines = lang.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            string[] langCodes = lines[0].Split('\t');
            string[] langNames = Enum.GetNames(typeof(LanguageCode));
            _availableLangs.Clear();
            foreach (string code in langCodes)
            {
                if (Array.IndexOf(langNames, code) >= 0)
                {
                    _availableLangs.Add((LanguageCode)Enum.Parse(typeof(LanguageCode), code));
                }
            }

            foreach (string s in lines)
            {
                if (s.StartsWith("#") || !s.StartsWith("^"))
                    continue;

                string[] line = s.Split('\t');

				for(int i=0; i<_availableLangs.Count; i++)
				{
				    Hashtable hash = table[_availableLangs[i]] as Hashtable;
                    if(hash == null)
                        continue;
				    string key = line[0];
                    if(hash.ContainsKey(key))
                        Debug.Log("Duplicate language key: " + key);
                    else
				        hash.Add(key, line[i + 1] != "" ? line[i + 1] : " ");
                }
            }

            return table;
        }

        #endregion
    }

    #region HELPER CLASSES

    /// <summary>
    /// Helper class, containing font parameters.
    /// </summary>
    [Serializable]
    public class I18NFonts
    {
        #region PUBLIC VARS

        /// <summary>
        /// Font language code.
        /// </summary>
        public LanguageCode lang;
        /// <summary>
        /// Font
        /// </summary>
        public Font font;
        /// <summary>
        /// True, when components should use custom line spacing.
        /// </summary>
        public bool customLineSpacing = false;
        /// <summary>
        /// Custom line spacing value.
        /// </summary>
        public float lineSpacing = 1.0f;
        /// <summary>
        /// True, when components should use custom font size.
        /// </summary>
        public bool customFontSizeOffset = false;
        /// <summary>
        /// Custom font size offset in percents.
        /// e.g. 55, -10
        /// </summary>
        public int fontSizeOffsetPercent = 0;
        /// <summary>
        /// True, when components should use custom alignment.
        /// </summary>
        public bool customAlignment = false;
        /// <summary>
        /// Custom alignment value.
        /// </summary>
        public TextAlignment alignment = TextAlignment.Left;

        #endregion
    }

    /// <summary>
    /// Helper class, containing sprite parameters.
    /// </summary>
    [Serializable]
    public class I18NSprites
    {
        #region PUBLIC VARS

        /// <summary>
        /// Sprite lang code.
        /// </summary>
        public LanguageCode language;
        /// <summary>
        /// Sprite.
        /// </summary>
        public Sprite image;

        #endregion
    }

    /// <summary>
    /// Helper class, containing sound parameters.
    /// </summary>
    [Serializable]
    public class I18NSounds
    {
        #region PUBLIC VARS

        /// <summary>
        /// Sound language code.
        /// </summary>
        public LanguageCode language;
        /// <summary>
        /// Audio clip.
        /// </summary>
        public AudioClip clip;

        #endregion
    }

    #endregion
}