module CH_BadelineBot

using ..Ahorn, Maple
@mapdef Entity "CherryHelper/BadelineBot" BadelineBot(x::Integer, y::Integer, tutorial::String="", nodes::Array{Tuple{Integer, Integer}, 1}=Tuple{Integer, Integer}[])

# Read from disk instead?
const baseGameTutorials = String[
    "combo", "superwalljump", "too_close", "too_far",
    "wavedash", "wavedashppt"
]

const placements = Ahorn.PlacementDict(
    "Hostile Player Playback (Cherry Helper)" => Ahorn.EntityPlacement(
        BadelineBot
    )
)

Ahorn.editingOptions(entity::BadelineBot) = Dict{String, Any}(
    "tutorial" => baseGameTutorials
)

Ahorn.nodeLimits(entity::BadelineBot) = 0, 2

const sprite = "characters/player_badeline/sitDown00"


function Ahorn.selection(entity::BadelineBot)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y, jx=0.5, jy=1.0)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BadelineBot) = Ahorn.drawSprite(ctx, sprite, 0, 0, jx=0.5, jy=1.0)

end