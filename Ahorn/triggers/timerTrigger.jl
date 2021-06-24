module CH_TimerTrigger

using ..Ahorn, Maple
@mapdef Trigger "CherryHelper/TimerTrigger" TimerResetTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8, refillTime=2.0)


const placements = Ahorn.PlacementDict(
    "Time Limit Controller Revert Trigger (Cherry Helper)" => Ahorn.EntityPlacement(
        TimerResetTrigger,
        "rectangle"
    ),
)

end
