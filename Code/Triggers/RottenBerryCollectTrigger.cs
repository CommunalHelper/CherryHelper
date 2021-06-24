using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
namespace Celeste.Mod.CherryHelper
{

    [CustomEntity("CherryHelper/RottenBerryCollectTrigger")]
    public class RottenBerryCollectTrigger : Trigger
    {

        // Token: 0x0600003C RID: 60 RVA: 0x00003897 File Offset: 0x00001A97
        public RottenBerryCollectTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        // Token: 0x0600003D RID: 61 RVA: 0x000038B4 File Offset: 0x00001AB4
        public override void OnEnter(Player player)
        {
            CherryHelper.Session.SafeFromRot = true;
        }

        public override void OnLeave(Player player)
        {
            CherryHelper.Session.SafeFromRot = false;
        }

        // Token: 0x04000029 RID: 41
        private static bool? NoRefills;

        // Token: 0x0400002A RID: 42
        private readonly RottenBerryCollectTrigger.DashesNum dashesNum;

        // Token: 0x02000039 RID: 57
        private enum DashesNum
        {
            // Token: 0x04000188 RID: 392
            Zero,
            // Token: 0x04000189 RID: 393
            One,
            // Token: 0x0400018A RID: 394
            Two
        }
    }
}
