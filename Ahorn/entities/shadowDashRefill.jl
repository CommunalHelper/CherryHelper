module CH_ShadowDashRefill

using ..Ahorn, Maple
@mapdef Entity "CherryHelper/ShadowDashRefill" ShadowDashRefill(x::Integer, y::Integer, oneUse::Bool=false)
const placements = Ahorn.PlacementDict(
    "Shadow Dash Refill (Cherry Helper)" => Ahorn.EntityPlacement(
        ShadowDashRefill
    ),
)

theSprite = "objects/shadowDashRefill/idle00"

function Ahorn.selection(entity::ShadowDashRefill)
    x, y = Ahorn.position(entity)
    sprite = theSprite

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ShadowDashRefill, room::Maple.Room)
    sprite =  theSprite
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end