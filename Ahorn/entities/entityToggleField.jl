module CH_Stealfield

using ..Ahorn, Maple

@mapdef Entity "CherryHelper/NightItemLockfield" EntityTheftField(x::Integer, y::Integer, width::Integer=8, height::Integer=8, ItemsToSteal::String="ZipMover",nightId::String="",startAppeared::Bool=false,AssistRectangleOnSolid::Bool=true)

const placements = Ahorn.PlacementDict(
    "Entity Toggle Field (Cherry Helper)" => Ahorn.EntityPlacement(
        EntityTheftField,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::EntityTheftField) = 8, 8
Ahorn.resizable(entity::EntityTheftField) = true, true

Ahorn.selection(entity::EntityTheftField) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::EntityTheftField, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (0.4, 0.4, 0.4, 0.4), (0.4, 0.4, 0.4, 1.0))
end

end