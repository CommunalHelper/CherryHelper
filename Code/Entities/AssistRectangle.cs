using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CherryHelper
{
    [CustomEntity("CherryHelper/AssistRect")]
    public class AssistRectangle : Entity
    {

        public Color color = Color.White;

        public float secretAlpha = 0.25f;
        public AssistRectangle(Vector2 position, int width, int height, Color color)
        {
            Position = position;
            Collider = new Hitbox(width, height);
            this.color = color;
            Active = false;
        }
        public AssistRectangle(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, Calc.HexToColor(data.Attr("color")))
        {

        }
        public override void Render()
        {
            int left = (int)Left;
            int top = (int)Top;
            float width = Width;
            float height = Height;
            
            if (!CullHelper.IsRectangleVisible(left, top, width, height))
            {
                return;
            }
            
            int right = (int)Right;
            int bottom = (int)Bottom;

            Draw.Rect(left + 4, top + 4, width - 8f, height - 8f, color * secretAlpha);
            for (float x = left; x < right - 3; x += 3f)
            {
                Draw.Rect(x, top, 2, 1, color);
                Draw.Rect(x, bottom - 1, 2, 1, color);
            }
            for (float y = top; y < bottom - 3; y += 3f)
            {
                Draw.Rect(left, y, 1, 2, color);
                Draw.Rect(right - 1, y, 1, 2, color);
            }
            Draw.Rect(left + 1, top, 1f, 2f, color);
            Draw.Rect(right - 2, top, 2f, 2f, color);
            Draw.Rect(left, bottom - 2, 2f, 2f, color);
            Draw.Rect(right - 2, bottom - 2, 2f, 2f, color);
            
            base.Render();
        }
    }
}
