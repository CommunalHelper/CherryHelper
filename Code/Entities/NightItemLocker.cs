//using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



namespace Celeste.Mod.CherryHelper
{

    public class NightWipe : ScreenWipe
    {
        private VertexPositionColor[] vertexBuffer = new VertexPositionColor[6];

        public Action<float> OnUpdate;

        public NightWipe(Scene scene, bool wipeIn, Action onComplete = null)
            : base(scene, wipeIn, onComplete)
        {
        }

        public override void Update(Scene scene)
        {
            base.Update(scene);
            if (OnUpdate != null)
            {
                OnUpdate(Percent);
            }
        }

        public override void Render(Scene scene)
        {
            Color color = ScreenWipe.WipeColor * (WipeIn ? (1f - Ease.CubeIn((int)Percent)) : Ease.CubeOut((int)Percent));
            vertexBuffer[0].Color = color;
            vertexBuffer[0].Position = new Vector3(-10f, -10f, 0f);
            vertexBuffer[1].Color = color;
            vertexBuffer[1].Position = new Vector3(base.Right, -10f, 0f);
            vertexBuffer[2].Color = color;
            vertexBuffer[2].Position = new Vector3(-10f, base.Bottom, 0f);
            vertexBuffer[3].Color = color;
            vertexBuffer[3].Position = new Vector3(base.Right, -10f, 0f);
            vertexBuffer[4].Color = color;
            vertexBuffer[4].Position = new Vector3(base.Right, base.Bottom, 0f);
            vertexBuffer[5].Color = color;
            vertexBuffer[5].Position = new Vector3(-10f, base.Bottom, 0f);
            ScreenWipe.DrawPrimitives(vertexBuffer);
        }
    }

    [CustomEntity("CherryHelper/NightItemLockfield")]
    [Tracked(true)]
    public class EntityToggleField : Entity
    {
        //public List<Type> SometimesFailCollision = new List<Type>();
        public List<String> itemsToBan;
        public List<Entity> instancesStolen = new List<Entity>();
        public bool stolenempty = true;
        public string nightId = "";
        public bool on = false;
        public bool lucid = false;
        public bool firstTime;
        public Dictionary<Entity, AssistRectangle> assign = new Dictionary<Entity, AssistRectangle>();
        public TransitionListener transbunny;
        private bool can;
        public bool assistOnSolid = true;
        public EntityToggleField(Vector2 position, int width, int height)
        : base(position)
        {
            base.Collider = new Hitbox(width, height);


        }

