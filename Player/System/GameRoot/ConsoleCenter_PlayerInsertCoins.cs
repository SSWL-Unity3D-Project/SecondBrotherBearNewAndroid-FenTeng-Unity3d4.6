using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;
using UnityEngine;
partial class ConsoleCenter
{
    //用户投入的游戏币
    public static int[] playerInsertCoins = new int[2];
    //用户获得奖励的游戏币
    public static int[] playerRewardCoins = new int[2];
    //当前用拥有的游戏币
    public static int GetPlayerCoins(IParkourPlayer_Xiong.PlayerIndex index)
    {
        return playerInsertCoins[(int)index] + playerRewardCoins[(int)index];
    }
    public static void RedPlayerCoins(int value, IParkourPlayer_Xiong.PlayerIndex index)
    {
        //一般到这都是够扣的

        //如果不够扣也要扣完
        //首先扣投入的游戏币
        playerInsertCoins[(int)index] -= value;
        if (playerInsertCoins[(int)index] >= 0)//够扣
            return;
        //给出剩余值
        value = -playerInsertCoins[(int)index];
        playerInsertCoins[(int)index] = 0;
        playerRewardCoins[(int)index] -= value;
        if (playerRewardCoins[(int)index] >= 0)//够扣
            return;
        playerRewardCoins[(int)index] = 0;
    }
    //触发投币的函数
    public static void OnPlayerInsertCoins(int coins, IParkourPlayer_Xiong.PlayerIndex index)
    {
        //播放投币声音
        SoundEffectPlayer.Play("insertcoins.wav");
        //刷新游戏记录
        //GameRecordData.InsertCoins(coins);
        //累计用户投入的游戏币
        //需要一个一个的加，每次都要检测是否可以奖励
        //有可能一次响应带来投了好几个币
        for (int i = 0; i < coins; i++)
        {
            playerInsertCoins[(int)index] += 1;
            //是否可以奖励一个？
            //如果是零则是关闭的
            if (UniGameOptionsDefine.InsertConinsAwardConins != 0)
            {
                if (playerInsertCoins[(int)index] % UniGameOptionsDefine.InsertConinsAwardConins == 0)
                {
                    RewardPlayerCoins(1, index);
                }
            }
        }
    }
    //触发奖励游戏币
    public static void RewardPlayerCoins(int coins, IParkourPlayer_Xiong.PlayerIndex index)
    {
        //播放奖励游戏币动画
        //播放奖励游戏币声音
        SoundEffectPlayer.Play("insertcoins.wav");
        //增加奖励的游戏币
        playerRewardCoins[(int)index] += coins;
    }
    //局域网对战胜利奖励游戏币
    public static void LanWinAwardConins(IParkourPlayer_Xiong.PlayerIndex index)
    {
        //如果是零则是关闭的
        if (UniGameOptionsDefine.LanWinAwardConins != 0)
        {
            RewardPlayerCoins(UniGameOptionsDefine.LanWinAwardConins, index);
        }

    }
    public static void WanWinAwardConins(IParkourPlayer_Xiong.PlayerIndex index)
    {
        if (UniGameOptionsDefine.WanWinAwardConins != 0)
        {
            RewardPlayerCoins(UniGameOptionsDefine.WanWinAwardConins, index);
        }

    }
    //是否可以开始游戏
    public static bool IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex index)
    {
        //如果设置为免费模式任何时候都可以启动
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
            return true;
        //如果触发了免费键,任何时候都可以启动游戏
        //if (GameOptionsDefine.OneChargeModeStartGame == GameOptionsDefine.GameChargeMode.Mode_Free)
        //    return true;
        //当前拥有的游戏币足够支付
        if (GetPlayerCoins(index) >= UniGameOptionsDefine.StartGameCoins)
            return true;
        return false;
    }
    //是否可以继续游戏
    public static bool IsCarContinueGame(IParkourPlayer_Xiong.PlayerIndex index)
    {
        
        //如果设置为免费模式任何时候都可以继续
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
            return true;
        //如果触发了免费键,任何时候都可以继续游戏
        //if (GameOptionsDefine.OneChargeModeContinueGame == GameOptionsDefine.GameChargeMode.Mode_Free)
        //return true;
        //当前拥有的游戏币足够支付
        if (GetPlayerCoins(index) >= UniGameOptionsDefine.ContinueGameCoins)
            return true;
        return false;
    }
    //用户开始游戏，扣币函数
    public static void PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex index)
    {
        //记录一次首进
        UniGameRecordData.PlayerFirstIntoGame();
        //如果是免费模式就不支付了
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
        {
            return;
        }
        //if (GameOptionsDefine.OneChargeModeStartGame == GameOptionsDefine.GameChargeMode.Mode_Free)
        //{
        //    //进去就关闭掉
        //    GameOptionsDefine.OneChargeModeStartGame = GameOptionsDefine.GameChargeMode.Mode_InsertCoins;
        //    GameRecordData.IntoGame(GameOptionsDefine.GameChargeMode.Mode_Free, true);
        //    return;
        //}
        //不足够支付不记录
        if (GetPlayerCoins(index) < UniGameOptionsDefine.StartGameCoins)
            throw new Exception("not have enough coins!");
        RedPlayerCoins(UniGameOptionsDefine.StartGameCoins,index);
    }
    //用户继续游戏，扣币函数
    public static void PlayerContinueGamePayCoins(IParkourPlayer_Xiong.PlayerIndex index)
    {
        //记录一次持续
        UniGameRecordData.PlayerContinueIntoGame();
        //如果是免费模式就不支付了
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
        {
            return;
        }
        //if (GameOptionsDefine.OneChargeModeContinueGame == GameOptionsDefine.GameChargeMode.Mode_Free)
        //{
        //    return;
        //}
        //不足够支付不记录
        if (GetPlayerCoins(index) < UniGameOptionsDefine.ContinueGameCoins)
            throw new Exception("not have enough coins!");
        RedPlayerCoins(UniGameOptionsDefine.ContinueGameCoins,index);
    }
}
