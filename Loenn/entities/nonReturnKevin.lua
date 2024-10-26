local drawableNPatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local axisOptions = {
    Both = "both",
    Vertical = "vertical",
    Horizontal = "horizontal",
}

local nonReturnKevin = {}

nonReturnKevin.name = "CherryHelper/NonReturnKevin"
nonReturnKevin.depth = 0
nonReturnKevin.minimumSize = { 24, 24 }
nonReturnKevin.fieldInformation = {
    axes = {
        options = axisOptions,
        editable = false
    },
    crushParticleColor1 = {
        fieldType = "color"
    },
    crushParticleColor2 = {
        fieldType = "color"
    },
    activateParticleColor1 = {
        fieldType = "color"
    },
    activateParticleColor2 = {
        fieldType = "color"
    },
    fillColor = {
        fieldType = "color"
    }
}

nonReturnKevin.ignoredFields = { "_name", "_id", "originX", "originY", "reskinnable" }

nonReturnKevin.placements = { }

for _, axis in pairs(axisOptions) do
    table.insert(nonReturnKevin.placements, {
        name = axis,
        data = {
            width = 24,
            height = 24,
            axes = axis,
            altTexture = false,
            chillout = false
        }
    })
end

for _, axis in pairs(axisOptions) do
    table.insert(nonReturnKevin.placements, {
        name = "reskinnable" .. axis,
        data = {
            reskinnable = true,
            width = 24,
            height = 24,
            axes = axis,
            chillout = false,
            spriteDirectory = "objects/noReturnKevin",
            crushParticleColor1 = "ff66e2",
            crushParticleColor2 = "68fcff",
            activateParticleColor1 = "5fcde4",
            activateParticleColor2 = "ffffff",
            fillColor = "242262",
        }
    })
end

local frameTextures = {
    none = "/block00",
    horizontal = "/block01",
    vertical = "/block02",
    both = "/block03"
}

local nPatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local smallFaceTexture = "/idle_face"
local giantFaceTexture = "/giant_block00"

function nonReturnKevin.sprite(room, entity)
    local spriteDirectory = entity.spriteDirectory
    local altTexture = entity.altTexture or false
    if spriteDirectory == nil then
        if altTexture then
            spriteDirectory = "objects/noReturnKevin"
        else
            spriteDirectory = "objects/crushblock"
        end
    end

    local fillColor = { 98 / 255, 34 / 255, 43 / 255 }

    if entity.fillColor ~= nil then
        local success, r, g, b = utils.parseHexColor(entity.fillColor)
        fillColor = { r, g, b }
    elseif altTexture then
        fillColor = { 36 / 255, 34 / 255, 98 / 255 }
    end

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local axes = entity.axes or "both"
    local chillout = entity.chillout

    local giant = height >= 48 and width >= 48 and chillout

    local faceTexture = spriteDirectory .. (giant and giantFaceTexture or smallFaceTexture)

    local frameTexture = spriteDirectory .. frameTextures[axes]

    local ninePatch = drawableNPatch.fromTexture(frameTexture, nPatchOptions, x, y, width, height)

    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, fillColor)

    local faceSprite = drawableSprite.fromTexture(faceTexture, entity)
    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))

    local sprites = ninePatch:getDrawableSprite()

    table.insert(sprites, 1, rectangle:getDrawableSprite())
    table.insert(sprites, 2, faceSprite)

    return sprites
end

return nonReturnKevin
