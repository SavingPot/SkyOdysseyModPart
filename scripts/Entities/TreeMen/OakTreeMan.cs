using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class OakTreeManProperties : TreeManProperties<OakTreeManProperties>
    {
        public override string BodyTexture() => "ori:oak_tree_man_body";
        public override string HeadTexture() => "ori:oak_tree_man_head";
        public override string RightArmTexture() => "ori:oak_tree_man_right_arm";
        public override string LeftArmTexture() => "ori:oak_tree_man_left_arm";
        public override string RightLegTexture() => "ori:oak_tree_man_right_leg";
        public override string LeftLegTexture() => "ori:oak_tree_man_left_leg";
    }


    [EntityBinding(EntityID.OakTreeMan)]
    public class OakTreeMan : TreeMan<OakTreeManProperties>
    {

    }
}
