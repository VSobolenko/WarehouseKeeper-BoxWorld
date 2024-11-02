using System;
using Game.InternalData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Game.Audio
{
[CreateAssetMenu(fileName = nameof(AudioSettings), menuName = GameData.EditorName + "/Audio Settings")]
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