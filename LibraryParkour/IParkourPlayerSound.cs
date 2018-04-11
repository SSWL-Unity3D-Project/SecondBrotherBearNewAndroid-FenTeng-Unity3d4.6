using System;
using System.Collections.Generic;
using UnityEngine;
public class IParkourPlayerSound : MonoBehaviourIgnoreGui
{
    ///*
    //* 角色声音处理方法：
    //* 1,同一个角色同时只能说一句话，也就是说如果发现角色正在说话就不在播放其他话了
    //* 2，正常的说话轮换角色，顺便为光头强，熊二，熊大。
    //* 3，某一个角色如果需要正常说话的时候发现要说话的角色正在说话就跳过，继续下一个角色。
    //* 4，角色有两种特殊的话：1，撞上障碍物，2，吃到东西得意的时候。
    //* 5，如果触发了特殊对话的时候，发现这个角色正在说话。就不在说话了
    //* 6, 游戏结束的语音使用2D播放
    //* */
    //public enum PlayerSoundType
    //{
    //    Type_Normal,
    //    Type_AddScore,
    //    Type_Barrier,
    //}
    //public string[] PlayerSound_NormalList = null;
    //public string[] PlayerSound_AddScoreList = null;
    //public string[] PlayerSound_BarrierList = null;
    ////声音播放组件
    //public AudioSource playerAudioSource = null;
    ////是否正在播放声音
    //public bool IsPlayAudio 
    //{ 
    //    get 
    //    {
    //        if (playerAudioSource == null)
    //            return false;
    //        return playerAudioSource.isPlaying;
    //    } 
    //}
    ////播放指定类型的声音
    //public bool PlayAudioSource(PlayerSoundType t)
    //{
    //    if (playerAudioSource == null)
    //        return false;
    //    if (IsPlayAudio)
    //        return false;
    //    string[] list = null;
    //    switch (t)
    //    {
    //        case PlayerSoundType.Type_Normal:
    //            list = PlayerSound_NormalList;
    //            break;
    //        case PlayerSoundType.Type_AddScore:
    //            list = PlayerSound_AddScoreList;
    //            break;
    //        case PlayerSoundType.Type_Barrier:
    //            list = PlayerSound_BarrierList;
    //            break;
    //    }
    //    if (list == null || list.Length == 0)
    //        return false;
    //    string name = list[FTLibrary.Command.FTRandom.Next(0, list.Length)];
    //    SoundEffectPlayer.SoundEffectData data = SoundEffectPlayer.FindSoundEffect(name);
    //    if (data == null)
    //        return false;
    //    AudioClip sound = null;
    //    if (data.isLanguage)
    //    {
    //        sound = UniGameResources.currentUniGameResources.LoadLanguageResource_AudioClip(data.resourceName);
    //    }
    //    else
    //    {
    //        sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(data.resourceName);
    //    }
    //    playerAudioSource.clip = sound;
    //    playerAudioSource.volume = data.volume;
    //    playerAudioSource.Play();
    //    return true;
    //}
}
