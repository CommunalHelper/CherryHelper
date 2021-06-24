module CH_AudioPlayTrigger

using ..Ahorn, Maple

@mapdef Trigger "CherryHelper/AudioPlayTrigger" AudioPlayTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8, AudioEventToPlay::String="event:/game/09_core/frontdoor_unlock",OnlyOnce::Bool=false)

const placements = Ahorn.PlacementDict(
    "Play Audio Event Trigger (Cherry Helper)" => Ahorn.EntityPlacement(
        AudioPlayTrigger,
        "rectangle"
    )
)

end