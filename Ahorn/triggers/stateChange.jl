module CH_PlayerState

using ..Ahorn, Maple
@mapdef Trigger "CherryHelper/PlayerStateChange" ChangePlayerStateTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8,onlyOnce::Bool=false,State::Integer=0)


const placements = Ahorn.PlacementDict(
    "Change Player State (Cherry Helper)" => Ahorn.EntityPlacement(
        ChangePlayerStateTrigger,
        "rectangle"
    ),
)

end
