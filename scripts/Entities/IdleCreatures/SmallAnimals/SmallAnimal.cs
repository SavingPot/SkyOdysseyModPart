namespace GameCore
{
    public abstract class SmallAnimal : IdleCreature
    {
        public abstract string Texture();

        protected override void Start()
        {
            base.Start();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(Texture());
        }
    }
}