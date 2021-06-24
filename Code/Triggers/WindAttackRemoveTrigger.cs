using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/SnowballStop")]
    public class WindAttackRemoveTrigger : Trigger
    {
        // Token: 0x06002683 RID: 9859 RVA: 0x00077FF6 File Offset: 0x000761F6
        public WindAttackRemoveTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
        }
        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.Level = base.SceneAs<Level>();
        }

        // Token: 0x06002684 RID: 9860 RVA: 0x000F51B8 File Offset: 0x000F33B8
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            bool flag = base.Scene.Entities.FindFirst<Snowball>() != null;
            if (flag)
            {
                Snowball entity = Scene.Entities.FindFirst<Snowball>();
                Level.Remove(entity);
                base.RemoveSelf();
            }
        }
        public Level Level;
    }
}
