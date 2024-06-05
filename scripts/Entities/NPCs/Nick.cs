using GameCore.UI;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Nick)]
    public class Nick : NPC
    {
        public NickProgress progress;
        public int autoTalkRadius = 5 * 5; //5^2



        public override void Initialize()
        {
            base.Initialize();

            #region 添加肢体
            MethodAgent.DebugRun(() =>
            {
                CreateModel();
                body = AddBodyPart("body", ModFactory.CompareTexture("ori:nick_body_naked").sprite, Vector2.zero, 3, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture("ori:nick_head").sprite, new(-0.03f, -0.06f), 6, body, BodyPartType.Head);
                rightArm = AddBodyPart("rightarm", ModFactory.CompareTexture("ori:nick_right_arm").sprite, Vector2.zero, 4, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftarm", ModFactory.CompareTexture("ori:nick_left_arm").sprite, Vector2.zero, 2, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightleg", ModFactory.CompareTexture("ori:nick_right_leg").sprite, Vector2.zero, 2, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftleg", ModFactory.CompareTexture("ori:nick_left_leg").sprite, Vector2.zero, 1, body, BodyPartType.LeftLeg);
            });
            #endregion
        }

        public override JObject ModifyCustomData(JObject data)
        {
            data = base.ModifyCustomData(data);

            //添加 "ori:nick"
            var npcJT = data["ori:npc"];
            if (!npcJT.TryGetJToken("ori:nick", out var nickJT))
            {
                npcJT.AddObject("ori:nick");
                nickJT = npcJT["ori:nick"];
            }

            //添加 "progress"
            if (!nickJT.TryGetJToken("progress", out var progressJT))
            {
                nickJT.AddProperty("progress", NickProgress.FirstMeet);
                progressJT = nickJT["progress"];
            }

            return data;
        }

        protected override void LoadFromNpcJT(JToken jt)
        {
            base.LoadFromNpcJT(jt);

            progress = Enum.Parse<NickProgress>(jt["ori:nick"]["progress"].ToString());
        }

        protected override void Update()
        {
            base.Update();

            if (progress == NickProgress.FirstMeet && Player.local && (Player.local.transform.position - transform.position).sqrMagnitude <= autoTalkRadius)
            {
                FirstMeetDialog(Player.local);
            }
        }

        public override void PlayerInteraction(Player player)
        {
            base.PlayerInteraction(player);

            switch (progress)
            {
                case NickProgress.FirstMeet:
                    FirstMeetDialog(player);
                    break;

                case NickProgress.Teaching_Attack:
                    player.pui.DisplayDialog(new("ori:nick", "ori:button",
                    new(GameUI.CompareText("ori:dialog.nick.teaching.attack_0").text.Replace("{button}", GControls.mode switch //TODO: Compare these buttons' texts instead of output directly, support multi-languages
                    {
                        ControlMode.Touchscreen => player.GetUsingItemChecked() == null ? "空白的按钮(互动按钮)" : $"有个{GameUI.CompareText(player.GetUsingItemChecked().data.id).text}的按钮(互动按钮)",
                        ControlMode.KeyboardAndMouse => "鼠标右键",
                        ControlMode.Gamepad => "手柄左触发器",
                        _ => ""
                    }), "ori:nick_head"),
                    new(GameUI.CompareText("ori:dialog.nick.teaching.attack_1").text.Replace("{button}", GControls.mode switch
                    {
                        ControlMode.Touchscreen => "像一把剑的按钮",
                        ControlMode.KeyboardAndMouse => "鼠标左键",
                        ControlMode.Gamepad => "手柄右触发器",
                        _ => ""
                    }), "ori:nick_head"),
                    new(GameUI.CompareText("ori:dialog.nick.teaching.attack_2").text, "ori:nick_head")));

                    ProgressDeeper();
                    break;

                case NickProgress.Teaching_Backpack:
                    if (!player.inventory.IsEmpty())
                    {
                        player.pui.DisplayDialog(new("ori:nick", "ori:button",
                        new(GameUI.CompareText("ori:dialog.nick.teaching.backpack_0").text, "ori:nick_head"),
                        new(GameUI.CompareText("ori:dialog.nick.teaching.backpack_1").text.Replace("{button}", GControls.mode switch
                        {
                            ControlMode.Touchscreen => "像一个锤子的按钮",
                            ControlMode.KeyboardAndMouse => "键盘Tab键",
                            ControlMode.Gamepad => "手柄Start",
                            _ => ""
                        }), "ori:nick_head"),
                        new(GameUI.CompareText("ori:dialog.nick.teaching.backpack_2").text.Replace("{button}", GControls.mode switch
                        {
                            ControlMode.Touchscreen => "像个卷轴的按钮",
                            ControlMode.KeyboardAndMouse => "键盘T键",
                            ControlMode.Gamepad => "手柄上十字键",
                            _ => ""
                        }), "ori:nick_head"),
                        new(GameUI.CompareText("ori:dialog.nick.teaching.backpack_3").text, "ori:nick_head")));

                        ProgressDeeper();
                    }
                    else
                    {
                        player.pui.DisplayDialog(new("ori:nick", "ori:button",
                        new DialogData.DialogDatum[] { new(GameUI.CompareText("ori:dialog.nick.teaching.backpack_0.error_before").text, "ori:nick_head") }));
                    }

                    break;

                case NickProgress.Teaching_Travel:
                    player.pui.DisplayDialog(new("ori:nick", "ori:button",
                    new(GameUI.CompareText("ori:dialog.nick.teaching.travel_0").text, "ori:nick_head"),
                    new(GameUI.CompareText("ori:dialog.nick.teaching.travel_1").text, "ori:nick_head"),
                    new(GameUI.CompareText("ori:dialog.nick.teaching.travel_2").text, "ori:nick_head")));

                    ProgressDeeper();
                    break;

                default:
                    if (Item.Null(player.inventory.breastplate))
                        player.pui.DisplayDialog(new("ori:nick", "ori:button",
                        new DialogData.DialogDatum(GameUI.CompareText(GM.instance.weather.id == WeatherID.Rain ? "ori:dialog.nick.hello.shirtless_rain" : "ori:dialog.nick.hello.shirtless").text.Replace("{name}", player.playerName), "ori:nick_head")));
                    else
                        player.pui.DisplayDialog(new("ori:nick", "ori:button",
                        new DialogData.DialogDatum(GameUI.CompareText(GM.instance.weather.id == WeatherID.Rain ? "ori:dialog.nick.hello.rain" : "ori:dialog.nick.hello.normal").text.Replace("{name}", player.playerName), "ori:nick_head")));
                    break;
            }
        }

        public void FirstMeetDialog(Player caller)
        {
            caller.pui.DisplayDialog(new("ori:nick", "ori:button",
            new(GameUI.CompareText("ori:dialog.nick.first_meet_0").text, "ori:nick_head"),
            new(GameUI.CompareText("ori:dialog.nick.first_meet_1").text, "ori:nick_head"),
            new(GameUI.CompareText("ori:dialog.nick.first_meet_2").text, "ori:nick_head"),
            new(GameUI.CompareText("ori:dialog.nick.first_meet_3").text.Replace("{button}", GControls.mode switch //TODO: Compare these buttons' texts instead of output directly, support multi-languages
            {
                ControlMode.Touchscreen => string.Empty,
                ControlMode.KeyboardAndMouse => "鼠标右键",
                ControlMode.Gamepad => "手柄左触发器",
                _ => ""
            }), "ori:nick_head")));

            ProgressDeeper();
        }

        public void ProgressDeeper()
        {
            progress++;
            customData["ori:npc"]["ori:nick"]["progress"] = (int)progress;
        }
    }

    public enum NickProgress : byte
    {
        FirstMeet,
        Teaching_Attack,
        Teaching_Backpack,
        Teaching_Travel,
        AfterTeaching,
    }
}
