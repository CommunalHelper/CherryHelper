module CH_DoorField

using ..Ahorn, Maple
sides=String[
    "A",
    "B",
    "C"]
@mapdef Entity "CherryHelper/DoorField" DoorField(x::Integer, y::Integer, width::Integer=8, height::Integer=8,  toLevel::String="",EndLevel::Bool=false, SIDOfMap::String="", LoadAnotherBin::Bool=false, Side::String="A")

const placements = Ahorn.PlacementDict(
    "Door Field (Room Teleport) (Cherry Helper)" => Ahorn.EntityPlacement(
         DoorField,
         "rectangle",
         Dict{String, Any}(
            "EndLevel" => false,
            "LoadAnotherBin" => false
        )
    ),
    "Door Field (End Level) (Cherry Helper)" => Ahorn.EntityPlacement(
         DoorField,
         "rectangle",
         Dict{String, Any}(
            "EndLevel" => true,
            "LoadAnotherBin" => false
        )
    ),
    "Door Field (Chapter Change) (Cherry Helper)" => Ahorn.EntityPlacement(
         DoorField,
         "rectangle",
         Dict{String, Any}(
            "EndLevel" => false,
            "LoadAnotherBin" => true,
            "SIDOfMap" => "Celeste/1-ForsakenCity",
            "Side" => "A"
        )
    )
)

Ahorn.minimumSize(entity::DoorField) = 8, 8
Ahorn.resizable(entity::DoorField) = true, true


Ahorn.editingOptions(entity::DoorField) = Dict{String, Any}(
    "Side" => sides
)
Ahorn.selection(entity::DoorField) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DoorField, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (0.0, 0.0, 0.0, 1.0), (0.2, 0.2, 0.6, 1.0))
end

end