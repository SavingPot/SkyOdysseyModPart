using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameCore
{
    public class Plant : Block, IPlant
    {
        public Entity entityInside { get; set; }
        Transform IPlant.transform => transform;


        public override void DoStart()
        {
            base.DoStart();

            PlantCenter.AddPlant(this);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            PlantCenter.RemovePlant(this);
        }



        public override void OnEntityEnter(Entity entity)
        {
            base.OnEntityEnter(entity);

            PlantCenter.OnEntityEnter(this, entity);
        }

        public override void OnEntityStay(Entity entity)
        {
            base.OnEntityStay(entity);

            PlantCenter.OnEntityStay(this, entity);
        }

        public override void OnEntityExit(Entity entity)
        {
            base.OnEntityExit(entity);

            PlantCenter.OnEntityExit(this, entity);
        }
    }

    public interface IPlant
    {
        Entity entityInside { get; set; }
        Transform transform { get; }
        void OnEntityEnter(Entity entity);
        void OnEntityStay(Entity entity);
        void OnEntityExit(Entity entity);
    }

    public static class PlantCenter
    {
        public static List<IPlant> plants = new();

        public static void AddPlant(IPlant plant)
        {
            plants.Add(plant);
        }

        public static void RemovePlant(IPlant plant)
        {
            plants.Remove(plant);
        }

        public static void OnEntityEnter(IPlant plant, Entity entity)
        {
            PlayWalkSound();
        }

        public static void OnEntityStay(IPlant plant, Entity entity)
        {
            //这放在 OnEntityStay 而不是 OnEntityEnter 是因为如果有一个实体在里面而一个实体离开，我们需要保持原来的朝向
            plant.entityInside = entity;
        }

        public static void OnEntityExit(IPlant plant, Entity entity)
        {
            plant.entityInside = null;

            PlayWalkSound();
        }

        public static void PlayWalkSound()
        {
            GAudio.Play(Random.Range(0, 3) switch
            {
                0 => AudioID.WalkThroughGrass0,
                1 => AudioID.WalkThroughGrass1,
                2 => AudioID.WalkThroughGrass2,
                _ => throw new()
            }, true);
        }

        public static void Update()
        {
            foreach (var plant in plants)
            {
                if (plant.entityInside == null)
                {
                    plant.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    var rotationZ = Mathf.Clamp(10 / (plant.entityInside.transform.position.x - plant.transform.position.x), -30f, 30f);
                    plant.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
                }
            }
        }
    }
}
