using System;
using Game.Localizations.Installers;
using TMPro;
using UnityEngine;

namespace Game.Localizations.Components
{
[RequireComponent(typeof(TextMeshProUGUI))]
[DisallowMultipleComponent]
public class LocalizableTMP : LocalizableBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private string fallback;
    [Header("Custom settings"), SerializeField] private bool disableStartedLocalization;
    [SerializeField] private TextMeshProUGUI targetText;

    private ILocalizationManager _localizationManager;

#if UNITY_EDITOR
    [Header("Editor features"), SerializeField] private LocalizationManagerType editorManagerType;
    [SerializeField] private LanguageType languageForTranslate;
#endif

    protected virtual void Initialize(ILocalizationManager localizationManager) =>
        _localizationManager = localizationManager;

    public void CustomInject(ILocalizationManager localizationManager) => _localizationManager = localizationManager;

    private void Start()
    {
        if (disableStartedLocalization)
            return;

        Localize();
        _localizationManager.OnChangeLocalization += Localize;
    }

    private void OnDestroy()
    {
        _localizationManager.OnChangeLocalization -= Localize;
    }

    public void Localize() => InternalLocalize(_localizationManager);

    private void InternalLocalize(ILocalizationManager manager)
    {
        if (manager == null || targetText == null)
        {
            Log.Error($"Translation is skipped for object \"{gameObject.name}\"");
            return;
        }

        targetText.text = manager.Localize(key, fallback);
    }

    private void OnValidate()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();
    }

    [ContextMenu("Editor localize")]
    public void EditorLocalize()
    {
        UpdateWithManager(InternalLocalize);
    }
    
    private void UpdateWithManager(Action<ILocalizationManager> actionWithManager)
    {
        var settings = LocalizationInstaller.Settings;
        if (settings == null)
        {
            Log.Error($"Can't find settings for manager");

            return;
        }

        ILocalizationManager manager = editorManagerType switch
        {
            LocalizationManagerType.LocalizationManager => new Managers.LocalizationManager(settings),
            _ => null,
        };
        manager?.SetLanguage(languageForTranslate);
        actionWithManager?.Invoke(manager);
    }
    
    private enum LocalizationManagerType : byte
    {
        LocalizationManager,
    }
}
}