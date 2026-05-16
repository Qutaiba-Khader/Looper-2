Public Class optionsWindow
    '================== Options not settable by the Options pane, but need to be saved ==================
    Dim options_MPCEXE As String ' this option is not settable from the Options pane, but we need to carry it into the new INI file
    Dim options_autoloadLastLooper As String ' the last looper file to load (this gets set when we quit the program)
    Dim options_inPointOffset As String ' how much more to add to an IN point to more fine-tune the point
    Dim options_outPointOffset As String ' how much more to take off of an OUT point to properly loop it (without skipping to the beginning)
    '================== Legacy options carried over from the old Looper ==================
    Dim options_dockMode As String ' this option as well is not settable (at least not yet), but carry it into the new file for legacy support
    Dim options_startPositionW, options_startPositionH As String ' the width and height of Looper (which can't be set in this version, as the control panel is always the same size)
    Dim options_MPCConfirm As String ' whether or not to ask to re-open MPC-HC when it quits - taking this out, as... I *want* to...

    Private Sub options_loadINIFile()
        If My.Computer.FileSystem.GetFileInfo(INIFile).Exists Then ' if there is an INI file (which by this point in the app, there SHOULD be, but just in case...
            Dim fileReader As String
            fileReader = My.Computer.FileSystem.ReadAllText(INIFile)

            ' This entire next section checks the .INI file for every single settable preference in Looper - if there's an entry in the .INI file
            ' for *any* preference, turn the checkbox on to indicate that we're saving that preference to the file

            '================== Load the [Prefs] section first ==================
            Dim options_loopPreviewLength As String = betweenTheLines(fileReader, "loopPreviewLength=", vbCrLf, "-1") ' how long to preview a slip operation for
            Dim options_loopSlipLength As String = betweenTheLines(fileReader, "loopSlipLength=", vbCrLf, "-1") ' the current slip length
            Dim options_defaultSpeed As String = betweenTheLines(fileReader, "defaultSpeed=", vbCrLf, "-1") ' the default playback speed

            Dim options_newEventName As String = betweenTheLines(fileReader, "newEventName=", vbCrLf, "-1") ' whether or not to show the playlist when Looper opens

            Dim options_hidePlaylistOnLaunch As String = betweenTheLines(fileReader, "hidePlaylistOnLaunch=", vbCrLf, "-1") ' whether or not to show the playlist when Looper opens
            Dim options_alwaysOnTop As String = betweenTheLines(fileReader, "alwaysOnTop=", vbCrLf, "-1") ' whether or not to always be on top from launch
            Dim options_loopButtonMode As String = betweenTheLines(fileReader, "loopButtonMode=", vbCrLf, "-1") ' what the loop button mode should be on launching

            Dim options_disableHotkeys As String = betweenTheLines(fileReader, "disableHotkeys=", vbCrLf, "-1") ' whether or not to disable hotkeys
            Dim options_allowMultipleInstances As String = betweenTheLines(fileReader, "allowMultipleInstances=", vbCrLf, "-1") ' whether or not to allow multiple instances of Looper
            Dim options_disableToolTips As String = betweenTheLines(fileReader, "disableToolTips=", vbCrLf, "-1") ' whether or not to show tooltips on the panels
            Dim options_pausePlaybackOnLoadEvent As String = betweenTheLines(fileReader, "pausePlaybackOnLoadEvent=", vbCrLf, "-1") ' whether to force pause when loading new events instead of playing them
            Dim options_autoPlayDialogs As String = betweenTheLines(fileReader, "autoPlayDialogs=", vbCrLf, "-1") ' whether to start playing as soon as dialog boxes close (text editing maybe?)
            options_autoloadLastLooper = betweenTheLines(fileReader, "autoloadLastLooper=", vbCrLf, Nothing) ' whether or not to show tooltips on the panels

            Dim options_dontForceLooperModeonOpen As String = betweenTheLines(fileReader, "dontForceLooperModeonOpen=", vbCrLf, "-1") ' whether to set Loop in Looper Mode after opening .looper file
            Dim options_autoplayFirstEvent As String = betweenTheLines(fileReader, "autoplayFirstEvent=", vbCrLf, "-1") ' whether to play the first event automatically when opening a .looper file
            Dim options_skipSaveConfirmations As String = betweenTheLines(fileReader, "skipSaveConfirmations=", vbCrLf, "-1") ' whether to skip save confirmation dialogs
            Dim options_skipEventNameEditor As String = betweenTheLines(fileReader, "skipEventNameEditor=", vbCrLf, "-1") ' whether to skip event name editor on add
            Dim options_autoSaveLooper As String = betweenTheLines(fileReader, "autoSaveLooper=", vbCrLf, "-1") ' whether to auto-save looper file on changes
            Dim options_autoCreateEventOnOut As String = betweenTheLines(fileReader, "autoCreateEventOnOut=", vbCrLf, "-1") ' whether to auto-create event on OUT point
            Dim options_osdOnInPoint As String = betweenTheLines(fileReader, "osdOnInPoint=", vbCrLf, "-1")
            Dim options_osdOnOutPoint As String = betweenTheLines(fileReader, "osdOnOutPoint=", vbCrLf, "-1")
            Dim options_osdOnAddEvent As String = betweenTheLines(fileReader, "osdOnAddEvent=", vbCrLf, "-1")
            Dim options_osdOnLoopModeChange As String = betweenTheLines(fileReader, "osdOnLoopModeChange=", vbCrLf, "-1")
            Dim options_osdOnSave As String = betweenTheLines(fileReader, "osdOnSave=", vbCrLf, "-1")
            Dim options_defaultLooperSavePath As String = betweenTheLines(fileReader, "defaultLooperSavePath=", vbCrLf, "-1") ' default save path for .looper files

            '================== Load the [System] section ==================
            options_MPCEXE = betweenTheLines(fileReader, "MPCEXE=", vbCrLf, "0") ' the path to MPC-HC

            options_inPointOffset = betweenTheLines(fileReader, "inPointOffset=", vbCrLf, "-1") ' how much more to add to an IN point to more fine-tune the point
            options_outPointOffset = betweenTheLines(fileReader, "outPointOffset=", vbCrLf, "-1") ' how much more to take off of an OUT point to properly loop it (without skipping to the beginning)
            '================== Load the [StartPos] section ==================
            Dim options_startPositionL As String = betweenTheLines(fileReader, "startPositionL=", vbCrLf, "-1") ' the left-most coordinate to open Looper at

            '================== Legacy options carried over from the old Looper (but keeping here for compatibility) ==================
            options_MPCConfirm = betweenTheLines(fileReader, "MPCConfirm=", vbCrLf, "-1") ' whether to ask to quit Looper when MPC-HC closes
            options_dockMode = betweenTheLines(fileReader, "dockMode=", vbCrLf, "-1") ' which side of MPC-HC that Looper docks to - not used in this version (yet)
            options_startPositionW = betweenTheLines(fileReader, "startPositionW=", vbCrLf, "-1") ' the width of Looper's window (deprecated, but kept for legacy)
            options_startPositionH = betweenTheLines(fileReader, "startPositionH=", vbCrLf, "-1") ' the height of Looper's window (deprecated, but kept for legacy)

            '================== Draw the GUI with the information above ==================
            previewTimeTF.Text = CStr(mainWindow.loopPreviewLength) ' get this value from mainWindow and check against the preference
            slipAmountTF.Text = CStr(mainWindow.loopSlipLength) ' ...

            If mainWindow.defaultSpeed = 100 Then ' if we're set to defaults, then load the current value of the speed slider to save
                defaultSpeedTF.Text = CStr(mainWindow.speedSlider.Value)
            Else ' load the value we currently have saved
                defaultSpeedTF.Text = CStr(mainWindow.defaultSpeed)
            End If

            If options_loopPreviewLength <> "-1" Then savePreviewTimeCB.Checked = True
            If options_loopSlipLength <> "-1" Then saveSlipTimeCB.Checked = True
            If options_defaultSpeed <> "-1" Then saveSpeedCB.Checked = True

            saveNewNameTF.Text = mainWindow.newEventString
            If options_newEventName <> "-1" Then saveNewNameCB.Checked = True

            hidePlaylistWndCB.Visible = False
            savePLWindowSizeCB.Visible = False
            If options_alwaysOnTop <> "-1" Then saveAOTCB.Checked = True
            If options_loopButtonMode <> "-1" Then saveCurrentLoopButtonCB.Checked = True
            If options_startPositionL <> "-1" Then saveLooperWndPosCB.Checked = True
            ' savePLWindowSizeCB removed — single window now saves size via saveLooperWndPosCB

            If options_disableHotkeys <> "-1" Then disableHK.Checked = True
            If options_allowMultipleInstances <> "-1" Then allowMICB.Checked = True
            If options_disableToolTips <> "-1" Then disableTTCB.Checked = True
            If options_pausePlaybackOnLoadEvent <> "-1" Then forcePauseCB.Checked = True
            If options_autoPlayDialogs <> "-1" Then disableAutoPlayCB.Checked = True
            If options_autoloadLastLooper <> Nothing Then autoloadCB.Checked = True

            If options_dontForceLooperModeonOpen <> "-1" Then keepModeCB.Checked = True
            If options_autoplayFirstEvent <> "-1" Then disableAutoPlayOnLoadCB.Checked = True
            If options_skipSaveConfirmations <> "-1" Then skipSaveConfirmCB.Checked = True
            If options_skipEventNameEditor <> "-1" Then skipEventNameCB.Checked = True
            If options_autoSaveLooper <> "-1" Then autoSaveCB.Checked = True
            If options_autoCreateEventOnOut <> "-1" Then autoCreateEventCB.Checked = True
            If options_osdOnInPoint <> "-1" Then osdInPointCB.Checked = True
            If options_osdOnOutPoint <> "-1" Then osdOutPointCB.Checked = True
            If options_osdOnAddEvent <> "-1" Then osdAddEventCB.Checked = True
            If options_osdOnLoopModeChange <> "-1" Then osdLoopModeCB.Checked = True
            If options_osdOnSave <> "-1" Then osdSaveCB.Checked = True
            If options_defaultLooperSavePath <> "-1" Then
                defaultSavePathTF.Text = options_defaultLooperSavePath
                saveDefaultPathCB.Checked = True
            End If
        End If
    End Sub

    Private Sub options_saveINIFile()
        Dim writingString As String = Nothing

        ' Check all numerical values - if they don't convert properly, use the default values
        Double.TryParse(previewTimeTF.Text, mainWindow.loopPreviewLength) ' change the current preview length to the length you just set
        If mainWindow.loopPreviewLength = 0 Then mainWindow.loopPreviewLength = 0.25 ' if the conversion didn't work, use the default value

        Double.TryParse(slipAmountTF.Text, mainWindow.loopSlipLength) ' change the current slip length to the length you just set
        If mainWindow.loopSlipLength = 0 Then mainWindow.loopSlipLength = 0.5 ' ...

        Integer.TryParse(defaultSpeedTF.Text, mainWindow.defaultSpeed) ' change the default playback speed to the speed you just set
        If mainWindow.defaultSpeed = 0 Then mainWindow.defaultSpeed = 100 ' ...

        mainWindow.newEventString = saveNewNameTF.Text ' change the New Event String to the new string

        '================== Make the [Prefs] section of the INI ==================
        If savePreviewTimeCB.Checked Or saveSlipTimeCB.Checked Or saveCurrentLoopButtonCB.Checked Or saveAOTCB.Checked Or
        forcePauseCB.Checked Or keepModeCB.Checked Or disableTTCB.Checked Or autoloadCB.Checked Or allowMICB.Checked Or
        disableAutoPlayCB.Checked Or disableAutoPlayOnLoadCB.Checked Or saveSpeedCB.Checked Or
        saveNewNameCB.Checked Or skipSaveConfirmCB.Checked Or skipEventNameCB.Checked Or autoSaveCB.Checked Or autoCreateEventCB.Checked Or
        osdInPointCB.Checked Or osdOutPointCB.Checked Or osdAddEventCB.Checked Or osdLoopModeCB.Checked Or osdSaveCB.Checked Or saveDefaultPathCB.Checked Or
        options_MPCConfirm <> "-1" Or options_dockMode <> "-1" Then
            writingString = "[Prefs]" & vbCrLf ' write the [Prefs] header of the INI file
        End If

        ' Once we write the [Prefs] header, then set these various options...
        If options_MPCConfirm <> "-1" Then writingString = writingString & "MPCConfirm=1" & vbCrLf ' add this back in, for legacy reasons
        If options_dockMode <> "-1" Then writingString = writingString & "dockMode=" & options_dockMode & vbCrLf ' write the docking mode, for legacy reasons

        ' Save the options from the Options panel
        If savePreviewTimeCB.Checked Then ' if the value is not the default value, write it to the INI file
            If mainWindow.loopPreviewLength <> 0.25 Then writingString = writingString & "loopPreviewLength=" & mainWindow.loopPreviewLength.ToString() & vbCrLf
        Else
            mainWindow.loopPreviewLength = 0.25 ' if we're not set to save this preference, set this back to defaults
        End If

        If saveSlipTimeCB.Checked Then
            If mainWindow.loopSlipLength <> 0.5 Then writingString = writingString & "loopSlipLength=" & mainWindow.loopSlipLength.ToString() & vbCrLf
        Else
            mainWindow.loopSlipLength = 0.5 ' ...
        End If

        If saveSpeedCB.Checked Then
            If mainWindow.defaultSpeed <> 100 Then writingString = writingString & "defaultSpeed=" & mainWindow.defaultSpeed.ToString() & vbCrLf
        Else
            mainWindow.defaultSpeed = 100 ' ...
        End If

        mainWindow.setSpeed(mainWindow.defaultSpeed) ' reset speed to current default
        mainWindow.SendMessage(CMD_SEND.CMD_PAUSE) ' don't automatically begin playback after adjusting speed

        If saveNewNameCB.Checked Then writingString = writingString & "newEventName=" & saveNewNameTF.Text & vbCrLf


        If saveAOTCB.Checked Then
            If mainWindow.TopMost Then
                writingString = writingString & "alwaysOnTop=1" & vbCrLf ' we want to boot with the window always on the top of other windows
            Else
                writingString = writingString & "alwaysOnTop=0" & vbCrLf ' we want to boot with the window NOT always on top
            End If
        End If

        If saveCurrentLoopButtonCB.Checked Then writingString = writingString & "loopButtonMode=" & mainWindow.loopModeButton.Text & vbCrLf

        If disableHK.Checked Then
            writingString = writingString & "disableHotkeys=1" & vbCrLf
            mainWindow.disableHotkeys = True
        Else
            mainWindow.disableHotkeys = False
        End If

        If allowMICB.Checked Then writingString = writingString & "allowMultipleInstances=1" & vbCrLf

        If disableTTCB.Checked Then
            writingString = writingString & "disableToolTips=1" & vbCrLf
            loadToolTips(False) ' if we choose to disable tool tips, then disable them now too
        Else
            loadToolTips(True) ' if we choose not to disable tool tips, then re-enable them (basically just re-set them)
        End If

        If forcePauseCB.Checked Then
            writingString = writingString & "pausePlaybackOnLoadEvent=1" & vbCrLf
            pausePlaybackOnLoadEvent = True
        Else
            pausePlaybackOnLoadEvent = False
        End If

        If disableAutoPlayCB.Checked Then
            writingString = writingString & "autoPlayDialogs=0" & vbCrLf
            mainWindow.autoPlayDialogs = False
        Else
            mainWindow.autoPlayDialogs = True
        End If

        If keepModeCB.Checked Then
            writingString = writingString & "dontForceLooperModeonOpen=1" & vbCrLf
            mainWindow.dontForceLooperModeonOpen = True
        Else
            mainWindow.dontForceLooperModeonOpen = False
        End If

        If disableAutoPlayOnLoadCB.Checked Then
            writingString = writingString & "autoplayFirstEvent=0" & vbCrLf
            mainWindow.autoplayFirstEvent = False
        Else
            mainWindow.autoplayFirstEvent = True
        End If

        If skipSaveConfirmCB.Checked Then
            writingString = writingString & "skipSaveConfirmations=1" & vbCrLf
            skipSaveConfirmations = True
        Else
            skipSaveConfirmations = False
        End If

        If skipEventNameCB.Checked Then
            writingString = writingString & "skipEventNameEditor=1" & vbCrLf
            skipEventNameEditor = True
        Else
            skipEventNameEditor = False
        End If

        If autoSaveCB.Checked Then
            writingString = writingString & "autoSaveLooper=1" & vbCrLf
            autoSaveLooper = True
        Else
            autoSaveLooper = False
        End If

        If autoCreateEventCB.Checked Then
            writingString = writingString & "autoCreateEventOnOut=1" & vbCrLf
            autoCreateEventOnOut = True
        Else
            autoCreateEventOnOut = False
        End If

        If osdInPointCB.Checked Then
            writingString = writingString & "osdOnInPoint=1" & vbCrLf
            osdOnInPoint = True
        Else
            osdOnInPoint = False
        End If

        If osdOutPointCB.Checked Then
            writingString = writingString & "osdOnOutPoint=1" & vbCrLf
            osdOnOutPoint = True
        Else
            osdOnOutPoint = False
        End If

        If osdAddEventCB.Checked Then
            writingString = writingString & "osdOnAddEvent=1" & vbCrLf
            osdOnAddEvent = True
        Else
            osdOnAddEvent = False
        End If

        If osdLoopModeCB.Checked Then
            writingString = writingString & "osdOnLoopModeChange=1" & vbCrLf
            osdOnLoopModeChange = True
        Else
            osdOnLoopModeChange = False
        End If

        If osdSaveCB.Checked Then
            writingString = writingString & "osdOnSave=1" & vbCrLf
            osdOnSave = True
        Else
            osdOnSave = False
        End If

        If saveDefaultPathCB.Checked And defaultSavePathTF.Text <> "" Then
            writingString = writingString & "defaultLooperSavePath=" & defaultSavePathTF.Text & vbCrLf
            defaultLooperSavePath = Environment.ExpandEnvironmentVariables(defaultSavePathTF.Text)
        Else
            defaultLooperSavePath = Nothing
        End If

        If autoloadCB.Checked Then
            If options_autoloadLastLooper = Nothing Then
                writingString = writingString & "autoloadLastLooper=__FILE__" & vbCrLf
            Else
                writingString = writingString & "autoloadLastLooper=" & options_autoloadLastLooper & vbCrLf
            End If
        End If

        '================== Make the [System] section of the INI ==================
        writingString = writingString & "[System]" & vbCrLf & "MPCEXE=" & options_MPCEXE & vbCrLf

        If options_inPointOffset <> "-1" Then writingString = writingString & "inPointOffset=" & options_inPointOffset & vbCrLf
        If options_outPointOffset <> "-1" Then writingString = writingString & "outPointOffset=" & options_outPointOffset & vbCrLf

        '================== Make the [StartPos] section of the INI ==================
        If saveLooperWndPosCB.Checked Then
            writingString = writingString & "[StartPos]" & vbCrLf
            writingString = writingString & "startPositionL=" & mainWindow.Left & vbCrLf
            writingString = writingString & "startPositionT=" & mainWindow.Top & vbCrLf
            writingString = writingString & "startPositionW=" & mainWindow.Width & vbCrLf
            writingString = writingString & "startPositionH=" & mainWindow.Height & vbCrLf
        End If

        '================== Make the [HotKeys] section of the INI ==================
        Dim headerWritten As Boolean = False

        For currentHK = 0 To UBound(mainWindow.hotKeyList)
            If Not mainWindow.hotKeyList(currentHK, 1) = Nothing OrElse hotKeyDisabled(currentHK) Then
                If Not headerWritten Then
                    writingString = writingString & "[HotKeys]" & vbCrLf
                    headerWritten = True
                End If

                If Not mainWindow.hotKeyList(currentHK, 1) = Nothing Then
                    writingString = writingString & (currentHK + 100) & "=" & mainWindow.hotKeyList(currentHK, 1) & vbCrLf
                End If

                If hotKeyDisabled(currentHK) Then
                    writingString = writingString & "D" & (currentHK + 100) & "=1" & vbCrLf
                End If
            End If
        Next

        System.IO.File.WriteAllText(INIFile, writingString)

        MessageBox.Show(Me, "Preferences saved.",
                        "Saved",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)

        Me.Close()
    End Sub

    Private Sub optionsWindow_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Me.Icon = My.Resources.icon
        currentVersionTF.Text = My.Application.Info.Version.ToString ' list the current build date in the Options window
        mainWindow.SendMessage(CMD_SEND.CMD_PAUSE)
        options_loadINIFile()
    End Sub

    Private Sub savePrefsButton_Click(sender As Object, e As EventArgs) Handles savePrefsButton.Click
        options_saveINIFile()
        If mainWindow.autoPlayDialogs = True Then mainWindow.SendMessage(CMD_SEND.CMD_PLAY) ' we saved our preferences, so start playing again
    End Sub

    Private Sub optionsWindow_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Me.Dispose()
    End Sub

    Private Sub customHotKeysButton_Click(sender As Object, e As EventArgs) Handles customHotKeysButton.Click
        hotKeySettings.ShowDialog()
        Me.DialogResult = DialogResult.None
    End Sub

    Private Sub cancelPrefsButton_Click(sender As Object, e As EventArgs) Handles cancelPrefsButton.Click
        Me.Close()
        If mainWindow.autoPlayDialogs = True Then mainWindow.SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled out of the Options dialog, so start playing again
    End Sub

    Private Sub browseSavePathButton_Click(sender As Object, e As EventArgs) Handles browseSavePathButton.Click
        Using folderDialog As New FolderBrowserDialog With {
            .Description = "Select default folder for saving .looper files",
            .ShowNewFolderButton = True
        }
            If defaultSavePathTF.Text <> "" Then
                folderDialog.SelectedPath = defaultSavePathTF.Text
            End If

            If folderDialog.ShowDialog() = DialogResult.OK Then
                defaultSavePathTF.Text = folderDialog.SelectedPath
                saveDefaultPathCB.Checked = True
            End If
        End Using
    End Sub
End Class