using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using MonoMod.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.UI;


namespace Celeste.Mod.CherryHelper
{
    public class CherryHelper : EverestModule
    {
        public static CherryHelper Instance;

        public CherryHelper()
        {
            Instance = this;
        }
        public override Type SessionType => typeof(CherryHelperSession);
        public static CherryHelperSession Session => (CherryHelperSession)Instance._Session;

        public override Type SaveDataType => typeof(CherryHelperSaveData);
        public static CherryHelperSaveData SaveData => (CherryHelperSaveData)Instance._SaveData;

        public Sprite TemporalSea_TestHatthew;


        public override void LoadContent(bool firstLoad)
        {
            CatEye.P_Respawn = new ParticleType(Refill.P_Regen)
            {
                SpeedMin = 30f,
                SpeedMax = 40f,
                SpeedMultiplier = 0.2f,
                DirectionRange = 6.2831855f,
                Color = Calc.HexToColor("ffee83"),
                Color2 = Calc.HexToColor("ffc537")
            };
            ShadowBumper.P_Launch = new ParticleType
            {
                Source = GFX.Game["particles/rect"],
                Color = Calc.HexToColor("FF00A6"),
                Color2 = Calc.HexToColor("9508DB"),
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                Size = 0.5f,
                SizeRange = 0.2f,
                RotationMode = ParticleType.RotationModes.Random,
                LifeMin = 0.6f,
                LifeMax = 1.2f,
                SpeedMin = 40f,
                SpeedMax = 140f,
                SpeedMultiplier = 0.1f,
                Acceleration = new Vector2(0f, 10f),
                DirectionRange = 0.6981317f
            };
        }
        public override void Load()
        {
            ShadowDashRefill.Load(); // load the Shadow Dash mechanic
            On.Celeste.Session.ctor += handleSavedFlags;
            On.Celeste.Session.Restart += handleSF2;
        }

        private Session handleSF2(On.Celeste.Session.orig_Restart orig, Session self, string intoLevel)
        {
            Session sesh = orig(self, intoLevel);
            if (SaveData == null) return sesh;
            bool foundSomething = false;
            foreach (SavedFlagsData data in SaveData.allSavedFlagsData)
            {
                if (sesh.Area.LevelSet == data.campaignId)
                {
                    data.handleSavedFlags(sesh);
                    foundSomething = true;
                }
            }
            if (!foundSomething)
            {
                SaveData.allSavedFlagsData.Add(new SavedFlagsData(sesh.Area.LevelSet, new Dictionary<string, bool>()));
            }
            return sesh;
            
        }

        private void handleSavedFlags(On.Celeste.Session.orig_ctor orig, Session self)
        {
            orig(self);
            if (SaveData == null) return;
            bool foundSomething = false;
            foreach (SavedFlagsData data in SaveData.allSavedFlagsData)
            {
                if (self.Area.LevelSet == data.campaignId)
                {
                    data.handleSavedFlags(self);
                    foundSomething = true;
                }
            }
            if (!foundSomething)
            {
                SaveData.allSavedFlagsData.Add(new SavedFlagsData(self.Area.LevelSet, new Dictionary<string, bool>()));
            }
            
        }


        
        public void SaveFlag(Session session, string flag, bool value)
        {
            bool foundSomething = false;
            foreach (SavedFlagsData data in SaveData.allSavedFlagsData)
            {
                if (session.Area.LevelSet == data.campaignId)
                {
                    if (data.savedFlags.ContainsKey(flag))
                    {
                        data.savedFlags.Remove(flag);
                    }
                    data.savedFlags.Add(flag, value);
                    foundSomething = true;
                }
            }
            if (!foundSomething)
            {
                SavedFlagsData data;
                SaveData.allSavedFlagsData.Add(data = new SavedFlagsData(session.Area.LevelSet, new Dictionary<string, bool>()));
                data.savedFlags.Add(flag, value);
            }
        }
        public static void ChangeFields(string nightId, bool Enable)
        {
            foreach (EntityToggleField field in EntityToggleField.FindById(Engine.Scene as Level, nightId))
            {
                if (Enable) field.on = true;
                else field.on = false;
            }
        }
        public override void Initialize()
        {
        }

        // Unload the entirety of your mod's content. Free up any native resources.
        public override void Unload()
        {
            ShadowDashRefill.Unload(); // unload the Shadow Dash mechanic
            On.Celeste.Session.ctor -= handleSavedFlags;
            On.Celeste.Session.Restart -= handleSF2;
        }
    }
}
