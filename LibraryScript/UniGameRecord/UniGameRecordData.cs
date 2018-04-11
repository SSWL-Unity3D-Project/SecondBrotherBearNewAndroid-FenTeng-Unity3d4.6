using System;
using System.IO;
using System.Text;
using UnityEngine;


class UniGameRecordData
{
    //投币统计信息
    public static UniInsertCoinsRecord insertCoinsRecord = null;




    public static void LoadAllRecord(UniGameResources gameresources)
    {
        //投币统计信息
        insertCoinsRecord = new UniInsertCoinsRecord(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath, "GameRecord\\InsertCoinsRecord.dat"),
                        gameresources);
    }
    //开始工作
    public static void StartWork()
    {
        
    }
    
    public static void SerializeRead(MemoryStream s)
    {
        BinaryWriter writer = new BinaryWriter(s);
        writer.Seek(0, SeekOrigin.Begin);
        //投币统计信息
        insertCoinsRecord.SerializeRead(writer);
    }
    public static void SerializeWrite(MemoryStream s)
    {
        s.Seek(0, SeekOrigin.Begin);
        BinaryReader reader = new BinaryReader(s);
        //投币统计信息
        insertCoinsRecord.SerializeWrite(reader);
    }
    //清除统计信息
    public static void ClearRecord()
    {
        //投币统计信息
        insertCoinsRecord.ClearRecord();
    }
    //玩家进行一次首进
    public static void PlayerFirstIntoGame()
    {
        insertCoinsRecord.PlayerFirstIntoGame();
    }
    //玩家持续进入游戏
    public static void PlayerContinueIntoGame()
    {
        insertCoinsRecord.PlayerContinueIntoGame();
    }
    public static void PrintReportForms(StreamWriter file)
    {
        //投币统计信息
        file.WriteLine("投币统计信息:");
        insertCoinsRecord.PrintReportForms(file);
        file.WriteLine("");
    }
    public static void BinaryDataToReportForms(string binaryDataFile,string reportFromsFile)
    {
        //首先预加载一次创建对象
        LoadAllRecord(GameRoot.gameResource);
        //读取二进制文件
        //byte[] fileData = File.ReadAllBytes(binaryDataFile);

        string str = File.ReadAllText(binaryDataFile);
        byte[] fileData = new byte[str.Length / 2];
        char[] bytedata = new char[2];
        for (int i = 0; i < fileData.Length; i++)
        {
            bytedata[0] = str[i * 2];
            bytedata[1] = str[i * 2 + 1];
            string temp = new string(bytedata);
            temp = "0x" + temp;
            int dddddd = FTLibrary.Command.FTConvert.AutoToInt32(temp);
            fileData[i] = Convert.ToByte(dddddd);
        }

        MemoryStream fileStream = new MemoryStream(fileData);
        SerializeWrite(fileStream);
        //使用SerializeRead函数写入到本地的信息记录文件内
        //SerializeRead
        //再重新加载一次
        LoadAllRecord(GameRoot.gameResource);

       
        //构造目标文件
        //向目标文件输出报表
        {
            StreamWriter file = new StreamWriter(reportFromsFile);
            PrintReportForms(file);
            file.Close();
        }
        
    }
}
