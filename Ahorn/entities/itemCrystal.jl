module CH_ItemCrystal

using ..Ahorn, Maple

@mapdef Entity "CherryHelper/ItemCrystal" ItemCrystal(x::Integer, y::Integer, item::String="Refill",dieIfDropped::Bool=false)

items=String[
    "Refill",
    "Touch_Switch",
    "Two_Refill",
    "Jellyfish",
    "Fireball",
    "Shadow_Dash_Refill",
    "Summit_Confetti",
    "Seeker",
    "Crystal_Heart",
    "Unlockable_Character",
    "Badeline_Chaser",
    "Cloud",
    "Frail_Cloud",
    "Green_Booster",
	"Red_Booster",
	"Core_Toggle",
	"Feather",
	"Theo_Crystal"
]

const placements = Ahorn.PlacementDict(
    "Item Crystal ($(replace(item,"_"=>" "))) (Cherry Helper)" => Ahorn.EntityPlacement(
        ItemCrystal,
        "rectangle",
        Dict{String, Any}(
            "item" => item
        )
    ) for item in items
)


sprite = "objects/itemCrystal/idle00.png"

Ahorn.editingOptions(entity::ItemCrystal) = Dict{String, Any}(
    "item" => items
)

function Ahorn.selection(entity::ItemCrystal)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x-10.5,y-22,21,22)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ItemCrystal, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, -10)

end