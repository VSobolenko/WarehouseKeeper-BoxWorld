using System;
using JetBrains.Annotations;

namespace Game.Localizations
{
public interface ILocalizationManager
{
    LanguageType ActiveLanguage { get; }
    
    event Action OnChangeLocalization;
    
    void SetLanguage(LanguageType language);
    
    string Localize(string key, string fallback = null);
    
    string LocalizeFormat(string key, string fallback, object[] args);
    string LocalizeFormat(string key, object arg0, object arg1, object arg2, string fallback = null);
    string LocalizeFormat(string key, object arg0, object arg1, string fallback = null);
    string LocalizeFormat(string key, object arg0, string fallback = null);
}
}