module CH_FallPortal

using ..Ahorn, Maple
sides=String[
    "A",
    "B",
    "C"]
@mapdef Entity "CherryHelper/FallTeleport" FallTeleportMirror(x::Integer, y::Integer, toLevel::String="",EndLevel::Bool=false, SIDOfMap::String="", LoadAnotherBin::Bool=false, Side::String="A", CustomFrameSprite::String="")
const placements = Ahorn.PlacementDict(
    "Custom Teleport Mirror (Room Teleport) (Cherry Helper)" => Ahorn.EntityPlacement(
         FallTeleportMirror,
         "rectangle",
         Dict{String, Any}(
            "EndLevel" => false,
            "LoadAnotherBin" => false
        )
    ),
    "Custom Teleport Mirror (End Level) (Cherry Helper)" => Ahorn.EntityPlacement(
         FallTeleportMirror,
         "rectangle",
         Dict{String, Any}(
            "EndLevel" => true,
            "LoadAnotherBin" => false
        )
    ),
    "Custom Teleport Mirror (Chapter Change) (Cherry Helper)" => Ahorn.EntityPlacement(
         FallTeleportMirror,
         "rectangle",
         Dict{String, Any}(
            "EndLevel" => false,
            "LoadAnotherBin" => true,
            "SIDOfMap" => "Celeste/1-ForsakenCity",
            "Side" => "A"
        )
    )
)



Ahorn.editingOptions(entity::FallTeleportMirror) = Dict{String, Any}(
    "Side" => sides
)
function Ahorn.selection(entity:: FallTeleportMirror)
    
    frameSprite = "objects/temple/portal/portalframe.png"
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle(frameSprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity:: FallTeleportMirror, room::Maple.Room)
    
    frameSprite = ((get(entity.data, "CustomSpritePath", "")=="") ? "objects/temple/portal/portalframe.png" : "objects/" + (get(entity.data, "CustomSpritePath", "")))
    Ahorn.drawSprite(ctx, frameSprite, 0, 0)

end

end