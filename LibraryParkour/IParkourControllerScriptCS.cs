using System;
using System.Collections.Generic;
using UnityEngine;
public class IParkourControllerScriptCS : ParkourControllerScriptCS
{
    //这里记录一下光头强，熊大，熊二
    public IParkourPlayer_GuangTouQiang playerGuangTouQiang { set; get; }
    public IParkourPlayer_Xiong2 playerXiong2 { set; get; }
    public IParkourPlayer_XiongDa playerXiongDa { set; get; }
    public IParkourPlayer_Camera playerCamera { set; get; }
   
    //游戏碰撞音效播放对象
    //public GameObject brokenAudioSource = null;
    //游戏碰撞声音队列
    //private List<AudioSource> brokenSoundList = new List<AudioSource>(8);
    //游戏的监听源
    //public AudioListenerCtrl gameAudioListenerCtrl;

    //环绕相机
    //public PlayerController m_LookAroundCamera = null;

    //镜头要跳转
    public GameObject m_cameraGotoChange = null;

    //当前状态
    public enum GameStatus
    {
        status_Standby,
        status_NewGame,
        status_ContinueGame,
    }
    public GameStatus gameStatus { get; set; }
    internal RaceSceneControl.IParkourSceneDefine currentSceneDefine { set; get; }
    public float gameTime { get { return currentSceneDefine.gameTime; } }



