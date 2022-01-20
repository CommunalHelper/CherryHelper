local rectangle = require("utils.utils")

local itemCrystal = {}

local itemCrystalOptions = {
    Refill = "Refill",
    Touch_Switch = "Touch_Switch",
    Two_Refill = "Two_Refill",
    Jellyfish = "Jellyfish",
    Fireball = "Fireball",
    Shadow_Dash_Refill = "Shadow_Dash_Refill",
    Summit_Confetti = "Summit_Confetti",
    Seeker = "Seeker",
    Crystal_Heart = "Crystal_Heart",
    Unlockable_Character = "Unlockable_Character",
    Badeline_Chaser = "Badeline_Chaser",
    Cloud = "Cloud",
    Frail_Cloud = "Frail_Cloud",
    Green_Booster = "Green_Booster",
	Red_Booster = "Red_Booster",
	Core_Toggle = "Core_Toggle",
	Feather = "Feather",
	Theo_Crystal = "Theo_Crystal"
}

itemCrystal.name = "CherryHelper/ItemCrystal"
itemCrystal.texture = "objects/itemCrystal/idle00"
itemCrystal.justification = {0.5, 0.5}
itemCrystal.fieldInformation = {
    item = {
        options = itemCrystalOptions,
        editable = false
    }
}
itemCrystal.placements = {}

for _, item1 in pairs(itemCrystalOptions) do
    table.insert(itemCrystal.placements, {
        name = item1,
        data = {
            item = item1,
            dieIfDropped = false
        }
    })
end

function itemCrystal.selection(room, entity)
    return utils.rectangle(entity.x - 11, entity.y - 11, 22, 22)
end

return itemCrystal