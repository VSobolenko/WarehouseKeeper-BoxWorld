using System.Linq;
using Game;
using Game.AssetContent;
using Game.Factories;
using Game.GUI.Windows;
using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.AppearanceWindows.Components.AppearanceItems;
using WarehouseKeeper.Directors;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Windows;

namespace WarehouseKeeper.UI.Windows.AppearanceWindows
{
internal class AppearanceWindowMediator : BaseMediator<AppearanceWindowView>
{
    private readonly WindowsDirector _windowsDirector;
    private readonly AppearanceItemFactory _appearanceItemFactory;
    private readonly PlayerResourcesDirector _playerResources;
    private readonly ResourcesDirector _resourcesDirector;
    private readonly IAddressablesManager _addressablesManager;
    private readonly IFactoryGameObjects _factoryGameObjects;

    private AppearanceItem[] _items;
    private AppearanceItem _displayedItem;
    private GameObject _displayedItemView;
    private AppearanceSectionType _displayedSection;

    public AppearanceWindowMediator(AppearanceWindowView window, 
                                    WindowsDirector windowsDirector,
                                    AppearanceItemFactory appearanceItemFactory,
                                    PlayerResourcesDirector playerResources,
                                    ResourcesDirector resourcesDirector,
                                    IAddressablesManager addressablesManager,
                                    IFactoryGameObjects factoryGameObjects) : base(window)
    {
        _windowsDirector = windowsDirector;
        _appearanceItemFactory = appearanceItemFactory;
        _playerResources = playerResources;
        _resourcesDirector = resourcesDirector;
        _addressablesManager = addressablesManager;
        _factoryGameObjects = factoryGameObjects;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        OpenUserSkinSkins();
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
        CleaViewItems();
        ClearItemView();
    }

    #region Button event handler

    private void ProceedButtonAction(AppearanceWindowAction action)
    {
        switch (action)
        {
            case AppearanceWindowAction.OnClickCloseYourself:
                _windowsDirector.CloseWindow(this);
                break;
            case AppearanceWindowAction.OnClickAnimationSkins:
                OpenAnimationSkins();
                break;
            case AppearanceWindowAction.OnClickBoxSkins:
                OpenBoxSkinsSkins();
                break;
            case AppearanceWindowAction.OnClickEffectSkins:
                OpenEffectSkins();
                break;
            case AppearanceWindowAction.OnClickUserSkins:
                OpenUserSkinSkins();
                break;
            case AppearanceWindowAction.OnClickBy:
                break;
            case AppearanceWindowAction.OnClickSelect:
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
            
        }
    }

    private void OpenAnimationSkins()
    {
        CleaViewItems();

        _items = _appearanceItemFactory.GetAnimationsItems(window.itemsRoot);
        _displayedSection = AppearanceSectionType.Animations;
    }
    
    private void OpenBoxSkinsSkins()
    {
        CleaViewItems();

        _items = _appearanceItemFactory.GetBoxSkinItems(window.itemsRoot);
        _displayedSection = AppearanceSectionType.BoxSkins;
    }
    
    private void OpenEffectSkins()
    {
        CleaViewItems();

        _items = _appearanceItemFactory.GetEffectItems(window.itemsRoot);
        _displayedSection = AppearanceSectionType.Effects;
    }
    
    private void OpenUserSkinSkins()
    {
        CleaViewItems();

        _items = _appearanceItemFactory.GetUserSkinItems(window.itemsRoot);
        _displayedSection = AppearanceSectionType.UserSkins;
    }
    
    #endregion

    private void CleaViewItems()
    {
        if (_items == null)
            return;
        foreach (var appearanceItem in _items)
            appearanceItem.Release();
    }

    private void ClearItemView()
    {
        if (_displayedItemView != null)
        {
            GameObject.Destroy(_displayedItem);
        }
    }
    
    private void SelectItem(AppearanceItem item)
    {
        ItemState itemState;
        if (IsCodeIdPurchased(_displayedSection, item.CodeId))
            itemState = IsCodeIdSelected(_displayedSection, item.CodeId) ? ItemState.Selected : ItemState.Received;
        else
            itemState = ItemState.Blocked;
        window.SetViewState(itemState);

        ClearItemView();
        var addressableKey = GetInstanceKey(_displayedSection, item.CodeId);
        _displayedItemView = InstantiateItemView(addressableKey);
    }
    
    private bool IsCodeIdPurchased(AppearanceSectionType sectionType, string codeId)
    {
        switch (sectionType)
        {
            case AppearanceSectionType.Animations:
                return _playerResources.UserData.Animations.Exists(codeId);
            case AppearanceSectionType.BoxSkins:
                return _playerResources.UserData.BoxSkins.Exists(codeId);
            case AppearanceSectionType.Effects:
                return _playerResources.UserData.Effects.Exists(codeId);
            case AppearanceSectionType.UserSkins:
                return _playerResources.UserData.UserSkins.Exists(codeId);
            default:
                Log.WriteError($"Unknown type {sectionType}");
                return false;
        }
    }
    
    private bool IsCodeIdSelected(AppearanceSectionType sectionType, string codeId)
    {
        switch (sectionType)
        {
            case AppearanceSectionType.Animations:
                return _playerResources.UserData.AnimationSelected == codeId;
            case AppearanceSectionType.BoxSkins:
                return _playerResources.UserData.BoxSkinSelected == codeId;
            case AppearanceSectionType.Effects:
                return _playerResources.UserData.EffectSelected == codeId;
            case AppearanceSectionType.UserSkins:
                return _playerResources.UserData.UserSkinSelected == codeId;
            default:
                Log.WriteError($"Unknown type {sectionType}");
                return false;
        }
    }
    
    private string GetInstanceKey(AppearanceSectionType sectionType, string codeId)
    {
        switch (sectionType)
        {
            case AppearanceSectionType.Animations:
                return _resourcesDirector.AnimationsAppearance.Items.FirstOrDefault( x=> x.keyId == codeId).addressableKey;
            case AppearanceSectionType.BoxSkins:
                return _resourcesDirector.BoxAppearance.Items.FirstOrDefault( x=> x.keyId == codeId).addressableKey;
            case AppearanceSectionType.Effects:
                return _resourcesDirector.EffectsAppearance.Items.FirstOrDefault( x=> x.keyId == codeId).addressableKey;
            case AppearanceSectionType.UserSkins:
                return _resourcesDirector.UserSkinsAppearance.Items.FirstOrDefault( x=> x.keyId == codeId).addressableKey;
            default:
                Log.WriteError($"Unknown type {sectionType}");
                return string.Empty;
        }
    }

    private GameObject InstantiateItemView(string addressableKey)
    {
        var prefab = _addressablesManager.LoadAsset<GameObject>(addressableKey);
        
        if (prefab == null)
        {
            Log.WriteError($"Addressable key prefab {addressableKey} missing");
            return null;
        }

        var instance = _factoryGameObjects.InstantiatePrefab(prefab);
        
        if (instance == null)
        {
            Log.WriteError($"Error instantiate {prefab.name} prefab");
            return null;
        }

        return instance;
    }
}
}