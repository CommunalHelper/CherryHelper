module CH_ItemToggleTrigger

using ..Ahorn, Maple

@mapdef Trigger "CherryHelper/NightTimeTrigger" EntityToggleTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8, nightId::String="", Enable::Bool=true)

const placements = Ahorn.PlacementDict(
    "Item Toggle Trigger (Cherry Helper)" => Ahorn.EntityPlacement(
        EntityToggleTrigger,
        "rectangle"
    )
)

end