module CH_AnterogradeController

using ..Ahorn, Maple
@mapdef Entity "CherryHelper/AnterogradeController" AnterogradeController(x::Integer, y::Integer,duration::Number=30.0,ColorGrade::String="sepia")

const placements = Ahorn.PlacementDict(
    "Time Limit Controller (Cherry Helper)" => Ahorn.EntityPlacement(
        AnterogradeController,
        "point"
    )
)


function Ahorn.selection(entity::AnterogradeController)
    x, y = Ahorn.position(entity)

    res = Ahorn.Rectangle[Ahorn.Rectangle(x - 12, y - 12, 24, 24)]
    return res
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::AnterogradeController, room::Maple.Room)
    x, y = Ahorn.position(entity)
    Ahorn.drawImage(ctx, "objects/anterogradeController/icon", x - 12, y - 12)
end

end