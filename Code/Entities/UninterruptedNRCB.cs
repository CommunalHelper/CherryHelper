using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/UI_NRCB")]
    public class UninterruptedNRCB : Solid
    {
        private string spriteDirectory;
        public UninterruptedNRCB(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, false)
        {
            axes = data.Enum("axes", Axes.Both);
            chillOut = data.Bool("chillOut", false);

            if (data.Bool("reskinnable")) {
                P_Crushing = new ParticleType(P_Crushing) {
                    Color = Calc.HexToColor(data.Attr("crushParticleColor1", "ff66e2")),
                    Color2 = Calc.HexToColor(data.Attr("crushParticleColor2", "68fcff")),
                };

                P_Activate = new ParticleType(P_Activate) {
                    Color = Calc.HexToColor(data.Attr("activateParticleColor1", "5fcde4")),
                    Color2 = Calc.HexToColor(data.Attr("activateParticleColor2", "ffffff")),
                };

                spriteDirectory = data.Attr("spriteDirectory", "objects/uninterruptedNRK");
                fill = Calc.HexToColor(data.Attr("fillColor", "242262"));
            } else if (data.Bool("altTexture", true)) {
                spriteDirectory = data.Attr("spriteDirectory", "objects/uninterruptedNRK");
                fill = Calc.HexToColor(data.Attr("fillColor", "242262"));
            } else {
                spriteDirectory = "objects/crushblock";
                fill = Calc.HexToColor("62222b");
            }


            this.idleImages = new List<Image>();
            this.activeTopImages = new List<Image>();
            this.activeRightImages = new List<Image>();
            this.activeLeftImages = new List<Image>();
            this.activeBottomImages = new List<Image>();
            this.OnDashCollide = new DashCollision(this.OnDashed);
            this.returnStack = new List<UninterruptedNRCB.MoveState>();
            this.giant = (Width >= 48f && Height >= 48f && chillOut);
            this.canActivate = true;
            this.attackCoroutine = new Coroutine(true);
            this.attackCoroutine.RemoveOnComplete = false;
            Add(this.attackCoroutine);
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(spriteDirectory + "/block");
            MTexture idle;
            switch (axes)
            {
                default:
                    idle = atlasSubtextures[3];
                    this.canMoveHorizontally = (this.canMoveVertically = true);
                    break;
                case Axes.Horizontal:
                    idle = atlasSubtextures[1];
                    this.canMoveHorizontally = true;
                    this.canMoveVertically = false;
                    break;
                case Axes.Vertical:
                    idle = atlasSubtextures[2];
                    this.canMoveHorizontally = false;
                    this.canMoveVertically = true;
                    break;
            }
            Add(face = new Sprite(GFX.Game, spriteDirectory + "/"));
            face.Position = new Vector2(Width, Height) / 2f;
            if (giant)
            {
                /*
                  <Loop id="idle" path="giant_block" frames="0" delay="0.08"/>
                  <Anim id="hurt"  path="giant_block" frames="8-12" delay="0.08" goto="idle"/>
                  <Anim id="hit" path="giant_block" frames="0-5" delay="0.08"/>
                  <Loop id="right" path="giant_block" frames="6,7"  delay="0.08"/>
                */
                face.AddLoop("idle", "giant_block", 0.08f, 0);
                face.Add("hurt", "giant_block", 0.08f, "idle", 8, 9, 10, 11, 12);
                face.Add("hit", "giant_block", 0.08f, 0, 1, 2, 3, 4, 5);
                face.AddLoop("right", "giant_block", 0.08f, 6, 7);
            }
            else
            {
                /*
                  <Loop id="idle" path="idle_face" delay="0.08"/>
                  <Anim id="hurt" path="hurt" frames="3-12" delay="0.08" goto="idle"/>
                  <Anim id="hit" path="hit" delay="0.08"/>
                  <Loop id="left" path="hit_left" delay="0.08"/>
                  <Loop id="right" path="hit_right" delay="0.08"/>
                  <Loop id="up" path="hit_up" delay="0.08"/>
                  <Loop id="down" path="hit_down" delay="0.08"/>
                */

                face.AddLoop("idle", "idle_face", 0.08f);
                face.Add("hurt", "hurt", 0.08f, "idle", 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
                face.Add("hit", "hit", 0.08f);
                face.AddLoop("left", "hit_left", 0.08f);
                face.AddLoop("right", "hit_right", 0.08f);
                face.AddLoop("up", "hit_up", 0.08f);
                face.AddLoop("down", "hit_down", 0.08f);
            }
            face.CenterOrigin();
            face.Play("idle");
            this.face.OnLastFrame = delegate (string f)
            {
                bool flag = f == "hit";
                if (flag)
                {
                    this.face.Play(this.nextFaceDirection, false, false);
                }
            };
            int num = (int)(Width / 8f) - 1;
            int num2 = (int)(Height / 8f) - 1;
            this.AddImage(idle, 0, 0, 0, 0, -1, -1);
            this.AddImage(idle, num, 0, 3, 0, 1, -1);
            this.AddImage(idle, 0, num2, 0, 3, -1, 1);
            this.AddImage(idle, num, num2, 3, 3, 1, 1);
            for (int i = 1; i < num; i++)
            {
                this.AddImage(idle, i, 0, Calc.Random.Choose(1, 2), 0, 0, -1);
                this.AddImage(idle, i, num2, Calc.Random.Choose(1, 2), 3, 0, 1);
            }
            for (int j = 1; j < num2; j++)
            {
                this.AddImage(idle, 0, j, 0, Calc.Random.Choose(1, 2), -1, 0);
                this.AddImage(idle, num, j, 3, Calc.Random.Choose(1, 2), 1, 0);
            }
            Add(new LightOcclude(0.2f));
            Add(this.returnLoopSfx = new SoundSource());
            Add(new WaterInteraction(() => this.crushDir != Vector2.Zero));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.level = SceneAs<Level>();
        }

        public override void Update()
        {
            if (!SceneAs<Level>().IsInBounds(this))
            {
                RemoveSelf();
            }
            base.Update();
            bool flag = this.crushDir == Vector2.Zero;
            if (flag)
            {
                this.face.Position = new Vector2(Width, Height) / 2f;
                bool flag2 = CollideCheck<Player>(this.Position + new Vector2(-1f, 0f));
                if (flag2)
                {
                    this.face.X -= 1f;
                }
                else
                {
                    bool flag3 = CollideCheck<Player>(this.Position + new Vector2(1f, 0f));
                    if (flag3)
                    {
                        this.face.X += 1f;
                    }
                    else
                    {
                        bool flag4 = CollideCheck<Player>(this.Position + new Vector2(0f, -1f));
                        if (flag4)
                        {
                            this.face.Y -= 1f;
                        }
                    }
                }
            }
            bool flag5 = this.currentMoveLoopSfx != null;
            if (flag5)
            {
                this.currentMoveLoopSfx.Param("submerged", (float)(this.Submerged ? 1 : 0));
            }
            bool flag6 = this.returnLoopSfx != null;
            if (flag6)
            {
                this.returnLoopSfx.Param("submerged", (float)(this.Submerged ? 1 : 0));
            }
        }

        public override void Render()
        {
            Vector2 position = this.Position;
            this.Position += Shake;
            Draw.Rect(X + 2f, Y + 2f, Width - 4f, Height - 4f, this.fill);
            base.Render();
            this.Position = position;
        }

        private bool Submerged
        {
            get
            {
                return Scene.CollideCheck<Water>(new Rectangle((int)(Center.X - 4f), (int)Center.Y, 8, 4));
            }
        }

        public void AddImage(MTexture idle, int x, int y, int tx, int ty, int borderX = 0, int borderY = 0)
        {
            MTexture subtexture = idle.GetSubtexture(tx * 8, ty * 8, 8, 8, null);
            Vector2 vector = new Vector2((float)(x * 8), (float)(y * 8));
            bool flag = borderX != 0;
            if (flag)
            {
                Add(new Image(subtexture)
                {
                    Color = Color.Black,
                    Position = vector + new Vector2((float)borderX, 0f)
                });
            }
            bool flag2 = borderY != 0;
            if (flag2)
            {
                Add(new Image(subtexture)
                {
                    Color = Color.Black,
                    Position = vector + new Vector2(0f, (float)borderY)
                });
            }
            Image image = new Image(subtexture);
            image.Position = vector;
            Add(image);
            this.idleImages.Add(image);
            bool flag3 = borderX != 0 || borderY != 0;
            if (flag3)
            {
                bool flag4 = borderX < 0;
                if (flag4)
                {
                    Image image2 = new Image(GFX.Game[spriteDirectory + "/lit_left"].GetSubtexture(0, ty * 8, 8, 8, null));
                    this.activeLeftImages.Add(image2);
                    image2.Position = vector;
                    image2.Visible = false;
                    Add(image2);
                }
                else
                {
                    bool flag5 = borderX > 0;
                    if (flag5)
                    {
                        Image image3 = new Image(GFX.Game[spriteDirectory + "/lit_right"].GetSubtexture(0, ty * 8, 8, 8, null));
                        this.activeRightImages.Add(image3);
                        image3.Position = vector;
                        image3.Visible = false;
                        Add(image3);
                    }
                }
                bool flag6 = borderY < 0;
                if (flag6)
                {
                    Image image4 = new Image(GFX.Game[spriteDirectory + "/lit_top"].GetSubtexture(tx * 8, 0, 8, 8, null));
                    this.activeTopImages.Add(image4);
                    image4.Position = vector;
                    image4.Visible = false;
                    Add(image4);
                }
                else
                {
                    bool flag7 = borderY > 0;
                    if (flag7)
                    {
                        Image image5 = new Image(GFX.Game[spriteDirectory + "/lit_bottom"].GetSubtexture(tx * 8, 0, 8, 8, null));
                        this.activeBottomImages.Add(image5);
                        image5.Position = vector;
                        image5.Visible = false;
                        Add(image5);
                    }
                }
            }
        }

        private void TurnOffImages()
        {
            foreach (Image image in this.activeLeftImages)
            {
                image.Visible = false;
            }
            foreach (Image image2 in this.activeRightImages)
            {
                image2.Visible = false;
            }
            foreach (Image image3 in this.activeTopImages)
            {
                image3.Visible = false;
            }
            foreach (Image image4 in this.activeBottomImages)
            {
                image4.Visible = false;
            }
        }

        public virtual DashCollisionResults OnDashed(Player player, Vector2 direction)
        {

            DashCollisionResults result;
            if (this.CanActivate(-direction))
            {
                this.Attack(-direction);
                result = DashCollisionResults.Rebound;
            }
            else
            {
                result = DashCollisionResults.NormalCollision;
            }
            return result;
        }

        public bool CanActivate(Vector2 direction)
        {
            bool flag = this.giant && direction.X <= 0f;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = this.canActivate && this.crushDir != direction;
                if (flag2)
                {
                    bool flag3 = direction.X != 0f && !this.canMoveHorizontally;
                    if (flag3)
                    {
                        result = false;
                    }
                    else
                    {
                        bool flag4 = direction.Y != 0f && !this.canMoveVertically;
                        result = !flag4;
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public void Attack(Vector2 direction)
        {

            Audio.Play("event:/game/06_reflection/crushblock_activate", Center);
            bool flag = this.currentMoveLoopSfx != null;
            if (flag)
            {
                this.currentMoveLoopSfx.Param("end", 1f);
                SoundSource sfx = this.currentMoveLoopSfx;
                Alarm.Set(this, 0.5f, delegate
                {
                    sfx.RemoveSelf();
                }, Alarm.AlarmMode.Oneshot);
            }
            Add(this.currentMoveLoopSfx = new SoundSource());
            this.currentMoveLoopSfx.Position = new Vector2(Width, Height) / 2f;
            this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop", null, 0f);
            this.face.Play("hit", false, false);
            this.crushDir = direction;
            this.canActivate = false;
            this.attackCoroutine.Replace(this.AttackSequence());
            ClearRemainder();
            this.TurnOffImages();
            ActivateParticles(this.crushDir);
            bool flag2 = this.crushDir.X < 0f;
            if (flag2)
            {
                foreach (Image image in this.activeLeftImages)
                {
                    image.Visible = true;
                }
                this.nextFaceDirection = "left";
            }
            else
            {
                bool flag3 = this.crushDir.X > 0f;
                if (flag3)
                {
                    foreach (Image image2 in this.activeRightImages)
                    {
                        image2.Visible = true;
                    }
                    this.nextFaceDirection = "right";
                }
                else
                {
                    bool flag4 = this.crushDir.Y < 0f;
                    if (flag4)
                    {
                        foreach (Image image3 in this.activeTopImages)
                        {
                            image3.Visible = true;
                        }
                        this.nextFaceDirection = "up";
                    }
                    else
                    {
                        bool flag5 = this.crushDir.Y > 0f;
                        if (flag5)
                        {
                            foreach (Image image4 in this.activeBottomImages)
                            {
                                image4.Visible = true;
                            }
                            this.nextFaceDirection = "down";
                        }
                    }
                }
            }
            bool flag6 = true;
            bool flag7 = this.returnStack.Count > 0;
            if (flag7)
            {
                UninterruptedNRCB.MoveState moveState = this.returnStack[this.returnStack.Count - 1];
                bool flag8 = moveState.Direction == direction || moveState.Direction == -direction;
                if (flag8)
                {
                    flag6 = false;
                }
            }
            bool flag9 = flag6;
            if (flag9)
            {
                this.returnStack.Add(new UninterruptedNRCB.MoveState(this.Position, this.crushDir));
            }
        }

        private void ActivateParticles(Vector2 dir)
        {
            bool flag = dir == Vector2.UnitX;
            float direction;
            Vector2 position;
            Vector2 positionRange;
            int num;
            if (flag)
            {
                direction = 0f;
                position = CenterRight - Vector2.UnitX;
                positionRange = Vector2.UnitY * (Height - 2f) * 0.5f;
                num = (int)(Height / 8f) * 4;
            }
            else
            {
                bool flag2 = dir == -Vector2.UnitX;
                if (flag2)
                {
                    direction = 3.14159274f;
                    position = CenterLeft + Vector2.UnitX;
                    positionRange = Vector2.UnitY * (Height - 2f) * 0.5f;
                    num = (int)(Height / 8f) * 4;
                }
                else
                {
                    bool flag3 = dir == Vector2.UnitY;
                    if (flag3)
                    {
                        direction = 1.57079637f;
                        position = BottomCenter - Vector2.UnitY;
                        positionRange = Vector2.UnitX * (Width - 2f) * 0.5f;
                        num = (int)(Width / 8f) * 4;
                    }
                    else
                    {
                        direction = -1.57079637f;
                        position = TopCenter + Vector2.UnitY;
                        positionRange = Vector2.UnitX * (Width - 2f) * 0.5f;
                        num = (int)(Width / 8f) * 4;
                    }
                }
            }
            num += 2;
            this.level.Particles.Emit(P_Activate, num, position, positionRange, direction);
        }

        private IEnumerator AttackSequence()
        {
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            this.StartShaking(0.4f);
            yield return 0.4f;
            this.StopPlayerRunIntoAnimation = false;
            bool slowing = false;
            float speed = 0f;
            Action som = null; for (; ; )
            {
                bool flag2 = !this.chillOut;
                if (flag2)
                {
                    speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
                }
                else
                {
                    bool flag3 = slowing || this.CollideCheck<SolidTiles>(this.Position + this.crushDir * 256f);
                    if (flag3)
                    {
                        speed = Calc.Approach(speed, 24f, 500f * Engine.DeltaTime * 0.25f);
                        bool flag4 = !slowing;
                        if (flag4)
                        {
                            slowing = true;
                            float duration = 0.5f;
                            Action onComplete;
                            if ((onComplete = som) == null)
                            {
                                onComplete = (som = delegate ()
                                {
                                    this.face.Play("hurt", false, false);
                                    this.currentMoveLoopSfx.Stop(true);
                                    this.TurnOffImages();
                                });
                            }
                            Alarm.Set(this, duration, onComplete, Alarm.AlarmMode.Oneshot);
                        }
                    }
                    else
                    {
                        speed = Calc.Approach(speed, 120f, 250f * Engine.DeltaTime);
                    }
                }
                bool flag5 = this.crushDir.X != 0f;
                bool hit;
                if (flag5)
                {
                    hit = this.MoveHCheck(speed * this.crushDir.X * Engine.DeltaTime);
                }
                else
                {
                    hit = this.MoveVCheck(speed * this.crushDir.Y * Engine.DeltaTime);
                }
                bool flag6 = hit;
                if (flag6)
                {
                    break;
                }
                bool flag7 = this.Scene.OnInterval(0.02f);
                if (flag7)
                {
                    bool flag8 = this.crushDir == Vector2.UnitX;
                    Vector2 at;
                    float dir;
                    if (flag8)
                    {
                        at = new Vector2(this.Left + 1f, Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
                        dir = 3.14159274f;
                    }
                    else
                    {
                        bool flag9 = this.crushDir == -Vector2.UnitX;
                        if (flag9)
                        {
                            at = new Vector2(this.Right - 1f, Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
                            dir = 0f;
                        }
                        else
                        {
                            bool flag10 = this.crushDir == Vector2.UnitY;
                            if (flag10)
                            {
                                at = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f), this.Top + 1f);
                                dir = -1.57079637f;
                            }
                            else
                            {
                                at = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f), this.Bottom - 1f);
                                dir = 1.57079637f;
                            }
                        }
                    }
                    level.Particles.Emit(P_Crushing, at, dir);
                    at = default(Vector2);
                }
                yield return null;
            }
            FallingBlock fallingBlock = this.CollideFirst<FallingBlock>(this.Position + this.crushDir);
            bool flag11 = fallingBlock != null;
            if (flag11)
            {
                fallingBlock.Triggered = true;
            }
            bool flag12 = this.crushDir == -Vector2.UnitX;
            if (flag12)
            {
                Vector2 add = new Vector2(0f, 2f);
                int i = 0;
                while ((float)i < this.Height / 8f)
                {
                    Vector2 at2 = new Vector2(this.Left - 1f, this.Top + 4f + (float)(i * 8));
                    bool flag13 = !this.Scene.CollideCheck<Water>(at2) && this.Scene.CollideCheck<Solid>(at2);
                    if (flag13)
                    {
                        this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at2 + add, 0f);
                        this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at2 - add, 0f);
                    }
                    at2 = default(Vector2);
                    int num = i;
                    i = num + 1;
                }
                add = default(Vector2);
            }
            else
            {
                bool flag14 = this.crushDir == Vector2.UnitX;
                if (flag14)
                {
                    Vector2 add2 = new Vector2(0f, 2f);
                    int j = 0;
                    while ((float)j < this.Height / 8f)
                    {
                        Vector2 at3 = new Vector2(this.Right + 1f, this.Top + 4f + (float)(j * 8));
                        bool flag15 = !this.Scene.CollideCheck<Water>(at3) && this.Scene.CollideCheck<Solid>(at3);
                        if (flag15)
                        {
                            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at3 + add2, 3.14159274f);
                            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at3 - add2, 3.14159274f);
                        }
                        at3 = default(Vector2);
                        int num = j;
                        j = num + 1;
                    }
                    add2 = default(Vector2);
                }
                else
                {
                    bool flag16 = this.crushDir == -Vector2.UnitY;
                    if (flag16)
                    {
                        Vector2 add3 = new Vector2(2f, 0f);
                        int k = 0;
                        while ((float)k < this.Width / 8f)
                        {
                            Vector2 at4 = new Vector2(this.Left + 4f + (float)(k * 8), this.Top - 1f);
                            bool flag17 = !this.Scene.CollideCheck<Water>(at4) && this.Scene.CollideCheck<Solid>(at4);
                            if (flag17)
                            {
                                this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at4 + add3, 1.57079637f);
                                this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at4 - add3, 1.57079637f);
                            }
                            at4 = default(Vector2);
                            int num = k;
                            k = num + 1;
                        }
                        add3 = default(Vector2);
                    }
                    else
                    {
                        bool flag18 = this.crushDir == Vector2.UnitY;
                        if (flag18)
                        {
                            Vector2 add4 = new Vector2(2f, 0f);
                            int l = 0;
                            while ((float)l < this.Width / 8f)
                            {
                                Vector2 at5 = new Vector2(this.Left + 4f + (float)(l * 8), this.Bottom + 1f);
                                bool flag19 = !this.Scene.CollideCheck<Water>(at5) && this.Scene.CollideCheck<Solid>(at5);
                                if (flag19)
                                {
                                    this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at5 + add4, -1.57079637f);
                                    this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at5 - add4, -1.57079637f);
                                }
                                at5 = default(Vector2);
                                int num = l;
                                l = num + 1;
                            }
                            add4 = default(Vector2);
                        }
                    }
                }
            }
            Audio.Play("event:/game/06_reflection/crushblock_impact", this.Center);
            this.level.DirectionalShake(this.crushDir, 0.3f);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            this.StartShaking(0.4f);
            this.StopPlayerRunIntoAnimation = true;
            SoundSource sfx = this.currentMoveLoopSfx;
            this.currentMoveLoopSfx.Param("end", 1f);
            this.currentMoveLoopSfx = null;
            canActivate = true;
            Alarm.Set(this, 0.5f, delegate
            {
                sfx.RemoveSelf();
            }, Alarm.AlarmMode.Oneshot);
            this.crushDir = Vector2.Zero;
            this.TurnOffImages();
            yield return this.face.PlayRoutine("hurt", false);
            this.face.Play("idle", false, false);
            yield break;
        }

        private bool MoveHCheck(float amount)
        {
            bool flag = MoveHCollideSolidsAndBounds(this.level, amount, true, null);
            bool result;
            if (flag)
            {
                bool flag2 = amount < 0f && Left <= (float)this.level.Bounds.Left;
                if (flag2)
                {
                    result = true;
                }
                else
                {
                    bool flag3 = amount > 0f && Right >= (float)this.level.Bounds.Right;
                    if (flag3)
                    {
                        result = true;
                    }
                    else
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            for (int j = 1; j >= -1; j -= 2)
                            {
                                Vector2 value = new Vector2((float)Math.Sign(amount), (float)(i * j));
                                bool flag4 = !CollideCheck<Solid>(this.Position + value);
                                if (flag4)
                                {
                                    this.MoveVExact(i * j);
                                    this.MoveHExact(Math.Sign(amount));
                                    return false;
                                }
                            }
                        }
                        result = true;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        private bool MoveVCheck(float amount)
        {
            bool flag = MoveVCollideSolidsAndBounds(this.level, amount, true, null);
            bool result;
            if (flag)
            {
                bool flag2 = amount < 0f && Top <= (float)this.level.Bounds.Top;
                if (flag2)
                {
                    result = true;
                }
                else
                {
                    bool flag3 = amount > 0f && Bottom >= (float)(this.level.Bounds.Bottom + 32);
                    if (flag3)
                    {
                        result = true;
                    }
                    else
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            for (int j = 1; j >= -1; j -= 2)
                            {
                                Vector2 value = new Vector2((float)(i * j), (float)Math.Sign(amount));
                                bool flag4 = !CollideCheck<Solid>(this.Position + value);
                                if (flag4)
                                {
                                    this.MoveHExact(i * j);
                                    this.MoveVExact(Math.Sign(amount));
                                    return false;
                                }
                            }
                        }
                        result = true;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static ParticleType P_Impact;

        public ParticleType P_Crushing = CrushBlock.P_Crushing;

        public ParticleType P_Activate = CrushBlock.P_Activate;

        private const float CrushSpeed = 120f;
        private const float CrushAccel = 250f;
        private const float ReturnSpeed = 30f;
        private const float ReturnAccel = 80f;
        public Color fill;
        public Axes axes;

        private Level level;

        private bool canActivate;

        private Vector2 crushDir;

        private List<UninterruptedNRCB.MoveState> returnStack;

        private Coroutine attackCoroutine;

        public bool canMoveVertically;

        public bool canMoveHorizontally;

        private bool chillOut;

        public bool giant;

        public Sprite face;

        private string nextFaceDirection;

        public List<Image> idleImages;

        public List<Image> activeTopImages;

        public List<Image> activeRightImages;

        public List<Image> activeLeftImages;

        public List<Image> activeBottomImages;

        private SoundSource currentMoveLoopSfx;

        private SoundSource returnLoopSfx;

        public enum Axes
        {
            Both,
            Horizontal,
            Vertical
        }
        public List<Image> ImgList;
        private struct MoveState
        {
            public MoveState(Vector2 from, Vector2 direction)
            {
                this.From = from;
                this.Direction = direction;
            }

            public Vector2 From;

            public Vector2 Direction;

        }
    }

}