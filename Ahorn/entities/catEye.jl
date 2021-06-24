module CH_CatEye

using ..Ahorn, Maple
@mapdef Entity "CherryHelper/CatEye" CatEye(x::Integer, y::Integer,state::String="Normal",Wiggles::Bool=true,respawnTime::Number=4.0)
states=String[
    "Normal",
    "Double_Dash",
    "Shadow_Dash"   
]
const placements = Ahorn.PlacementDict(
    "Cat Eye ($(replace(state,"_"=>" ")), Cherry Helper)" => Ahorn.EntityPlacement(
        CatEye,
        "rectangle",
        Dict{String, Any}(
            "state" => state
        )
    ) for state in states

)

function Ahorn.selection(entity::CatEye)
    x, y = Ahorn.position(entity)
    sprite = "danger/cateye/cateye00.png"

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CatEye, room::Maple.Room)
    if get(entity.data, "state", "Normal") == "Normal"
        theSprite = "danger/cateye/cateye00.png"
    elseif get(entity.data, "state", "Normal") == "Double_Dash"
        theSprite = "danger/twodashcateye/cateye00.png"
    elseif get(entity.data, "state", "Normal") == "Shadow_Dash"
        theSprite = "danger/shadowcateye/cateye00.png"
    end
    sprite =  theSprite
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end