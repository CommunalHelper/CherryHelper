using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using Monocle;
using System.Collections;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/PlayerStateChange")]
    public class ChangePlayerStateTrigger:Trigger
    {
        public int state;
        private bool onlyOnce;
        public IEnumerator emptyCoroutine()
        {
            yield break;
        }
        public override void Update()
        {
            base.Update();
            if (CollideFirst<Player>()?.StateMachine.State==18)
            {
                OnEnter(CollideFirst<Player>());
            }
        }

        public ChangePlayerStateTrigger(EntityData data, Vector2 offset):base(data,offset) {
            base.Collider = new Hitbox(data.Width, data.Height);
            state = data.Int("State");
            onlyOnce = data.Bool("onlyOnce", false);
        }
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (state >= 25)
            {
                state = 0;
            }
            
            if (state == 24)
                Logger.Log("CherryHelper/ChangePlayerStateTrigger", "Fling Bird State crashes the game, please refrain from using state 24.");
            else
            player.StateMachine.ForceState(state);
            
            if (onlyOnce) base.RemoveSelf();
        }
    }
}
