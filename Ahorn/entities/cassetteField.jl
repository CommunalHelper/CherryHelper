module CherryHelperCassetteFieldModule

using ..Ahorn, Maple

@mapdef Entity "CherryHelper/CassetteField" CassetteField(x::Integer, y::Integer, width::Integer=8, height::Integer=8, index::Integer=0, tempo::Number=1.0,Type::String="ZipMover")
const colorsA=[(0.0, 0.53, 1.0, 0.4),(1.0, 0.23, 0.79, 0.4),(1.0, 0.79, 0.0, 0.4),(0.54, 1.0, 0.22, 0.4)]
const colorsB=[(0.0, 0.53, 1.0, 1.0),(1.0, 0.23, 0.79, 1.0),(1.0, 0.79, 0.0, 1.0),(0.54, 1.0, 0.22, 1.0)]

Ahorn.minimumSize(entity::CassetteField) = 8, 8
Ahorn.resizable(entity::CassetteField) = true, true

Ahorn.selection(entity::CassetteField) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CassetteField, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))
	index = Int(get(entity.data, "index",0))
    Ahorn.drawRectangle(ctx, 0, 0, width, height, colorsA[index+1], colorsB[index+1])
end

const colorNames = Dict{String, Int}(
    "Blue" => 0,
    "Rose" => 1,
    "Bright Sun" => 2,
    "Malachite" => 3
)

const placements = Ahorn.PlacementDict(
    "Cassette Field ($index - $color, Cherry Helper)" => Ahorn.EntityPlacement(
        CassetteField,
        "rectangle",
        Dict{String, Any}(
            "index" => index,
        )
    ) for (color, index) in colorNames
)

Ahorn.editingOptions(entity::CassetteField) = Dict{String, Any}(
    "index" => colorNames
)


Ahorn.minimumSize(entity::CassetteField) = 16, 16
Ahorn.resizable(entity::CassetteField) = true, true

Ahorn.selection(entity::CassetteField) = Ahorn.getEntityRectangle(entity)

end