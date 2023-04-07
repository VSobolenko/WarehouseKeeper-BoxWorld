using Game.Localizations;
using TMPro;
using UnityEngine;
using WarehouseKeeper.Directors.Game.UserResources;

namespace WarehouseKeeper.UI.Windows
{
internal class PlayerResourcesView : MonoBehaviour
{
    [Header("Amber"), SerializeField] private TextMeshProUGUI _amberText;
    [Header("Hints"), SerializeField] private TextMeshProUGUI _hintsText;

    private PlayerResourcesDirector _playerResourcesDirector;
    private ILocalizationManager _localizationManager;
    
    public void AutoInitialize(PlayerResourcesDirector playerResourcesDirector, ILocalizationManager localizationManager = null)
    {
        Setup(playerResourcesDirector.UserData.Amber.Value, playerResourcesDirector.UserData.Hints.Value);
        
        if (_playerResourcesDirector == null)
            _playerResourcesDirector = playerResourcesDirector;
        if (_localizationManager == null)
            _localizationManager = localizationManager;

        if (_playerResourcesDirector != null)
            _playerResourcesDirector.OnUpdateUserData += InternalEventUpdate;
    }

    public void AutoDispose()
    {
        if (_playerResourcesDirector != null)
            _playerResourcesDirector.OnUpdateUserData -= InternalEventUpdate;
    }
    
    private void InternalEventUpdate()
    {
        if (_playerResourcesDirector == null)
            return;

        Setup(_playerResourcesDirector.UserData.Amber.Value, _playerResourcesDirector.UserData.Hints.Value);
    }
    
    public void Setup(int countAmber, int countHints)
    {
        _amberText.text = countAmber.ToString();
        _hintsText.text = countHints.ToString();
    }
}
}