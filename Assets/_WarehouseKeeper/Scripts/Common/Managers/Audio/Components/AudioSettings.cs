using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace WarehouseKeeper.Audio
{
[CreateAssetMenu(fileName = "AudioSettings", menuName = "Warehouse Keeper/Audio Settings", order = 0)]

internal class AudioSettings : ScriptableObject
{
    [field: SerializeField] public AudioMixerData[] Mixers { get; private set; }
    [field: SerializeField] public Source SourceGo { get; private set; }
    [field: SerializeField] public AssetReference SourceRef { get; private set; }
}

[Serializable]
internal class AudioMixerData
{
    public AudioMixerGroup mixerGroup;
    public ChanelType mixerType;
}
}