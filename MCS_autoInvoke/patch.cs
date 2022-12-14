using BepInEx;
using BepInEx.Configuration;
using Fungus;
using GUIPackage;
using HarmonyLib;
using KBEngine;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YSGame.Fight;

namespace MCS_autoInvoke
{
    [BepInPlugin("MCS_autoFireSpin", "MCS_autoFireSpin", "0.0.1")]
    public class Entrance: BaseUnityPlugin
    {
        public delegate void Log(object data);
        public static Log LogDebug;
        public static Log LogInfo;

        public static bool firing = false;

        ConfigEntry<KeyCode> hotkey;

        public void Update()
        {
            if (Input.GetKeyDown(hotkey.Value) 
                && UIFightPanel.Inst.FightCenterButtonController.ButtonType != UIFightCenterButtonType.None 
                && UIFightPanel.Inst.UIFightState != UIFightState.敌人回合
                && !_fight_type_noallowed.Contains(Tools.instance.monstarMag.FightType)
                && !firing)
            {
                var avatar = Tools.instance.getPlayer();
                System.Collections.Generic.List<GUIPackage.Skill> skills = avatar.skill;
                var fi = skills.FindIndex(s => s.skill_Name.StartsWith("鹤喙针"));
                if (fi == -1)
                {
                    UIPopTip.Inst.Pop("未装备鹤喙针");
                    return;
                }

                var hyz = skills[fi];

                RoundManager.instance.StartCoroutine(FireSpin(hyz));
            }
        }

        public IEnumerator Start()
        {
            LogDebug = Logger.LogDebug;
            LogInfo = Logger.LogInfo;

            hotkey = Config.Bind("config", "hotkey", KeyCode.F12, "hotkeys");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            LogInfo("auto Invoke started!");
            LogInfo(nameof(RoundManager.startRound));

            yield break;
        }

        public static readonly StartFight.FightEnumType[] _fight_type_noallowed = new StartFight.FightEnumType[] { StartFight.FightEnumType.JieDan, 
            StartFight.FightEnumType.JieYing, StartFight.FightEnumType.ZhuJi, StartFight.FightEnumType.HuaShen, 
            StartFight.FightEnumType.天劫秘术领悟, StartFight.FightEnumType.煅体, StartFight.FightEnumType.FeiSheng };

        [HarmonyPatch(typeof(RoundManager), "gameStart")]
        public class PatchGameStart
        {
            static public void Postfix()
            {
                LogInfo("detected battle begin!");
                KBEngine.Avatar player = (KBEngine.Avatar)KBEngineApp.app.player();

                // 领域不影响天劫
                if (player.getLevelType() >= 5 && !_fight_type_noallowed.Contains(Tools.instance.monstarMag.FightType))
                {
                    int i = player.HuaShenLingYuSkill.I;
                    GUIPackage.Skill newSkill = new GUIPackage.Skill(SkillDatebase.instence.Dict[i][1].skill_ID, 0, 10);
                    UIFightPanel.Inst.HuaShenLingYuBtn.mouseUpEvent.RemoveAllListeners();
                    UIFightPanel.Inst.HuaShenLingYuBtn.mouseUpEvent.AddListener(() =>
                    {
                        if (UIFightPanel.Inst.UIFightState == UIFightState.敌人回合)
                        {
                            UIPopTip.Inst.Pop("敌方回合，无法使用");
                        }
                        else
                        {
                            player.skill.Add(newSkill);
                            player.spell.spellSkill(newSkill.skill_ID);
                            UIFightPanel.Inst.HuaShenLingYuBtn.transform.parent.gameObject.SetActive(false);
                        }
                    });
                }
            }
        }

        // 1133 - 鹤喙针3，1143 - 鹤回翔3
        // 测试下重复点击能否排错
        public static IEnumerator FireSpin(GUIPackage.Skill skill)
        {
            LogInfo("we are in coroutine now!");
            if (firing)
            {
                yield break;
            } else
            {
                firing = true;
            }
            var lqs = UIFightPanel.Inst.PlayerLingQiController.SlotList;

            var enemy = (Avatar)KBEngineApp.app.entities[11];

            var sum = 0;

            // 手动赋予自己1层缠绕
            var player = Tools.instance.getPlayer();
            var bm = player.buffmag;
            bm.entity.spell.addBuff(8, 1); // buffid 8 - 缠绕

            UIFightLingQiSlot.IgnoreEffect = true;
            while (lqs[1].LingQiCount > 4 && lqs[2].LingQiCount > 4 
                //&& sum < 50 
                && enemy.HP > 0 
                && UIFightPanel.Inst.FightCenterButtonController.ButtonType != UIFightCenterButtonType.None)
            {
                // 压住对面的练鼓
                var jianshang = enemy.buffmag.GetBuffSum(4);
                if (jianshang > 15)
                {
                    // 压对面的减伤
                    enemy.buffmag.RemoveBuff(4);
                }

                // 发射
                RoundManager.instance.SetChoiceSkill(ref skill);
                RoundManager.instance.UseSkill();

                sum++;
                yield return new WaitForSeconds(0.25f);
            }

            UIFightLingQiSlot.IgnoreEffect = false;
            UIFightPanel.Inst.RefreshLingQiCount(true);
            UIPopTip.Inst.Pop("鹤喙针释放完成, 共释放" + sum.ToString() + "次");

            //bm.RemoveBuff(8);
            firing = false;
        }

    }
}
