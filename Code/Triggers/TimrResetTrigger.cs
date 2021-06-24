using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/TimerTrigger")]
    class TimerResetTrigger : Trigger
    {
        private float refillTime = 2f;
        private DynData<Tween> tweenData;
        public TimerResetTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            refillTime = data.Float("refillTime", 2f);
        }
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            
        }
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            foreach (AnterogradeController ant in SceneAs<Level>().Tracker.GetEntities<AnterogradeController>())
                ant.externalPause = true; 
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            float i;
            foreach (AnterogradeController ant in SceneAs<Level>().Tracker.GetEntities<AnterogradeController>())
            {
                i = ant.tween.Percent;
                ant.externalPause = false;
                new DynData<Level>(SceneAs<Level>())["colorGradeEase"] = i * 0.95f;
            }

        }
        public override void OnStay(Player player)
        {
            base.OnStay(player);
            foreach (AnterogradeController ant in SceneAs<Level>().Tracker.GetEntities<AnterogradeController>())
            {
                tweenData = new DynData<Tween>(ant.tween);
                tweenData.Set<float>("Percent", Calc.LerpClamp(ant.tween.Percent, 0, refillTime * Engine.DeltaTime));
                new DynData<Level>(SceneAs<Level>())["colorGradeEase"] = ant.tween.Percent * 1.05f;
            }
        }

    }
}
