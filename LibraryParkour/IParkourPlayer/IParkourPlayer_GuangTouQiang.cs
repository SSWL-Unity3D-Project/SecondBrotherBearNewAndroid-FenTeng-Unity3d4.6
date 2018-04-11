using System;
using System.Collections.Generic;
using UnityEngine;
public class IParkourPlayer_GuangTouQiang : IParkourPlayerController
{
    public override PlayerType playerType { get { return PlayerType.Type_GuangTouQiang; } }
    protected override void Start()
    {
        base.Start();
        //光头强需要强制打开AI控制，关闭输入控制
        IsAIController = true;
        IsInputController = false;
    }
    //光头强只会和障碍物碰撞，不会吃东西
    protected override void OnTriggerEnter(Collider other)
    {
        BrokenObjectController broken = other.gameObject.GetComponent<BrokenObjectController>();

        if (broken == null)
        {
            
            return;
        }

        if (broken.IsLocker)
        {
            //Debug.Log(other.gameObject + "#跳出");
            return;
        }
        if (broken.brokenHandleType != BrokenObjectController.BrokenHandleType.HandleType_Barrier)
            return;
        //Debug.Log(other.gameObject + "#########");
        broken.IsLocker = true;
        //处理被撞物品
        broken.StartBroken(hControllerScriptCS as IParkourControllerScriptCS);
        //触发碰撞消息
        playerAIControler.OnEventHitBrokenObject();
        //播放碰撞语言
        SoundEffectPlayer.Play("touchbroken.wav");
        //playerSoundControler.PlayAudioSource(IParkourPlayerSound.PlayerSoundType.Type_Barrier);
    }
}
