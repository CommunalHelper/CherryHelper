module CH_UninterruptedNonReturnKevin


using ..Ahorn, Maple


@mapdef Entity "CherryHelper/UI_NRCB" UninterruptedNRCB(x::Integer, y::Integer, width::Integer=32, height::Integer=32, axes::String="both", chillout::Bool=false, spriteDirectory::String="objects/uninterruptedNRK", fillColor::String="242262")


const placements = Ahorn.PlacementDict(
    "Uninterruptable Non-Return Kevin (Classic) (Cherry Helper)" => Ahorn.EntityPlacement(
       UninterruptedNRCB,
        "rectangle",
        Dict{String, Any}(
            "spriteDirectory" => "objects/crushblock",
            "fillColor" => "62222b"
        )
    ),
    "Uninterruptable Non-Return Kevin (Resprite) (Cherry Helper)" => Ahorn.EntityPlacement(
       UninterruptedNRCB,
        "rectangle",
        Dict{String, Any}(
            "spriteDirectory" => "objects/uninterruptedNRK",
            "fillColor" => "242262"
        )
    ),
)


Ahorn.editingOptions(entity::UninterruptedNRCB) = Dict{String, Any}(
    "axes" => Maple.kevin_axes
)

Ahorn.minimumSize(entity::UninterruptedNRCB) = 24, 24
Ahorn.resizable(entity::UninterruptedNRCB) = true, true

Ahorn.selection(entity::UninterruptedNRCB) = Ahorn.getEntityRectangle(entity)

# Todo - Use randomness to decide on Kevin border
function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::UninterruptedNRCB, room::Maple.Room)
    # Backwards compatibility for alt texture toggle
    if (get(entity.data, "altTexture", true))
        restrainedTex = get(entity.data, "spriteDirectory", "objects/uninterruptedNRK")
        fillColor = get(entity.data, "fillColor", "242262")
    else
        restrainedTex = "objects/crushblock"
        fillColor = "62222b"
    end

    kevinColor = Ahorn.argb32ToRGBATuple(parse(Int, fillColor, base=16))[1:3] ./ 255

    frameImage = Dict{String, String}(
        "none" => string(restrainedTex, "/block00"),
        "horizontal" => string(restrainedTex, "/block01"),
        "vertical" => string(restrainedTex, "/block02"),
        "both" => string(restrainedTex, "/block03")
    )
    axes = lowercase(get(entity.data, "axes", "both"))
    chillout = get(entity.data, "chillout", false)
    
    smallFace = string(restrainedTex,"/idle_face")
    giantFace = string(restrainedTex,"/giant_block00")

    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    giant = height >= 48 && width >= 48 && chillout
    face = giant ? giantFace : smallFace
    frame = frameImage[lowercase(axes)]
    faceSprite = Ahorn.getSprite(face, "Gameplay")

    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    Ahorn.drawRectangle(ctx, 2, 2, width - 4, height - 4, kevinColor)
    Ahorn.drawImage(ctx, faceSprite, div(width - faceSprite.width, 2), div(height - faceSprite.height, 2))

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, 0, 8, 0, 8, 8)
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, height - 8, 8, 24, 8, 8)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, 0, (i - 1) * 8, 0, 8, 8, 8)
        Ahorn.drawImage(ctx, frame, width - 8, (i - 1) * 8, 24, 8, 8, 8)
    end

    Ahorn.drawImage(ctx, frame, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, 0, 24, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, 0, height - 8, 0, 24, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, height - 8, 24, 24, 8, 8)
end

end