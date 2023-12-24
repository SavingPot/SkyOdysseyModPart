using GameCore.High;
using SP.Tools.Unity;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class Ladder : Block
    {
        public static float entityFallingSpeed = 2;
        public static float entityClimbingSpeed = 11;
        public static float resistance = 1.75f;

        public override void OnEntityStay(Entity entity)
        {
            if (!entity.isPlayer)
                return;

            Player player = (Player)entity;

            player.fallenY = player.transform.position.y;

            if (player.rb)
            {
                if (PlayerControls.HoldJump(player))
                {
                    if (player.rb.velocity.y < entityClimbingSpeed)
                    {
                        player.AddVelocityY(resistance);
                    }
                    else if (player.rb.velocity.y > entityClimbingSpeed)
                    {
                        player.AddVelocityY(-resistance);
                    }
                }
                else
                {
                    if (player.rb.velocity.y < -entityFallingSpeed)
                    {
                        player.AddVelocityY(resistance);
                    }
                    else if (player.rb.velocity.y > -entityFallingSpeed)
                    {
                        player.AddVelocityY(-resistance);
                    }
                }
            }
        }
    }
}
