using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/AudioPlayTrigger")]
    public class AudioPlayTrigger : Trigger
    {
        private string eventToPlay;
        private bool onlyOnce;
        public AudioPlayTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            eventToPlay = data.Attr("AudioEventToPlay");
            onlyOnce = data.Bool("OnlyOnce");
        }
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            Audio.Play(eventToPlay);
            if (onlyOnce) RemoveSelf();
        }
    }
}
