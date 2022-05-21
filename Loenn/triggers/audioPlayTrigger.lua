local audioPlay = {}

audioPlay.name = "CherryHelper/AudioPlayTrigger"
audioPlay.placements = {
    name = "trigger",
    data = {
        AudioEventToPlay="event:/game/09_core/frontdoor_unlock",
        OnlyOnce=false
    }
}

return audioPlay