using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;

namespace Celeste.Mod.CherryHelper
{
    // Celeste.EntityToggleBell


    [Tracked(false)]
    [CustomEntity("CherryHelper/EntityToggleBell")]
    public class EntityToggleBell : Entity
    {


        private Sprite sprite;

        public Wiggler ScaleWiggler;

        private Wiggler moveWiggler;

        private Vector2 moveWiggleDir;

        private BloomPoint bloom;

        private float timer;

        private string nightId;
        private float bounceSfxDelay;

        private SoundEmitter sfx;

        private HoldableCollider holdableCollider;

        private EntityID entityID;
        private bool canToll = true;

        public EntityToggleBell(Vector2 position, string nightId)
        {
            this.nightId = nightId;
            Add(holdableCollider = new HoldableCollider(OnHoldable));
            Add(new MirrorReflection());
        }

        public EntityToggleBell(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            nightId = data.Attr("nightId", "");
        }
        public override void Render()
        {
            sprite.DrawOutline();
            base.Render();
        }
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Add(sprite = GFX.SpriteBank.Create("entityBell"));
            Level level = base.Scene as Level;
            base.Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(OnPlayer));
            Add(bloom = new BloomPoint(0.75f, 16f));
            moveWiggler = Wiggler.Create(0.8f, 2f);
            moveWiggler.StartZero = true;
            Add(moveWiggler);
        }

        public override void Update()
        {
            bounceSfxDelay -= Engine.DeltaTime;
            timer += Engine.DeltaTime;
            sprite.Position = Vector2.UnitY * (float)Math.Sin(timer * 2f) * 2f + moveWiggleDir * moveWiggler.Value * -8f;
            base.Update();
            if (Scene.OnInterval(0.1f))
            {
                SceneAs<Level>().Particles.Emit(HeartGem.P_BlueShine, 1, base.Center, Vector2.One * 8f);
            }
        }

        public void OnHoldable(Holdable h)
        {
            Player entity = base.Scene.Tracker.GetEntity<Player>();
            if (entity != null && h.Dangerous(holdableCollider))
            {
                Add(new Coroutine(Toll()));
            }
        }

        public void OnPlayer(Player player)
        {
            if (player.DashAttacking && canToll)
            {
                Add(new Coroutine(Toll()));
                Add(new Coroutine(tollDelay()));
            }
            if (bounceSfxDelay <= 0f && !player.DashAttacking)
            {

                Audio.Play("event:/cherryhelper/solobell");
                bounceSfxDelay = 0.1f;
            }
            BellLaunch(player, Center, false, false, (player.DashAttacking && canToll));
            moveWiggler.Start();
            moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
            if (moveWiggleDir.X <= 0) sprite.Play("ring_once_l");
            else sprite.Play("ring_once_r");
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        }

        private Vector2 BellLaunch(Player self, Vector2 from, bool snapUp, bool sidesOnly, bool bellToll)
        {
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            Vector2 vector = (self.Center - from).SafeNormalize(-Vector2.UnitY);
            float num = Vector2.Dot(vector, Vector2.UnitY);
            if (snapUp && num <= -0.7f)
            {
                vector.X = 0f;
                vector.Y = -1f;
            }
            else if (num <= 0.65f && num >= -0.55f)
            {
                vector.Y = 0f;
                vector.X = Math.Sign(vector.X);
            }
            if (sidesOnly && vector.X != 0f)
            {
                vector.Y = 0f;
                vector.X = Math.Sign(vector.X);
            }
            self.Speed = 280f * vector;
            if (self.Speed.Y <= 50f)
            {
                self.Speed.Y = Math.Min(-150f, self.Speed.Y);
                self.AutoJump = true;
            }
            if (Input.MoveX.Value == Math.Sign(self.Speed.X))
            {
                self.Speed.X *= 1.2f;
            }
            SlashFx.Burst(base.Center, self.Speed.Angle());
            if (!self.Inventory.NoRefills)
            {
                self.RefillDash();
            }
            self.RefillStamina();
            new DynData<Player>(self).Set<float>("dashCooldownTimer", 0.2f);
            self.StateMachine.State = 7;
            if (bellToll) self.Speed = self.Speed * 0.8f;
            else self.Speed = self.Speed * 0.6f;
            return vector * 0.75f;
        }

        public IEnumerator Toll()
        {
            sprite.Play("ringading");
            Audio.Play("event:/cherryhelper/belltoll", Position);
            yield return 0.2f;
            foreach (EntityToggleField field in EntityToggleField.FindById(SceneAs<Level>(), nightId))
            {
                field.on = !field.on;
            }
            yield break;
        }

        private IEnumerator tollDelay()
        {
            canToll = false;
            yield return 1;
            canToll = true;
            yield break;
        }
    }

}
