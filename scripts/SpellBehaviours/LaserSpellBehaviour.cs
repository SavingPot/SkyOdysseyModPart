using System.Collections.Generic;
using System.Numerics;
using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools.Unity;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace GameCore
{
    public static class LaserLightPool
    {
        public class LaserLight : MonoBehaviour
        {
            public LineRenderer line { get; private set; }
            public Light2D light { get; private set; }
            public Vector3 originPoint { get; private set; }
            public Vector3 endPoint { get; private set; }

            private void Awake()
            {
                //添加线渲染器
                line = gameObject.AddComponent<LineRenderer>();
                line.material = new(GInit.instance.defaultMat)
                {
                    //TODO: 贴图
                    //mainTexture = GInit.instance.spriteUnknown.texture
                };
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.useWorldSpace = false;

                //设置光照组件
                light = gameObject.GetComponent<Light2D>();
                light.lightType = Light2D.LightType.Freeform;
                light.intensity = 0.1f;
            }

            public void SetPoints(Vector3 originPoint, Vector3 endPoint)
            {
                //设置激光的位置
                transform.position = originPoint;
                this.originPoint = originPoint;
                this.endPoint = endPoint;

                //转换世界坐标为本地坐标
                var localOriginPoint = transform.InverseTransformPoint(originPoint);
                var localEndPoint = transform.InverseTransformPoint(endPoint);

                //设置线的端点
                line.SetPosition(0, localOriginPoint);
                line.SetPosition(1, localEndPoint);

                //设置光亮的形状
                light.SetShapePath(new Vector3[] {
                    localOriginPoint + new Vector3(0, 0.1f),
                    localOriginPoint - new Vector3(0, 0.1f),
                    localEndPoint - new Vector3(0, 0.1f),
                    localEndPoint + new Vector3(0, 0.1f),
                });
            }

            private void Update()
            {
                //如果激光已经结束, 则回收
                if (Mathf.Abs(originPoint.x - endPoint.x) < 0.1f && Mathf.Abs(originPoint.y - endPoint.y) < 0.1f)
                    LaserLightPool.Recover(this);

                //让起点慢慢接近终点
                var newZero = Vector3.Lerp(originPoint, endPoint, Tools.deltaTime * 6);
                SetPoints(newZero, endPoint);
            }
        }



        public static readonly Stack<LaserLight> stack = new();

        public static LaserLight Get(Vector3 originPoint, Vector3 endPoint)
        {
            LaserLight result;



            if (stack.Count > 0)
            {
                result = stack.Pop();
                result.gameObject.SetActive(true);
            }
            else
            {
                //创建子物体
                var light = GameObject.Instantiate(GInit.instance.GetLightPrefab());
                light.gameObject.name = "ori:laser_light";

                //设置结果
                result = light.gameObject.AddComponent<LaserLight>();
            }



            //设置激光的位置
            result.SetPoints(originPoint, endPoint);


            return result;
        }

        public static void Recover(LaserLight obj)
        {
            obj.gameObject.SetActive(false);
            stack.Push(obj);
        }
    }



    [SpellBinding(SpellID.Laser)]
    public class LaserSpellBehaviour : SpellBehaviour
    {
        public const string LaserLightCommandId = "ori:laser_light";
        public const int LaserDamage = 4;
        public const int LaserLength = 12;
        public static Vector2 LaserImpactForce { get; } = new(2, 0);

        public override void Release(Vector2 releaseDirection, Vector2 releasePosition, Player player)
        {
            JObject jo = new();
            jo.AddObject("ori:laser_light");
            jo["ori:laser_light"].AddProperty("direction", new float[] { releaseDirection.x, releaseDirection.y });
            player.ServerToClientsExecuteParamRemoteCommand(LaserLightCommandId, jo);
        }

        public static void LaserLight(Entity entity, JObject jo)
        {
            //获取参数
            var direction = jo["ori:laser_light"]["direction"].ToVector3().normalized;
            var originPoint = entity.transform.position;

            //发射射线
            var rayHit = RayTools.Hit(originPoint, direction, LaserLength);
            Vector3 endPoint = rayHit.transform != null ? rayHit.point : originPoint + direction * LaserLength;


            //显示激光
            var laser = LaserLightPool.Get(originPoint, endPoint);



            //攻击伤害
            if (rayHit.transform && rayHit.transform.gameObject.TryGetComponent<Entity>(out var hitEntity))
            {
                hitEntity.TakeDamage(LaserDamage, 0.1f, originPoint, originPoint.x < hitEntity.transform.position.x ? LaserImpactForce : -LaserImpactForce);
            }
        }

        public LaserSpellBehaviour(ISpellContainer spellContainer, Spell instance) : base(spellContainer, instance)
        {

        }
    }
}