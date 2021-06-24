// Celeste.BadelineBot
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/BadelineBot")]
    public class BadelineBot : Entity
    {
        public Vector2 LastPosition;

        public List<Player.ChaserState> Timeline;

        public PlayerSprite Sprite;

        public PlayerHair Hair;

        private Vector2 start;

        private float time;

        private int index;

        private float loopDelay;

        private float startDelay;

        public float TrimStart = 0f;

        public float TrimEnd = 0f;

        public readonly float Duration;

        private float rangeMinX = float.MinValue;

        private float rangeMaxX = float.MaxValue;

        private bool ShowTrail;

        public Vector2 DashDirection
        {
            get;
            private set;
        }

        public float Time => time;

        public int FrameIndex => index;

        public int FrameCount => Timeline.Count;

        public BadelineBot(EntityData e, Vector2 offset)
            : this(e.Position + offset, PlaybackData.Tutorials[e.Attr("tutorial")])
        {
            if (e.Nodes != null && e.Nodes.Length != 0)
            {
                rangeMinX = base.X;
                rangeMaxX = base.X;
                Vector2[] array = e.NodesOffset(offset);
                for (int i = 0; i < array.Length; i++)
                {
                    Vector2 vector = array[i];
                    rangeMinX = Math.Min(rangeMinX, vector.X);
                    rangeMaxX = Math.Max(rangeMaxX, vector.X);
                }
            }
            startDelay = 1f;
        }

        public BadelineBot(Vector2 start, List<Player.ChaserState> timeline)
        {
            this.start = start;
            base.Collider = new Hitbox(8f, 11f, -4f, -11f);
            Timeline = timeline;
            Position = start;
            time = 0f;
            index = 0;
            Duration = timeline[timeline.Count - 1].TimeStamp;
            TrimStart = 0f;
            TrimEnd = Duration;
            Sprite = new PlayerSprite(PlayerSpriteMode.MadelineAsBadeline);
            Add(Hair = new PlayerHair(Sprite));
            Add(Sprite);
            base.Collider = new Hitbox(8f, 4f, -4f, -4f);
            base.Depth = 9008;
            SetFrame(0);
            for (int i = 0; i < 10; i++)
            {
                Hair.AfterUpdate();
            }
            Add(new PlayerCollider(OnPlayer));
            Add(new VertexLight(new Vector2(0f, -8f), Color.PaleVioletRed, 1f, 20, 60));
            Visible = false;
            index = Timeline.Count;
        }
        private void OnPlayer(Player player)
        {
            player.Die((player.Position - Position).SafeNormalize());
        }
        private void Restart()
        {
            Audio.Play("event:/new_content/char/tutorial_ghost/appear", Position);
            Visible = true;
            time = TrimStart;
            index = 0;
            loopDelay = 0.25f;
            while (time > Timeline[index].TimeStamp)
            {
                index++;
            }
            SetFrame(index);
        }

        public void SetFrame(int index)
        {
            Player.ChaserState chaserState = Timeline[index];
            string currentAnimationID = Sprite.CurrentAnimationID;
            bool flag = base.Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f));
            Vector2 dashDirection = DashDirection;
            Position = start + chaserState.Position;
            if (chaserState.Animation != Sprite.CurrentAnimationID && chaserState.Animation != null && Sprite.Has(chaserState.Animation))
            {
                Sprite.Play(chaserState.Animation, restart: true);
            }
            Sprite.Scale = chaserState.Scale;
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }
            DashDirection = chaserState.DashDirection;
            if (base.Scene == null)
            {
                return;
            }
            if (!flag && base.Scene != null && CollideCheck<Solid>(Position + new Vector2(0f, 1f)))
            {
                Audio.Play("event:/char/badeline/landing", Position);
            }
            if (!(currentAnimationID != Sprite.CurrentAnimationID))
            {
                return;
            }
            string currentAnimationID2 = Sprite.CurrentAnimationID;
            int currentAnimationFrame = Sprite.CurrentAnimationFrame;
            if (currentAnimationID2 == "jumpFast" || currentAnimationID2 == "jumpSlow")
            {
                Audio.Play("event:/char/badeline/jump", Position);
            }
            else if (currentAnimationID2 == "dreamDashIn")
            {
                Audio.Play("event:/char/badeline/dreamblock_sequence", Position);
            }
            else if (currentAnimationID2 == "dash")
            {
                if (DashDirection.Y != 0f)
                {
                    Audio.Play("event:/char/badeline/jump_super", Position);
                }
                else if (chaserState.Scale.X > 0f)
                {
                    Audio.Play("event:/char/badeline/dash_red_right", Position);
                }
                else
                {
                    Audio.Play("event:/char/badeline/dash_red_left", Position);
                }
            }
            else if (currentAnimationID2 == "climbUp" || currentAnimationID2 == "climbDown" || currentAnimationID2 == "wallslide")
            {
                Audio.Play("event:/char/badeline/grab", Position);
            }
            else if ((currentAnimationID2.Equals("runSlow_carry") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("runFast") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("runSlow") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("walk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("runStumble") && currentAnimationFrame == 6) || (currentAnimationID2.Equals("flip") && currentAnimationFrame == 4) || (currentAnimationID2.Equals("runWind") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("idleC") && Sprite.Mode == PlayerSpriteMode.MadelineNoBackpack && (currentAnimationFrame == 3 || currentAnimationFrame == 6 || currentAnimationFrame == 8 || currentAnimationFrame == 11)) || (currentAnimationID2.Equals("carryTheoWalk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15)))
            {
                Audio.Play("event:/char/badeline/footstep", Position);
            }
        }

        public override void Update()
        {
            Hair.Color = Calc.HexToColor("9B3FB5");
            if (startDelay > 0f)
            {
                startDelay -= Engine.DeltaTime;
            }
            LastPosition = Position;
            base.Update();
            if (index >= Timeline.Count - 1 || Time >= TrimEnd)
            {
                if (Visible)
                {
                    Audio.Play("event:/new_content/char/tutorial_ghost/disappear", Position);
                }
                Visible = false;
                Position = start;
                loopDelay -= Engine.DeltaTime;
                if (loopDelay <= 0f)
                {
                    Player player = (base.Scene == null) ? null : base.Scene.Tracker.GetEntity<Player>();
                    if (player == null || (player.X > rangeMinX && player.X < rangeMaxX))
                    {
                        Restart();
                    }
                }
            }
            else if (startDelay <= 0f)
            {
                SetFrame(index);
                time += Engine.DeltaTime;
                while (index < Timeline.Count - 1 && time >= Timeline[index + 1].TimeStamp)
                {
                    index++;
                }
            }
            if (Visible && base.Scene != null && base.Scene.OnInterval(0.1f))
            {
                TrailManager.Add(Position, Sprite, Hair, Sprite.Scale, Player.NormalHairColor, base.Depth + 1);
            }
        }
    }
}