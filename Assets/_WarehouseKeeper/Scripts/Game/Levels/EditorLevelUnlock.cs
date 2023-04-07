using UnityEngine;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Levels;
using Zenject;

namespace WarehouseKeeper
{
    public class EditorLevelUnlock : MonoBehaviour
    {
#if UNITY_EDITOR
        
        public int unlockId = 0;

        [Inject] private PlayerResourcesDirector _playerResourcesDirector;
        [Inject] private LevelRepositoryDirector _levelRepository;
        
        [ContextMenu("UNLOCK")]
        public void UnlockAndPrev()
        {
            for (int i = 0; i < unlockId; i++)
            {
                if (_levelRepository.GetLevelData(i) != null)
                    continue;
                _levelRepository.CreateEmptyData(i);
            }
        }
        
#endif
    }
}
