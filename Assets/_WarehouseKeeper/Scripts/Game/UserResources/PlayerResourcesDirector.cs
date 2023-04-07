using System;
using Game.Repositories;
using WarehouseKeeper.Directors.Game.UserResources.BaseModels;
using Zenject;

namespace WarehouseKeeper.Directors.Game.UserResources
{
internal class PlayerResourcesDirector : IInitializable
{
    private readonly IRepository<UserData> _repository;
    private readonly ResourcesDirector _resourcesDirector;

    private const int UserDataId = 0;
    public event Action OnUpdateUserData;
    public PlayerResourcesDirector(IRepository<UserData> repository, ResourcesDirector resourcesDirector)
    {
        _repository = repository;
        _resourcesDirector = resourcesDirector;
    }
    
    public UserData UserData { get; private set; }
    
    public void Initialize()
    {
        var data = _repository.Read(UserDataId);
        //Possible migration place
        InitializeUserData(data);
    }
    
    private void InitializeUserData(UserData userData)
    {
        UserData = userData;
        if (userData != null)
            return;
        
        UserData = new UserData
        {
            Id = UserDataId,
            Version = 1,
            Hints = new NumericalResource(3),
            Amber = new NumericalResource(0),
            AdsDisable = false,
            
            Animations = new StringUniqueContainerResources(_resourcesDirector.AnimationsAppearance.DefaultKeysId),
            BoxSkins = new StringUniqueContainerResources(_resourcesDirector.BoxAppearance.DefaultKeysId),
            Effects = new StringUniqueContainerResources(_resourcesDirector.EffectsAppearance.DefaultKeysId),
            UserSkins = new StringUniqueContainerResources(_resourcesDirector.UserSkinsAppearance.DefaultKeysId),

            AnimationSelected = _resourcesDirector.AnimationsAppearance.StartedId,
            BoxSkinSelected = _resourcesDirector.BoxAppearance.StartedId,
            EffectSelected = _resourcesDirector.EffectsAppearance.StartedId,
            UserSkinSelected = _resourcesDirector.UserSkinsAppearance.StartedId,
        };
        _repository.Create(UserData);
    }

    public void UpdateData(Action<UserData> userDataAction)
    {
        userDataAction?.Invoke(UserData);
        _repository.Update(UserData);
        OnUpdateUserData?.Invoke();
    }

    public void UpdateData()
    {
        _repository.Update(UserData);
        OnUpdateUserData?.Invoke();
    }
    
    public void Reset()
    {
        _repository.Delete(UserData);
        UserData = null;
        InitializeUserData(null);
        OnUpdateUserData?.Invoke();
    }
}
}