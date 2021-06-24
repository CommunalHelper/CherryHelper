module CH_AntiSnowballs

using ..Ahorn, Maple
@mapdef Trigger "CherryHelper/SnowballStop" WindAttackRemoveTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8)


const placements = Ahorn.PlacementDict(
    "Snowball Stop (Cherry Helper)" => Ahorn.EntityPlacement(
        WindAttackRemoveTrigger,
        "rectangle"
    ),
)

end
