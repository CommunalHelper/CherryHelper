using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using Mono.Cecil.Cil;
using System.Reflection;
using MonoMod.Cil;
using Celeste.Mod.Entities;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/AnterogradeController")]
    [Tracked]
    public class AnterogradeController : Entity
    {
        public Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn);
        private bool startedTween = false;
        public float duration = 30;
        private float origBase;
        private string colorGrade = "sepia";
        private bool paused = false;
        private float percentLeft = 0;
        private float audioCooldown = 0.5f;
        private float postDeathDelay = 1f;
        private int postPauseFrames = 20;
        public bool externalPause = false;
        

        public AnterogradeController(Vector2 position, float duration,string colorGrade, float postDeathDelay)
        {
            SetupTween();
            this.duration = duration;
            this.colorGrade = colorGrade;
            this.postDeathDelay = postDeathDelay;
            IL.Celeste.BloomRenderer.Apply += onBloomRendererApply;
        }
        public AnterogradeController(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            duration = data.Float("duration", 30);
            colorGrade = data.Attr("ColorGrade", "sepia");
            postDeathDelay = data.Float("PostDeathDelay", 1f);
            SetupTween();
            // resets colorgrade as a failsafe
            Add(new TransitionListener
            {
                OnOutBegin = delegate
                {
                    Depth = 10000;
                    tween.Stop();
                    IL.Celeste.BloomRenderer.Apply -= onBloomRendererApply;
                    DynData<Level> dynData = new DynData<Level>(SceneAs<Level>());
                    SceneAs<Level>().Session.ColorGrade = SceneAs<Level>().Session.MapData.Data.ColorGrade;
                    dynData["lastColorGrade"] = colorGrade;
                    Audio.SetMusicParam("AnterogradeFilter", 0f);
                   dynData["colorGradeEase"] = 1f - tween.Percent;
                }
            });
        }
        private IEnumerator deathRoutine() {
            yield return null;
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (SaveData.Instance.Assists.Invincible) yield break;
            if (player != null)
            {
                // unique death animation for this effect
                Engine.TimeRate = 0.01f;
                IL.Celeste.PlayerDeadBody.ctor += skipDeathAnimationNextTime;
                Celeste.Freeze(postDeathDelay);
                if (SceneAs<Level>().Session.Level == SceneAs<Level>().Session.MapData.StartLevel().Name)
                    player.Die(Vector2.Zero, false, false);
                else player.Die(Vector2.Zero, false, true);
                
            }
        }
        private void SetupTween()
        {
            Audio.SetMusicParam("AnterogradeFilter", 0f);
            Add(tween);
            tween.OnUpdate = delegate
            {
                if (postPauseFrames < 20)
                {
                    postPauseFrames += 1;
                }
                else {
                    // sets up sepiatone, taken from max480's Helping Hand's ColorGradeFadeTrigger
                    DynData<Level> data = new DynData<Level>(SceneAs<Level>());
                    data["lastColorGrade"] = SceneAs<Level>().Session.MapData.Data.ColorGrade;
                    SceneAs<Level>().Session.ColorGrade = colorGrade;
                    data["colorGradeEase"] = tween.Percent * 1.05f;
                    Audio.SetMusicParam("AnterogradeFilter", tween.Percent * 1.05f); 
                }
            };
            tween.OnComplete = delegate
            {
                Add(new Coroutine(deathRoutine()));
            };
        }
        public override void Removed(Scene scene)
        {
            IL.Celeste.BloomRenderer.Apply -= onBloomRendererApply;
            base.Removed(scene);
        }
        
        public override void Update()
        {
            base.Update();
            Player entity = SceneAs<Level>().Tracker.GetEntity<Player>();
            
            
            if (entity != null)
            {
                // check if the player has just started the intro
                int[] states = {11, 12, 13, 14, 15, 18 };
                bool inputCheck = Input.MoveX != 0 || Input.MoveY != 0 || Input.Jump.Check || Input.Dash.Check;
                
                foreach (int state in states)
                {
                   
                    // if the player has moved after dying or beginning the level
                    if (!startedTween && inputCheck && entity.StateMachine.State != state)
                    {
                        startedTween = true;
                        tween.Start(duration);
                    }
                    
                }
                if (startedTween)
                {
                    if ((entity.StateMachine.State == 11 || externalPause)&& !paused)
                    {
                        percentLeft = tween.Percent;
                        paused = true;
                        tween.Stop();
                    }
                    else if ((entity.StateMachine.State == 11 || externalPause) && paused)
                    {
                        DynData<Level> data = new DynData<Level>(SceneAs<Level>());
                        data["lastColorGrade"] = SceneAs<Level>().Session.MapData.Data.ColorGrade;
                        SceneAs<Level>().Session.ColorGrade = colorGrade;
                        data["colorGradeEase"] = percentLeft * 1.05f;
                        Audio.SetMusicParam("AnterogradeFilter", percentLeft * 1.05f);
                    }
                    else if (!(entity.StateMachine.State == 11 || externalPause) && paused)
                    {
                        paused = false;
                        DynData<Tween> data = new DynData<Tween>(tween);
                        postPauseFrames = 0;
                        if (!externalPause) postPauseFrames = 20;
                        tween.Start();
                        data.Set<float>("Percent",percentLeft);
                    }
                }


            }
            // resets colorgrade after death
            if (!startedTween)
            {
                DynData<Level> data = new DynData<Level>(SceneAs<Level>());
                Audio.SetMusicParam("AnterogradeFilter", audioCooldown);
                audioCooldown -= 0.25f * Engine.DeltaTime;
                SceneAs<Level>().Session.ColorGrade = SceneAs<Level>().Session.MapData.Data.ColorGrade;
            }
        }
        // shamelessly pilfered from ExtendedVariants
        private void onBloomRendererApply(ILContext il)
        {
            ILCursor iLCursor = new ILCursor(il);
            while (iLCursor.TryGotoNext(MoveType.After, (Instruction instr) => instr.MatchLdfld<BloomRenderer>("Base")))
            {
                Logger.Log("CherryHelper/AnterogradeBloomDeath", $"Modding bloom base at {iLCursor.Index} in IL code for BloomRenderer.Apply");
                iLCursor.EmitDelegate<Func<float, float>>(modBloomBase);
            }
            iLCursor.Index = 0;
            while (iLCursor.TryGotoNext(MoveType.After, (Instruction instr) => instr.MatchLdfld<BloomRenderer>("Strength")))
            {
                Logger.Log("CherryHelper/AnterogradeBloomDeath", $"Modding bloom strength at {iLCursor.Index} in IL code for BloomRenderer.Apply");
                iLCursor.EmitDelegate<Func<float, float>>(modBloomStrength);
            }
        }

        private float modBloomBase(float vanilla)
        {
            return MathHelper.Lerp(vanilla, 0, tween.Percent); 
        }

        private float modBloomStrength(float vanilla)
        {
            return MathHelper.Lerp(vanilla, 1, tween.Percent);
        }
        
       
        public static IEnumerator SkipDeathB(PlayerDeadBody self)
        {
            yield return null;
            // basically the contents of PlayerDeadBody.End(), invoking it didn't seem to work.
            DynData<PlayerDeadBody> data = new DynData<PlayerDeadBody>(self);
            data.Get<PlayerSprite>("sprite").Play("fallSlow");
            Audio.SetMusicParam("AnterogradeFilter", 0f);
            Level level = self.SceneAs<Level>();
            if (self.DeathAction == null)
            {
                self.DeathAction = level.Reload;
            }
            level.DoScreenWipe(wipeIn: false, self.DeathAction);
            IL.Celeste.PlayerDeadBody.ctor -= skipDeathAnimationNextTime;
        }

        // we cannot hook DeathRoutine on the fly because it gets optimized (it is too small).
        // we can hook its usage in the PlayerDeadBody constructor though
        private static void skipDeathAnimationNextTime(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            while (cursor.TryGotoNext(instr => instr.MatchCallvirt<PlayerDeadBody>("DeathRoutine")))
            {
                // call SkipDeathB instead!
                cursor.Next.OpCode = OpCodes.Call;
                cursor.Next.Operand = typeof(AnterogradeController).GetMethod("SkipDeathB");
            }
        }
    }
}
