module CH_SaveDataFlagTrigger

using ..Ahorn, Maple

@mapdef Trigger "CherryHelper/SaveFlagTrigger" SaveFlagTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8, Flag::String="", State::Bool=true, CurrentState::Bool=false)

const placements = Ahorn.PlacementDict(
    "Save Data Flag Trigger (Cherry Helper)" => Ahorn.EntityPlacement(
        SaveFlagTrigger,
        "rectangle"
    )
)

end