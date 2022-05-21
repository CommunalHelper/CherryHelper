local drawableNPatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")

local axisOptions = {
    Both = "both",
    Vertical = "vertical",
    Horizontal = "horizontal",
}

local nonReturnKevin = {}

nonReturnKevin.name = "CherryHelper/NonReturnKevin"
nonReturnKevin.depth = 0
nonReturnKevin.minimumSize = {24, 24}
nonReturnKevin.fieldInformation = {
    axes = {
        options = axisOptions,
        editable = false
    }
}
nonReturnKevin.placements = {}

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
        name = "alt" .. axis,
        data = {
            width = 24,
            height = 24,
            axes = axis,
            altTexture = true,
            chillout = false
        }
    })
end

local frameTextures = {
    none = "objects/crushblock/block00",
    horizontal = "objects/crushblock/block01",
    vertical = "objects/crushblock/block02",
    both = "objects/crushblock/block03"
}

local altFrameTextures = {
    none = "objects/noReturnKevin/block00",
    horizontal = "objects/noReturnKevin/block01",
    vertical = "objects/noReturnKevin/block02",
    both = "objects/noReturnKevin/block03"
}

local nPatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local kevinColor = {98 / 255, 34 / 255, 43 / 255}
local smallFaceTexture = "objects/crushblock/idle_face"
local giantFaceTexture = "objects/crushblock/giant_block00"

local altKevinColor = {36 / 255, 34 / 255, 98 / 255}
local altSmallFaceTexture = "objects/noReturnKevin/idle_face"
local altGiantFaceTexture = "objects/noReturnKevin/giant_block00"

function nonReturnKevin.sprite(room, entity)
    local altTexture = entity.altTexture or false

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local axes = entity.axes or "both"
    local chillout = entity.chillout

    local giant = height >= 48 and width >= 48 and chillout

    local faceTexture = giant and giantFaceTexture or smallFaceTexture
    if altTexture then
        faceTexture = giant and altGiantFaceTexture or altSmallFaceTexture
    end

    local frameTexture = frameTextures[axes] or frameTextures["both"]
    if altTexture then
        frameTexture = altFrameTextures[axes] or altFrameTextures["both"]
    end

    local ninePatch = drawableNPatch.fromTexture(frameTexture, nPatchOptions, x, y, width, height)

    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, kevinColor)
    if altTexture then
        rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, altKevinColor)
    end

    local faceSprite = drawableSprite.fromTexture(faceTexture, entity)
    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))

    local sprites = ninePatch:getDrawableSprite()

    table.insert(sprites, 1, rectangle:getDrawableSprite())
    table.insert(sprites, 2, faceSprite)

    return sprites
end

return nonReturnKevin