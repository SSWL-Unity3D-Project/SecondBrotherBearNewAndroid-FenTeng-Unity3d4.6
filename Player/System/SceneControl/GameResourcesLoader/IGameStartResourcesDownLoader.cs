//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Collections;
//using UnityEngine;
//class IGameStartResourcesDownLoader : UniGameStartResourcesDownLoader
//{
//    protected SceneControlRoot sceneControlRoot = null;
//    protected IEnumerator Start()
//    {
//        //开始创建显示游戏登陆画面
//        //构造进度条等信息，但不显示
//        //显示提示字，游戏检测更新

//        //一般这个组件和Root的场景控制绑定在一个对象上这里查找
//        sceneControlRoot = GetComponent<SceneControlRoot>();

//        //延迟一定的时间开始进行资源包更新的检测
//        //延迟的目的是让整个界面显示出来
//        yield return new WaitForSeconds(0.1f);

//        //可以准备开始资源包检测工作了
//        yield return StartCoroutine(CheckNetResourcesPackage());
//    }
//    //开始下载
//    protected override void OnStartDownload()
//    {
//        //如果有下载则会响应这个函数
//        //创建下载进度条并且开始显示下载进度信息
//        Debug.Log("OnStartDownload");
//    }
//    //资源包下载检测完成
//    protected override void OnDownloadOver()
//    {
//        base.OnDownloadOver();
//        //资源下载完成会调用这个函数
//        //进行一些处理过程
//        sceneControlRoot.OnGameStartResourcesLoaderOver();
//    }
//    private void Update()
//    {
//        if (!IsDownloading)
//            return;
//        //开始界面刷新过程
//    }
//    //错误处理
//    protected override void OnDownloadErr(ResourcesLoaderErr errcode, string err)
//    {
//        Debug.Log(errcode + " " + err);
//    }
//}
