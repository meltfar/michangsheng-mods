using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*{
  "id": 20350,
  "Name": "",
  "IsTag": false,
  "FirstName": "",
  "face": 0,
  "fightFace": 0,
  "isImportant": true,
  "IsKnowPlayer": false,
  "QingFen": 0,
  "CyList": [],
  "TuPoMiShu": [],
  "Title": "金虹剑派长老",
  "ChengHaoID": 106,
  "GongXian": 0,
  "Type": 2,
  "LiuPai": 15,
  "MenPai": 3,
  "AvatarType": 1,
  "Level": 9,
  "WuDaoValue": 0,
  "WuDaoValueLevel": 0,
  "EWWuDaoDian": 0,
  "BindingNpcID": 3059,
  "SexType": 1,
  "YuanYingTime": "0140-03-06",
  "JinDanAddSpeed": 5720,
  "HuaShengTime": "0730-01-01",
  "YuanYingAddSpeed": 19855,
  "HP": 1176,
  "dunSu": 31,
  "shengShi": 34,
  "shaQi": 0,
  "shouYuan": 498,
  "NextExp": 6480000,
  "equipWeapon": 0,
  "equipWeaponPianHao": [70],
  "equipWeapon2PianHao": [70],
  "equipClothingPianHao": [86, 102, 101],
  "equipRingPianHao": [118, 126, 158],
  "equipClothing": 0,
  "equipRing": 0,
  "LingGen": [50, 10, 10, 10, 10],
  "skills": [6, 512],
  "JinDanType": [0],
  "staticSkills": [28, 5174, 33, 5083, 5278],
  "xiuLianSpeed": 4509,
  "yuanying": 0,
  "MoneyType": 5,
  "IsRefresh": 0,
  "dropType": 0,
  "canjiaPaiMai": 0,
  "paimaifenzu": [1],
  "wudaoType": 87,
  "XinQuType": 201,
  "gudingjiage": 0,
  "sellPercent": 0,
  "useItem": null,
  "wuDaoSkillList": [101, 113, 121, 133, 701, 712, 721, 731, 901, 912],
  "wuDaoJson": {
    "1": { "id": 1, "level": 4, "exp": 70000 },
    "2": { "id": 2, "level": 0, "exp": 0 },
    "3": { "id": 3, "level": 0, "exp": 0 },
    "4": { "id": 4, "level": 0, "exp": 0 },
    "5": { "id": 5, "level": 0, "exp": 0 },
    "6": { "id": 6, "level": 0, "exp": 0 },
    "7": { "id": 7, "level": 4, "exp": 70000 },
    "8": { "id": 8, "level": 0, "exp": 0 },
    "9": { "id": 9, "level": 4, "exp": 70000 },
    "10": { "id": 10, "level": 0, "exp": 0 },
    "21": { "id": 21, "level": 0, "exp": 0 },
    "22": { "id": 22, "level": 0, "exp": 0 }
  },
  "ziZhi": 92,
  "wuXin": 65,
  "XingGe": 7,
  "NPCTag": 1,
  "IsNeedHelp": false,
  "age": 3001,
  "ActionId": 1,
  "isTanChaUnlock": false,
  "exp": 6480000,
  "Status": { "StatusId": 2, "StatusTime": 60000 }
}
*/

namespace MCS_NPCManager
{
    [BepInPlugin("rs.rime.MCS_NPCManager", "MCS_NPCManager", "1.0.0")]
    public class MainWindow : BaseUnityPlugin
    {
        static ConfigEntry<KeyCode> hotkey;

        public delegate void Log(object data);
        public static Log LogDebug;
        public static Log LogInfo;
        public static Log LogWarning;

        public void Start()
        {
            LogDebug = Logger.LogDebug;
            LogInfo = Logger.LogInfo;
            LogWarning = Logger.LogWarning;

            hotkey = Config.Bind("config", "hotkey", KeyCode.U, "hotkey");

            LogInfo($"MCS_NPCManager loaded, current multiple is {hotkey.Value}");
        }

        public void test()
        {
            // log 
            // UnityExplorer.ExplorerCore.Log

            // 这个方法可以设置NPC属性
            // var bailu = jsonData.instance.AvatarJsonData["20313"];
            // bailu["shengShi"].i = 120;

            // AvatarJsonData包含重要的重要字段
            // id, 跟AvatarRandomJsonData一致，是唯一的同步方式, int
            // HuaShengTime 化神时间 "0700-01-01" string
            // YuanYingAddSpeed  元婴修炼速度 19855 int
            // shouYuan 寿元 498 int
            // HP int
            // ziZhi int
            // wuXin 悟性 int
            // dunSu 速度 int
            // shengShi 神识 int

            foreach (var k in jsonData.instance.AvatarJsonData.list)
            {
                var v = k["isImportant"];
                var st = k["SexType"];
                if (v != null && st != null && v.b && st.i == 2)
                {
                    LogInfo(k["Name"].str.ToCN() + " ===> " + k.ToString());
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(hotkey.Value))
            {
                windowShow = !windowShow;
            }
        }

        private bool windowShow = false;
        private Rect windowRect = new Rect(50, 50, 500, 300);

        public void OnGUI() {
            if (windowShow)
            {
                windowRect = GUILayout.Window(1, windowRect, WindowFunc, "NPC");
            }
        }

        private bool checkGameLoaded()
        {
            return jsonData.instance.AvatarRandomJsonData.list.Count > 1;
        }

        public void WindowFunc(int id)
        {
            // x
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("x"))
            {
                windowShow = false;
            }
            GUILayout.EndHorizontal();  

            GUILayout.BeginHorizontal();
            GUILayout.Label("wodetian");
            if (GUILayout.Button("OK"))
            {
                LogInfo("dianjileanniu");
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();

            if (GUI.changed)
            {
                // some input happened;
                LogInfo("changed");
            }
        }
    }
}
