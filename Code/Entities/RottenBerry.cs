using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.CherryHelper
{




    [RegisterStrawberry(true, false)]
    [CustomEntity("CherryHelper/RottenBerry")]
    class RottenBerry : Entity, IStrawberry, IStrawberrySeeded
    {


        public List<GenericStrawberrySeed> Seeds { get; }
        public string gotSeedFlag => "collected_seeds_of_" + ID.ToString();
        public bool WaitingOnSeeds
        {
            get
            {
                if (Seeds != null)
                    return !SceneAs<Level>().Session.GetFlag(gotSeedFlag) && Seeds.Count > 0;
                else
                    return false;
            }
        }



        public static ParticleType P_Glow = Strawberry.P_Glow;
        public static ParticleType P_GhostGlow = Strawberry.P_GhostGlow;


        public EntityID ID;
        public Follower Follower;

        public Level Level { get; private set; }


        private Sprite sprite;
        private Sprite ghostSprite;
        private Wiggler wiggler;
        private BloomPoint bloom;
        private VertexLight light;
        private Tween lightTween;
        private Vector2 start;
        private float wobble = 0f;
        private float collectTimer = 0f;
        private bool collected = false;
        private bool isOwned;



        public RottenBerry(EntityData data, Vector2 offset, EntityID gid)
        {
            ID = gid;
            Position = (start = data.Position + offset);

            isOwned = SaveData.Instance.CheckStrawberry(ID);
            Depth = -100;
            Collider = new Hitbox(14f, 14f, -7f, -7f);
            Add(new PlayerCollider(OnPlayer));
            Add(new MirrorReflection());
            Add(Follower = new Follower(ID, null, OnLoseLeader));
            Follower.FollowDelay = 0.3f;



            if (data.Nodes != null && data.Nodes.Length != 0)
            {
                Seeds = new List<GenericStrawberrySeed>();
                for (int i = 0; i < data.Nodes.Length; i++)
                {
                    Seeds.Add(new GenericStrawberrySeed(this, offset + data.Nodes[i], i, isOwned));
                }
            }
        }


        public override void Added(Scene scene)
        {
            base.Added(scene);

            this.Level = base.SceneAs<Level>();

            sprite = GFX.SpriteBank.Create((isOwned ? "ghostrottenberry" : "rottenberry"));
            Add(sprite);

            sprite.Play("idle");



            sprite.OnFrameChange = OnAnimate;



            wiggler = Wiggler.Create
                (
                    0.4f,
                    4f,
                    delegate (float v)
                    {
                        sprite.Scale = Vector2.One * (1f + v * 0.35f);
                    },
                    false,
                    false
                );
            Add(wiggler);





            bloom = new BloomPoint(isOwned ? 0.25f : 0.5f, 12f);
            Add(bloom);


            light = new VertexLight(Color.White, 1f, 16, 24);
            lightTween = light.CreatePulseTween();
            Add(light);
            Add(lightTween);


            if (Seeds != null && Seeds.Count > 0 && !SceneAs<Level>().Session.GetFlag(gotSeedFlag))
            {
                foreach (GenericStrawberrySeed seed in Seeds)
                    scene.Add(seed);


                Visible = false;
                Collidable = false;
                bloom.Visible = light.Visible = false;
            }


            if (SceneAs<Level>().Session.BloomBaseAdd > 0.1f)
                bloom.Alpha *= 0.5f;
        }


        public override void Update()
        {

            if (WaitingOnSeeds)
                return;

            if (!collected)
            {

                wobble += Engine.DeltaTime * 4f;
                //sprite.Y = bloom.Y = light.Y = (float)Math.Sin(wobble) * 2f;


                if (Follower.Leader != null)
                {
                    Player player = Follower.Leader.Entity as Player;


                    if (Follower.DelayTimer <= 0f && StrawberryRegistry.IsFirstStrawberry(this))
                    {
                        if (player != null && player.Scene != null &&
                            !player.StrawberriesBlocked && player.OnSafeGround &&
                            player.StateMachine.State != 13)
                        {

                            collectTimer += Engine.DeltaTime;
                            if (collectTimer > 0.15f)
                                OnCollect();
                        }
                        else
                        {
                            collectTimer = Math.Min(collectTimer, 0f);
                        }
                        if (CherryHelper.Session.SafeFromRot || SaveData.Instance.Assists.Invincible) OnCollect();
                    }

                    else if (Follower.FollowIndex > 0)
                        collectTimer = -0.15f;
                }
            }


            if (Follower.Leader != null && Scene.OnInterval(0.08f))
            {
                ParticleType type;
                if (!isOwned)
                    type = P_Glow;
                else
                    type = P_GhostGlow;

                SceneAs<Level>().ParticlesFG.Emit(type, Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
            }
            base.Update();
        }


        private void OnAnimate(string id)
        {


            int numFrames = 35;

            if (sprite.CurrentAnimationFrame == numFrames - 4)
            {
                lightTween.Start();



                bool visuallyObstructed = CollideCheck<FakeWall>() || CollideCheck<Solid>();
                if (!collected && visuallyObstructed)
                {
                    Audio.Play("event:/game/general/strawberry_pulse", Position);
                    SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.1f);
                }
                else
                {
                    Audio.Play("event:/game/general/strawberry_pulse", Position);
                    SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.2f);
                }
            }
        }


        public void OnPlayer(Player player)
        {

            if (Follower.Leader != null || collected || WaitingOnSeeds)
                return;


            Audio.Play(isOwned ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", Position);
            player.Leader.GainFollower(Follower);
            wiggler.Start();
            Depth = -1000000;
        }





        public void OnCollect()
        {

            if (collected)
                return;
            if (!CherryHelper.Session.SafeFromRot || SaveData.Instance.Assists.Invincible)
            {
                Player entity = this.Level.Tracker.GetEntity<Player>();
                bool flag2 = entity != null;
                if (flag2)
                {
                    Audio.Play("event:/game/general/strawberry_laugh", this.Position);
                    entity.Die(-Vector2.UnitX * (float)entity.Facing, false, true);
                    return;

                }
            }
            collected = true;


            int collectIndex = 0;

            if (Follower.Leader != null)
            {
                Player player = Follower.Leader.Entity as Player;
                collectIndex = player.StrawberryCollectIndex;
                player.StrawberryCollectIndex++;
                player.StrawberryCollectResetTimer = 2.5f;
                Follower.Leader.LoseFollower(Follower);
            }


            SaveData.Instance.AddStrawberry(ID, false);


            Session session = SceneAs<Level>().Session;
            session.DoNotLoad.Add(ID);
            session.Strawberries.Add(ID);
            session.UpdateLevelStartDashes();


            Add(new Coroutine(CollectRoutine(collectIndex), true));
        }

        private IEnumerator CollectRoutine(int collectIndex)
        {
            Level level = SceneAs<Level>();
            Tag = Tags.TransitionUpdate;
            Depth = -2000010;
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            sprite.Play("collect");


            int color = !isOwned ? 0 : 1;

            Audio.Play("event:/game/general/strawberry_get", this.Position, "colour", (float)color, "count", (float)collectIndex);

            while (sprite.Animating)
            {
                yield return null;
            }
            Scene.Add(new StrawberryPoints(Position, isOwned, collectIndex, false));
            RemoveSelf();
            yield break;
        }





        private void OnLoseLeader()
        {
            if (collected)
                return;

            Alarm.Set(this, 0.15f, delegate
            {
                Vector2 vector = (start - Position).SafeNormalize();
                float num = Vector2.Distance(Position, start);
                float scaleFactor = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
                Vector2 control = start + vector * 16f + vector.Perpendicular() * scaleFactor * (float)Calc.Random.Choose(1, -1);
                SimpleCurve curve = new SimpleCurve(Position, start, control);
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Max(num / 100f, 0.4f), true);
                tween.OnUpdate = delegate (Tween f)
                {
                    Position = curve.GetPoint(f.Eased);
                };
                tween.OnComplete = delegate (Tween f)
                {
                    Depth = 0;
                };
                Add(tween);
            }, Alarm.AlarmMode.Oneshot);
        }




        public void CollectedSeeds()
        {
            SceneAs<Level>().Session.SetFlag(gotSeedFlag, true);
            Visible = true;
            Collidable = true;
            bloom.Visible = light.Visible = true;
        }
    }
}
