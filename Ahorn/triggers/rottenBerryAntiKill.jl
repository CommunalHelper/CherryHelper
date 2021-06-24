module CH_RotberryAntiKill


using ..Ahorn, Maple
@mapdef Trigger "CherryHelper/RottenBerryCollectTrigger" RottenBerryCollectTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8)


const placements = Ahorn.PlacementDict(
    "Rotten Berry Safety (Cherry Helper)" => Ahorn.EntityPlacement(
        RottenBerryCollectTrigger,
        "rectangle"
    ),
)

end
