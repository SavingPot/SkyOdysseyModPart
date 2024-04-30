using GameCore.High;
using SP.Tools.Unity;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class Ladder : Block
    {
        public static float entityFallingSpeed = 2;
        public static float entityClimbingSpeed = 10;
        public static float resistance = 1.75f;

        public override void OnEntityStay(Entity entity)
        {
            if (entity is Creature creature)
            {
                creature.fallenY = creature.transform.position.y;

                if (entity.isPlayer)
                {
                    Player player = (Player)entity;

                    if (player.rb)
                    {
                        if (Player.PlayerCanControl(player) && player.playerController.HoldingJump())
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
    }
}
