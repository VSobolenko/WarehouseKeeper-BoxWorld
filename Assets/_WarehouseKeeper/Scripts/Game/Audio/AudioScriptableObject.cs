using UnityEngine;
using WarehouseKeeper.Audio;

namespace WarehouseKeeper.Directors.Game.Audio
{
[CreateAssetMenu(fileName = "AudioData", menuName = "Warehouse Keeper/Audio Data", order = 0)]
internal class AudioScriptableObject : ScriptableObject
{
    [field: SerializeField] public Sound LobbySettings { get; private set; }
    [field: SerializeField] public Sound GameSettings { get; private set; }
    [field: SerializeField] public Sound UISettings { get; private set; }
    [field: SerializeField] public AudioClip LobbyBackground { get; private set; }
    [field: SerializeField] public AudioClip GameBackground { get; private set; }
    [field: SerializeField] public AudioClip ButtonClick { get; private set; }
}
}