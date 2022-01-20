local catEye = {}

local catEyeOptions = {
    Normal = "Normal",
    Double_Dash = "Double_Dash",
    Shadow_Dash = "Shadow_Dash"
}

catEye.name = "CherryHelper/CatEye"
catEye.justification = {0.5, 0.5}
catEye.fieldInformation = {
    state = {
        options = catEyeOptions,
        editable = false
    }
}
catEye.placements = {}

for _, type in pairs(catEyeOptions) do
    table.insert(catEye.placements, {
        name = type,
        data = {
            state = type,
            Wiggles = true,
            respawnTime = 4.0
        }
    })
end

function catEye.texture(room, entity)
    if entity.state == catEyeOptions.Normal then
        return "danger/cateye/cateye00"
    elseif entity.state == catEyeOptions.Double_Dash then
        return "danger/twodashcateye/cateye00"
    elseif entity.state == catEyeOptions.Shadow_Dash then
        return "danger/shadowcateye/cateye00"
    end
end

return catEye