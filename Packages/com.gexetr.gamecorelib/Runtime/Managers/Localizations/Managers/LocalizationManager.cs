using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Game.Localizations.Components;
using Game.PreferencesSaveType;
using UnityEngine;

namespace Game.Localizations.Managers
{
internal class LocalizationManager : ILocalizationManager
{
    private readonly LocalizationSettings _settings;
    private readonly Dictionary<string, List<string>> _localization = new(15);
    
    private const string StartedComment = "//";
    private const string DefaultTranslate = "<color=#00FF00>UNKNOWN :( TRANSLATE</color>";
    private const string LocalizationSaveKey = "Active_Language_Type";

    private readonly EnumSavableValue<LanguageType> _language;
    
    public event Action OnChangeLocalization;
    public LanguageType ActiveLanguage => _language.Value;

    public LocalizationManager(LocalizationSettings settings)
    {
        _settings = settings;
        var language = GetSystemLocalizationOrDefault();
        _language = new EnumSavableValue<LanguageType>(LocalizationSaveKey, language);
        LoadLocalization();
    }

    #region Initializing

    private LanguageType GetSystemLocalizationOrDefault()
    {
        var systemLanguage = Application.systemLanguage;
        foreach (var languageBinder in _settings.LanguageBind)
        {
            if (!languageBinder.bindSystemTypes.Contains(systemLanguage)) 
                continue;

            return languageBinder.localizedType;
        }

        return _settings.DefaultLocalization;
    }

    private void LoadLocalization()
    {
        try
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_settings.LocalizationDoc.text);

            foreach (XmlNode key in xmlDocument["Keys"]?.ChildNodes)
            {
                var keyStr = key?.Attributes["Key"]?.Value;
                if (string.IsNullOrEmpty(keyStr))
                    continue;
                if (keyStr.StartsWith(StartedComment))
                    continue;

                var values = new List<string>(3);
                foreach (XmlNode translate in key["Translates"]?.ChildNodes)
                {
                    if (translate?.InnerText == null)
                        Log.Warning($"Null translate! Key={keyStr}");
                    values.Add(translate?.InnerText == null ? string.Empty : translate.InnerText);
                }

                if (_localization.ContainsKey(keyStr))
                {
                    Log.Warning($"Duplicate localization key: {keyStr}. Translate skipped");
                    continue;
                }
                _localization[keyStr] = values;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Exception in load localization from file: {e.Message}");
        }
    }

    #endregion

    public void SetLanguage(LanguageType language)
    {
        _language.Value = language;
        OnChangeLocalization?.Invoke();
    }
    
    public string Localize(string key, string fallback = null) => Localize(ref key, ref fallback);

    public string LocalizeFormat(string key, string fallback, params object[] args) =>
        string.Format(Localize(ref key, ref fallback), args);

    public string LocalizeFormat(string key, object arg0, object arg1, object arg2, string fallback = null) =>
        string.Format(Localize(ref key, ref fallback), arg0, arg1, arg2);

    public string LocalizeFormat(string key, object arg0, object arg1, string fallback = null) =>
        string.Format(Localize(ref key, ref fallback), arg0, arg1);

    public string LocalizeFormat(string key, object arg0, string fallback = null) =>
        string.Format(Localize(ref key, ref fallback), arg0);
    
    private string Localize(ref string key, ref string fallback)
    {
        if (string.IsNullOrEmpty(key))
#if DEVELOPMENT_BUILD
            return $"[{key}]-" + DefaultTranslate;
#else
            return string.IsNullOrEmpty(fallback) ? DefaultTranslate : fallback;
#endif

        if (_localization.ContainsKey(key) && _localization[key].Count > (int) ActiveLanguage)
            return _localization[key][(int) ActiveLanguage];

        if (string.IsNullOrEmpty(fallback) == false)
#if DEVELOPMENT_BUILD
            return $"[{key}]-" + DefaultTranslate;
#else
            return fallback;
#endif
        
       
#if DEVELOPMENT_BUILD
        return $"[{key}]-" + DefaultTranslate;
#else
        return DefaultTranslate;
#endif
    }
}
}