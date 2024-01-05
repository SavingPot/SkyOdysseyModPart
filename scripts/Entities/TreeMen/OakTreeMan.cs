using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.OakTreeMan)]
    public class OakTreeMan : TreeMan
    {
        protected override void Awake()
        {
            base.Awake();

            BodyTexture = "ori:oak_tree_man_body";
            HeadTexture = "ori:oak_tree_man_head";
            RightArmTexture = "ori:oak_tree_man_right_arm";
            LeftArmTexture = "ori:oak_tree_man_left_arm";
            RightLegTexture = "ori:oak_tree_man_right_leg";
            LeftLegTexture = "ori:oak_tree_man_left_leg";
        }
    }
}
