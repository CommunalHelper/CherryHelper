using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.CherryHelper
{

    [CustomEntity("CherryHelper/ItemCrystalPedestal")]
    public class ItemCrystalPedestal : Entity
    {
        public Image sprite;
        private ItemCrystalCollider collid;

        public ItemCrystalPedestal(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Position = data.Position + offset;
            Add(sprite = GFX.SpriteBank.Create("itemCrystalPedestal"));
            Add(collid = new ItemCrystalCollider(OnHoldable, new Hitbox(24f, 24f, -12f, -8f)));
            collid.Visible = true;
            collid.Active = true;
        }
        public void OnHoldable(ItemCrystal crystal)
        {
            if (crystal == null) return;
            crystal.PlayerRelease();
            crystal.PlayerRelease();
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.4f, start: true);
            tween.OnUpdate = delegate (Tween t)
            {
                crystal.PlayerRelease();
                crystal.Position = Vector2.Lerp(crystal.Position, Position, t.Eased);
                crystal.sprite.Play("fill");
            };
            tween.OnComplete = delegate
            {
                crystal.Position = Position;
                crystal.Add(new Coroutine(crystal.DestroySequence()));
            };
            Add(tween);


        }
    };
}
