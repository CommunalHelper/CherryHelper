module CH_Rotberry

using ..Ahorn, Maple
@mapdef Entity "CherryHelper/RottenBerry" RottenBerry(x::Integer, y::Integer, checkpointID::Integer=-1, order::Integer=-1)

const placements = Ahorn.PlacementDict(
    "Rotten Berry (Cherry Helper)" => Ahorn.EntityPlacement(
        RottenBerry
    ),
)




const fallback = "collectables/rottenberry/normal00"

function Ahorn.selection(entity::RottenBerry)
    x, y = Ahorn.position(entity)
    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle("collectables/rottenberry/normal00", x, y)]
    return res
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::RottenBerry)
    x, y = Ahorn.position(entity)
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::RottenBerry, room::Maple.Room)
    x, y = Ahorn.position(entity)
    Ahorn.drawSprite(ctx, "collectables/rottenberry/normal00", x, y)
end

end