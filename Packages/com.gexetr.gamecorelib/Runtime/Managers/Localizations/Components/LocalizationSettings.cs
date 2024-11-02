using System;
using Game.InternalData;
using UnityEngine;

namespace Game.Localizations.Components
{
[CreateAssetMenu(fileName = nameof(LocalizationSettings), menuName = GameData.EditorName + "/Localization Settings")]
internal class LocalizationSettings : ScriptableObject
{
    [field: SerializeField] public LanguageType DefaultLocalization { get; private set; }
    [field: SerializeField] public TextAsset LocalizationDoc { get; private set; }
    [field: SerializeField] public LanguageBinder[] LanguageBind { get; private set; }
    
    [Serializable]
    internal struct LanguageBinder
    {
        public LanguageType localizedType;
        public SystemLanguage[] bindSystemTypes;
    }
}
}