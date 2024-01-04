using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.CherryHelper
{

    [Tracked(false)]
    [CustomEntity("CherryHelper/DoorField")]
    public class DoorField : Entity
    {
        public string toLevel;
        private bool EndLevel;
        private Vector2 spawnOffset;

        public string NextChapterSSID = "";
        private bool SeamlessNextChapter;
        private AreaMode ChapterAreaMode;
        private TalkComponent talkie;
        public DoorField(Vector2 position, int width, int height)
            : base(position)
        {
            base.Collider = new Hitbox(width, height);
        }
        public override void Added(Scene scene)
        {
            Add(talkie = new TalkComponent(new Rectangle(0, 0, (int)Collider.Width, (int)Collider.Height), new Vector2(0.5f * Collider.Width, 0.5f * Collider.Height), OnEnter));
            talkie.Enabled = true;
            talkie.Visible = true;
            talkie.PlayerMustBeFacing = false;
            base.Added(scene);
        }
        public DoorField(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height)
        {
            toLevel = data.Attr("toLevel");
            EndLevel = data.Bool("EndLevel", false);
            NextChapterSSID = data.Attr("SIDOfMap");
            SeamlessNextChapter = data.Bool("LoadAnotherBin", true);
            spawnOffset = new Vector2(data.Int("spawnOffsetX", 0), data.Int("spawnOffsetY", 0));
            switch (data.Attr("Side"))
            {
                case "A":
                    ChapterAreaMode = AreaMode.Normal;
                    break;
                case "B":
                    ChapterAreaMode = AreaMode.BSide;
                    break;
                case "C":
                    ChapterAreaMode = AreaMode.CSide;
                    break;
            }
        }

        private void OnEnter(Player obj)
        {
            Add(new Coroutine(EnterRoutine(obj)));
        }

        private IEnumerator EnterRoutine(Player obj)
        {
            obj.StateMachine.State = 11;
            obj.DummyAutoAnimate = false;
            obj.Sprite.Play("lookUp");
            yield return 0.8f;
            Level level = SceneAs<Level>();
            Player entity = level.Tracker.GetEntity<Player>();
            if (entity != null)
            {
                SpotlightWipe.FocusPoint = entity.Position - level.Camera.Position - new Vector2(0f, 8f);
            }
            yield return new SpotlightWipe(level, wipeIn: false);
            Add(new Coroutine(Teleport(obj)));
        }

        private IEnumerator Teleport(Player player)
        {

            yield return 1.7f;
            Level level = SceneAs<Level>();
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

                    Leader.StoreStrawberries(player.Leader);
                    level.Remove(player);
                    level.UnloadLevel();
                    level.Session.Keys.Clear();
                    level.Session.Level = toLevel;
                    level.Session.RespawnPoint = level.GetSpawnPoint(spawnOffset + new Vector2(level.Bounds.Left, level.Bounds.Top));
                    level.LoadLevel(Player.IntroTypes.None);
                    Audio.SetMusicParam("fade", 1f);
                    Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
                    level.Camera.Y -= 8f;
                };
        }

    }

}
