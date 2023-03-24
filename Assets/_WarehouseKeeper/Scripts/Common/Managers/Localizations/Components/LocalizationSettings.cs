using System;
using UnityEngine;

namespace WarehouseKeeper.Localizations.Components
{
[CreateAssetMenu(fileName = "LocalizationSettings", menuName = "Warehouse Keeper/Localization Settings", order = 0)]
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