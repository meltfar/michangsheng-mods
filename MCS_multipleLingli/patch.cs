using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YSGame.EquipRandom;

namespace MCS_multipleLingli
{
    [BepInPlugin("MCS_multipleLingqi_zp", "MCS_multiLingqi", "1.0.0")]
    public class PatchLingliPlugin: BaseUnityPlugin
    {
        static ConfigEntry<int> multiple;

        public delegate void Log(object data);
        public static Log LogDebug;
        public static Log LogInfo;
        public static Log LogWarning;

        public void Start()
        {
            LogDebug = Logger.LogDebug;
            LogInfo = Logger.LogInfo;
            LogWarning = Logger.LogWarning;

            multiple = Config.Bind("config", "multiple", 9, "multiple");
            var harmony = Harmony.CreateAndPatchAll(typeof(PatchLingliPlugin));

            LogInfo($"MCS_multi Lingqi loaded, current multiple is {multiple.Value}");
        }

        public List<JSONObject> GetAllImportantNPCByGender(int gender = 2)
        {
            List<JSONObject> result = new List<JSONObject>();
            foreach (var k in jsonData.instance.AvatarJsonData.keys)
            {
                var v = jsonData.instance.AvatarJsonData[k];

                if (v["isImportant"]!=null && v["isImportant"].b 
                    && v["SexType"]!=null && v["SexType"].I == gender) 
                {
                    var name = jsonData.instance.AvatarRandomJsonData[v["id"].i.ToString()]["Name"].str.ToCN();
                    UnityExplorer.ExplorerCore.Log(name + ": " + v["id"].str);
                    result.Add(v);
                }
            }

            return result;
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
                if (v!=null && st !=null && v.b && st.i == 2)
                {
                    UnityExplorer.ExplorerCore.Log(k["Name"].str.ToCN() + " ===> " + k.ToString());
                }
            }
        }

        [HarmonyPatch(typeof(ShowXiaoGuoManager), nameof(ShowXiaoGuoManager.getTotalCiTiao))]
        [HarmonyPostfix]
        public static void PatchGetTotalCiTiao(ShowXiaoGuoManager __instance)
        {
            try
            {
                foreach (var k in __instance.entryDictionary.Keys.ToList())
                {
                    __instance.entryDictionary[k] *= multiple.Value;
                }
            }
            catch (System.Exception ex)
            {
                LogWarning($"Exception in patch of void ShowXiaoGuoManager::getTotalCiTiao():\n{ex}");
            }
        }



        [HarmonyPatch(typeof(RandomEquip), nameof(RandomEquip.RandomEquipName))]
        [HarmonyPrefix]
        static void PatchRandomEquipName()
        {
            try
            {
                var init = typeof(RandomEquip).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Static);
                init.Invoke(null, null);
            } 
            catch
            {

            }
        }

    }
}
