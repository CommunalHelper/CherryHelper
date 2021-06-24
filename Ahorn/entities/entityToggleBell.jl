module CH_EntityToggleBell

using ..Ahorn, Maple
@mapdef Entity "CherryHelper/EntityToggleBell" EntityToggleBell(x::Integer, y::Integer, nightId::String="")

const placements = Ahorn.PlacementDict(
    "Entity Toggle Bell (Cherry Helper)" => Ahorn.EntityPlacement(
        EntityToggleBell
    )
)


const sprite = "objects/itemToggleBell/bell00.png"

function Ahorn.selection(entity::EntityToggleBell)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::EntityToggleBell, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end