        public EntityToggleField(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height)
        {
            itemsToBan = new List<string>(data.Attr("ItemsToSteal").Split(','));
            nightId = data.Attr("nightId");
            lucid = data.Bool("lucid");
            assistOnSolid = data.Bool("AssistRectangleOnSolid", true);
            if (data.Bool("startAppeared"))
            {
                on = true;
                firstTime = false;
            }
            else
            {
                firstTime = true;
                Add(transbunny = new TransitionListener
                {
                    OnInBegin = delegate
                    {
                        foreach (Entity entity in SceneAs<Level>().Entities)
                        {
                            if (entity.Collider != null)
                            {
                                if (Collider.Bounds.Contains(entity.Collider.Bounds))
                                {
                                    foreach (string item in itemsToBan)
                                    {
                                        if (item.Length > 0)
                                        {
                                            Type CheckType = this.GetType();
                                            if (FakeAssembly.GetFakeEntryAssembly().GetType("Celeste." + item, false, true) != null)
                                            {
                                                CheckType = FakeAssembly.GetFakeEntryAssembly().GetType("Celeste." + item, false, true);
                                            }
                                            else if (FakeAssembly.GetFakeEntryAssembly().GetType("Celeste.Mod." + item, false, true) != null)
                                            {
                                                CheckType = FakeAssembly.GetFakeEntryAssembly().GetType("Celeste.Mod." + item, false, true);
                                            }
                                            else if (FakeAssembly.GetFakeEntryAssembly().GetType(item, false, true) != null)
                                            {
                                                CheckType = FakeAssembly.GetFakeEntryAssembly().GetType(item, false, true);
                                            }

                                            if (entity.GetType() == CheckType)
                                            {
                                                instancesStolen.Add(entity);
                                                stolenempty = false;
                                                HandleVfx();
                                                if (entity is Solid && assistOnSolid)
                                                {
                                                    assign.Add(entity, new AssistRectangle(entity.Position, (int)entity.Width, (int)entity.Height, Color.White * 0.3f));
                                                    assign.TryGetValue(entity, out AssistRectangle toadd);
                                                    toadd.secretAlpha = 0.1f;
                                                    SceneAs<Level>().Add(toadd);
                                                }
                                                entity.RemoveSelf();
                                                firstTime = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
        public static List<EntityToggleField> FindById(Level level, string nightId)
        {
            List<EntityToggleField> hoohoo = new List<EntityToggleField>();
            foreach (EntityToggleField field in level.Tracker.GetEntities<EntityToggleField>().OfType<EntityToggleField>())
            {
                if (field.nightId == nightId)
                {
                    hoohoo.Add(field);
                }
            }
            return hoohoo;
        }
        private IEnumerator FlickerBlackhole()
        {
            Tween tween3 = Tween.Create(Tween.TweenMode.Oneshot, null, 0.3f, start: true);
            tween3.OnUpdate = delegate (Tween t)
            {
                Glitch.Value = 0.5f * (1f - t.Eased);
            };
            Add(tween3);
            yield break;
        }
        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(new Coroutine(delayForAdded()));
        }
        private IEnumerator delayForAdded()
        {
            can = false;
            yield return 0.2;
            can = true;
        }
        public void HandleVfx()
        {
            Level level = SceneAs<Level>();
            if (!firstTime && can)
            {
                if (!Settings.Instance.DisableFlashes)
                {
                    if (lucid)
                    {
                        Audio.Play("event:/cherryhelper/night_time");
                        level.Add(new NightWipe(base.Scene, true));
                    }
                    if (!lucid)
                    {
                        Add(new Coroutine(FlickerBlackhole()));
                    }
                }
            }
        }
        public override void Update()
        {
            base.Update();
            Level level = SceneAs<Level>();
            if (on && !stolenempty)
            {
                HandleVfx();
                foreach (Entity item in instancesStolen)
                {
                    level.Add(item);
                    if (item is Solid && assistOnSolid)
                    {
                        AssistRectangle tokill;
                        assign.TryGetValue(item, out tokill);
                        tokill.RemoveSelf();
                        assign.Remove(item);
                        Player player = level.Tracker.GetEntity<Player>();
                        if (item.Collider.Bounds.Contains((int)player.X, (int)player.Y) && player != null) player.Die(Vector2.Zero, false, false);
                    }
                };
                instancesStolen.Clear();
                stolenempty = true;
                firstTime = false;
            }
            else if (!on && stolenempty)
            {
                foreach (Entity entity in level.Entities)
                {
                    if (entity.Collider != null)
                    {
                        if (Collider.Bounds.Contains(entity.Collider.Bounds))
                        {
                            foreach (string item in itemsToBan)
                            {
                                if (item.Length > 0)
                                {
                                    Type CheckType = this.GetType();
                                    if (FakeAssembly.GetFakeEntryAssembly().GetType("Celeste." + item, false, true) != null)
                                    {
                                        CheckType = FakeAssembly.GetFakeEntryAssembly().GetType("Celeste." + item, false, true);
                                    }
                                    else if (FakeAssembly.GetFakeEntryAssembly().GetType("Celeste.Mod." + item, false, true) != null)
                                    {
                                        CheckType = FakeAssembly.GetFakeEntryAssembly().GetType("Celeste.Mod." + item, false, true);
                                    }
                                    else if (FakeAssembly.GetFakeEntryAssembly().GetType(item, false, true) != null)
                                    {
                                        CheckType = FakeAssembly.GetFakeEntryAssembly().GetType(item, false, true);
                                    }

                                    if (entity.GetType() == CheckType)
                                    {
                                        instancesStolen.Add(entity);
                                        stolenempty = false;
                                        HandleVfx();
                                        if (entity is Solid && assistOnSolid)
                                        {
                                            assign.Add(entity, new AssistRectangle(entity.Position, (int)entity.Width, (int)entity.Height, Color.White *0.3f));
                                            assign.TryGetValue(entity, out AssistRectangle toadd);
                                            toadd.secretAlpha = 0.1f;
                                            SceneAs<Level>().Add(toadd);
                                        }
                                        entity.RemoveSelf();
                                        firstTime = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}

