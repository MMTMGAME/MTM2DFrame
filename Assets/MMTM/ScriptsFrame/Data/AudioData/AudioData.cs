using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "audioData", menuName = "ScriptableObject/音效数据", order = 0)]
public class AudioData : ScriptableObject
{
    [Serializable]
    public struct Audio
    {
        public string name;
        [DarkTonic.MasterAudio.SoundGroupAttribute]
        public string audioname;
        [Range(0,1)]
        public float volume;
        public bool isLoop;
    }
    
    [Label("要用到的音频")]
    public List<Audio> NeedAudioes;
}
