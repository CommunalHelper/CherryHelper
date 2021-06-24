using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.CherryHelper
{


    public class CS_FallTeleport : CutsceneEntity
    {
        private class Fader : Entity
        {
            public float Target = 0f;

            public bool Ended;

            public float fade = 0f;

            public Fader()
            {
                base.Depth = -1000000;
            }

            public override void Update()
            {
                fade = Calc.Approach(fade, Target, Engine.DeltaTime * 0.5f);
                if (Target <= 0f && fade <= 0f && Ended)
                {
                    RemoveSelf();
                }
                base.Update();
            }

            public override void Render()
            {
                Camera camera = (base.Scene as Level).Camera;
                if (fade > 0f)
                {
                    Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * fade);
                }
                Player entity = base.Scene.Tracker.GetEntity<Player>();
                if (entity != null && !entity.OnGround(2))
                {
                    entity.Render();
                }
            }
        }

        private Player player;

        private FallTeleportMirror portal;
        public string toLevel;
        private bool EndLevel;

        public string NextChapterSSID = "";
        private bool SeamlessNextChapter;
        private AreaMode ChapterAreaMode;
        private Fader fader;

        private SoundSource sfx;

        public CS_FallTeleport(Player player, FallTeleportMirror portal, string toLevel, bool EndLevel, bool SeamlessNextChapter, string NextChapterSSID, AreaMode ChapterAreaMode)
        {
            this.player = player;
            this.portal = portal;
            this.toLevel = toLevel;
            this.EndLevel = EndLevel;
            this.NextChapterSSID = NextChapterSSID;
            this.SeamlessNextChapter = SeamlessNextChapter;
            this.ChapterAreaMode = ChapterAreaMode;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level)));
            level.Add(fader = new Fader());
            level.InCutscene = false;
            level.CancelCutscene();
        }

        private IEnumerator Cutscene(Level level)
        {
            player.StateMachine.State = 11;
            player.StateMachine.Locked = true;
            player.Dashes = 1;
            Audio.SetMusic(null);
            Add(sfx = new SoundSource());
            sfx.Position = portal.Center;
            Add(new Coroutine(CenterCamera()));
            //yield return player.DummyWalkToExact((int)portal.X);
            //yield return 0.25f;
            //yield return player.DummyWalkToExact((int)portal.X - 16);
            //yield return 0.5f;
            //yield return player.DummyWalkToExact((int)portal.X + 16);
            //yield return 0.25f;
            //player.Facing = Facings.Left;
            //yield return 0.25f;
            //yield return player.DummyWalkToExact((int)portal.X);
            //yield return 0.1f;
            player.DummyAutoAnimate = false;
            player.Sprite.Play("lookUp");
            yield return 1f;
            player.DummyAutoAnimate = true;
            portal.Activate();
            sfx.Play("event:/new_content/game/10_farewell/glitch_short");
            Add(new Coroutine(MusicFadeOutBSide()));
            Add(new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 3f, 12f)));
            yield return 0.25f;
            player.ForceStrongWindHair.X = -1f;
            //yield return player.DummyWalkToExact((int)player.X + 12, walkBackwards: true);
            yield return 0.5f;
            player.Facing = Facings.Right;
            player.DummyAutoAnimate = false;
            sfx.Play("event:/new_content/game/10_farewell/glitch_long");
            player.DummyGravity = false;
            player.Sprite.Play("fallFast");
            player.Sprite.Rate = 1f;
            Vector2 target = portal.Center + new Vector2(0f, 8f);
            Vector2 from = player.Position;
            for (float p2 = 0f; p2 < 1f; p2 += Engine.DeltaTime * 2f)
            {
                player.Position = from + (target - from) * Ease.SineInOut(p2);
                yield return null;
            }
            player.ForceStrongWindHair.X = 0f;
            fader.Target = 1f;
            yield return 2f;
            player.Sprite.Play("fallFast");
            yield return 1f;
            while ((portal.DistortionFade -= Engine.DeltaTime * 2f) > 0f)
            {
                yield return null;
            }
            player.Sprite.Visible = false;
            player.Hair.Visible = false;
            sfx.Play("event:/char/badeline/disappear");
            yield return 2f;


            if (SeamlessNextChapter && NextChapterSSID != "")
            {
                if (EndLevel)
                {
                    level.RegisterAreaComplete();
                }
                LevelEnter.Go(new Session(AreaData.Get(NextChapterSSID).ToKey(ChapterAreaMode)), false);
            }
            else if (EndLevel)
                level.CompleteArea(false, true, true);
            else
                level.OnEndOfFrame += delegate
                {
                    if (fader != null && !WasSkipped)
                    {
                        fader.Tag = Tags.Global;
                        fader.fade = 0f;
                        fader.Target = 0f;
                        fader.Ended = true;
                    }
                    Leader.StoreStrawberries(player.Leader);
                    level.Remove(player);
                    level.UnloadLevel();
                    level.Session.Keys.Clear();
                    level.Session.Level = toLevel;
                    level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
                    level.LoadLevel(Player.IntroTypes.WakeUp);
                    Audio.SetMusicParam("fade", 1f);
                    Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
                    level.Camera.Y -= 8f;
                    if (fader != null)
                    {
                        fader.RemoveTag(Tags.Global);
                    }
                };
        }

        private IEnumerator CenterCamera()
        {
            Camera camera = Level.Camera;
            Vector2 target = portal.Center - new Vector2(160f, 90f);
            while ((camera.Position - target).Length() > 1f)
            {
                camera.Position += (target - camera.Position) * (1f - (float)Math.Pow(0.009999999776482582, Engine.DeltaTime));
                yield return null;
            }
        }

        private IEnumerator MusicFadeOutBSide()
        {
            for (float p = 1f; p > 0f; p -= Engine.DeltaTime)
            {
                Audio.SetMusicParam("fade", p);
                yield return null;
            }
            Audio.SetMusicParam("fade", 0f);
        }

        public override void OnEnd(Level level)
        {
            level.OnEndOfFrame += delegate
            {
                if (fader != null && !WasSkipped)
                {
                    fader.Tag = Tags.Global;
                    fader.fade = 0f;
                    fader.Target = 0f;
                    fader.Ended = true;
                }
                Leader.StoreStrawberries(player.Leader);
                level.Remove(player);
                level.UnloadLevel();
                level.Session.Keys.Clear();
                level.Session.Level = toLevel;
                level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
                level.LoadLevel(Player.IntroTypes.WakeUp);
                Audio.SetMusicParam("fade", 1f);
                Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
                level.Camera.Y -= 8f;
                if (fader != null)
                {
                    fader.RemoveTag(Tags.Global);
                }
            };
        }
    }

}
