using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Celeste.Mod.CherryHelper
{

    [CustomEntity(new string[]
    {
    "CherryHelper/FallTeleport"
    })]
    // Celeste.FallTeleportMirror

    public class FallTeleportMirror : Entity
    {
        private struct Debris
        {
            public Vector2 Direction;

            public float Percent;

            public float Duration;

            public bool Enabled;
        }

        private class Bg : Entity
        {
            private MirrorSurface surface;

            private Vector2[] offsets;

            private List<MTexture> textures;

            public Bg(Vector2 position)
                : base(position)
            {
                base.Depth = 9500;
                textures = GFX.Game.GetAtlasSubtextures("objects/temple/portal/reflection");
                Vector2 value = new Vector2(10f, 4f);
                offsets = new Vector2[textures.Count];
                for (int i = 0; i < offsets.Length; i++)
                {
                    offsets[i] = value + new Vector2(Calc.Random.Range(-4, 4), Calc.Random.Range(-4, 4));
                }
                Add(surface = new MirrorSurface());
                surface.OnRender = delegate
                {
                    for (int j = 0; j < textures.Count; j++)
                    {
                        surface.ReflectionOffset = offsets[j];
                        textures[j].DrawCentered(Position, surface.ReflectionColor);
                    }
                };
            }

            public override void Render()
            {
                MTexture mTexture = GFX.Game["objects/temple/portal/surface"];
                mTexture.DrawCentered(Position);
            }
        }

        public static ParticleType P_CurtainDrop;

        public float DistortionFade = 1f;

        private bool canTrigger;

        private int switchCounter = 0;

        private VirtualRenderTarget buffer;

        private float bufferAlpha = 0f;

        private float bufferTimer = 0f;

        private Debris[] debris = new Debris[50];

        private Color debrisColorFrom = Calc.HexToColor("f442d4");

        private Color debrisColorTo = Calc.HexToColor("000000");

        private MTexture debrisTexture = GFX.Game["particles/blob"];

        private string toLevel;
        private bool endLevel;
        public bool SeamlessNextChapter;
        public string NextChapterSSID;
        public AreaMode ChapterAreaMode;
        public string customSpritePath;
        public FallTeleportMirror(Vector2 position)
            : base(position)
        {
            base.Depth = 2000;
            base.Collider = new Hitbox(100f, 54f, -50f, -22f);
            Add(new PlayerCollider(OnPlayer));
            canTrigger = true;
        }

        public FallTeleportMirror(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
            toLevel = data.Attr("toLevel");
            endLevel = data.Bool("EndLevel", false);
            NextChapterSSID = data.Attr("SIDOfMap");
            SeamlessNextChapter = data.Bool("LoadAnotherBin", true);
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
            customSpritePath = data.Attr("CustomFrameSprite", "");

        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            scene.Add(new Bg(Position));
        }

        public void OnSwitchHit(int side)
        {
            Add(new Coroutine(OnSwitchRoutine(side)));
        }

        private IEnumerator OnSwitchRoutine(int side)
        {
            yield return 0.4f;
            switchCounter++;
            Session session = (Scene as Level).Session;
            if (session.Area.Mode == AreaMode.Normal)
            {
                LightingRenderer lighting = (Scene as Level).Lighting;
                float lightTarget = Math.Max(0f, lighting.Alpha - 0.2f);
                while ((lighting.Alpha -= Engine.DeltaTime) > lightTarget)
                {
                    yield return null;
                }
            }
            yield return 0.15f;
            if (switchCounter < 2)
            {
                yield break;
            }
            yield return 0.1f;
            yield return 0.1f;
            Level level = SceneAs<Level>();
        }

        public void Activate()
        {
            Add(new Coroutine(ActivateRoutine()));
        }

        private IEnumerator ActivateRoutine()
        {
            Level level = Scene as Level;
            LightingRenderer light = level.Lighting;
            float debrisStart = 0f;
            Add(new BeforeRenderHook(BeforeRender));
            Add(new DisplacementRenderHook(RenderDisplacement));
            while (true)
            {
                bufferAlpha = Calc.Approach(bufferAlpha, 1f, Engine.DeltaTime);
                bufferTimer += 4f * Engine.DeltaTime;
                light.Alpha = Calc.Approach(light.Alpha, 0.2f, Engine.DeltaTime * 0.25f);
                if (debrisStart < (float)debris.Length)
                {
                    int index = (int)debrisStart;
                    debris[index].Direction = Calc.AngleToVector(Calc.Random.NextFloat((float)Math.PI * 2f), 1f);
                    debris[index].Enabled = true;
                    debris[index].Duration = 0.5f + Calc.Random.NextFloat(0.7f);
                }
                debrisStart += Engine.DeltaTime * 10f;
                for (int i = 0; i < debris.Length; i++)
                {
                    if (debris[i].Enabled)
                    {
                        debris[i].Percent %= 1f;
                        debris[i].Percent += Engine.DeltaTime / debris[i].Duration;
                    }
                }
                yield return null;
            }
        }

        private void BeforeRender()
        {
            if (buffer == null)
            {
                buffer = VirtualContent.CreateRenderTarget("temple-portal", 120, 64);
            }
            Vector2 position = new Vector2(buffer.Width, buffer.Height) / 2f;
            MTexture mTexture = GFX.Game["objects/temple/portal/portal"];
            Engine.Graphics.GraphicsDevice.SetRenderTarget(buffer);
            Engine.Graphics.GraphicsDevice.Clear(Color.Black);
            Draw.SpriteBatch.Begin();
            for (int i = 0; (float)i < 10f; i++)
            {
                float num = bufferTimer % 1f * 0.1f + (float)i / 10f;
                Color color = Color.Lerp(Color.Black, Color.Purple, num);
                float scale = num;
                float rotation = (float)Math.PI * 2f * num;
                mTexture.DrawCentered(position, color, scale, rotation);
            }
            Draw.SpriteBatch.End();
        }

        private void RenderDisplacement()
        {
            Draw.Rect(base.X - 60f, base.Y - 32f, 120f, 64f, new Color(0.5f, 0.5f, 0.25f * DistortionFade * bufferAlpha, 1f));
        }

        public override void Render()
        {
            base.Render();
            if (buffer != null)
            {
                Draw.SpriteBatch.Draw((RenderTarget2D)buffer, Position + new Vector2((-120f) / 2f, (-64f) / 2f), Color.White * bufferAlpha);
            }
            if (customSpritePath == "" || GFX.Game["objects/" + customSpritePath].get_AtlasPath() == null)
            {
                GFX.Game["objects/temple/portal/portalframe"].DrawCentered(Position);
            }
            else
            {
                GFX.Game["objects/" + customSpritePath].DrawCentered(Position);
            }
            Level level = base.Scene as Level;
            for (int i = 0; i < this.debris.Length; i++)
            {
                Debris debris = this.debris[i];
                if (debris.Enabled)
                {
                    float num = Ease.SineOut(debris.Percent);
                    Vector2 position = Position + debris.Direction * (1f - num) * (190f - level.Zoom * 30f);
                    Color color = Color.Lerp(debrisColorFrom, debrisColorTo, num);
                    float scale = Calc.LerpClamp(1f, 0.2f, num);
                    debrisTexture.DrawCentered(position, color, scale, (float)i * 0.05f);
                }
            }
        }

        private void OnPlayer(Player player)
        {
            if (canTrigger)
            {
                canTrigger = false;
                base.Scene.Add(new CS_FallTeleport(player, this, toLevel, endLevel, SeamlessNextChapter, NextChapterSSID, ChapterAreaMode));
            }
        }

        public override void Removed(Scene scene)
        {
            Dispose();
            base.Removed(scene);
        }

        public override void SceneEnd(Scene scene)
        {
            Dispose();
            base.SceneEnd(scene);
        }

        private void Dispose()
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
            buffer = null;
        }
    }


}
