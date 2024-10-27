using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using Monocle;

namespace Celeste.Mod.CherryHelper
{

    [CustomEntity(new string[]
    {
    "CherryHelper/SaveFlagTrigger"
    })]
    [Tracked(false)]
    public class SaveFlagTrigger : Trigger
    {


        public string Flag;

        public bool State;

        public bool CurrentState;

        public SaveFlagTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            Flag = data.Attr("Flag");
            State = data.Bool("State", false);
            CurrentState = data.Bool("CurrentState", true);
        }

        public override void OnEnter(Player player)
        {

            base.OnEnter(player);
            Level level = SceneAs<Level>();
            if (CurrentState)
            {
                CherryHelper.Instance.SaveFlag(level.Session, Flag, level.Session.GetFlag(Flag));
                // More of a failsafe thing than anything.
                level.Session.SetFlag(Flag, level.Session.GetFlag(Flag));
                Logger.Log("CherryHelper/SaveFlagTrigger", $"Setting state to {level.Session.GetFlag(Flag)}");
            }
            else
            {
                CherryHelper.Instance.SaveFlag(level.Session, Flag, State);
                level.Session.SetFlag(Flag, State);
            }
        }
    }

}
