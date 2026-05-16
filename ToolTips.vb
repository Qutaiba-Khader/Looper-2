Module ToolTips
    Public Sub loadToolTips(ByVal loadToolTips As Boolean)
        If loadToolTips = True Then
            mainWindow.toolTips.SetToolTip(mainWindow.loopModeButton, "Change the loop mode from Off, Loop, Playlist and Shuffle" &
                               vbCrLf & vbCrLf & "Shortcut Keys -" & vbCrLf &
                               "Ctrl-L : Switch modes manually" & vbCrLf &
                               "Ctrl-1 : Directly go to OFF Mode" & vbCrLf &
                               "Ctrl-2 : Directly go to Loop Mode" & vbCrLf &
                               "Ctrl-3 : Directly go to Playlist Mode (you must have at least 2 events in the Playlist)" & vbCrLf &
                               "Ctrl-4 : Directly go to Shuffle Mode (you must have at least 2 events in the Playlist)")

            mainWindow.toolTips.SetToolTip(mainWindow.inTF, "Shows the current event's IN point (A point of the A/B loop)")

            mainWindow.toolTips.SetToolTip(mainWindow.inPointSlipLeftButton, "Slip the IN point back" &
                               vbCrLf & vbCrLf & "Shortcut Keys -" & vbCrLf &
                               "[ : Slip IN Point back " & mainWindow.loopSlipLength & " second(s)" & vbCrLf &
                               "Shift-[ : Slip IN Point back 1/20th of a second")

            mainWindow.toolTips.SetToolTip(mainWindow.inPointButton, "Set the IN point to the current MPC-HC/BE position" &
                               vbCrLf & vbCrLf & "Shortcut Key: I")

            mainWindow.toolTips.SetToolTip(mainWindow.inPointSlipRightButton, "Slip the IN point forward" &
                               vbCrLf & vbCrLf & "Shortcut Keys -" & vbCrLf &
                               "] : Slip IN Point forward " & mainWindow.loopSlipLength & " second(s)" & vbCrLf &
                               "Shift-] : Slip IN Point forward 1/20th of a second")

            mainWindow.toolTips.SetToolTip(mainWindow.alwaysOnTopButton, "Toggle Always on Top" &
                               vbCrLf & vbCrLf & "Shortcut Key: Ctrl-T")

            mainWindow.toolTips.SetToolTip(mainWindow.outTF, "Shows the current event's OUT point (B point of the A/B loop)")

            mainWindow.toolTips.SetToolTip(mainWindow.outPointSlipLeftButton, "Slip the OUT point back" &
                               vbCrLf & vbCrLf & "Shortcut Keys -" & vbCrLf &
                               "; : Slip OUT Point back " & mainWindow.loopSlipLength & " second(s)" & vbCrLf &
                               "Shift-; : Slip OUT Point back 1/20th of a second")

            mainWindow.toolTips.SetToolTip(mainWindow.outPointButton, "Set the OUT point to the current MPC-HC/BE position" &
                               vbCrLf & vbCrLf & "Shortcut Key: O")

            mainWindow.toolTips.SetToolTip(mainWindow.outPointSlipRightButton, "Slip the OUT point forward" &
                               vbCrLf & vbCrLf & "Shortcut Keys -" & vbCrLf &
                               "' : Slip OUT Point forward " & mainWindow.loopSlipLength & " second(s)" & vbCrLf &
                               "Shift-' : Slip OUT Point forward 1/20th of a second")

            mainWindow.toolTips.SetToolTip(mainWindow.speedDownButton, "Slow playback down by 5%" & vbCrLf & "Shortcut: Ctrl-Down")
            mainWindow.toolTips.SetToolTip(mainWindow.speedUpButton, "Speed playback up by 5%" & vbCrLf & "Shortcut: Ctrl-Up")
            mainWindow.toolTips.SetToolTip(mainWindow.speedValueLabel, "Current playback speed" & vbCrLf & "Shortcut: Ctrl-R to reset to 100%")

            mainWindow.toolTips.SetToolTip(mainWindow.loadOptionsWindowButton, "Options" & vbCrLf & "Shortcut: Ctrl-,")
            mainWindow.toolTips.SetToolTip(mainWindow.showPlayingFileInExplorerButton, "Load .looper file (left click) or import (right click)" &
                            vbCrLf & "Shortcut: Ctrl-L / Shift-Ctrl-L")

            mainWindow.toolTips.SetToolTip(mainWindow.prevEventButton, "Previous event" & vbCrLf & "Shortcut: Ctrl-Page Up")
            mainWindow.toolTips.SetToolTip(mainWindow.nextEventButton, "Next event" & vbCrLf & "Shortcut: Ctrl-Page Down")
            mainWindow.toolTips.SetToolTip(mainWindow.saveButton, "Save .looper file" & vbCrLf & "Shortcut: Ctrl-S")
            mainWindow.toolTips.SetToolTip(mainWindow.deleteEventButton, "Delete selected events" & vbCrLf & "Shortcut: Delete")
            mainWindow.toolTips.SetToolTip(mainWindow.modifyEventButton, "Modify selected event with current IN/OUT/Speed")
            mainWindow.toolTips.SetToolTip(mainWindow.addEventButton, "Add new event" & vbCrLf & "Shortcut: Ctrl-N")
            mainWindow.toolTips.SetToolTip(mainWindow.searchTF, "Filter events by name, file, or path")
            mainWindow.toolTips.SetToolTip(mainWindow.fixButton, "Fix broken file paths using Everything search")
            mainWindow.toolTips.SetToolTip(mainWindow.fixToolButton, "Launch the standalone Looper Fix Tool")

            mainWindow.toolTips.SetToolTip(mainWindow.eventsList, "Events list — double-click to load an event" &
                                               vbCrLf & vbCrLf & "Shortcut Keys -" & vbCrLf &
                                               "F2 - Edit event name" & vbCrLf &
                                               "F3 - Edit repeat count" & vbCrLf &
                                               "Ctrl-A - Select all" & vbCrLf &
                                               "Shift-Ctrl-D - Deselect all")

            mainWindow.toolTips.Active = True
        Else
            mainWindow.toolTips.Active = False
        End If
    End Sub
End Module
