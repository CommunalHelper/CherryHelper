local entityToggleField = {}

entityToggleField.name = "CherryHelper/NightItemLockfield"
entityToggleField.fillColor = {0.4, 0.4, 0.4, 0.4}
entityToggleField.borderColor = {0.4, 0.4, 0.4, 1.0}
entityToggleField.placements = {
    {
        name = "normal",
        data = {
            ItemsToSteal = "ZipMover",
            nightId = "",
            startAppeared = false,
            AssistRectangleOnSolid = false,
            width = 16,
            height = 16
        }
    }
}

return entityToggleField