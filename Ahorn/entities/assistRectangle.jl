module CH_AssistRect

using ..Ahorn, Maple

@mapdef Entity "CherryHelper/AssistRect" AssistRect(x::Integer, y::Integer, width::Integer=8, height::Integer=8,color::String="#492632")
function getColor(color)
    return ((Ahorn.argb32ToRGBATuple(parse(Int, replace(color, "#" => ""), base=16))[1:3] ./ 255)..., 1.0)
end

function getSemiColor(color)
    return ((Ahorn.argb32ToRGBATuple(parse(Int, replace(color, "#" => ""), base=16))[1:3] ./ 255)..., 0.25)
end
const placements = Ahorn.PlacementDict(
    "Outline Indicator (Cherry Helper)" => Ahorn.EntityPlacement(
        AssistRect,
        "rectangle"
    )
)

Ahorn.minimumSize(entity::AssistRect) = 8, 8
Ahorn.resizable(entity::AssistRect) = true, true

Ahorn.selection(entity::AssistRect) = Ahorn.getEntityRectangle(entity)

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::AssistRect, room::Maple.Room)
    num, num3 = Ahorn.position(entity)
    Height = Int(get(entity.data, "height", 8))
    Width = Int(get(entity.data, "width", 8))
    num2 = num+Width
    num4 = num3+Height
    rawColor = get(entity.data, "color", "#ffffff")
    color = getColor(rawColor)
    Ahorn.setSourceColor(ctx, color)
    Ahorn.set_line_width(ctx, 0.1);
    Ahorn.drawRectangle(ctx,num + 4, num3 + 4, Width - 8, Height - 8, (0.0,0.0,0.0,0.0), getSemiColor(rawColor));
    for num5 = num:3:Number(num2-3)
        Ahorn.move_to(ctx, num5, num3)
        Ahorn.line_to(ctx, num5 + 2, num3)
        Ahorn.move_to(ctx, num5, num4-1)
        Ahorn.line_to(ctx, num5 + 2, num4-1)
        Ahorn.stroke(ctx)
    end
    for num6 = num3:3:Number(num4-3)
        Ahorn.move_to(ctx, num+1, num6)
        Ahorn.line_to(ctx, num + 1, num6+2)
        Ahorn.move_to(ctx, num2, num6)
        Ahorn.line_to(ctx, num2, num6+2)
        Ahorn.stroke(ctx)
    end
    Ahorn.drawRectangle(ctx,num + 1, num3, 1, 2, color);
    Ahorn.drawRectangle(ctx,num2 - 2, num3, 2, 2, color);
    Ahorn.drawRectangle(ctx,num, num4 - 2, 2, 2, color);
    Ahorn.drawRectangle(ctx,num2 - 2, num4 - 2, 2, 2, color);
end

end