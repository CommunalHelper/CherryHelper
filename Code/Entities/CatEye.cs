using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.CherryHelper
{

    [CustomEntity("CherryHelper/CatEye")]
    public class CatEye : Entity
    {
        private const float ResetTime = 0.8f;

        private Sprite sprite;
        private VertexLight lights;
        private float resetTimer;

        private Level level;
        public double randnum;
        private SineWave sine;
        public float respawnTime;
        private float atY;

        private SoundSource spawnSfx;

        private Collider bounceCollider;

        private float respawnTimer;
        private Wiggler wiggler;

        public static ParticleType P_Respawn;
        public bool wiggles;
        private VertexLight light;
        public States state;

        public enum States
        {
            Normal,
            Double_Dash,
            Shadow_Dash
        }
        public CatEye(Vector2 Position)
        {
            base.Collider = new Hitbox(16f, 4f, -7f, 2f);
            bounceCollider = new Hitbox(16f, 9f, -7f, -6f);
            Add(new PlayerCollider(OnPlayer));
            Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
            Add(wiggler = Wiggler.Create(0.75f, 4f, delegate (float v)
            {
                sprite.Scale = (sprite.Scale = Vector2.One * (1f + v * 0.2f));
            }, false, false));
            Add(spawnSfx = new SoundSource());
            light = new VertexLight(Color.White, 1f, 16, 24);
            Add(light);
        }
        public CatEye(EntityData data, Vector2 offset, EntityID GID) : this(data.Position + offset)
        {
            Position = data.Position + offset;
            Enum.TryParse(data.Attr("state"), out state);
            wiggles = data.Bool("Wiggles", true);
            respawnTime = data.Float("respawnTime", 4);
            Random rand = new Random(Convert.ToInt32(data.Position.X) * Convert.ToInt32(data.Position.Y));
            randnum = rand.NextDouble();
            if (wiggles)
                Add(sine = new SineWave(0.5f, Convert.ToSingle(randnum)));
            Y += Convert.ToSingle(randnum);
        }
        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
            switch (state)
            {
                case States.Normal:
                    Add(sprite = GFX.SpriteBank.Create("cateye"));
                    sprite.Play("spin");
                    break;
                case States.Double_Dash:
                    Add(sprite = GFX.SpriteBank.Create("twineye"));
                    sprite.Play("spin");
                    break;
                case States.Shadow_Dash:
                    Add(sprite = GFX.SpriteBank.Create("demoneye"));
                    sprite.Play("spin");
                    break;
            }

        }


        private void Destroy()
        {
            Collidable = false;
            sprite.Play("break");
            this.respawnTimer = respawnTime;
        }
        private void OnPlayer(Player player)
        {
            player.Die(new Vector2(-1f, 0f));
            Destroy();
            Audio.Play("event:/cherryhelper/cateye_death", Position);
        }

        private void OnPlayerBounce(Player player)
        {
            if (!CollideCheck(player))
            {
                Celeste.Freeze(0.1f);
                player.Bounce(base.Top - 2f);
                if (state == States.Double_Dash) player.UseRefill(true);
                else if (state == States.Shadow_Dash) CherryHelper.Session.HasShadowDash = true;
                Destroy();
                light.Alpha = 0;
                Audio.Play("event:/cherryhelper/cateye_death", Position);
            }
        }
        public override void Update()
        {
            base.Update();
            if (base.Scene.OnInterval(3f) && !(respawnTimer > 0f) && (sprite != null))
            {
                sprite.Play("patrol", true, false);
            }
            if (wiggles)
                Y += 0.1f * sine.Value;
            if (this.respawnTimer > 0f)
            {
                this.respawnTimer -= Engine.DeltaTime;
                bool flag2 = this.respawnTimer <= 0f;
                if (flag2)
                {
                    this.Respawn();
                }
            }
        }
        private void Respawn()
        {
            Player player = level.Tracker.GetEntity<Player>();
            if (!this.Collidable && player != null && !player.Dead)
            {
                light.Alpha = 1;
                Audio.Play("event:/cherryhelper/cateye_respawn", Position);
                this.Collidable = true;
                this.sprite.Visible = true;
                sprite.Play("spin");
                this.wiggler.Start();
                this.level.ParticlesFG.Emit(P_Respawn, 16, this.Position, Vector2.One * 2f);
            }
        }
        public override void Render()
        {
            if (sprite != null)
                sprite.DrawOutline();

            base.Render();
        }
    }

}
