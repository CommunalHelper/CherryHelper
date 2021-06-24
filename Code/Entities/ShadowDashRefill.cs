using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity(new string[]
       {
        "CherryHelper/ShadowDashRefill"
       })]
    public class ShadowDashRefill : Entity

    {
        public ShadowDashRefill(Vector2 position, bool oneUse) : base(position)
        {
            base.Collider = new Hitbox(16f, 16f, -8f, -8f);
            base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
            this.oneUse = oneUse;
            string str;
            str = "objects/shadowDashRefill/";
            this.p_shatter = Refill.P_Shatter;
            this.p_regen = Refill.P_Regen;
            this.p_glow = Refill.P_Glow;
            base.Add(this.outline = new Image(GFX.Game[str + "outline"]));
            this.outline.CenterOrigin();
            this.outline.Visible = false;
            base.Add(this.sprite = new Sprite(GFX.Game, str + "idle"));
            this.sprite.AddLoop("idle", "", 0.1f);
            this.sprite.Play("idle", false, false);
            this.sprite.CenterOrigin();
            base.Add(this.flash = new Sprite(GFX.Game, str + "flash"));
            this.flash.Add("flash", "", 0.05f);
            this.flash.OnFinish = delegate (string anim)
            {
                this.flash.Visible = false;
            };
            this.flash.CenterOrigin();
            base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate (float v)
            {
                this.sprite.Scale = (this.flash.Scale = Vector2.One * (1f + v * 0.2f));
            }, false, false));
            base.Add(new MirrorReflection());
            base.Add(this.bloom = new BloomPoint(0.8f, 16f));
            base.Add(this.light = new VertexLight(Color.White, 1f, 16, 48));
            base.Add(this.sine = new SineWave(0.6f, 0f));
            this.sine.Randomize();
            this.UpdateY();
            base.Depth = -100;
        }

        // Token: 0x06002D7F RID: 11647 RVA: 0x0013099D File Offset: 0x0012EB9D
        public ShadowDashRefill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("oneUse", false))
        {
        }

        // Token: 0x06002D80 RID: 11648 RVA: 0x001309CB File Offset: 0x0012EBCB
        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.level = base.SceneAs<Level>();
        }

        // Token: 0x06002D81 RID: 11649 RVA: 0x001309E4 File Offset: 0x0012EBE4
        public override void Update()
        {
            base.Update();
            bool flag = this.respawnTimer > 0f;
            if (flag)
            {
                this.respawnTimer -= Engine.DeltaTime;
                bool flag2 = this.respawnTimer <= 0f;
                if (flag2)
                {
                    this.Respawn();
                }
            }
            else
            {
                bool flag3 = base.Scene.OnInterval(0.1f);
                if (flag3)
                {
                    this.level.ParticlesFG.Emit(BadelineBoost.P_Ambience, 1, this.Position, Vector2.One * 5f);
                }
            }
            this.UpdateY();
            this.light.Alpha = Calc.Approach(this.light.Alpha, this.sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            this.bloom.Alpha = this.light.Alpha * 0.8f;
            bool flag4 = base.Scene.OnInterval(2f) && this.sprite.Visible;
            if (flag4)
            {
                this.flash.Play("flash", true, false);
                this.flash.Visible = true;
            }
        }

        // Token: 0x06002D82 RID: 11650 RVA: 0x00130B24 File Offset: 0x0012ED24
        private void Respawn()
        {
            bool flag = !this.Collidable;
            if (flag)
            {
                this.Collidable = true;
                this.sprite.Visible = true;
                this.outline.Visible = false;
                base.Depth = -100;
                this.wiggler.Start();
                Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_return" : "event:/game/general/diamond_return", this.Position);
                this.level.ParticlesFG.Emit(BadelineBoost.P_Move, 16, this.Position, Vector2.One * 2f);
            }
        }

        // Token: 0x06002D83 RID: 11651 RVA: 0x00130BC8 File Offset: 0x0012EDC8
        private void UpdateY()
        {
            this.flash.Y = (this.sprite.Y = (this.bloom.Y = this.sine.Value * 2f));
        }

        // Token: 0x06002D84 RID: 11652 RVA: 0x00130C14 File Offset: 0x0012EE14
        public override void Render()
        {
            bool visible = this.sprite.Visible;
            if (visible)
            {
                this.sprite.DrawOutline(1);
            }
            base.Render();
        }

        // Token: 0x06002D85 RID: 11653 RVA: 0x00130C48 File Offset: 0x0012EE48
        private void OnPlayer(Player player)
        {
            if (CherryHelper.Session.HasShadowDash == false)
            {
                Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", this.Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                this.Collidable = false;
                base.Add(new Coroutine(this.RefillRoutine(player), true));
                player.UseRefill(false);
                CherryHelper.Session.HasShadowDash = true;
                this.respawnTimer = 2.5f;
            }
        }

        // Token: 0x06002D86 RID: 11654 RVA: 0x00130CB6 File Offset: 0x0012EEB6
        private IEnumerator RefillRoutine(Player player)
        {
            Celeste.Freeze(0.05f);
            yield return null;
            this.level.Shake(0.3f);
            this.sprite.Visible = (this.flash.Visible = false);
            bool flag = !this.oneUse;
            if (flag)
            {
                this.outline.Visible = true;
            }
            this.Depth = 8999;
            yield return 0.05f;
            float angle = player.Speed.Angle();
            this.level.ParticlesFG.Emit(Refill.P_ShatterTwo, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
            this.level.ParticlesFG.Emit(Refill.P_ShatterTwo, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
            SlashFx.Burst(this.Position, angle);
            bool flag2 = this.oneUse;
            if (flag2)
            {
                this.RemoveSelf();
            }
            yield break;
        }
        public static void Load()
        {
            On.Celeste.Player.Die += shadowDash;
            On.Celeste.Player.DashEnd += shadowDashEnd;
            On.Celeste.Player.DashBegin += shadowDashBegin;
            On.Celeste.PlayerHair.GetHairColor += shadowDashHairColor;
        }
        public static void Unload()
        {
            On.Celeste.Player.Die -= shadowDash;
            On.Celeste.Player.DashEnd -= shadowDashEnd;
            On.Celeste.Player.DashBegin -= shadowDashBegin;
            On.Celeste.PlayerHair.GetHairColor -= shadowDashHairColor;
        }

        public static Color shadowDashHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
        {
            if (CherryHelper.Session.HasShadowDash)
            {
                return Color.FromNonPremultiplied(self.Color.R / 4, self.Color.G / 4, self.Color.B / 4, 255);
            }
            else
            {
                return orig(self, index);
            }

        }

        private static PlayerDeadBody shadowDash(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
        {
            if (!CherryHelper.Session.ShadowDashActive || evenIfInvincible)
            {
                CherryHelper.Session.HasShadowDash = false;
                CherryHelper.Session.ShadowDashActive = false;
                return orig(self, direction, evenIfInvincible, registerDeathInStats);
            }
            return null;
        }
        private static void shadowDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
        {

            if (CherryHelper.Session.HasShadowDash)
            {
                CherryHelper.Session.ShadowDashActive = true;
                shadowEndDelayCoroutine?.RemoveSelf();
            }
            CherryHelper.Session.HasShadowDash = false;
            orig(self);
        }
        private static void shadowDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self)
        {
            orig(self);
            if (self.StateMachine.State != 2 && CherryHelper.Session.ShadowDashActive)
            {
                shadowEndDelayCoroutine = new Coroutine(ShadowEndDelay());
                self.Add(shadowEndDelayCoroutine);
            }
        }
        private static IEnumerator ShadowEndDelay()
        {
            yield return 0.03f;
            CherryHelper.Session.ShadowDashActive = false;
        }
        private static Coroutine shadowEndDelayCoroutine;

        // Token: 0x040029E9 RID: 10729
        public static ParticleType P_Shatter;

        // Token: 0x040029EA RID: 10730
        public static ParticleType P_Regen;

        // Token: 0x040029EB RID: 10731
        public static ParticleType P_Glow;

        // Token: 0x040029EC RID: 10732
        public static ParticleType P_ShatterTwo;

        // Token: 0x040029ED RID: 10733
        public static ParticleType P_RegenTwo;

        // Token: 0x040029EE RID: 10734
        public static ParticleType P_GlowTwo;

        // Token: 0x040029EF RID: 10735
        private Sprite sprite;

        // Token: 0x040029F0 RID: 10736
        private Sprite flash;

        // Token: 0x040029F1 RID: 10737
        private Image outline;

        // Token: 0x040029F2 RID: 10738
        private Wiggler wiggler;

        // Token: 0x040029F3 RID: 10739
        private BloomPoint bloom;

        // Token: 0x040029F4 RID: 10740
        private VertexLight light;

        // Token: 0x040029F5 RID: 10741
        private Level level;

        // Token: 0x040029F6 RID: 10742
        private SineWave sine;

        // Token: 0x040029F7 RID: 10743
        private bool twoDashes;

        // Token: 0x040029F8 RID: 10744
        private bool oneUse;

        // Token: 0x040029F9 RID: 10745
        private ParticleType p_shatter;

        // Token: 0x040029FA RID: 10746
        private ParticleType p_regen;

        // Token: 0x040029FB RID: 10747
        private ParticleType p_glow;

        // Token: 0x040029FC RID: 10748
        private float respawnTimer;
    }
}