    protected override void Awake()
    {
        base.Awake();
        if (playerList != null && playerList.Length != 0)
        {
            for (int i = 0; i < playerList.Length;i++ )
            {
                if (playerList[i] is IParkourPlayer_GuangTouQiang)
                {
                    playerGuangTouQiang = playerList[i] as IParkourPlayer_GuangTouQiang;
                }
                else if (playerList[i] is IParkourPlayer_Xiong2)
                {
                    playerXiong2 = playerList[i] as IParkourPlayer_Xiong2;
                }
                else if (playerList[i] is IParkourPlayer_XiongDa)
                {
                    playerXiongDa = playerList[i] as IParkourPlayer_XiongDa;
                }
                else if (playerList[i] is IParkourPlayer_Camera)
                {
                    playerCamera = playerList[i] as IParkourPlayer_Camera;
                }
            }
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    private static void ReplacePlayerWheelParticle(IParkourPlayerController player,RaceSceneControl.IParkourSceneDefine sceneDefine)
    {
        GameObject particleObject = (GameObject)Instantiate((GameObject)sceneDefine.frontWheelParticlePrefabs);
        particleObject.transform.parent = player.particleList[0].transform.parent;
        particleObject.transform.localPosition = player.particleList[0].transform.localPosition;
        particleObject.transform.localRotation = player.particleList[0].transform.localRotation;
        UnityEngine.Object.Destroy(player.particleList[0].gameObject);
        player.particleList[0] = particleObject.GetComponent<ParticleSystem>();

        particleObject = (GameObject)Instantiate((GameObject)sceneDefine.backWheelParticlePrefabs);
        particleObject.transform.parent = player.particleList[1].transform.parent;
        particleObject.transform.localPosition = player.particleList[1].transform.localPosition;
        particleObject.transform.localRotation = player.particleList[1].transform.localRotation;
        UnityEngine.Object.Destroy(player.particleList[1].gameObject);
        player.particleList[1] = particleObject.GetComponent<ParticleSystem>();
    }
    internal virtual void Initialization(RaceSceneControl.IParkourSceneDefine sceneDefine,
                    GameStatus status,IParkourPlayer_Xiong.PlayerIndex[] players)
    {
        currentSceneDefine = sceneDefine;
        gameStatus = status;
        //激活为当前监听源
        //AudioListenerCtrl.activeAudio = gameAudioListenerCtrl;
        //首先替换掉赛道预置对象
        hCheckPointsMainCS.hPatchesRandomizerCS.IsRandomPath = currentSceneDefine.IsRandomPath;
        hCheckPointsMainCS.hPatchesRandomizerCS.patchesPrefabs = currentSceneDefine.patchesPrefabs;
        //替换掉角色的粒子
        ReplacePlayerWheelParticle(playerGuangTouQiang, currentSceneDefine);
        ReplacePlayerWheelParticle(playerXiong2, currentSceneDefine);
        ReplacePlayerWheelParticle(playerXiongDa, currentSceneDefine);
        //设置角色状态
        switch(gameStatus)
        {
            case GameStatus.status_Standby:
                {
                    playerGuangTouQiang.IsAIController = true;
                    playerGuangTouQiang.IsInputController = false;
                    playerXiong2.IsAIController = true;
                    playerXiong2.IsInputController = false;
                    playerXiong2.IsEnterStart = false;
                    playerXiongDa.IsAIController = true;
                    playerXiongDa.IsInputController = false;
                    playerXiongDa.IsEnterStart = false;
                }
                break;
            case GameStatus.status_NewGame:
            case GameStatus.status_ContinueGame:
                {
                    playerGuangTouQiang.IsAIController = true;
                    playerGuangTouQiang.IsInputController = false;
                    if (players == null || players.Length == 0)
                    {
                        playerXiong2.IsAIController = true;
                        playerXiong2.IsInputController = false;
                        playerXiongDa.IsAIController = true;
                        playerXiongDa.IsInputController = false;
                    }
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < players.Length;i++ )
                        {
                            if (playerXiong2.playerIndex == players[i] && playerXiong2.IsEnterStart)
                            {
                                playerXiong2.IsAIController = false;
                                playerXiong2.IsInputController = true;
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            playerXiong2.IsAIController = true;
                            playerXiong2.IsInputController = false;
                        }

                        flag = false;
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (playerXiongDa.playerIndex == players[i] && playerXiongDa.IsEnterStart)
                            {
                                playerXiongDa.IsAIController = false;
                                playerXiongDa.IsInputController = true;
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            playerXiongDa.IsAIController = true;
                            playerXiongDa.IsInputController = false;
                        }
                    }
                    
                }
                break;
        }
        //先暂定游戏刷新
        isGamePaused = true;
    }
    ////当前正在播放声音的角色对象
    //private IParkourPlayerController playSoundPlayer = null;
    //private IParkourPlayerController nextPlaySoundPlayer
    //{
    //    get
    //    {
    //        if (playSoundPlayer == null)
    //        {
    //            return playerXiongDa;
    //        }
    //        else if (playSoundPlayer == playerGuangTouQiang)
    //        {
    //            return playerXiong2;
    //        }
    //        else if (playSoundPlayer == playerXiong2)
    //        {
    //            return playerXiongDa;
    //        }
    //        else if (playSoundPlayer == playerXiongDa)
    //        {
    //            return playerGuangTouQiang;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //}
    //private void HandlePlayerSoundInGameing()
    //{
    //    if (playSoundPlayer == null)
    //        playSoundPlayer = nextPlaySoundPlayer;
    //    //检测当前角色是否仍然在播放
    //    //正在播放就不处理了
    //    if (playSoundPlayer.playerSoundControler.IsPlayAudio)
    //        return;
    //    //这个角色播放完了跳到下一个角色
    //    for (int i = 0; i < 3;i++ )
    //    {
    //        playSoundPlayer = nextPlaySoundPlayer;
    //        //检测是否在播放，如果在播放就跳到下一个
    //        if (!playSoundPlayer.playerSoundControler.IsPlayAudio)
    //            break;
    //    }
    //    if (playSoundPlayer.playerSoundControler.IsPlayAudio)
    //        return;
    //    //让这个角色播放
    //    playSoundPlayer.playerSoundControler.PlayAudioSource(IParkourPlayerSound.PlayerSoundType.Type_Normal);
    //}
    ////播放碰撞声音
    //public void PlayBrokenSound(string name,Vector3 position)
    //{
    //    SoundEffectPlayer.SoundEffectData data = SoundEffectPlayer.FindSoundEffect(name);
    //    if (data == null)
    //        return;
    //    AudioClip sound = null;
    //    if (data.isLanguage)
    //    {
    //        sound = UniGameResources.currentUniGameResources.LoadLanguageResource_AudioClip(data.resourceName);
    //    }
    //    else
    //    {
    //        sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(data.resourceName);
    //    }
    //    AudioSource audioSource = null;
    //    if (brokenSoundList.Count != 0 && !brokenSoundList[0].isPlaying)
    //    {
    //        audioSource = brokenSoundList[0];
    //        brokenSoundList.RemoveAt(0);
    //    }
    //    else
    //    {
    //        if (brokenAudioSource == null)
    //            return;
    //        GameObject obj = (GameObject)Instantiate((GameObject)brokenAudioSource);
    //        obj.transform.parent = transform;
    //        obj.transform.localPosition = Vector3.zero;
    //        obj.transform.localRotation = Quaternion.identity;
    //        audioSource = obj.GetComponent<AudioSource>();
    //    }
    //    audioSource.transform.localPosition = transform.InverseTransformPoint(position);
    //    audioSource.clip = sound;
    //    audioSource.volume = data.volume;
    //    audioSource.Play();
    //    brokenSoundList.Add(audioSource);
    //}
    
    
}
