using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Helpers;
using System.Collections;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.CherryHelper
{
    [Tracked]
    [CustomEntity("CherryHelper/CassetteField")]
    public class CassetteField : CassetteBlock
    {
        private Collider detectionCollider;
        public List<String> itemsToBan;
        public List<Entity> trackedEntities = new List<Entity>();
        public bool stolenempty = true;
        public bool firstTime;
        public Dictionary<Entity, AssistRectangle> assign = new Dictionary<Entity, AssistRectangle>();
        public TransitionListener transbunny;
        private bool can;
        public bool assistOnSolid = true;
        public CassetteField(Vector2 position, EntityID id, float width, float height, int index, float tempo) : base(position, id, width, height, index, tempo)
        {
            detectionCollider = new Hitbox(Width, Height);
            Collider = null;
            Visible = false;
        }

        public CassetteField(EntityData data, Vector2 offset): base (data, offset)
        {
            itemsToBan = new List<string>(data.Attr("Type").Split(','));
            detectionCollider = new Hitbox(Width, Height);
            Collider = null;
            Visible = false;
            DynData<CassetteBlock> self = new DynData<CassetteBlock>(this);
            self.Get<LightOcclude>("occluder").RemoveSelf();
        }
        public override void Update()
        {

            foreach (Entity entity in trackedEntities)
            {
                entity.Active = Activated;
                entity.Collidable = Activated;
            }
            base.Update();
        }
        public override void Awake(Scene scene)
        {

            base.Awake(scene);
            DisappearEntities(scene);
        }

        private void DisappearEntities(Scene scene)
        {
            
            foreach (Entity entity in (scene as Level).Entities)
            {
                if (entity.Collider != null)
                {
                    if (detectionCollider.Bounds.Contains(entity.Collider.Bounds))
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
                                    trackedEntities.Add(entity);
                                    
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
}
