using System;
using WarehouseKeeper.Directors.Game.UserResources.BaseModels;
using WarehouseKeeper.Repositories;

namespace WarehouseKeeper.Directors.Game.UserResources
{
[Serializable]
public class UserData : IHasBasicId
{
    public int Id { get; set; }
    public int Version { get; set; }
    public NumericalResource Hints { get; set; }
    public NumericalResource Amber { get; set; }
    public bool AdsDisable { get; set; }
    
    //Appearance
    public StringUniqueContainerResources Animations { get; set; }
    public StringUniqueContainerResources BoxSkins { get; set; }
    public StringUniqueContainerResources Effects { get; set; }
    public StringUniqueContainerResources UserSkins { get; set; }
    public string AnimationSelected { get; set; }
    public string EffectSelected { get; set; }
    public string BoxSkinSelected { set; get; }
    public string UserSkinSelected { get; set; }
}
}