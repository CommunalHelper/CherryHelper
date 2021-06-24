using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CherryHelper
{

    [CustomEntity(new string[]
    {
    "CherryHelper/NightTimeTrigger"
    })]
    [Tracked(false)]
    public class EntityToggleTrigger : Trigger
    {

        public EntityToggleField field;
        public string nightId;

        public bool Enable;

        public EntityToggleTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            nightId = data.Attr("nightId");
            Enable = data.Bool("Enable");
        }

        public override void OnEnter(Player player)
        {

            base.OnEnter(player);
            CherryHelper.ChangeFields(nightId, Enable);
        }
    }

}
