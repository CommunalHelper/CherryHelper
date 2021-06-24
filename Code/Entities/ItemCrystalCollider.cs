using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.CherryHelper
{
    [Tracked(false)]
    public class ItemCrystalCollider : Component
    {
        public Action<ItemCrystal> OnCollide;

        public Collider Collider;

        public Collider FeatherCollider;

        public ItemCrystalCollider(Action<ItemCrystal> onCollide, Collider collider = null)
            : base(active: false, visible: false)
        {
            OnCollide = onCollide;
            Collider = collider;
        }

        public bool Check(ItemCrystal ItemCrystal)
        {
            Collider collider = Collider;
            if (collider == null)
            {
                if (ItemCrystal.CollideCheck(base.Entity))
                {
                    OnCollide(ItemCrystal);
                    return true;
                }
                return false;
            }
            Collider collider2 = base.Entity.Collider;
            base.Entity.Collider = collider;
            bool flag = ItemCrystal.CollideCheck(base.Entity);
            base.Entity.Collider = collider2;
            if (flag)
            {
                OnCollide(ItemCrystal);
                return true;
            }
            return false;
        }

        public override void DebugRender(Camera camera)
        {
            if (Collider != null)
            {
                Collider collider = base.Entity.Collider;
                base.Entity.Collider = Collider;
                Collider.Render(camera, Color.Green);
                base.Entity.Collider = collider;
            }
        }
    }


}
