module CH_ItemCrystalPedestalPedestal

using ..Ahorn, Maple

@mapdef Entity "CherryHelper/ItemCrystalPedestal" ItemCrystalPedestal(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Item Crystal Pedestal (Cherry Helper)" => Ahorn.EntityPlacement(
        ItemCrystalPedestal,
        "rectangle"
    )
)


sprite = "objects/itemCrystalPedestal/pedestal00.png"

function Ahorn.selection(entity::ItemCrystalPedestal)
    x, y = Ahorn.position(entity)
    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle("objects/itemCrystalPedestal/pedestal00.png", x, y)]
    return res
end


Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ItemCrystalPedestal, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end