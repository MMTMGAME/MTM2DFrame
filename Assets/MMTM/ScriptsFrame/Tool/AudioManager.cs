using System;
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using Lean.Pool;
using NaughtyAttributes;
using UnityEngine;

public class AudioManager : MonoBehaviour,IOnDespawn
{
    [SerializeField]
    private AudioData audioData;
    public bool isOnEnablePlaySound = false;
    [ShowIf("isOnEnablePlaySound")]
    [DarkTonic.MasterAudio.SoundGroupAttribute]
    public string onShowPlaySoundName;
    
    List<AudioData.Audio> NeedAudioes;

    //储存没有跟随玩家的声音，万一声音是循环播放，那就会一直播放下去,所以需要解决一下
    private Dictionary<string,PlaySoundResult> loopSounds;

    private void Awake()
    {
        if(audioData!=null) NeedAudioes = audioData.NeedAudioes;
    }

    private void OnEnable()
    {
        if(isOnEnablePlaySound)
        MasterAudio.PlaySound3DAtVector3(onShowPlaySoundName, transform.position);
    }

    public void OnDespawn()
    {
        StopSounds();
    }

    public void StopSounds()
    {
        MasterAudio.StopAllSoundsOfTransform(transform);

        if (loopSounds != null)
        {
            foreach (var result in loopSounds)
            {
                if(result.Value.SoundPlayed) result.Value.ActingVariation.Stop();
            }
            loopSounds.Clear();
        }


       
    }

    public void StopLoopSound(string soundName)
    {
        if (loopSounds.TryGetValue(soundName, out var value))
        {
            value.ActingVariation.Stop();
            loopSounds.Remove(soundName);
        }
    }

    public void PlayAuidoTransform(string audioName)
    {
        var audio = NeedAudioes.FirstOrDefault(p => p.name.Contains(audioName));
        if((audio.isLoop && loopSounds!=null && loopSounds.ContainsKey(audioName)))return;
        if (audio.name.Length > 0)
        {
            var result = MasterAudio.PlaySound3DAtTransform(audio.audioname, transform, audio.volume);
           result.ActingVariation.VarAudio.loop = audio.isLoop;
           if (audio.isLoop)
           {
               if (loopSounds == null) loopSounds = new Dictionary<string, PlaySoundResult>();
               loopSounds.Add(audioName,result);
           }
        }
    }
    public void PlayAuidoVector3(string audioName)
    {
        var audio = NeedAudioes.FirstOrDefault(p => p.name.Contains(audioName));
        if((audio.isLoop && loopSounds!=null && loopSounds.ContainsKey(audioName)))return;
        if (audio.name.Length > 0)
        {
            var result = MasterAudio.PlaySound3DAtVector3(audio.audioname, transform.position, audio.volume);
            if (result != null)
            {
                result.ActingVariation.VarAudio.loop = audio.isLoop;
                if (audio.isLoop)
                {
                    if (loopSounds == null) loopSounds = new Dictionary<string, PlaySoundResult>();
                    loopSounds.Add(audioName,result);
                }
            }


        }
    }

    public void NowPlayAudio(string name)
    {
        DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform(name,transform);
    }

    public Action onDespawn { get; set; }
    public Action onDespawnClear { get; set; }
}
