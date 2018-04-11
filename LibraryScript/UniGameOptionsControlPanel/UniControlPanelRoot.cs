using System;

class UniControlPanelRoot : UniControlPanelBase
{
    protected override void InitializationPanelItem()
    {
        //<item name="ControlPanel_4" packagename="" packageid="0x0" path="游戏设定" version="0"/>
        //case 0x2B87E8FA://ControlPanel_4 CRC32HashCode
        AddTextItem(0x2B87E8FA, GameRoot.gameResource.LoadLanguageResource_Text("ControlPanel_4"));


        //<item name="ControlPanel_5" packagename="" packageid="0x0" path="投币设定" version="0"/>
        //case 0x5C80D86C://ControlPanel_5 CRC32HashCode
        AddTextItem(0x5C80D86C, GameRoot.gameResource.LoadLanguageResource_Text("ControlPanel_5"));

        //<item name="ControlPanel_7" packagename="" packageid="0x0" path="退出" version="0"/>
        //case 0xB28EB940://ControlPanel_7 CRC32HashCode
        AddTextItem(0xB28EB940, GameRoot.gameResource.LoadLanguageResource_Text("ControlPanel_7"));

        //<item name="ControlPanel_8" packagename="" packageid="0x0" path="恢复出差设置(重启生效)" version="0"/>
        //case 0x2231A4D1://ControlPanel_8 CRC32HashCode
        AddTextItem(0x2231A4D1, GameRoot.gameResource.LoadLanguageResource_Text("ControlPanel_8"));
        
    }
    protected override void OnEnterItem(uint ItemId)
    {
        switch(ItemId)
        {
            case 0x2B87E8FA://ControlPanel_4 CRC32HashCode
                optionsProcess.CurrentControlPanelName = "ControlPanelGameOptions.prefab";
                break;
            case 0x5C80D86C://ControlPanel_5 CRC32HashCode
                optionsProcess.CurrentControlPanelName = "ControlPanelInsertCoins.prefab";
                break;

            case 0xB28EB940://ControlPanel_7 CRC32HashCode
                //GameRoot.gameProcessControl.ActivateProcess(typeof(SelfCheckingModalProcess));
                RestartGame();
                break;
            case 0x2231A4D1://ControlPanel_8 CRC32HashCode
                UniGameOptionsDefine.RemoveAllOptions();
                SetItemText(0x2231A4D1, GameRoot.gameResource.LoadLanguageResource_Text("ControlPanel_8") + "[succeed]");
                break;
        }
    }
}
