Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

' MPC-HC Looper VB - Looper re-written in Visual Basic
' (c) 2014-2025 Zach Glenwright / Gull's Wing Media Productions
' http://www.gullswingmedia.com

' ======================================================================================
' ================= THINGS TO DO =======================================================
' ======================================================================================
' ----------------- MOST IMPORTANT: -----------------
'
' ----------------- THINGS TO LOOK INTO: -----------------
' - Figure out some kind of speed offset for setting the IN point, based on the current speed (slower = less offset)
' - Make "autosave looper" a preference, and also "keep original .looper file" backup stuff
' - See if Pinging a server will speed things up for loading a list (so inactive servers won't slow the thing down)
' - Write a Trimmer sub-window for Looper that lets you trim .looper files down

Public Class mainWindow
    Public newEventString As String = "New Loop Event" ' the name for new events (by default, just "New Loop Event" as it was in older versions of Looper)

    Dim currentPosition As Double ' the current position of MPC-HC
    Public currentDuration As Double ' the current duration of the file in MPC-HC
    Public currentPlayingFile As String ' the current file playing in MPC-HC

    Public clearINOUTPoint As Boolean = True ' whether or not to clear IN and OUT when loading a new file

    Dim currentOrRemaining As Boolean ' whether or not we're looking at current or remaining time

    ReadOnly RNG As New Random()
    Public randomNumberList(0) As Integer ' array of randomized item events

    ' INI file values stored for later use
    Private inPointOffset As Double = 0 ' how much more to add to an IN point to more fine-tune the point
    Private outPointOffset As Double = 0.15 ' how much more to take off of an OUT point to properly loop it (without skipping to the beginning)

    Public loopSlipLength As Double = 0.5 ' how long a loop slips when you click the slip buttons
    Public loopPreviewLength As Double = 0.25 ' how long a loop previews for before going back to the loop
    Public defaultSpeed As Integer = 100 ' the default playback speed

    Public autoPlayDialogs As Boolean = True ' whether to start playing as soon as dialog boxes close (text editing maybe?)

    Public autoloadLastLooper As String ' whether to not to automatically load the last looper from close
    Public autoplayFirstEvent As Boolean = True ' set this to True to allow auto-playing the first event when opening a .looper file
    Public dontForceLooperModeonOpen As Boolean = False ' whether to set Loop in Looper Mode after opening .looper file

    ' Threads for background work
    Public posThread, frontThread As Thread ' threads to check on MPC-HC position and hotkeys

    ' Handles to... handle inside of the threads above ^^
    Public MPCHandle As IntPtr ' MPC-HC's hWnd
    Private activeHwnd As IntPtr ' the currently active window
    Private myHandle As IntPtr ' the current hWnd for me

    ' Working with hotkeys
    Public disableHotkeys As Boolean = False ' whether or not to disable hotkeys
    Public hotkeysActive As Boolean ' whether hotkeys are active currently or not

    Public isQuitting As Boolean = False ' if we are in the process of quitting or not

    Dim waitForSecondTick As Integer = -1 ' wait for the 2nd response from MPC-HC to change the speed

    Public loadingEvent As Boolean = False ' if we're in the process of loading an event (for the NOWPLAYING function)
    Public loadingEvent_Speed As Integer = 100

    Private slipAction As Integer = 0 ' the current slip action to undertake - 1/2 - slip IN left/right 3/4 - slip OUT left/right

    ' CUSTOM HOTKEY PARAMS: description of shortcut (0), default setting modifier (1), key (2), current setting modifier (3), current setting (4)
    Public hotKeyList(,) As String = {{"|73", Nothing, "Set In Point"},
                                      {"|79", Nothing, "Set Out Point"},
                                      {"C|73", Nothing, "Clear In Point"},
                                      {"C|79", Nothing, "Clear Out Point"},
                                      {"C|103", Nothing, "Clear IN and OUT Points"},
                                      {"|219", Nothing, "Trim IN Point Left"},
                                      {"S|219", Nothing, "Trim IN Point Left by default value"},
                                      {"|221", Nothing, "Trim IN Point Right"},
                                      {"S|221", Nothing, "Trim IN Point Right by default value"},
                                      {"|186", Nothing, "Trim OUT Point Left"},
                                      {"S|186", Nothing, "Trim OUT Point Left by default value"},
                                      {"|222", Nothing, "Trim OUT Point Right"},
                                      {"S|222", Nothing, "Trim OUT Point Right by default value"},
                                      {"S|76", Nothing, "Change Loop Mode"},
                                      {"C|49", Nothing, "Change Loop Mode to OFF"},
                                      {"C|50", Nothing, "Change Loop Mode to LOOP MODE"},
                                      {"C|51", Nothing, "Change Loop Mode to PLAYLIST MODE"},
                                      {"C|52", Nothing, "Change Loop Mode to SHUFFLE MODE"},
                                      {"C|84", Nothing, "Change Always on Top seting"},
                                      {"C|81", Nothing, "Quit MPC-HC/BE Looper"},
                                      {"C|188", Nothing, "Load Options Pane"},
                                      {"A|45", Nothing, "Open Path to Current File in Explorer"},
                                      {"C|78", Nothing, "Create New Event"},
                                      {"|46", Nothing, "Delete currently selected Event(s)"},
                                      {"C|83", Nothing, "Save current Events List (overwrite)"},
                                      {"CS|83", Nothing, "Save the Selected Events to new Looper file"},
                                      {"C|76", Nothing, "Load .looper file"},
                                      {"CS|76", Nothing, "Import .looper into Current Events List"},
                                      {"C|33", Nothing, "Go one event back in Events List"},
                                      {"C|34", Nothing, "Go one event forward in Events List"},
                                      {"C|38", Nothing, "Speed Up playback"},
                                      {"C|40", Nothing, "Slow down playback"},
                                      {"C|82", Nothing, "Reset to default speed"},
                                      {"C|70", Nothing, "Go to the Search Field of the Playlist window"},
                                      {"C|65", Nothing, "Select all events in Events List"},
                                      {"CS|68", Nothing, "De-Select all events in Events List"}}

    ' ======================================================================================
    ' ================= PLAYLIST WINDOW VARIABLES (merged from playlistWindow) ==============
    ' ======================================================================================

    Public currentLooperFile As String = Nothing ' the current .looper file loaded in the playlist

    Public currentPlayingEvent As Integer = -1 ' which event is currently playing
    Public actualPlayingEvent As Integer = -1 ' the actual event that's playing (keeping track for getting out of Shuffle Mode)

    Dim totalNumberOfEvents As Integer = 100 ' the number to start ordering events from

    Dim isModified As Boolean = False ' whether or not we've modified the current looper playlist
    Dim insertItem As Boolean = False ' whether or not to allow inserting items into the events list
    Dim SelectedLSI As ListViewItem.ListViewSubItem ' for modifying event list names and loops

    Public stilLLoading As Boolean = False ' set this to True when doing loadEvent() so the program doesn't ask to load multiple things at once

    Dim inSearchMode As Boolean = False ' if we're in search mode or not
    Dim currentSearchPlayingEvent As Integer = -1 ' the ID# (from (0) of the event in the events list) of the current playing event's position in the playlist
    Dim eventsListArray(0, 0) As Object ' an array to hold the events list items during searching

    Public normalFont As New Font("Segoe UI", 9) ' the normal "non-playing" font for the events list
    Public boldFont As New Font("Segoe UI", 9, FontStyle.Bold) ' the bold "playing" font for the events list

    ReadOnly errorColor As Color = Color.FromArgb(58, 26, 26)
    ReadOnly foundColor As Color = Color.FromArgb(30, 30, 48)
    ReadOnly relativeColor As Color = Color.FromArgb(26, 42, 58)
    ReadOnly errorForeColor As Color = Color.FromArgb(255, 136, 136)
    ReadOnly errorFileCellColor As Color = Color.FromArgb(255, 102, 102)
    ReadOnly playingColor As Color = Color.FromArgb(26, 58, 42)
    ReadOnly playingForeColor As Color = Color.FromArgb(136, 255, 136)
    ReadOnly normalForeColor As Color = Color.White

    ' ======================================================================================
    ' ================= DLL CALLS AND STRUCTURES ===========================================
    ' ======================================================================================

    Public Structure CopyData
        Public dwData As IntPtr
        Public cbData As Integer
        Public lpData As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Public Structure MPC_OSDDATA
        Public nMsgPos As Integer
        Public nDurationMS As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public strMsg As String
    End Structure

    ' Returns the current foreground window (to check to see whether hotkeys should be enabled or not)
    Private Declare Function GetForegroundWindow Lib "user32" () As IntPtr

    ' Set specific window (MPC-HC) as the frontmost window
    Public Declare Function SetForegroundWindow Lib "user32" (ByVal hWnd As IntPtr) As Integer

    ' Registers a hotkey with the system
    Private Declare Function RegisterHotKey Lib "user32" _
        (ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer

    ' Unregisters a hotkey with the system
    Private Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) As Integer

    ' Sends a WM_COPYDATA request to MPC-HC to allow controlling the player or to another Looper instance (if we're running a single-instance-only session)
    Private Declare Auto Function SendMessageA Lib "user32" _
        (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByRef lParam As CopyData) As Boolean

    ' Find a window with a specific title (for finding old Looper instances for the above ^^^ call to the other instance)
    Private Declare Auto Function FindWindow Lib "user32" _
        (ByVal ClassName As String, ByVal WindowTitle As String) As IntPtr

    Const WM_HOTKEY As Integer = &H312 ' if the message received is a hotkey message
    Const WM_COPYDATA As Integer = &H4A ' if the message received is a WM_COPYDATA message

    Public Function SendMessage(ByVal cmd As Integer, Optional msg As String = "", Optional customHwnd As Boolean = False, Optional toHwnd As IntPtr = Nothing) As Boolean
        Dim DataStruct As New CopyData With {
            .dwData = CType(cmd, IntPtr),
            .cbData = (msg.Length + 1) * Marshal.SystemDefaultCharSize,
            .lpData = Marshal.StringToHGlobalUni(msg)
        }

        Dim returnValue As Boolean

        If customHwnd = True Then
            returnValue = SendMessageA(toHwnd, WM_COPYDATA, myHandle, DataStruct) ' SendMessage to a custom window (another Looper instance)
        Else
            returnValue = SendMessageA(MPCHandle, WM_COPYDATA, myHandle, DataStruct) ' SendMessage to MPC-HC
        End If

        Marshal.FreeHGlobal(DataStruct.lpData)

        DataStruct = Nothing
        Return returnValue
    End Function

    Public Sub SendOSD(msg As String, Optional position As Integer = 1, Optional durationMs As Integer = 1500)
        Dim osdData As New MPC_OSDDATA With {
            .nMsgPos = position,
            .nDurationMS = durationMs,
            .strMsg = msg
        }

        Dim pOsdData As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(osdData))
        Marshal.StructureToPtr(osdData, pOsdData, False)

        Dim cds As New CopyData With {
            .dwData = CType(CMD_SEND.CMD_OSDSHOWMESSAGE, IntPtr),
            .cbData = Marshal.SizeOf(osdData),
            .lpData = pOsdData
        }

        SendMessageA(MPCHandle, WM_COPYDATA, myHandle, cds)
        Marshal.FreeHGlobal(pOsdData)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Select Case m.Msg
            Case WM_HOTKEY ' if the message received is a hotkey event
                handleHotKeyEvent(m.WParam.ToInt32)
            Case WM_COPYDATA ' if the message received is from MPC-HC or another Looper session
                Dim CopyDataStruct As CopyData = DirectCast(Marshal.PtrToStructure(m.LParam, GetType(CopyData)), CopyData)
                Dim theMessage As String = Marshal.PtrToStringUni(CopyDataStruct.lpData)

                Select Case CopyDataStruct.dwData
                    Case CType(&H5000F634, IntPtr) ' recieving a message from another Looper instance (so we don't need a temp file like we've been doing)
                        loadLooperFile(True, theMessage)
                    Case CType(CMD_RECEIVED.CMD_CONNECT, IntPtr) ' received when MPC-HC connects to Looper for the first time
                        MPCHandle = m.WParam ' the current hWnd of the current MPC-HC instance
                        Log("MPC-HC connected, hWnd=" & MPCHandle.ToString())

                        clearInPoint()
                        clearOutPoint()
                        SetForegroundWindow(MPCHandle) ' make MPC-HC the foreground window

                        If My.Application.CommandLineArgs.Count > 0 Then ' if we gave Looper a .looper file to open, then open it
                            Dim pathToOpen = My.Application.CommandLineArgs(0)

                            If Strings.InStr(pathToOpen, ".looper") <> 0 Then ' if the file we passed to Looper has a .looper extension, then open it as a Looper file
                                loadLooperFile(True, pathToOpen)
                            End If
                        Else ' if we didn't, but we're set up to open the last file from the INI file, then open that...
                            If autoloadLastLooper <> Nothing Then
                                Dim autoloadArray() As String = Split(autoloadLastLooper, "|")

                                If autoloadArray(0) <> Nothing Then ' if there's a filename in the array instead of Nothing
                                    If autoloadArray(1) <> "-1" Then ' and there's an event number that's not -1
                                        loadLooperFile(True, autoloadArray(0), CInt(autoloadArray(1))) ' load the looper file, at the event specified
                                    Else
                                        loadLooperFile(True, autoloadArray(0)) ' load the looper file at the beginning
                                    End If
                                End If
                            End If
                        End If
                    Case CType(CMD_RECEIVED.CMD_CURRENTPOSITION, IntPtr) ' received when MPC-HC gets a request to update the current media position
                        Double.TryParse(theMessage, currentPosition)
                        currentPosition += 0.5 ' the position MPC-HC returns is actually -0.5 (half a second) behind what the actual position is

                        If currentOrRemaining = False Then ' show the current position
                            If currentPositionLabel.Text <> NumberToTimeString(currentPosition) Then
                                currentPositionLabel.Text = NumberToTimeString(currentPosition)
                            End If
                        Else ' show the time remaining in the current event/loop
                            If TimeStringToNumber(outTF.Text) - currentPosition > 0 Then
                                If currentPositionLabel.Text <> "-" & NumberToTimeString(TimeStringToNumber(outTF.Text) - currentPosition) Then
                                    currentPositionLabel.Text = "-" & NumberToTimeString(TimeStringToNumber(outTF.Text) - currentPosition)
                                End If
                            Else
                                If currentPositionLabel.Text <> "-0:00.000" Then
                                    currentPositionLabel.Text = "-0:00.000"
                                End If
                            End If
                        End If

                        ' ----------------- DO THE LOOP DO THE LOOP DO THE LOOP! -----------------
                        Select Case loopModeButton.Text
                            Case "LOOP" ' if we're in LOOP MODE
                                If Not {outTF.Text, inTF.Text}.Contains(Nothing) Then ' if both the IN and OUT point have values
                                    If currentPosition > TimeStringToNumber(outTF.Text) Then ' and the current position reported is past the value in the OUT point
                                        SendMessage(CMD_SEND.CMD_SETPOSITION, CStr(TimeStringToNumber(inTF.Text) - 0.5)) ' move MPC-HC back to the IN point
                                    End If
                                End If
                            Case "PLAYLIST", "SHUFFLE" ' if we're in PLAYLIST or SHUFFLE modes
                                If Not {outTF.Text, inTF.Text}.Contains(Nothing) Then
                                    If outTF.Text <> Nothing Then ' if the OUT point field has a value set in it
                                        If currentPosition > TimeStringToNumber(outTF.Text) Then
                                            Dim repeatCount As Double
                                            Double.TryParse(loopsRepeatTF.Text, repeatCount)

                                            If repeatCount <> 1 Then ' if we still have more loop repetitions to do, then jump back and decrement that counter
                                                SendMessage(CMD_SEND.CMD_SETPOSITION, CStr(TimeStringToNumber(inTF.Text) - 0.5))
                                                loopsRepeatTF.Text = CStr(repeatCount - 1)
                                            Else ' we've reached the last loop repeat, so jump to the next event
                                                If eventsList.Items.Count > 1 Then
                                                    loadPrevNextEvent(1) ' load the next event
                                                Else ' if we have only one event, then we're basically in loop mode, so just treat it that way!
                                                    SendMessage(CMD_SEND.CMD_SETPOSITION, CStr(TimeStringToNumber(inTF.Text) - 0.5))
                                                End If
                                            End If
                                        End If
                                    Else ' if the OUT point has no value set in it, then...
                                        SendMessage(CMD_SEND.CMD_GETNOWPLAYING) ' force updating the NOWPLAYING info to get the duration
                                        outTF.Text = NumberToTimeString(currentDuration + 0.5 - outPointOffset) ' set the OUT point to the duration
                                        modifyEvent(currentPlayingEvent) ' and modify the current event to reflect that duration
                                    End If
                                End If
                        End Select
                    Case CType(CMD_RECEIVED.CMD_NOWPLAYING, IntPtr) ' received when MPC-HC gets a request to update the current file + duration info
                        Dim nowInfoArray() As String = Split(theMessage, "|")

                        If currentPlayingFile <> nowInfoArray(UBound(nowInfoArray) - 1) Then ' if the filenames don't match, then change the current information
                            currentPlayingFile = nowInfoArray(UBound(nowInfoArray) - 1)
                            Double.TryParse(nowInfoArray(UBound(nowInfoArray)), currentDuration)
                            Log("Now playing: " & currentPlayingFile)
                        End If

                        ' if we're loading an event from a file, and it first recieved the NOWPLAYING message, then set the event up NOW
                        If loadingEvent = True Then
                            SendMessage(CMD_SEND.CMD_SETPOSITION, CStr(TimeStringToNumber(inTF.Text) - 0.5))
                            setSpeed(loadingEvent_Speed)

                            If pausePlaybackOnLoadEvent Then
                                SendMessage(CMD_SEND.CMD_PAUSE) ' pause the playback if you have the preference set to do that
                            End If

                            ' if the OUT point is set as "0" or "0:00" - the Cstr() function sets that to 0 - it gets the current duration (this is
                            ' for files that are added to the playlist by dragging and dropping them on to it.)
                            If CStr(TimeStringToNumber(outTF.Text)) = "0" Then
                                outTF.Text = NumberToTimeString(currentDuration + 0.5 - outPointOffset)
                                modifyEvent(currentPlayingEvent)
                            End If

                            ' if we're not in OFF or LOOP modes, then make MPC-HC the foreground window to make it easier to do MPC-HC things
                            ' (like pausing once the playback starts, making larger, etc.)
                            If getMode() Then
                                SetForegroundWindow(MPCHandle)
                            End If

                            ' re-initialize these values to their defaults
                            loadingEvent = False
                            loadingEvent_Speed = 100

                            stilLLoading = False
                        Else ' we're not loading an event
                            If waitForSecondTick = 0 Then ' if MPC-HC loaded the file, we want to wait for 2 responses from this message before checking speed
                                waitForSecondTick = 1 ' skip this time around and wait for the next one
                            ElseIf waitForSecondTick = 1 Then ' OK, we're there, now check the speed
                                If speedSlider.Value <> defaultSpeed Then
                                    setSpeed(defaultSpeed)
                                End If

                                waitForSecondTick = -1 ' reset the variable so we're not checking it again (until the next time we need to)
                            End If
                        End If

                        nowInfoArray = Nothing ' unset the variable to clear it from memory
                    Case CType(CMD_RECEIVED.CMD_STATE, IntPtr)
                        If Convert.ToInt32(theMessage) = LOADSTATE.MLS_LOADED Then ' received when MPC-HC "finishes" loading a file
                            If clearINOUTPoint = True Then ' re-initialize everything to defaults
                                If loadingEvent = False Then waitForSecondTick = 0 ' check the speed to see if we need to change it... the next time around
                                clearInPoint()
                                clearOutPoint()
                            Else
                                clearINOUTPoint = True ' don't do anything, but turn this back off, so the next file (if it's skipped in MPC-HC) does the above ^^^
                            End If
                        End If
                    Case CType(CMD_RECEIVED.CMD_DISCONNECT, IntPtr) ' received when MPC-HC quits on its own - we can't come back from this, so see if we want to save
                        Log("MPC-HC disconnected")
                        unInitialize(True)
                        Me.Close()
                End Select

                CopyDataStruct = Nothing ' uninitialize the variable
                theMessage = Nothing ' uninitialize the variable
        End Select

        MyBase.WndProc(m) ' do anything else that needs to be done
    End Sub

    ' ======================================================================================
    ' ================= FORM FUNCTIONS =====================================================
    ' ======================================================================================

    Private Sub mainWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Log("=== Looper 2 starting ===")
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture

        Me.Icon = My.Resources.icon ' Set the Form icon to the main program icon

        If My.Computer.Keyboard.CtrlKeyDown = True Then ' re-initialize Looper from the beginning
            findLooperWindow.ShowDialog() ' show the "first launch" dialog to find MPC-HC's executable file
        End If

        If My.Computer.Keyboard.ShiftKeyDown = True Then ' load the defaults (Loop Mode, slip/preview, window positions, etc.) for Looper
            loadINIFileForDefaults() ' load only the MPC-HC values, but the rest are default
            changeAlwaysOnTop() ' change Always on Top to be true, so Looper sits on top of everything else
        Else ' load Looper like it normally would (this is where we'll USUALLY be)
            loadINIFile() ' load the values from the INI file, and set Looper up from those
        End If

        myHandle = Me.Handle ' my hWnd (for hotkey testing)

        posThread = New Thread(AddressOf doTheLoop) With {
            .IsBackground = True
        }

        posThread.Start()

        frontThread = New Thread(AddressOf isForeground) With {
            .IsBackground = True
        }

        frontThread.Start()

        setHotKeys() ' initialize the hotkeys

        Me.Text = "Looper 2 by Zach Glenwright" ' change the title to the actual title (this is done for the "find the other instance" function below)
        Me.Show() ' finally show the window, once initialization is done
    End Sub

    Private Function PrevInstance() As Boolean
        If UBound(Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)) > 0 Then ' if we're already running a Looper process, and we're not supposed to have new processes
            If My.Application.CommandLineArgs.Count > 0 Then ' if we have a command line (file to launch)
                Dim oldLooperHwnd As IntPtr = FindWindow(Nothing, "Looper 2 by Zach Glenwright") ' find the old hWnd
                Dim pathToOpen = My.Application.CommandLineArgs(0) ' get the path to the file

                If Strings.InStr(pathToOpen, ".looper") <> 0 Then ' if the path has ".looper" in it
                    SendMessage(&H5000F634, pathToOpen, True, oldLooperHwnd) ' send the path to the old Looper instance
                End If
            End If

            Return True ' we're running another instance of Looper
        Else
            Return False ' this is the first instance that's running
        End If
    End Function

    Private Sub mainWindow_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        selectedEventsButtonTriggers()
    End Sub

    Private Sub mainWindow_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If isQuitting = False Then ' we're not already trying to quit
            clearHotKeys() ' turn off the hotkeys

            If (e.CloseReason = CloseReason.UserClosing) Then
                Dim confirmSave = unInitialize() ' ask if we want to save, want to quit, etc.

                If confirmSave = DialogResult.Cancel Then ' if we chose NOT to quit, then turn things back on
                    setHotKeys()
                    isQuitting = False

                    If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled quitting, so start playing again

                    e.Cancel = True ' if we cancelled, then don't quit...
                End If
            End If
        End If

        saveLastPlayedFile() ' save the last played .looper and event to the INI file
        saveWindowPosition() ' save window position and size
    End Sub

    ' ======================================================================================
    ' ================= THREADS ============================================================
    ' ======================================================================================

    Private Sub doTheLoop()
        While True
            If isQuitting = False Then
                If currentPlayingFile <> Nothing Then
                    SendMessage(CMD_SEND.CMD_GETCURRENTPOSITION) ' get the current position from MPC-HC
                End If

                System.Threading.Thread.Sleep(30)
            Else ' we're quitting, so don't get the current position at this time
                System.Threading.Thread.Sleep(100) ' wait a little bit longer during the thread to see if we cancel quit
            End If
        End While
    End Sub

    Public Sub isForeground()
        While True
            If isQuitting = False Then
                activeHwnd = GetForegroundWindow() ' see which window is currently the foreground window

                If {myHandle, MPCHandle}.Contains(activeHwnd) Then ' if the foreground window is either the main window or MPC-HC's instance's window
                    If hotkeysActive = False Then
                        Try
                            Me.Invoke(Sub() setHotKeys()) ' turn hotkeys on
                        Catch ex As Exception
                            ' If there's an error, then bow out gracefully
                        End Try
                    End If
                Else ' if there is another window currently active
                    If hotkeysActive = True Then
                        Try
                            Me.Invoke(Sub() clearHotKeys()) ' turn hotkeys off
                        Catch ex As Exception
                            ' If there's an error, then bow out gracefully
                        End Try
                    End If
                End If

                System.Threading.Thread.Sleep(50)
                activeHwnd = Nothing ' uninitialize the variable
            Else ' we're trying to quit, so don't check the hotkey situation at this time
                System.Threading.Thread.Sleep(100) ' and wait a bit longer before checking again
            End If
        End While
    End Sub

    ' ======================================================================================
    ' ================= FILESYSTEM FUNCTIONS ===============================================
    ' ======================================================================================

    Private Sub loadINIFile()
        While Not My.Computer.FileSystem.GetFileInfo(INIFile).Exists ' if the INI file for Looper doesn't exist
            If Not My.Computer.FileSystem.GetFileInfo(INIFile).Directory.Exists Then ' if the preferences folder doesn't exist
                My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetFileInfo(INIFile).Directory.ToString) ' then create it!
            End If

            findLooperWindow.ShowDialog() ' then show the "First Start" dialog
        End While

        Dim fileReader As String
        fileReader = My.Computer.FileSystem.ReadAllText(INIFile)

        ' Load MPC-HC's information first, so if we're only supposed to run one instance, we can bow out here...
        Dim MPCEXE As String = betweenTheLines(fileReader, "MPCEXE=", vbCrLf, "0") ' the path to MPC-HC

        Dim allowMultipleInstances As Boolean
        Boolean.TryParse(betweenTheLines(fileReader, "allowMultipleInstances=", vbCrLf, "B_False"), allowMultipleInstances)

        If allowMultipleInstances = False Then
            If PrevInstance() Then ' we have multiple instances turned off, check to see if we're already running looper
                End ' quit out of the new instance of Looper (if we're already running Looper)
            Else
                If MPCEXE <> "0" Then linkMPC(MPCEXE, allowMultipleInstances) ' launch MPC-HC as either it's own instance, or multiple instances
            End If
        Else
            If MPCEXE <> "0" Then linkMPC(MPCEXE, allowMultipleInstances) ' launch MPC-HC as either it's own instance, or multiple instances
        End If

        ' INI values that need to be stored in variables for later use
        Double.TryParse(betweenTheLines(fileReader, "loopPreviewLength=", vbCrLf, "0.25"), loopPreviewLength)
        Double.TryParse(betweenTheLines(fileReader, "loopSlipLength=", vbCrLf, "0.50"), loopSlipLength)
        Integer.TryParse(betweenTheLines(fileReader, "defaultSpeed=", vbCrLf, "100"), defaultSpeed)

        Double.TryParse(betweenTheLines(fileReader, "inPointOffset=", vbCrLf, "0"), inPointOffset)
        Double.TryParse(betweenTheLines(fileReader, "outPointOffset=", vbCrLf, "0.15"), outPointOffset)

        Boolean.TryParse(betweenTheLines(fileReader, "disableHotkeys=", vbCrLf, "B_False"), disableHotkeys)
        Boolean.TryParse(betweenTheLines(fileReader, "autoplayFirstEvent=", vbCrLf, "B_True"), autoplayFirstEvent)
        Boolean.TryParse(betweenTheLines(fileReader, "autoPlayDialogs=", vbCrLf, "B_True"), autoPlayDialogs)
        Boolean.TryParse(betweenTheLines(fileReader, "dontForceLooperModeonOpen=", vbCrLf, "B_False"), dontForceLooperModeonOpen)
        Boolean.TryParse(betweenTheLines(fileReader, "pausePlaybackOnLoadEvent=", vbCrLf, "B_False"), pausePlaybackOnLoadEvent)
        Boolean.TryParse(betweenTheLines(fileReader, "skipSaveConfirmations=", vbCrLf, "B_False"), skipSaveConfirmations)
        Boolean.TryParse(betweenTheLines(fileReader, "skipEventNameEditor=", vbCrLf, "B_False"), skipEventNameEditor)
        Boolean.TryParse(betweenTheLines(fileReader, "autoSaveLooper=", vbCrLf, "B_False"), autoSaveLooper)
        Boolean.TryParse(betweenTheLines(fileReader, "autoCreateEventOnOut=", vbCrLf, "B_False"), autoCreateEventOnOut)
        Boolean.TryParse(betweenTheLines(fileReader, "osdOnInPoint=", vbCrLf, "B_False"), osdOnInPoint)
        Boolean.TryParse(betweenTheLines(fileReader, "osdOnOutPoint=", vbCrLf, "B_False"), osdOnOutPoint)
        Boolean.TryParse(betweenTheLines(fileReader, "osdOnAddEvent=", vbCrLf, "B_False"), osdOnAddEvent)
        Boolean.TryParse(betweenTheLines(fileReader, "osdOnLoopModeChange=", vbCrLf, "B_False"), osdOnLoopModeChange)
        Boolean.TryParse(betweenTheLines(fileReader, "osdOnSave=", vbCrLf, "B_False"), osdOnSave)
        defaultLooperSavePath = betweenTheLines(fileReader, "defaultLooperSavePath=", vbCrLf, Nothing)
        If defaultLooperSavePath IsNot Nothing Then defaultLooperSavePath = Environment.ExpandEnvironmentVariables(defaultLooperSavePath)

        newEventString = betweenTheLines(fileReader, "newEventName=", vbCrLf, "New Loop Event") ' the name for new un-named events on the events list

        ' INI values that don't need to be stored in global variables (these all get used only when Looper first starts up)
        Dim alwaysOnTop As String = betweenTheLines(fileReader, "alwaysOnTop=", vbCrLf, "True") ' whether or not to always be on top from launch
        If alwaysOnTop = "True" Then changeAlwaysOnTop() ' we're set to be in non-Topmost mode on launch, so switch it

        Dim startPositionL As String = betweenTheLines(fileReader, "startPositionL=", vbCrLf, "-1") ' the left-most coordinate to open Looper at
        If startPositionL <> "-1" Then Me.Left = CInt(startPositionL)

        Dim startPositionT As String = betweenTheLines(fileReader, "startPositionT=", vbCrLf, "-1") ' the top-most coordinate to open Looper at
        If startPositionT <> "-1" Then Me.Top = CInt(startPositionT)

        Dim startPositionW As String = betweenTheLines(fileReader, "startPositionW=", vbCrLf, "-1")
        If startPositionW <> "-1" Then Me.Width = CInt(startPositionW)

        Dim startPositionH As String = betweenTheLines(fileReader, "startPositionH=", vbCrLf, "-1")
        If startPositionH <> "-1" Then Me.Height = CInt(startPositionH)

        Dim loopButtonMode As String = betweenTheLines(fileReader, "loopButtonMode=", vbCrLf, "LOOP")
        Select Case loopButtonMode
            Case "Off", "OFF" : switchToOffMode()
            Case "Loop Mode", "LOOP" ' default, no switch needed
            Case "Playlist Mode", "PLAYLIST" : switchToPlaylistMode()
            Case "Shuffle Mode", "SHUFFLE" : switchToShuffleMode()
        End Select

        autoloadLastLooper = betweenTheLines(fileReader, "autoloadLastLooper=", vbCrLf, Nothing) ' should we launch the last looper file when closed?

        Dim disableToolTips As Boolean
        Boolean.TryParse(betweenTheLines(fileReader, "disableToolTips=", vbCrLf, "B_False"), disableToolTips)

        If disableToolTips = False Then loadToolTips(True) ' really no need to do the check here, this value is the boolean

        ' Check to see if there are any custom hotkeys
        Dim currentHK As String = Nothing

        For checkHotKey = 100 To 135 Step 1
            currentHK = betweenTheLines(fileReader, checkHotKey & "=", vbCrLf, "-1") ' check to see if there's an entry for the specific hotkey
            If currentHK <> "-1" Then hotKeyList(checkHotKey - 100, 1) = currentHK ' if there is, then set it

            ' Check if this hotkey is disabled
            Dim disabledHK As String = betweenTheLines(fileReader, "D" & checkHotKey & "=", vbCrLf, "-1")
            If disabledHK <> "-1" Then hotKeyDisabled(checkHotKey - 100) = True
        Next
    End Sub

    Private Sub loadINIFileForDefaults() ' if we launch defaults at the beginning, then we just need the MPC-HC values, so we can run the program
        While Not My.Computer.FileSystem.GetFileInfo(INIFile).Exists ' if the INI file for Looper doesn't exist
            If Not My.Computer.FileSystem.GetFileInfo(INIFile).Directory.Exists Then ' if the preferences folder doesn't exist
                My.Computer.FileSystem.CreateDirectory(My.Computer.FileSystem.GetFileInfo(INIFile).Directory.ToString) ' then create it!
            End If

            findLooperWindow.ShowDialog() ' then show the "First Start" dialog
        End While

        Dim fileReader As String
        fileReader = My.Computer.FileSystem.ReadAllText(INIFile)

        Dim MPCEXE As String = betweenTheLines(fileReader, "MPCEXE=", vbCrLf, "0") ' the path to MPC-HC

        Dim allowMultipleInstances As Boolean
        Boolean.TryParse(betweenTheLines(fileReader, "allowMultipleInstances=", vbCrLf, "B_False"), allowMultipleInstances)

        If MPCEXE <> "0" Then
            linkMPC(MPCEXE, allowMultipleInstances) ' launch MPC-HC as either it's own instance, or multiple instances
        Else
            ' TODO: Mirror back to the INI creation process and start over?  Or just display an error, and then quit?
        End If
    End Sub

    Private Sub linkMPC(ByVal MPCEXE As String, ByVal allowMultipleInstances As Boolean)
        Dim MPC_Path As String

        If allowMultipleInstances = False Then ' just open the normal instance of MPC-HC (or tie-in to an already running one)
            MPC_Path = MPCEXE & " /slave " + Me.Handle.ToString
        Else ' open a new instance of MPC-HC
            MPC_Path = MPCEXE & " /new /slave " + Me.Handle.ToString
        End If

        Shell(MPC_Path, AppWinStyle.NormalFocus, False, -1)
    End Sub

    Private Sub showFileInExplorer()
        If currentPlayingFile IsNot Nothing Then
            Process.Start("explorer.exe", "/select, """ & currentPlayingFile & """")
        End If
    End Sub

    Private Sub saveWindowPosition()
        Try
            Dim fileReader As String = My.Computer.FileSystem.ReadAllText(INIFile)

            ' Update or add startPositionL
            Dim writingString As String = fileReader
            If Strings.InStr(writingString, "startPositionL=") > 0 Then
                Dim startOffset = Strings.InStr(writingString, "startPositionL=")
                Dim endOffset = Strings.InStr(startOffset, writingString, vbCrLf)
                writingString = Strings.Left(writingString, startOffset - 1) & "startPositionL=" & Me.Left & vbCrLf & Strings.Mid(writingString, endOffset + 2)
            End If

            If Strings.InStr(writingString, "startPositionT=") > 0 Then
                Dim startOffset = Strings.InStr(writingString, "startPositionT=")
                Dim endOffset = Strings.InStr(startOffset, writingString, vbCrLf)
                writingString = Strings.Left(writingString, startOffset - 1) & "startPositionT=" & Me.Top & vbCrLf & Strings.Mid(writingString, endOffset + 2)
            End If

            If Strings.InStr(writingString, "startPositionW=") > 0 Then
                Dim startOffset = Strings.InStr(writingString, "startPositionW=")
                Dim endOffset = Strings.InStr(startOffset, writingString, vbCrLf)
                writingString = Strings.Left(writingString, startOffset - 1) & "startPositionW=" & Me.Width & vbCrLf & Strings.Mid(writingString, endOffset + 2)
            Else
                ' Add it after startPositionT line
                If Strings.InStr(writingString, "startPositionT=") > 0 Then
                    Dim startOffset = Strings.InStr(writingString, "startPositionT=")
                    Dim endOffset = Strings.InStr(startOffset, writingString, vbCrLf)
                    writingString = Strings.Left(writingString, endOffset + 1) & "startPositionW=" & Me.Width & vbCrLf & Strings.Mid(writingString, endOffset + 2)
                End If
            End If

            If Strings.InStr(writingString, "startPositionH=") > 0 Then
                Dim startOffset = Strings.InStr(writingString, "startPositionH=")
                Dim endOffset = Strings.InStr(startOffset, writingString, vbCrLf)
                writingString = Strings.Left(writingString, startOffset - 1) & "startPositionH=" & Me.Height & vbCrLf & Strings.Mid(writingString, endOffset + 2)
            Else
                ' Add it after startPositionW line
                If Strings.InStr(writingString, "startPositionW=") > 0 Then
                    Dim startOffset = Strings.InStr(writingString, "startPositionW=")
                    Dim endOffset = Strings.InStr(startOffset, writingString, vbCrLf)
                    writingString = Strings.Left(writingString, endOffset + 1) & "startPositionH=" & Me.Height & vbCrLf & Strings.Mid(writingString, endOffset + 2)
                End If
            End If

            System.IO.File.WriteAllText(INIFile, writingString)
        Catch ex As Exception
        End Try
    End Sub

    ' ======================================================================================
    ' ================= HOTKEY HANDLERS ====================================================
    ' ======================================================================================

    Public Sub assignHotKey(myHandle As IntPtr, myID As Integer)
        If hotKeyDisabled(myID - 100) Then Return ' skip disabled hotkeys

        Dim hkMod, hkKey As Integer
        Dim currentSettings(2) As String

        If hotKeyList(myID - 100, 1) Is Nothing Then
            currentSettings = Split(hotKeyList(myID - 100, 0), "|")
        Else
            currentSettings = Split(hotKeyList(myID - 100, 1), "|")
        End If

        If currentSettings(0).Contains("C") Then hkMod += KeyModifier.Control
        If currentSettings(0).Contains("S") Then hkMod += KeyModifier.Shift
        If currentSettings(0).Contains("A") Then hkMod += KeyModifier.Alt

        hkKey = CType(currentSettings(1), Integer)
        If hkKey <> 0 Then RegisterHotKey(myHandle, myID, hkMod, hkKey) ' if the Integer conversion worked, then assign the hotkey
    End Sub

    Public Sub setHotKeys(Optional hotKeyID As Integer = -1)
        If disableHotkeys = False Then
            'IN and OUT hotkeys
            If hotKeyID = -1 Or hotKeyID = 100 Then assignHotKey(Me.Handle, 100)  ' set IN point
            If hotKeyID = -1 Or hotKeyID = 101 Then assignHotKey(Me.Handle, 101)  ' set OUT point
            If hotKeyID = -1 Or hotKeyID = 102 Then assignHotKey(Me.Handle, 102)  ' clear IN point
            If hotKeyID = -1 Or hotKeyID = 103 Then assignHotKey(Me.Handle, 103)  ' clear OUT point
            If hotKeyID = -1 Or hotKeyID = 104 Then assignHotKey(Me.Handle, 104)  ' clear IN and OUT points

            ' Trimming IN and OUT
            If hotKeyID = -1 Or hotKeyID = 105 Then assignHotKey(Me.Handle, 105)  ' trim the IN point left
            If hotKeyID = -1 Or hotKeyID = 106 Then assignHotKey(Me.Handle, 106)  ' trim the IN point left by the default value
            If hotKeyID = -1 Or hotKeyID = 107 Then assignHotKey(Me.Handle, 107)  ' trim the IN point right
            If hotKeyID = -1 Or hotKeyID = 108 Then assignHotKey(Me.Handle, 108)  ' trim the IN point right by the default value
            If hotKeyID = -1 Or hotKeyID = 109 Then assignHotKey(Me.Handle, 109)  ' trim the OUT point left
            If hotKeyID = -1 Or hotKeyID = 110 Then assignHotKey(Me.Handle, 110)  ' trim the OUT point left by the default value
            If hotKeyID = -1 Or hotKeyID = 111 Then assignHotKey(Me.Handle, 111)  ' trim the OUT point right
            If hotKeyID = -1 Or hotKeyID = 112 Then assignHotKey(Me.Handle, 112)  ' trim the OUT point right by the default value

            ' GUI hotkeys
            If hotKeyID = -1 Or hotKeyID = 113 Then assignHotKey(Me.Handle, 113)  ' change loop mode
            If hotKeyID = -1 Or hotKeyID = 114 Then assignHotKey(Me.Handle, 114)  ' change loop mode to OFF
            If hotKeyID = -1 Or hotKeyID = 115 Then assignHotKey(Me.Handle, 115)  ' change loop mode to LOOP MODE
            If hotKeyID = -1 Or hotKeyID = 116 Then assignHotKey(Me.Handle, 116)  ' change loop mode to PLAYLIST MODE
            If hotKeyID = -1 Or hotKeyID = 117 Then assignHotKey(Me.Handle, 117)  ' change loop mode to SHUFFLE MODE

            If hotKeyID = -1 Or hotKeyID = 118 Then assignHotKey(Me.Handle, 118)  ' change always on top setting
            If hotKeyID = -1 Or hotKeyID = 119 Then assignHotKey(Me.Handle, 119)  ' quit MPC-HC Looper
            If hotKeyID = -1 Or hotKeyID = 120 Then assignHotKey(Me.Handle, 120)  ' load options pane
            If hotKeyID = -1 Or hotKeyID = 121 Then assignHotKey(Me.Handle, 121)  ' open path to file in Explorer

            ' Working with events
            If hotKeyID = -1 Or hotKeyID = 122 Then assignHotKey(Me.Handle, 122) ' create new event
            If hotKeyID = -1 Or hotKeyID = 123 Then assignHotKey(Me.Handle, 123) ' delete currently selected event(s)
            If hotKeyID = -1 Or hotKeyID = 124 Then assignHotKey(Me.Handle, 124) ' save current events list (overwrite)
            If hotKeyID = -1 Or hotKeyID = 125 Then assignHotKey(Me.Handle, 125) ' save the selected items as a new Looper file
            If hotKeyID = -1 Or hotKeyID = 126 Then assignHotKey(Me.Handle, 126) ' load new events list
            If hotKeyID = -1 Or hotKeyID = 127 Then assignHotKey(Me.Handle, 127) ' import .looper file into the current events list
            If hotKeyID = -1 Or hotKeyID = 128 Then assignHotKey(Me.Handle, 128) ' go to one event prior in the events list
            If hotKeyID = -1 Or hotKeyID = 129 Then assignHotKey(Me.Handle, 129) ' go to one event next in the events list
            If hotKeyID = -1 Or hotKeyID = 130 Then assignHotKey(Me.Handle, 130) ' Speed up playback in MPC-HC
            If hotKeyID = -1 Or hotKeyID = 131 Then assignHotKey(Me.Handle, 131) ' Slow down playback in MPC-HC
            If hotKeyID = -1 Or hotKeyID = 132 Then assignHotKey(Me.Handle, 132) ' Reset speed to default speed in MPC-HC

            If hotKeyID = -1 Or hotKeyID = 133 Then assignHotKey(Me.Handle, 133) ' Go to the Search field
            If hotKeyID = -1 Or hotKeyID = 134 Then assignHotKey(Me.Handle, 134) ' Select ALL the events in the events list
            If hotKeyID = -1 Or hotKeyID = 135 Then assignHotKey(Me.Handle, 135) ' Select NONE of the events in the events list

            If hotKeyID = -1 Then
                hotkeysTF.Text = "HOTKEYS ON"
                hotkeysTF.ForeColor = Color.Green
                hotkeysActive = True
            End If
        End If
    End Sub

    Public Sub clearHotKeys(Optional hotKeyID As Integer = -1)
        If disableHotkeys = False Then
            If hotKeyID = -1 Or hotKeyID = 100 Then UnregisterHotKey(Me.Handle, 100) ' set IN point
            If hotKeyID = -1 Or hotKeyID = 101 Then UnregisterHotKey(Me.Handle, 101) ' set OUT point
            If hotKeyID = -1 Or hotKeyID = 102 Then UnregisterHotKey(Me.Handle, 102) ' clear IN point
            If hotKeyID = -1 Or hotKeyID = 103 Then UnregisterHotKey(Me.Handle, 103) ' clear OUT point
            If hotKeyID = -1 Or hotKeyID = 104 Then UnregisterHotKey(Me.Handle, 104) ' clear IN and OUT points

            If hotKeyID = -1 Or hotKeyID = 110 Then UnregisterHotKey(Me.Handle, 105) ' trim the IN point left
            If hotKeyID = -1 Or hotKeyID = 111 Then UnregisterHotKey(Me.Handle, 106) ' trim the IN point left by the default value
            If hotKeyID = -1 Or hotKeyID = 112 Then UnregisterHotKey(Me.Handle, 107) ' trim the IN point right
            If hotKeyID = -1 Or hotKeyID = 113 Then UnregisterHotKey(Me.Handle, 108) ' trim the IN point right by the default value
            If hotKeyID = -1 Or hotKeyID = 114 Then UnregisterHotKey(Me.Handle, 109) ' trim the OUT point left
            If hotKeyID = -1 Or hotKeyID = 115 Then UnregisterHotKey(Me.Handle, 110) ' trim the OUT point left by the default value
            If hotKeyID = -1 Or hotKeyID = 116 Then UnregisterHotKey(Me.Handle, 111) ' trim the OUT point right
            If hotKeyID = -1 Or hotKeyID = 117 Then UnregisterHotKey(Me.Handle, 112) ' trim the OUT point right by the default value

            If hotKeyID = -1 Or hotKeyID = 120 Then UnregisterHotKey(Me.Handle, 113) ' change loop mode
            If hotKeyID = -1 Or hotKeyID = 121 Then UnregisterHotKey(Me.Handle, 114) ' change loop mode to OFF
            If hotKeyID = -1 Or hotKeyID = 122 Then UnregisterHotKey(Me.Handle, 115) ' change loop mode to LOOP MODE
            If hotKeyID = -1 Or hotKeyID = 123 Then UnregisterHotKey(Me.Handle, 116) ' change loop mode to PLAYLIST MODE
            If hotKeyID = -1 Or hotKeyID = 124 Then UnregisterHotKey(Me.Handle, 117) ' change loop mode to SHUFFLE MODE

            If hotKeyID = -1 Or hotKeyID = 125 Then UnregisterHotKey(Me.Handle, 118) ' change always on top setting
            If hotKeyID = -1 Or hotKeyID = 126 Then UnregisterHotKey(Me.Handle, 119) ' quit MPC-HC Looper
            If hotKeyID = -1 Or hotKeyID = 127 Then UnregisterHotKey(Me.Handle, 120) ' load options pane
            If hotKeyID = -1 Or hotKeyID = 128 Then UnregisterHotKey(Me.Handle, 121) ' open path to file in Explorer

            If hotKeyID = -1 Or hotKeyID = 129 Then UnregisterHotKey(Me.Handle, 122) ' create new event
            If hotKeyID = -1 Or hotKeyID = 130 Then UnregisterHotKey(Me.Handle, 123) ' delete currently selected event(s)
            If hotKeyID = -1 Or hotKeyID = 131 Then UnregisterHotKey(Me.Handle, 124) ' save current events list
            If hotKeyID = -1 Or hotKeyID = 132 Then UnregisterHotKey(Me.Handle, 125) ' save the selected items as a new Looper file
            If hotKeyID = -1 Or hotKeyID = 133 Then UnregisterHotKey(Me.Handle, 126) ' load new events list
            If hotKeyID = -1 Or hotKeyID = 134 Then UnregisterHotKey(Me.Handle, 127) ' import .looper file into the current events list
            If hotKeyID = -1 Or hotKeyID = 135 Then UnregisterHotKey(Me.Handle, 128) ' go to one event prior in the events list
            If hotKeyID = -1 Or hotKeyID = 136 Then UnregisterHotKey(Me.Handle, 129) ' go to one event next in the events list
            If hotKeyID = -1 Or hotKeyID = 137 Then UnregisterHotKey(Me.Handle, 130) ' Speed up playback in MPC-HC
            If hotKeyID = -1 Or hotKeyID = 138 Then UnregisterHotKey(Me.Handle, 131) ' Slow down playback in MPC-HC
            If hotKeyID = -1 Or hotKeyID = 139 Then UnregisterHotKey(Me.Handle, 132) ' Reset speed to 100% in MPC-HC
            If hotKeyID = -1 Or hotKeyID = 140 Then UnregisterHotKey(Me.Handle, 133) ' Go to the Search field

            If hotKeyID = -1 Or hotKeyID = 141 Then UnregisterHotKey(Me.Handle, 134) ' Select ALL the events in the events list
            If hotKeyID = -1 Or hotKeyID = 142 Then UnregisterHotKey(Me.Handle, 135) ' Select NONE of the events in the events list

            hotkeysTF.Text = "HOTKEYS OFF"
            hotkeysTF.ForeColor = Color.Red
            hotkeysActive = False
        End If
    End Sub

    Public Function getMode() As Boolean
        If {"OFF", "LOOP"}.Contains(loopModeButton.Text) Then
            Return True ' if we're in Loop Mode or OFF, then we can do things with IN/OUT points
        Else
            Return False ' otherwise, don't allow it!
        End If
    End Function

    Public Sub handleHotKeyEvent(hotkeyID As Integer)
        Select Case hotkeyID
            Case 100 ' set IN point
                If getMode() Then setInPoint()
            Case 101 ' set OUT point
                If getMode() Then setOutPoint()
            Case 102 ' clear IN point
                If getMode() Then clearInPoint()
            Case 103 ' clear OUT point
                If getMode() Then clearOutPoint()
            Case 104 ' clear IN and OUT points
                If getMode() Then clearInPoint()
                If getMode() Then clearOutPoint()

            Case 105 ' trim the IN point left
                If getMode() Then slipPoint(1)
            Case 106 ' trim the IN point left by the default value
                If getMode() Then slipPoint(1, 0.05)
            Case 107 ' trim the IN point right
                If getMode() Then slipPoint(2)
            Case 108 ' trim the IN point right by the default value
                If getMode() Then slipPoint(2, 0.05)
            Case 109 ' trim the OUT point left
                If getMode() Then slipPoint(3)
            Case 110 ' trim the OUT point left by the default value
                If getMode() Then slipPoint(3, 0.05)
            Case 111 ' trim the OUT point right
                If getMode() Then slipPoint(4)
            Case 112 ' trim the OUT point right by the default value
                If getMode() Then slipPoint(4, 0.05)

            Case 113 ' change loop mode
                switchLoopMode()
            Case 114 ' change loop mode to OFF
                switchToOffMode()
            Case 115 ' change loop mode to LOOP MODE
                switchToLoopMode()
            Case 116 ' change loop mode to PLAYLIST MODE
                switchToPlaylistMode()
            Case 117 ' change loop mode to SHUFFLE MODE
                switchToShuffleMode()

            Case 118 ' change always on top setting
                changeAlwaysOnTop()
            Case 119 ' quit MPC-HC Looper
                Me.Close()
            Case 120 ' load options pane
                optionsWindow.ShowDialog()
            Case 121 ' open path to file in Explorer
                showFileInExplorer()

            Case 122 ' create new event
                If getMode() Then addEvent()
            Case 123 ' delete currently selected event(s)
                If getMode() Then deleteEvents()
            Case 124 ' save current events list
                If getMode() Then saveLooperFile()
            Case 125 ' save the selected items as a new Looper file
                saveLooperFile(True)
            Case 126 ' load new events list
                loadLooperFile()
            Case 127 ' import .looper file into the current events list
                loadLooperFile(False)
            Case 128 ' go to one event prior in the events list
                loadPrevNextEvent(-1)
            Case 129 ' go to one event next in the events list
                loadPrevNextEvent(1)
            Case 130 ' Speed up playback in MPC-HC
                If getMode() Then setSpeed(speedSlider.Value + 10)
            Case 131 ' Slow down playback in MPC-HC
                If getMode() Then setSpeed(speedSlider.Value - 10)
            Case 132 ' Reset speed to 100% in MPC-HC
                If getMode() Then setSpeed(defaultSpeed)
            Case 133 ' Go to the Search field
                searchTF.Select()

            Case 134 ' Select ALL the events in the events list
                If eventsList.Items.Count > 0 Then
                    eventsList.BeginUpdate()

                    For a As Integer = 0 To eventsList.Items.Count - 1
                        eventsList.Items(a).Selected = True
                    Next

                    eventsList.EndUpdate()
                End If
            Case 135 ' Select NONE of the events in the events list
                If eventsList.Items.Count > 0 Then
                    eventsList.BeginUpdate()

                    For a As Integer = 0 To eventsList.Items.Count - 1
                        eventsList.Items(a).Selected = False
                    Next

                    eventsList.EndUpdate()
                End If

        End Select
    End Sub

    ' ======================================================================================
    ' ================= BUTTON / CLICK HANDLERS ============================================
    ' ======================================================================================

    Private Sub inPointButton_Click(sender As Object, e As EventArgs) Handles inPointButton.Click
        setInPoint()
    End Sub

    Private Sub outPointButton_Click(sender As Object, e As EventArgs) Handles outPointButton.Click
        setOutPoint()
    End Sub


    Private Sub currentPositionLabel_Click(sender As Object, e As EventArgs) Handles currentPositionLabel.Click
        switchCurrentAndRemaining()
    End Sub

    Private Sub alwaysOnTopButton_Click(sender As Object, e As EventArgs) Handles alwaysOnTopButton.Click
        changeAlwaysOnTop()
    End Sub

    Private Sub currentRemainingStatus_Click(sender As Object, e As EventArgs) Handles currentRemainingStatus.Click
        switchCurrentAndRemaining()
    End Sub

    Private Sub loadOptionsWindowButton_Click(sender As Object, e As EventArgs) Handles loadOptionsWindowButton.Click
        optionsWindow.ShowDialog()
    End Sub

    Private Sub showPlayingFileInExplorerButton_Click(sender As Object, e As EventArgs) Handles showPlayingFileInExplorerButton.Click
        loadLooperFile()
    End Sub

    Private Sub loopModeButton_Click(sender As Object, e As EventArgs) Handles loopModeButton.Click
        switchLoopMode()
    End Sub

    Private Sub switchLoopMode()
        If loopModeButton.Text = "LOOP" Then
            If eventsList.Items.Count > 0 Then
                switchToPlaylistMode()
            Else
                switchToOffMode()
            End If
        ElseIf loopModeButton.Text = "PLAYLIST" Then
            switchToOffMode()
        ElseIf loopModeButton.Text = "OFF" Then
            switchToLoopMode()
        End If
    End Sub

    Private Sub switchEditingControls(Optional onOrOff As Boolean = True)
        inTF.Enabled = onOrOff
        outTF.Enabled = onOrOff

        inPointSlipLeftButton.Enabled = onOrOff
        inPointButton.Enabled = onOrOff
        inPointSlipRightButton.Enabled = onOrOff

        outPointSlipLeftButton.Enabled = onOrOff
        outPointButton.Enabled = onOrOff
        outPointSlipRightButton.Enabled = onOrOff

        speedSlider.Enabled = onOrOff
        speedDownButton.Enabled = onOrOff
        speedUpButton.Enabled = onOrOff

        addEventButton.Enabled = onOrOff

        If onOrOff = True Then
            If eventsList.SelectedItems.Count > 0 Then
                modifyEventButton.Enabled = True ' activate the Modify Event button if we have something selected
            End If
        Else
            modifyEventButton.Enabled = False ' turn the Modify Event button off
        End If
    End Sub

    Public Sub switchToOffMode()
        cancelRandomization()

        loopModeButton.Text = "OFF"
        loopModeButton.BackColor = Color.FromArgb(42, 42, 42)
        loopModeButton.ForeColor = Color.White
        loopModeButton.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 85)

        switchEditingControls()
        If osdOnLoopModeChange Then SendOSD("Loop Mode: Off")
    End Sub

    Public Sub switchToLoopMode()
        cancelRandomization()

        loopModeButton.Text = "LOOP"
        loopModeButton.BackColor = Color.FromArgb(42, 74, 58)
        loopModeButton.ForeColor = Color.FromArgb(136, 255, 204)
        loopModeButton.FlatAppearance.BorderColor = Color.FromArgb(68, 170, 136)

        switchEditingControls()
        If osdOnLoopModeChange Then SendOSD("Loop Mode: Loop")
    End Sub

    Public Sub switchToPlaylistMode()
        If eventsList.Items.Count > 0 Then
            cancelRandomization()

            loopModeButton.Text = "PLAYLIST"
            loopModeButton.BackColor = Color.FromArgb(42, 42, 90)
            loopModeButton.ForeColor = Color.FromArgb(170, 170, 255)
            loopModeButton.FlatAppearance.BorderColor = Color.FromArgb(102, 102, 170)

            switchEditingControls(False)
            If osdOnLoopModeChange Then SendOSD("Loop Mode: Playlist")
        End If
    End Sub

    Public Sub switchToShuffleMode()
        Dim totalEvents = eventsList.Items.Count ' get the total number of events currently in the playlist

        If totalEvents > 0 Then
            ReDim randomNumberList(0) ' erase the values currently stored in the array (to ensure it won't have any extra values)

            loopModeButton.Text = "SHUFFLE"
            loopModeButton.BackColor = Color.FromArgb(74, 42, 74)
            loopModeButton.ForeColor = Color.FromArgb(255, 170, 255)
            loopModeButton.FlatAppearance.BorderColor = Color.FromArgb(170, 102, 170)

            switchEditingControls(False) ' turn off editing controls, so you can't use them in Shuffle mode

            randomNumberList = Enumerable.Range(0, totalEvents).OrderBy(Function(r) RNG.Next()).Take(totalEvents + 1).ToArray ' re-initialize the array

            ' Force the playlist to start at the beginning of the random order
            currentPlayingEvent = -1
            loadPrevNextEvent(1)

            If osdOnLoopModeChange Then SendOSD("Loop Mode: Shuffle")
        End If
    End Sub

    Public Sub cancelRandomization()
        ReDim randomNumberList(0)
        If loopModeButton.Text = "SHUFFLE" Then currentPlayingEvent = actualPlayingEvent ' get the current event from the currently playing random event
    End Sub

    Private Sub switchCurrentAndRemaining()
        If currentOrRemaining = False Then
            currentOrRemaining = True
            currentRemainingStatus.Text = "REMAINING"
        Else
            currentOrRemaining = False
            currentRemainingStatus.Text = "CURRENT"
        End If
    End Sub

    Private Sub changeAlwaysOnTop()
        If Me.TopMost Then
            Me.TopMost = False
            alwaysOnTopButton.Text = ChrW(&HE77A)
            alwaysOnTopButton.BackColor = Color.FromArgb(51, 51, 51)
        Else
            Me.TopMost = True
            alwaysOnTopButton.Text = ChrW(&HE718)
            alwaysOnTopButton.BackColor = Color.FromArgb(42, 42, 90)
        End If
    End Sub

    ' ======================================================================================
    ' ================= SLIP CLICK/HOLD DOWN HANDLERS ======================================
    ' ======================================================================================

    Private Sub inPointSlipLeftButton_Click(sender As Object, e As EventArgs) Handles inPointSlipLeftButton.Click
        slipPoint(1)
    End Sub

    Private Sub inPointSlipRightButton_Click(sender As Object, e As EventArgs) Handles inPointSlipRightButton.Click
        slipPoint(2)
    End Sub

    Private Sub outPointSlipLeftButton_Click(sender As Object, e As EventArgs) Handles outPointSlipLeftButton.Click
        slipPoint(3)
    End Sub

    Private Sub outPointSlipRightButton_Click(sender As Object, e As EventArgs) Handles outPointSlipRightButton.Click
        slipPoint(4)
    End Sub

    Private Sub inPointSlipLeftButton_MouseDown(sender As Object, e As MouseEventArgs) Handles inPointSlipLeftButton.MouseDown
        slipAction = 1 ' we're slipping the IN point left
        slipTimer.Start()
    End Sub

    Private Sub inPointSlipLeftButton_MouseUp(sender As Object, e As MouseEventArgs) Handles inPointSlipLeftButton.MouseUp
        slipTimer.Stop()
    End Sub

    Private Sub inPointSlipRightButton_MouseDown(sender As Object, e As MouseEventArgs) Handles inPointSlipRightButton.MouseDown
        slipAction = 2 ' we're slipping the IN point right
        slipTimer.Start()
    End Sub

    Private Sub inPointSlipRightButton_MouseUp(sender As Object, e As MouseEventArgs) Handles inPointSlipRightButton.MouseUp
        slipTimer.Stop()
    End Sub

    Private Sub outPointSlipLeftButton_MouseDown(sender As Object, e As MouseEventArgs) Handles outPointSlipLeftButton.MouseDown
        slipAction = 3 ' we're slipping the OUT point left
        slipTimer.Start()
    End Sub

    Private Sub outPointSlipLeftButton_MouseUp(sender As Object, e As MouseEventArgs) Handles outPointSlipLeftButton.MouseUp
        slipTimer.Stop()
    End Sub

    Private Sub outPointSlipRightButton_MouseDown(sender As Object, e As MouseEventArgs) Handles outPointSlipRightButton.MouseDown
        slipAction = 4 ' we're slipping the OUT point right
        slipTimer.Start()
    End Sub

    Private Sub outPointSlipRightButton_MouseUp(sender As Object, e As MouseEventArgs) Handles outPointSlipRightButton.MouseUp
        slipTimer.Stop()
    End Sub

    Private Sub slipTimer_Tick(sender As Object, e As EventArgs) Handles slipTimer.Tick
        slipPoint(slipAction) ' slip the specified point
    End Sub

    ' ======================================================================================
    ' ================= IN/OUT/TIME HANDLERS ===============================================
    ' ======================================================================================

    Public Sub setInPoint()
        SendMessage(CMD_SEND.CMD_GETCURRENTPOSITION)
        Threading.Thread.Sleep(50) ' wait for position response to arrive
        inTF.Text = NumberToTimeString(currentPosition - 0.25 + inPointOffset)

        If TimeStringToNumber(inTF.Text) > TimeStringToNumber(outTF.Text) Then
            outTF.Text = Nothing ' clear the OUT point without resetting to end-of-video
        End If

        checkAgainstCurrentPlayingEvent()

        If osdOnInPoint Then SendOSD("In Point")
    End Sub

    Public Sub setOutPoint()
        SendMessage(CMD_SEND.CMD_GETCURRENTPOSITION)
        Threading.Thread.Sleep(50) ' wait for position response to arrive
        outTF.Text = NumberToTimeString(currentPosition + outPointOffset)

        checkAgainstCurrentPlayingEvent()

        If osdOnOutPoint AndAlso Not autoCreateEventOnOut Then SendOSD("Out Point")

        If autoCreateEventOnOut Then addEvent()
    End Sub

    Public Sub clearInPoint()
        inTF.Text = "0:00"

        checkAgainstCurrentPlayingEvent() ' check to see if the IN and OUT points match the currently playing event (for highlight)
    End Sub

    Public Sub clearOutPoint()
        SendMessage(CMD_SEND.CMD_GETNOWPLAYING)
        outTF.Text = NumberToTimeString(currentDuration + 0.5 - outPointOffset)

        checkAgainstCurrentPlayingEvent() ' check to see if the IN and OUT points match the currently playing event (for highlight)
    End Sub

    Private Sub slipPoint(ByVal thePoint As Integer, Optional slipAmount As Double = -1)
        If slipAmount = -1 Then slipAmount = loopSlipLength ' set the slipping amount to your preset if we're not slipping by a specific number
        Dim newPoint As Double ' the new point after slipping

        Select Case thePoint
            Case 1 ' we're slipping the IN point back
                newPoint = TimeStringToNumber(inTF.Text) - slipAmount
                If newPoint < 0 Then newPoint = 0 ' if the resulting IN point is less than zero, then just set the IN to 0
            Case 2 ' we're slipping the IN point forwards
                newPoint = TimeStringToNumber(inTF.Text) + slipAmount
            Case 3 ' we're slipping the OUT point back
                newPoint = TimeStringToNumber(outTF.Text) - slipAmount
            Case 4 ' we're slipping the OUT point forwards
                newPoint = TimeStringToNumber(outTF.Text) + slipAmount
        End Select

        If thePoint < 3 Then ' we're slipping the IN point, so we don't need to do a preview slip-back
            inTF.Text = NumberToTimeString(newPoint)
            SendMessage(CMD_SEND.CMD_SETPOSITION, CStr((newPoint - 0.5)))
        Else ' we're slipping the OUT point, so do the slip-back
            outTF.Text = NumberToTimeString(newPoint)
            SendMessage(CMD_SEND.CMD_SETPOSITION, CStr((newPoint - 0.5 - loopPreviewLength)))
        End If

        checkAgainstCurrentPlayingEvent() ' check to see if the IN and OUT points match the currently playing event (for highlight)
    End Sub

    ' ======================================================================================
    ' ================= MANUALLY SETTING IN AND OUT POINTS =================================
    ' ======================================================================================

    Private Sub inTF_Enter(sender As Object, e As EventArgs) Handles inTF.Enter
        clearHotKeys()
        hotkeysActive = True  'tell the hotkey thread that hotkeys are still active, so it doesn't try to clear them (this is for text entry ONLY)
    End Sub

    Private Sub inTF_KeyPress(sender As Object, e As KeyPressEventArgs) Handles inTF.KeyPress
        ' ----------------- IF THE ENTERED CHARACTER IS NOT A NUMBER, THE CHARACTER . OR THE CHARACTER :, THEN DON'T ALLOW IT! -----------------
        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not e.KeyChar = "." AndAlso Not e.KeyChar = ":" Then
            e.Handled = True
        End If
    End Sub

    Private Sub inTF_Leave(sender As Object, e As EventArgs) Handles inTF.Leave
        setHotKeys()

        Dim timeString As Double
        Dim timeStringTest = Double.TryParse(inTF.Text, timeString)

        If timeStringTest = True Then ' if we can coerce the current value to a double, then convert it to a time string when leaving the control - otherwise it's fine!
            inTF.Text = NumberToTimeString(CDbl(inTF.Text))
        End If
    End Sub

    Private Sub outTF_Enter(sender As Object, e As EventArgs) Handles outTF.Enter
        clearHotKeys()
        hotkeysActive = True  'tell the hotkey thread that hotkeys are still active, so it doesn't try to clear them (this is for text entry ONLY)
    End Sub

    Private Sub outTF_KeyPress(sender As Object, e As KeyPressEventArgs) Handles outTF.KeyPress
        ' ----------------- IF THE ENTERED CHARACTER IS NOT A NUMBER, THE CHARACTER . OR THE CHARACTER :, THEN DON'T ALLOW IT! -----------------
        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not e.KeyChar = "." AndAlso Not e.KeyChar = ":" Then
            e.Handled = True
        End If
    End Sub

    Private Sub outTF_Leave(sender As Object, e As EventArgs) Handles outTF.Leave
        setHotKeys()

        Dim timeString As Double
        Dim timeStringTest = Double.TryParse(outTF.Text, timeString)

        If timeStringTest = True Then ' if we can coerce the current value to a double, then convert it to a time string when leaving the control - otherwise it's fine!
            outTF.Text = NumberToTimeString(CDbl(outTF.Text))
        End If
    End Sub

    ' ======================================================================================
    ' ================= SPEED HANDLERS =====================================================
    ' ======================================================================================

    Private Sub speedSlider_Scroll(sender As Object, e As EventArgs) Handles speedSlider.Scroll
        setSpeed()
    End Sub

    Public Sub setSpeed(Optional theSpeed As Integer = -1)
        If theSpeed <> -1 Then
            speedSlider.Value = theSpeed
        End If

        speedValueLabel.Text = speedSlider.Value & "%"
        SendMessage(CMD_SEND.CMD_SETSPEED, Convert.ToString(speedSlider.Value / 100))
    End Sub


    Private Sub speedDownButton_Click(sender As Object, e As EventArgs) Handles speedDownButton.Click
        Dim newVal As Integer = speedSlider.Value - 5
        If newVal < 0 Then newVal = 0
        speedSlider.Value = newVal
        setSpeed()
    End Sub

    Private Sub speedUpButton_Click(sender As Object, e As EventArgs) Handles speedUpButton.Click
        Dim newVal As Integer = speedSlider.Value + 5
        If newVal > 200 Then newVal = 200
        speedSlider.Value = newVal
        setSpeed()
    End Sub

    ' ======================================================================================
    ' ================= FIX / FIX TOOL HANDLERS ============================================
    ' ======================================================================================

    Private Sub fixButton_Click(sender As Object, e As EventArgs) Handles fixButton.Click
        If currentLooperFile Is Nothing OrElse currentLooperFile = "" Then
            MessageBox.Show(Me, "No .looper file is currently loaded.", "Fix Paths", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Check for broken paths
        Dim brokenPaths As New List(Of String)
        For Each item As ListViewItem In eventsList.Items
            If item.BackColor = errorColor Then
                brokenPaths.Add(item.SubItems(7).Text)
            End If
        Next

        If brokenPaths.Count = 0 Then
            MessageBox.Show(Me, "All file paths are valid. Nothing to fix.", "Fix Paths", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Check if Everything is available
        Try
            If Not EverythingAPI.IsEverythingAvailable() Then
                Dim startResult = MessageBox.Show(Me, "Everything search is not running. Would you like to start it?", "Fix Paths", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If startResult = DialogResult.Yes Then
                    Try
                        Dim everythingPath = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Everything", "Everything.exe")
                        If Not IO.File.Exists(everythingPath) Then
                            everythingPath = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Everything", "Everything.exe")
                        End If
                        If IO.File.Exists(everythingPath) Then
                            Process.Start(everythingPath, "-minimized")
                            Threading.Thread.Sleep(2000)
                        Else
                            MessageBox.Show(Me, "Could not find Everything.exe. Please install Everything (voidtools.com) to use Fix.", "Fix Paths", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                    Catch ex As Exception
                        MessageBox.Show(Me, "Could not start Everything: " & ex.Message, "Fix Paths", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End Try
                Else
                    Return
                End If
            End If
        Catch ex As DllNotFoundException
            MessageBox.Show(Me, "Everything64.dll not found. Please install Everything (voidtools.com) or place Everything64.dll next to Looper 2.exe.", "Fix Paths", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Log("Fix: Everything64.dll not found")
            Return
        Catch ex As Exception
            MessageBox.Show(Me, "Error checking Everything: " & ex.Message, "Fix Paths", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Log("Fix: Error - " & ex.Message)
            Return
        End Try

        ' Show confirm dialog
        Dim confirmMsg = brokenPaths.Count & " broken path(s) found. Search for correct locations using Everything?" & vbCrLf & vbCrLf
        For i = 0 To Math.Min(brokenPaths.Count - 1, 9)
            confirmMsg &= "• " & IO.Path.GetFileName(brokenPaths(i)) & vbCrLf
        Next
        If brokenPaths.Count > 10 Then confirmMsg &= "... and " & (brokenPaths.Count - 10) & " more"

        If MessageBox.Show(Me, confirmMsg, "Fix Broken Paths?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        ' Run the fix
        Log("Fix: Starting fix for " & brokenPaths.Count & " broken paths")
        Dim result = LooperFixer.FixLooperFile(currentLooperFile, True)
        Log("Fix: Completed - " & result.FixedEvents & " fixed, " & result.UnfixableEvents & " failed")

        ' Reload the file
        loadLooperFile(True, currentLooperFile)
    End Sub

    Private Sub fixToolButton_Click(sender As Object, e As EventArgs) Handles fixToolButton.Click
        Dim toolPath = IO.Path.Combine(Application.StartupPath, "LooperFixTool.exe")
        If IO.File.Exists(toolPath) Then
            Log("Fix-Tool: Launching LooperFixTool.exe")
            Process.Start(toolPath)
        Else
            MessageBox.Show(Me, "LooperFixTool.exe not found in application folder.", "Fix Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' ======================================================================================
    ' ================= PLAYLIST / EVENT LIST FUNCTIONS (merged from playlistWindow) ========
    ' ======================================================================================

    ' Show a MessageBox that stays on top of MPC-HC, then restore previous Z-order
    Private Function ShowTopMostMessageBox(msg As String, title As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultBtn As MessageBoxDefaultButton) As DialogResult
        Dim wasTopMost As Boolean = Me.TopMost ' remember previous state
        Me.TopMost = True
        Me.Activate()
        Dim result = MessageBox.Show(Me, msg, title, buttons, icon, defaultBtn)
        Me.TopMost = wasTopMost ' restore previous TopMost state
        If MPCHandle <> IntPtr.Zero Then SetForegroundWindow(MPCHandle) ' give focus back to MPC-HC
        Return result
    End Function

    Private Sub eventsList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles eventsList.SelectedIndexChanged
        selectedEventsButtonTriggers()
    End Sub

    Private Sub selectedEventsButtonTriggers()
        If eventsList.SelectedItems.Count > 0 Then
            saveButton.Enabled = True ' Enable the save button (so you can save the subset of events)

            If getMode() Then ' if we're in OFF or Loop mode
                If eventsList.SelectedItems.Count = 1 Then
                    modifyEventButton.Text = "Modify"
                    modifyEventButton.Enabled = True
                Else ' if we have 2 contiginous events selected, let's merge 'em
                    If eventsList.SelectedItems(0).Index + eventsList.SelectedItems.Count = eventsList.SelectedItems(eventsList.SelectedItems.Count - 1).Index + 1 Then
                        modifyEventButton.Text = "Merge"
                        modifyEventButton.Enabled = True
                    Else ' if we have scattered events, then disable the merging
                        modifyEventButton.Text = "Modify"
                        modifyEventButton.Enabled = False
                    End If
                End If
            Else ' if we're not in OFF or Loop mode
                modifyEventButton.Text = "Modify"
                modifyEventButton.Enabled = False
            End If

            If loopModeButton.Text <> "SHUFFLE" Then
                deleteEventButton.Enabled = True ' if we're not in Shuffle mode, then allow deleting the selected event
            Else
                deleteEventButton.Enabled = False ' don't allow deleting events in Shuffle mode
            End If
        Else ' if we have nothing selected, change the text for the modify button, but turn it off
            modifyEventButton.Text = "Modify"
            modifyEventButton.Enabled = False
            deleteEventButton.Enabled = False

            If isModified = False Then
                saveButton.Enabled = False ' if we haven't modified anything, then disallow this
            Else
                saveButton.Enabled = True ' otherwise make it enabled
            End If
        End If
    End Sub

    Private Sub eventsList_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles eventsList.MouseDoubleClick
        switchToLoopMode() ' double clicking an item returns to Loop mode ALWUS
        loadEvent(eventsList.SelectedItems(0).Index) ' load the currently selected event
    End Sub

    Public Function unInitialize(Optional forceQuit As Boolean = False) As Integer
        SendMessage(CMD_SEND.CMD_PAUSE) ' pause the player when going to the Quit looper window
        isQuitting = True ' set this to true to cause the Threads to skip checking position/hotkeys for now

        If isModified = True Then ' if isModified is true, then ask to save first
            If skipSaveConfirmations Then
                If autoSaveLooper And currentLooperFile IsNot Nothing Then
                    autoSaveCurrentFile() ' auto-save silently before quitting
                End If
                Return DialogResult.No ' skip the dialog entirely
            End If

            Dim MsgBoxMsg As String
            Dim MsgBoxButtons As MessageBoxButtons

            If forceQuit = False Then ' if we're just quitting normally (MPC-HC is still active and running)
                MsgBoxButtons = MessageBoxButtons.YesNoCancel

                MsgBoxMsg = "Do you want to save the current playlist before quitting?" _
                        & vbCrLf & vbCrLf & "Click Yes to save and quit" _
                        & vbCrLf & "Click No to quit without saving" _
                        & vbCrLf & "Click Cancel to go back to Looper without quitting"
            Else ' if we're being forced to quit (MPC-HC has closed, and we're quitting regardless)
                MsgBoxButtons = MessageBoxButtons.YesNo

                MsgBoxMsg = "Do you want to save the current playlist before quitting?" _
                        & vbCrLf & vbCrLf & "Click Yes to save and quit" _
                        & vbCrLf & "Click No to quit without saving"
            End If

            Dim returnValue = ShowTopMostMessageBox(MsgBoxMsg,
                                              "Save Playlist Before Quitting?",
                                              MsgBoxButtons,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button1)

            If returnValue = DialogResult.Yes Then
                Return saveLooperFile() ' save the looper file
            Else ' if the result is "No" or "Cancel", then just return that value
                Return returnValue
            End If
        Else ' if we're not set to "isModified", then just quit out
            Return DialogResult.No ' return a value, so we don't get an exception
        End If
    End Function

    Public Sub saveLastPlayedFile()
        Try
            Dim fileReader As String
            fileReader = My.Computer.FileSystem.ReadAllText(INIFile)

            Dim autoloadLastLooperVal As String = betweenTheLines(fileReader, "autoloadLastLooper=", vbCrLf, Nothing) ' should we launch the last looper file when closed?

            If autoloadLastLooperVal <> Nothing Then ' if the preference is "Nothing" then we're not set to enable it
                Dim startOffset = Strings.InStr(fileReader, "autoloadLastLooper=") ' the beginning of the old preference in the INI file
                Dim endOffset = Strings.InStr(startOffset, fileReader, vbCrLf) ' the end of the line above ^^

                Dim writingString = Strings.Left(fileReader, startOffset - 1) ' the INI file before the above preference
                writingString += "autoloadLastLooper=" & currentLooperFile & "|" & currentPlayingEvent & vbCrLf ' the new preference to be set
                writingString += Strings.Mid(fileReader, endOffset + 2) ' the rest of the INI file after that preference

                System.IO.File.WriteAllText(INIFile, writingString) ' write the "new" INI file
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' ======================================================================================
    ' ================= FILESYSTEM FUNCTIONS (playlist) ====================================
    ' ======================================================================================

    Private Sub setIsModified(ByVal setModified As Integer)
        If setModified = 1 Then ' if we've made changes, then draw a * in the title of the playlist to show that
            isModified = True ' we've modified something
            saveButton.Enabled = True

            If currentLooperFile = Nothing Then ' if we have nothing loaded, then we're in "Untitled" mode
                If Me.Text <> "Looper 2 - Untitled *" Then
                    Me.Text = "Looper 2 - Untitled *"
                End If
            Else ' if something IS loaded, then show that
                If Me.Text <> "Looper 2 - " & My.Computer.FileSystem.GetFileInfo(currentLooperFile).Name.ToString & " *" Then
                    Me.Text = "Looper 2 - " & My.Computer.FileSystem.GetFileInfo(currentLooperFile).Name.ToString & " *"
                End If
            End If
        Else ' if we haven't made changes, then DO NOT draw a * in the title of the playlist to show that
            isModified = False ' we haven't modified anything
            saveButton.Enabled = False

            If currentLooperFile = Nothing Then ' if we have nothing loaded, then we're in "Untitled" mode
                If Me.Text <> "Looper 2 - Untitled" Then
                    Me.Text = "Looper 2 - Untitled"
                End If
            Else ' if something IS loaded, then show that
                If Me.Text <> "Looper 2 - " & My.Computer.FileSystem.GetFileInfo(currentLooperFile).Name.ToString Then
                    Me.Text = "Looper 2 - " & My.Computer.FileSystem.GetFileInfo(currentLooperFile).Name.ToString
                End If
            End If
        End If
    End Sub

    Public Sub loadLooperFile(Optional clearEventList As Boolean = True, Optional looperFile As String = Nothing, Optional eventToPlay As Integer = -1)
        SendMessage(CMD_SEND.CMD_PAUSE) ' pause the player when going to the Load Looper window

        Dim confirmSave As Integer = DialogResult.No ' this doesn't specifically NEED to be no, it just needs to NOT be cancel

        If clearEventList = True Then ' if we're not loading a brand-new looper file
            If isModified = True Then ' and if the list has been modified
                If skipSaveConfirmations Then
                    If autoSaveLooper And currentLooperFile IsNot Nothing Then
                        autoSaveCurrentFile() ' auto-save silently
                    End If
                    confirmSave = DialogResult.No ' skip dialog
                Else
                    ' then ask if we want to save this list first before overwriting it
                    confirmSave = ShowTopMostMessageBox("Do you want to save the current playlist before opening another .looper file?" _
                                     & vbCrLf & vbCrLf & "Click Yes to save" _
                                     & vbCrLf & "No to continue without saving" _
                                     & vbCrLf & "Cancel to go back to Looper without doing anything",
                                                  "Save Playlist Before Opening?",
                                                  MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question,
                                                  MessageBoxDefaultButton.Button1)
                End If

                If confirmSave = DialogResult.Yes Then
                    confirmSave = saveLooperFile() ' save the looper file (or cancel the process, either by dialog or by file-choose-dialog)
                End If
            End If
        End If

        If confirmSave <> DialogResult.Cancel Then ' as long as you didn't click 'cancel' above, then...
            If looperFile = Nothing Then ' if we haven't specified a looper file to force opening, then ask for one
                Dim openDialogTitle As String

                If clearEventList = True Then
                    openDialogTitle = "Load a .looper File"
                Else
                    openDialogTitle = "Import a .looper File into the Current Playlist"
                End If

                Using looperFileDialog As New OpenFileDialog With {
                    .Title = openDialogTitle,
                    .Filter = "Looper files|*.looper",
                    .InitialDirectory = If(defaultLooperSavePath IsNot Nothing, defaultLooperSavePath, "")
                }

                    If looperFileDialog.ShowDialog = DialogResult.OK Then ' if we cancelled, this value will remain Nothing, and then nothing below will trigger
                        looperFile = looperFileDialog.FileName ' if we didn't cancel, then open the file selected
                    End If
                End Using
            End If

            If looperFile <> Nothing Then ' if we agreed to load a new file, then load it... otherwise, do nothing!
                currentLooperFile = looperFile ' if you choose a new file, then load that name in, but if not, then don't do anything else

                If clearEventList = True Then
                    totalNumberOfEvents = 100 ' reset the first index to 100

                    currentPlayingEvent = -1 ' we're not playing an event
                    actualPlayingEvent = -1 ' we're NOT playing an event

                    inSearchMode = False ' we're not in search mode t'neither
                    currentSearchPlayingEvent = -1 ' WE'RE NOT PLAYING AN EVENT! (and not in search mode)
                    ReDim eventsListArray(0, 0) ' delete the master array if it exists, because we're going to need to recreate it anyway

                    cancelRandomization() ' clear out the randomized list
                    eventsList.Items.Clear() ' remove all of the current events from the list

                    setIsModified(0) ' we're not modified, so sort that all out
                End If

                Dim fileReader As String
                fileReader = My.Computer.FileSystem.ReadAllText(currentLooperFile)

                Dim eventListArray() As String = Split(fileReader, vbCrLf) ' read all of the events into an array, split at line endings

                For a As Integer = 0 To eventListArray.Length - 1
                    If eventListArray(a) <> "" Then ' if the current read isn't a blank space (the extra line after 99% of .looper files)
                        Dim currentEvent() As String = Split(eventListArray(a), "|")
                        addEventFromFile(currentEvent(0), currentEvent(1), currentEvent(2), currentEvent(3), a)
                    End If
                Next

                fileReader = Nothing ' un-initialize the variable

                If clearEventList = True Then ' if we're regularly loading a file, then do these things
                    If autoplayFirstEvent = True Then
                        If eventsList.Items.Count > 1 Then
                            loadPrevNextEvent(1) ' jump to the first playable event (instead of loadEvent(), do loadPrevNextEvent() to seek to the next "good" event)
                        Else
                            loadEvent(0, True) ' we only have one event to load, so try to load that...
                        End If
                    Else ' if we're not set to autoplay the first event, but we have this option (auto-playing after leaving dialogs) turned on...
                        If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we're out of the dialog, so start playing
                    End If

                    If dontForceLooperModeonOpen = False Then
                        switchToLoopMode() ' if we're not set to not automatically go to looper mode, then switch it
                    Else
                        If loopModeButton.Text = "SHUFFLE" Then
                            switchToShuffleMode() ' force the shuffle mode generator to re-generate the randomized list
                        End If
                    End If

                    Dim totalEventsToCheck As Integer ' the total number of events the .looper file specified

                    If eventListArray(eventListArray.Length - 1) = "" Then
                        totalEventsToCheck = eventListArray.Length - 1 ' if the last item in the looper file is "", then subtract that from the events check
                    Else
                        totalEventsToCheck = eventListArray.Length ' otherwise, the events check is correct
                    End If

                    If eventsList.Items.Count <> totalEventsToCheck Then ' we have less items in the events list than we did in the .looper file, so something's wrong
                        MessageBox.Show((totalEventsToCheck - eventsList.Items.Count) & " event(s) stored in this .looper file have file paths that aren't defined (blank), so those events weren't added to the playlist.",
                                             "Warning!",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Warning,
                                             MessageBoxDefaultButton.Button1)
                    End If

                Else ' else, if we're merging .looper files, do these things...
                    If Not getMode() Then ' we're in Shuffle or Playlist modes
                        switchToLoopMode() ' force switch to Loop Mode
                        currentPlayingEvent += 1
                    End If

                    currentLooperFile = Nothing ' we're no longer operating out of the same file, so reset this
                    setIsModified(1)

                    If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we merged a .looper into the list, so start playing again
                End If

                eventListArray = Nothing ' un-initialize the variable
                tallyTotalList() ' calculate the time of all events in the events list
                updateEventCountLabel()
            Else
                If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled loading, so start playing again
            End If
        Else
            If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled loading (and saving), so start playing again
        End If
    End Sub

    Private Function findFile(Optional specificEvent As Integer = -1) As Integer
        Dim previousPath, pathToFind, pathName, pathExt As String
        Dim missingEvents As New List(Of Integer)

        SendMessage(CMD_SEND.CMD_PAUSE) ' force pause when checking to load events

        For a As Integer = 0 To eventsList.Items.Count - 1
            If eventsList.Items(a).BackColor = errorColor Then
                missingEvents.Add(a) ' make a list of events that have missing paths (so we don't need to load the entire events list every time)
            End If
        Next

        Dim findFilesPrompt As Integer = DialogResult.No ' if there's just one event, then we just need to find one, so we don't need the dialog asking

        If missingEvents.Count > 1 Then
            findFilesPrompt = ShowTopMostMessageBox("The current playlist has " & missingEvents.Count & " events that point" & vbCrLf &
            "to file paths that can not be located." & vbCrLf & vbCrLf &
            "Would you like to locate all of those missing files," & vbCrLf &
            "just the current event's missing file," & vbCrLf &
            "or skip looking for files for now?" & vbCrLf & vbCrLf &
            "Click Yes to find missing files for the entire playlist" & vbCrLf &
            "Click No to find only this event's missing file" & vbCrLf &
            "Click Cancel to cancel finding missing files for now",
                                  "Missing Files!",
                                  MessageBoxButtons.YesNoCancel,
                                  MessageBoxIcon.Question,
                                  MessageBoxDefaultButton.Button1)

            If findFilesPrompt = DialogResult.Yes Then
                specificEvent = -1 ' look for all of the files and not just one specific one
            End If
        End If

        If findFilesPrompt <> DialogResult.Cancel Then ' if we didn't cancel out of finding events (above)
            previousPath = Nothing

            For a As Integer = 0 To missingEvents.Count - 1 ' iterate through the empty paths now
                pathToFind = eventsList.Items(missingEvents(a)).SubItems(7).Text ' the original "missing" file path
                pathName = My.Computer.FileSystem.GetFileInfo(pathToFind).Name ' the name of the file (minus the directory)
                pathExt = Mid(My.Computer.FileSystem.GetFileInfo(pathToFind).Extension, 2) ' the extension of the file

                If specificEvent = -1 Or specificEvent = missingEvents(a) Then ' if the event we're requesting is this specific one OR isn't a specific event...
                    If eventsList.Items(missingEvents(a)).BackColor = errorColor Then ' if this event still has an error, then...
                        ' Set the initial directory to the missing file's original directory (or previous found path)
                        Dim initialDir As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                        If previousPath IsNot Nothing Then
                            initialDir = previousPath
                        Else
                            Try
                                Dim origDir = My.Computer.FileSystem.GetFileInfo(pathToFind).DirectoryName
                                If My.Computer.FileSystem.DirectoryExists(origDir) Then initialDir = origDir
                            Catch
                            End Try
                        End If

                        Using fileFind As New OpenFileDialog With {
                           .Title = "Where is the file originally found at " & pathToFind & " now?",
                           .InitialDirectory = initialDir,
                           .FileName = pathName,
                           .Filter = "Matching|" & pathName & "|" & pathExt & " Files|*." & pathExt & "|All Files|*.*"
                            }

                            If fileFind.ShowDialog = DialogResult.OK Then
                                setIsModified(1)
                                previousPath = My.Computer.FileSystem.GetFileInfo(fileFind.FileName).DirectoryName.ToString ' check this against the next file

                                ' Get the old broken directory to fix all events sharing it
                                Dim brokenDir As String = My.Computer.FileSystem.GetFileInfo(pathToFind).DirectoryName

                                For b As Integer = 0 To missingEvents.Count - 1 ' scan through the rest of the missing items to see if paths are restored
                                    If eventsList.Items(missingEvents(b)).SubItems(7).Text = pathToFind Then ' if this event matches the exact "missing" file path
                                        eventsList.Items(missingEvents(b)).SubItems(7).Text = fileFind.FileName ' replace it with the newly found file
                                        eventsList.Items(missingEvents(b)).BackColor = foundColor
                                        eventsList.Items(missingEvents(b)).ForeColor = Color.White
                                    Else ' if this event doesn't match the same file, check if it's in the same broken directory
                                        Dim otherPathToFind As String = eventsList.Items(missingEvents(b)).SubItems(7).Text
                                        Dim otherPathName As String = My.Computer.FileSystem.GetFileInfo(otherPathToFind).Name
                                        Dim otherPathDir As String = My.Computer.FileSystem.GetFileInfo(otherPathToFind).DirectoryName

                                        ' Fix all events that share the same broken directory
                                        If My.Computer.FileSystem.GetFileInfo(previousPath & "\" & otherPathName).Exists Then
                                            eventsList.Items(missingEvents(b)).SubItems(7).Text = previousPath & "\" & otherPathName
                                            eventsList.Items(missingEvents(b)).BackColor = foundColor
                                            eventsList.Items(missingEvents(b)).ForeColor = Color.White
                                        End If
                                    End If
                                Next
                            Else ' we didn't find a result, or cancelled, so pass that back to the loadEvent() Sub
                                If specificEvent <> -1 Or a = missingEvents.Count - 1 Then ' if we're either looking at a single event or the last event
                                    Return DialogResult.Cancel ' we cancelled out of finding this file
                                End If
                            End If
                        End Using
                    End If
                End If
            Next
        End If

        Return findFilesPrompt ' if we cancelled, let's let the loadEvent() procedure know it
    End Function

    Public Function saveLooperFile(Optional saveSubset As Boolean = False) As Integer
        If saveSubset = False And isModified = False Then
            Return DialogResult.Cancel ' if we're not modified, and we're saving the entire file, quit out (as there's nothing to save)
        End If

        Dim checkOverwrite As Integer = DialogResult.Ignore

        If eventsList.Items.Count > 0 Then ' if we have nothing in the playlist, there's no reason to save it~!
            SendMessage(CMD_SEND.CMD_PAUSE) ' pause the player when going to the Save Looper window

            Dim savefile As String = Nothing
            Dim savingList As New List(Of Integer)

            If saveSubset = True Then ' if you want to save the entire playlist, then just add all the items to the list to save
                For a As Integer = 0 To eventsList.SelectedIndices.Count - 1
                    savingList.Add(eventsList.SelectedIndices(a))
                Next
            Else ' if we only want to save a few of them, then just add those ones
                For a As Integer = 0 To eventsList.Items.Count - 1
                    savingList.Add(a)
                Next
            End If

            If currentLooperFile = Nothing Or saveSubset = True Then
                savefile = returnSaveFile(saveSubset) ' if saveSubset is true, this will change the title on the Save... prompt
            Else
                If skipSaveConfirmations Then
                    checkOverwrite = DialogResult.Yes ' skip the overwrite confirmation
                    savefile = currentLooperFile
                Else
                    checkOverwrite = ShowTopMostMessageBox("Are you sure you want to overwrite the current .looper file?" & vbCrLf & vbCrLf &
                                                "Choose Yes to overwrite the current .looper file" & vbCrLf &
                                                "No to save to a New .looper file" & vbCrLf &
                                                "Or Cancel to abort saving completely",
                                                  "Overwrite Current File?",
                                                  MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question,
                                                  MessageBoxDefaultButton.Button1)

                    If checkOverwrite = DialogResult.Yes Then
                        savefile = currentLooperFile
                    ElseIf checkOverwrite = DialogResult.No Then
                        savefile = returnSaveFile()
                    ElseIf checkOverwrite = DialogResult.Cancel Then
                        ' don't do anything, because we chose to cancel, so saveFile = Nothing
                    End If
                End If
            End If

            If savefile <> Nothing Then
                Dim writeString As String

                ' Write the file using the Streamwriter process, to save to hidden files (if we come across those) -
                ' The other method (the normal, one-code-line method) comes up with an AccessException when we try
                ' to do that, so we need to do it this way (the *proper* way, innit?)
                Using writeFile As IO.FileStream = IO.File.Open(savefile, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                    Using writer As IO.StreamWriter = New IO.StreamWriter(writeFile, System.Text.Encoding.Unicode)
                        For a As Integer = 0 To savingList.Count - 1
                            writeString = Nothing

                            If eventsList.Items(savingList(a)).SubItems(2).Text <> "100" Then ' if the speed isn't 100, then add that to the name in the looper file
                                writeString = writeString & "<S:" & eventsList.Items(savingList(a)).SubItems(2).Text & ">"
                            End If

                            If eventsList.Items(savingList(a)).SubItems(3).Text <> "1" Then ' if the repeats are more than 1, then add that to the name in the looper file
                                writeString = writeString & "<L:" & eventsList.Items(savingList(a)).SubItems(3).Text & ">"
                            End If


                            writeString = writeString & eventsList.Items(savingList(a)).SubItems(1).Text & "|" &
                            eventsList.Items(savingList(a)).SubItems(4).Text & "|" &
                            eventsList.Items(savingList(a)).SubItems(5).Text & "|" &
                            eventsList.Items(savingList(a)).SubItems(7).Text

                            writer.WriteLine(writeString) ' write this specific event to the .looper file
                        Next

                        writeFile.SetLength(writeFile.Position) ' set the EOF
                    End Using

                    writeFile.Close() ' close file for writing
                End Using

                If isQuitting = False Then
                    If saveSubset = False Then ' if we are saving or saving a new Looper file, then show it - otherwise, you just saved a subclip
                        currentLooperFile = savefile ' change the current looper file name
                        setIsModified(0) ' we saved a file, so we're no longer in modified state
                        If osdOnSave Then SendOSD("Looper Saved")

                        For a As Integer = 0 To eventsList.Items.Count - 1
                            If eventsList.Items(a).BackColor <> errorColor Then ' only if the events aren't red (missing)
                                eventsList.Items(a).BackColor = foundColor
                                eventsList.Items(a).ForeColor = Color.White
                            End If

                            eventsList.Items(a).SubItems(0).Text = CStr(100 + a) ' re-organize the current order as the "correct" order
                            eventsList.Items(a).Selected = False ' de-select this item when re-saving it
                        Next
                    Else
                        Dim loadSubset = ShowTopMostMessageBox("You just saved a selection of events to a new .looper file." & vbCrLf &
                                            "Would you like to open that new file?" & vbCrLf & vbCrLf &
                                            "Choose Yes to open the new .looper file" & vbCrLf &
                                            "Choose No to continue using the current file",
                                              "Open Looper file?",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button2)

                        If loadSubset = DialogResult.Yes Then
                            loadLooperFile(True, savefile)
                        End If
                    End If
                End If

                If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we finished saving, so start playing again
            Else
                checkOverwrite = DialogResult.Cancel
                If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled saving, so start playing again
            End If
        End If

        Return checkOverwrite
    End Function

    Private Function returnSaveFile(Optional saveSubset As Boolean = False) As String
        Dim saveTitle As String

        If saveSubset = True Then
            saveTitle = "Save Current Selection of Events as New Looper File..."
        Else
            saveTitle = "Save New Looper file..."
        End If

        Dim saveFileDialog As New SaveFileDialog With {
            .AddExtension = True,
            .DefaultExt = ".looper",
            .OverwritePrompt = True,
            .Filter = "Looper files|*.looper",
            .Title = saveTitle
        }

        ' ----------------- CHECK TO SAVE SIDECAR FILE -----------------
        Dim filesMatching As Integer = 1
        Dim checkAgainst As String = eventsList.Items(0).SubItems(7).Text ' the file path of the first event

        For a As Integer = 1 To eventsList.Items.Count - 1
            If eventsList.Items(a).SubItems(7).Text = checkAgainst Then filesMatching += 1 ' if the filename is the same in this event, then increment counter
        Next

        If filesMatching = eventsList.Items.Count Then ' if the filenames are the same for every event, then default to saving a sidecar file
            saveFileDialog.FileName = My.Computer.FileSystem.GetFileInfo(checkAgainst).Name & ".looper"
            saveFileDialog.InitialDirectory = My.Computer.FileSystem.GetFileInfo(checkAgainst).Directory.ToString
        ElseIf defaultLooperSavePath IsNot Nothing AndAlso defaultLooperSavePath <> "" Then
            saveFileDialog.InitialDirectory = defaultLooperSavePath ' use the default save path from preferences
        End If

        ' ----------------- SHOW THE DIALOG AND RETURN -----------------
        If saveFileDialog.ShowDialog(Me) = DialogResult.OK Then
            Dim result = saveFileDialog.FileName
            saveFileDialog.Dispose()
            Return result
        Else
            saveFileDialog.Dispose()
            Return Nothing
        End If
    End Function

    Private Function checkAlternateFileExists(ByVal filePath As String) As String
        Dim checkFile = My.Computer.FileSystem.GetFileInfo(filePath) ' check to see if the currently specified file exists

        If checkFile.Exists Then
            Return filePath
        Else ' we need to find the file elsewhere
            Dim looperBase = My.Computer.FileSystem.GetFileInfo(currentLooperFile).Directory.ToString ' get the .looper file's base path

            If My.Computer.FileSystem.GetFileInfo(looperBase & "\" & checkFile.Name.ToString).Exists Then ' if the file exists in the looper directory
                Return looperBase & "\" & checkFile.Name.ToString ' then mark the file as *existing*, but in a different directory
            Else
                Return "Not Found" ' mark the file as not existing in the original *or* the .looper file directory
            End If
        End If
    End Function

    ' ======================================================================================
    ' ================= EVENT FUNCTIONS ====================================================
    ' ======================================================================================

    Private Function returnListViewItem(ByVal eventName As String,
                                        ByVal eventSpeed As String,
                                        ByVal eventRepeats As String,
                                        ByVal eventINPoint As String,
                                        ByVal eventOUTPoint As String,
                                        ByVal eventPath As String,
                                        Optional eventPosX As String = "0.5",
                                        Optional eventPosY As String = "0.5",
                                        Optional eventZoomX As String = "1",
                                        Optional eventZoomY As String = "1",
                                        Optional eventNum As String = Nothing,
                                        Optional newDur As String = Nothing) As ListViewItem

        Dim newItemArray(11) As String

        If eventNum = Nothing Then
            newItemArray(0) = CStr(totalNumberOfEvents) ' find the event number based on the current "new" event number
        Else
            newItemArray(0) = eventNum ' use the specified number as the new event number
        End If

        newItemArray(1) = eventName
        newItemArray(2) = eventSpeed
        newItemArray(3) = eventRepeats
        newItemArray(4) = eventINPoint
        newItemArray(5) = eventOUTPoint

        If newDur = Nothing Then ' if we haven't specified a duration, then let's calculate it
            If eventOUTPoint <> Nothing Then ' if the out point isn't "nothing", then calculate the duration
                newItemArray(6) = NumberToTimeString(TimeStringToNumber(eventOUTPoint) - TimeStringToNumber(eventINPoint))
            Else ' if we don't know what the duration is (there's no out point set), then it's endless, man...
                newItemArray(6) = Nothing
            End If
        Else
            newItemArray(6) = newDur ' we already calculated the duration in an earlier step (for combining events), so just use that value
        End If

        newItemArray(7) = eventPath
        newItemArray(8) = eventPosX
        newItemArray(9) = eventPosY
        newItemArray(10) = eventZoomX
        newItemArray(11) = eventZoomY

        Dim newEvent As New ListViewItem(newItemArray)
        Return newEvent
    End Function


    Private Sub addEventFromFile(ByVal eventName As String, ByVal eventINPoint As String, ByVal eventOutPoint As String,
                         ByVal eventFilePath As String, Optional newEventCount As Integer = 0)

        ' if the eventFilePath returned from the Looper file has nothing in it, we can't check to see if the file exists
        ' so we have to skip adding this event
        If eventFilePath <> Nothing Then
            ' Color the incoming .looper file as different colors based on file availability
            Dim fileExists As String = checkAlternateFileExists(eventFilePath)
            Dim newEventColor As Color
            Dim newEventForeColor As Color = Color.White

            If fileExists = "Not Found" Then
                newEventColor = errorColor
                newEventForeColor = errorForeColor
            ElseIf fileExists = eventFilePath Then
                newEventColor = foundColor
            Else
                newEventColor = relativeColor
                eventFilePath = fileExists
            End If

            Dim customZoom = betweenTheLines(eventName, "<Z:", ">", "0.5,0.5,1.0,1.0").Split(Convert.ToChar(","))

            Dim newEvent As ListViewItem = returnListViewItem(removeParams(eventName), betweenTheLines(eventName, "<S:", ">", "100"),
                                                              betweenTheLines(eventName, "<L:", ">", "1"), eventINPoint, eventOutPoint, eventFilePath,
                                                              customZoom(0), customZoom(1), customZoom(2), customZoom(3))

            If insertItem = True Then
                If eventsList.SelectedItems.Count > 0 Then ' if something is actually selected
                    Dim newIndex = eventsList.SelectedItems.Item(eventsList.SelectedItems.Count - 1).Index + 1 + newEventCount
                    eventsList.Items.Insert(newIndex, newEvent)
                    eventsList.Items(newIndex).BackColor = newEventColor
                    eventsList.Items(newIndex).ForeColor = newEventForeColor
                    If newEventColor = errorColor Then
                        eventsList.Items(newIndex).SubItems(7).ForeColor = errorFileCellColor
                        eventsList.Items(newIndex).SubItems(7).Font = New Font(normalFont, FontStyle.Italic)
                    End If
                Else
                    eventsList.Items.Add(newEvent)
                    eventsList.Items(eventsList.Items.Count - 1).BackColor = newEventColor
                    eventsList.Items(eventsList.Items.Count - 1).ForeColor = newEventForeColor
                    If newEventColor = errorColor Then
                        eventsList.Items(eventsList.Items.Count - 1).SubItems(7).ForeColor = errorFileCellColor
                        eventsList.Items(eventsList.Items.Count - 1).SubItems(7).Font = New Font(normalFont, FontStyle.Italic)
                    End If
                End If
            Else
                eventsList.Items.Add(newEvent)
                eventsList.Items(eventsList.Items.Count - 1).BackColor = newEventColor
                eventsList.Items(eventsList.Items.Count - 1).ForeColor = newEventForeColor
                If newEventColor = errorColor Then
                    eventsList.Items(eventsList.Items.Count - 1).SubItems(7).ForeColor = errorFileCellColor
                    eventsList.Items(eventsList.Items.Count - 1).SubItems(7).Font = New Font(normalFont, FontStyle.Italic)
                End If
            End If
        End If

        totalNumberOfEvents += 1 ' increment the total number of events counter (to add one at the end)
    End Sub

    Public Sub addEvent()
        If currentPlayingFile <> Nothing Then ' if MPC-HC isn't playing anything, then don't DO anything
            If inSearchMode = True Then
                restoreFromSearch() 'if we're in Search mode and decide to add an event, get out of Search mode first
            End If

            SendMessage(CMD_SEND.CMD_GETNOWPLAYING) ' force refresh of NOWPLAYING info

            eventsList.ListViewItemSorter = Nothing ' add the event at the very end of the list if the list is sorted

            ' Filename/Event Number Substitution (suggestion by @inBytes on GitHub) - for example, having:
            ' "[#]_[[F]] New Event" as the default name will make events called
            ' "1_[file.mp4] New Event" as the 1st event in the list
            ' "2_[file.mp4] New Event" as the 2nd event in the list, etc.

            ' replace [F] with the currently playing file's filename
            Dim newEventName As String = Replace(newEventString, "[F]", System.IO.Path.GetFileName(currentPlayingFile))
            ' replace [f] with the currently playing file's filename (minus the extension)
            newEventName = Replace(newEventName, "[f]", System.IO.Path.GetFileNameWithoutExtension(currentPlayingFile))
            ' replace [#] with the current # in the events list
            newEventName = Replace(newEventName, "[#]", (eventsList.Items.Count + 1).ToString())

            ' add the event to the events list
            Dim newEvent = returnListViewItem(newEventName, speedSlider.Value.ToString, "1", inTF.Text, outTF.Text,
                                              currentPlayingFile)
            Dim newIndex As Integer

            If insertItem = True Then
                If eventsList.SelectedItems.Count > 0 Then ' if something is actually selected
                    newIndex = eventsList.SelectedItems.Item(eventsList.SelectedItems.Count - 1).Index + 1 ' the new index will be where the item was inserted, plus one
                    eventsList.Items.Insert(eventsList.SelectedItems.Item(eventsList.SelectedItems.Count - 1).Index + 1, newEvent)
                Else ' if nothing is selected, then just add them as you normally would
                    newIndex = eventsList.Items.Count ' the new index will be the count, minus one - or in this case, literally just the count, before the one added
                    eventsList.Items.Add(newEvent)
                End If
            Else ' if nothing is selected, and "inserting" is turned off, then just add them to the end of the list
                newIndex = eventsList.Items.Count ' the new index will be the count, minus one - or in this case, literally just the count, before the one added
                eventsList.Items.Add(newEvent)
            End If

            eventsList.BeginUpdate() ' suspend UI redraws during batch updates

            currentPlayingEvent = newIndex ' mark the newly inserted (or added) event as the current playing event
            setIsModified(1) ' we've added something, so we're now modified

            eventsList.SelectedIndices.Clear() ' clear all other selections
            highlightEvent(newIndex) ' mark the new event as the currently playing event by bolding the text
            eventsList.Items(newIndex).EnsureVisible()
            eventsList.Items(newIndex).Selected = True

            eventsList.EndUpdate() ' resume UI redraws

            tallyTotalList() ' re-tabulate the event list tally
            updateEventCountLabel()
            totalNumberOfEvents += 1 ' increment the total number of events counter (to add one at the end)

            If skipEventNameEditor = False Then
                editListViewItem(newIndex) ' go into the text editor
            End If

            Dim didAutoSave As Boolean = False
            If autoSaveLooper And currentLooperFile IsNot Nothing Then
                autoSaveCurrentFile(True) ' auto-save after adding event, suppress OSD
                didAutoSave = True
            End If

            If osdOnAddEvent OrElse (osdOnSave AndAlso didAutoSave) Then
                Dim osdMsg As String = "Event Added: " & eventsList.Items(newIndex).SubItems(1).Text
                If didAutoSave AndAlso osdOnSave Then osdMsg &= " — Saved"
                SendOSD(osdMsg)
            End If
        End If
    End Sub

    Public Sub deleteEvents()
        If eventsList.Items.Count > 0 Then ' if the events list has more than one event
            SendMessage(CMD_SEND.CMD_PAUSE) ' pause the player when asking if you want to delete files

            Dim selectedItemsCount = eventsList.SelectedItems.Count ' get the count of events we're choosing to delete

            If selectedItemsCount > 0 Then ' if we have *at least* one item selected, then try to delete it
                Dim confirmDelete As DialogResult
                Dim MsgBoxMsg As String ' if we're in Search mode, then write a little description that tells you that

                If selectedItemsCount = eventsList.Items.Count Then ' we're choosing to delete ALL the events (which is basically the same as "Clear List")
                    If inSearchMode = False Then
                        MsgBoxMsg =
                            "You've chosen to delete everything in the current playlist!" & vbCrLf & vbCrLf &
                            "Deleting ALL of the events from the playlist is the same thing" & vbCrLf &
                            "as clicking on the Clear All button.  All of the events will be" & vbCrLf &
                            "removed and Looper will start a brand-new session." & vbCrLf & vbCrLf &
                            "Would you like to remove all of the current events and" & vbCrLf &
                            "start a completely New Looper session?"
                    Else
                        MsgBoxMsg =
                            "NOTE: You're currently in Search Mode and" & vbCrLf &
                            "You've chosen to remove everything in the current playlist!" & vbCrLf & vbCrLf &
                            "Removing all of these events in Search Mode will clear" & vbCrLf &
                            "the search and go back to Normal (non-Search) mode, but will" & vbCrLf &
                            "not actually delete any of the events from the playlist." & vbCrLf & vbCrLf &
                            "Would you like to do that?"
                    End If

                    confirmDelete = ShowTopMostMessageBox(MsgBoxMsg,
                                              "Clear Entire Playlist?",
                                              MessageBoxButtons.OKCancel,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button1)
                Else ' we have selected some items, but not ALL of them
                    Dim titleText As String ' change the title if we're in Search mode
                    If inSearchMode = False Then
                        MsgBoxMsg = "Are you sure that you want to delete "
                        titleText = "Delete Events?"
                    Else
                        MsgBoxMsg =
                        "NOTE: You're currently in Search Mode" & vbCrLf & vbCrLf &
                        "In Search Mode, if you remove an event, it won't actually delete it" & vbCrLf &
                        "from the master playlist, just from this list of search results." & vbCrLf & vbCrLf &
                        "If you want to delete the event from the master playlist completely," & vbCrLf &
                        "clear the search (by clicking on the X to the right of the Search button)" & vbCrLf &
                        "before attempting to remove the event." & vbCrLf & vbCrLf &
                        "Are you sure that you want to remove "
                        titleText = "Remove Events?"
                    End If

                    If selectedItemsCount > 1 Then ' we have more than one event selected
                        MsgBoxMsg += "these " & selectedItemsCount & " events?"
                    Else
                        MsgBoxMsg += "this event?"
                    End If

                    confirmDelete = ShowTopMostMessageBox(MsgBoxMsg,
                                              titleText,
                                              MessageBoxButtons.OKCancel,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button1)
                End If

                If confirmDelete = DialogResult.OK Then
                    If selectedItemsCount = eventsList.Items.Count Then
                        If inSearchMode = False Then
                            clearEventsList()
                        Else
                            restoreFromSearch()
                        End If
                    Else
                        eventsList.BeginUpdate() ' prevent UI flicker during bulk delete
                        For a As Integer = selectedItemsCount - 1 To 0 Step -1 ' delete items from the end to the back, so the index stays correct
                            eventsList.Items.RemoveAt(eventsList.SelectedItems.Item(a).Index)
                        Next
                        eventsList.EndUpdate()

                        setIsModified(1)
                        tallyTotalList() ' re-tabulate the event list tally
                        updateEventCountLabel()

                        If autoSaveLooper And currentLooperFile IsNot Nothing Then
                            autoSaveCurrentFile() ' auto-save after delete
                        End If
                    End If
                End If
            End If

            If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we finished deleting (or cancelling), so start playing again
        End If
    End Sub

    Public Sub modifyEvent(Optional theEvent As Integer = -1)
        If eventsList.Items.Count > 0 Then ' if we have an event in the playlist
            If theEvent = -1 Then ' if we're not set to modify a specific event
                theEvent = eventsList.SelectedItems(0).Index ' set the event to the currently selected event
            End If

            eventsList.Items(theEvent).SubItems(2).Text = speedSlider.Value.ToString ' the new speed for this event
            eventsList.Items(theEvent).SubItems(4).Text = inTF.Text ' the new IN point for this event
            eventsList.Items(theEvent).SubItems(5).Text = outTF.Text ' the new OUT point for this event
            eventsList.Items(theEvent).SubItems(6).Text = NumberToTimeString(TimeStringToNumber(outTF.Text) - TimeStringToNumber(inTF.Text)) ' the new duration for this event

            setIsModified(1) ' we've modified the event
            tallyTotalList() ' re-tabulate the event list tally

            If autoSaveLooper And currentLooperFile IsNot Nothing Then
                autoSaveCurrentFile() ' auto-save after modify
            End If
        End If
    End Sub

    Private Sub autoSaveCurrentFile(Optional suppressOSD As Boolean = False)
        ' Silently save the current looper file without any dialogs
        If currentLooperFile IsNot Nothing And eventsList.Items.Count > 0 Then
            Dim sb As New StringBuilder(eventsList.Items.Count * 120) ' pre-allocate buffer

            For a As Integer = 0 To eventsList.Items.Count - 1
                Dim item = eventsList.Items(a)
                Dim speed = item.SubItems(2).Text
                Dim repeats = item.SubItems(3).Text

                If speed <> "100" Then sb.Append("<S:").Append(speed).Append(">")
                If repeats <> "1" Then sb.Append("<L:").Append(repeats).Append(">")


                sb.Append(item.SubItems(1).Text).Append("|")
                sb.Append(item.SubItems(4).Text).Append("|")
                sb.Append(item.SubItems(5).Text).Append("|")
                sb.AppendLine(item.SubItems(7).Text)
            Next

            Using writeFile As IO.FileStream = IO.File.Open(currentLooperFile, IO.FileMode.Create, IO.FileAccess.Write)
                Using writer As New IO.StreamWriter(writeFile, System.Text.Encoding.Unicode)
                    writer.Write(sb.ToString())
                End Using
            End Using

            setIsModified(0) ' we just saved, so clear the modified flag
            If osdOnSave AndAlso Not suppressOSD Then SendOSD("Looper Saved")
        End If
    End Sub

    Private Sub mergeEvents()
        Dim newInTime As String = eventsList.SelectedItems(0).SubItems(4).Text ' the IN time from the first event is the new IN time
        Dim newOutTime As String = eventsList.SelectedItems(eventsList.SelectedItems.Count - 1).SubItems(5).Text ' the OUT time from the last event is the new OUT time
        Dim newDur As String = NumberToTimeString(TimeStringToNumber(newOutTime) - TimeStringToNumber(newInTime))

        Dim mergeEventsResult = ShowTopMostMessageBox("Do you want to merge these " & eventsList.SelectedItems.Count & " events together into one event?" & vbCrLf & vbCrLf &
                                        "The event's new IN time will be: " & newInTime & vbCrLf &
                                        "The event's new OUT time will be: " & newOutTime & vbCrLf &
                                        "The event's new duration will be: " & newDur,
                                              "Merge Events?",
                                              MessageBoxButtons.OKCancel,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button1)

        If mergeEventsResult = DialogResult.OK Then
            eventsList.BeginUpdate() ' stop updating the events list so we can do the next step without glitching the view out

            ' ----------------- TRY TO GET OLD INDEX FROM NAME (betweenTheLines()) - IF NOT, THEN JUST FALL BACK TO GETTING FROM EVENT # -----------------
            Dim newEventName As String = "Events ["
            newEventName += betweenTheLines(eventsList.SelectedItems(0).SubItems(1).Text, newEventName, "-", (CInt(eventsList.SelectedItems(0).SubItems(0).Text) - 100).ToString)
            newEventName += "-" & (CInt(eventsList.SelectedItems(eventsList.SelectedItems.Count - 1).SubItems(0).Text) - 100) & "]"

            ' ----------------- BUILD NEW EVENT FROM THE OTHER EVENTS -----------------
            Dim combinedEvent = returnListViewItem(newEventName, "100", "1", newInTime, newOutTime,
                                                   eventsList.SelectedItems(0).SubItems(7).Text,
                                                   "0.5", "0.5", "1.0", "1.0",
                                                   eventsList.SelectedItems(eventsList.SelectedItems.Count - 1).SubItems(0).Text,
                                                   newDur)
            Dim newEventIdx As Integer = eventsList.SelectedItems(0).Index ' the index to insert the combined event into (get this before deleting below!)

            For a As Integer = eventsList.SelectedItems.Count - 1 To 0 Step -1
                eventsList.Items.RemoveAt(eventsList.SelectedItems.Item(a).Index) ' delete the old events from the events list
            Next

            eventsList.Items.Insert(newEventIdx, combinedEvent) ' insert the combined "new" event in the old event's place
            eventsList.Items(newEventIdx).BackColor = foundColor
            eventsList.Items(newEventIdx).ForeColor = Color.White

            eventsList.EndUpdate() ' start updating the events list again after we're done modifying it
            setIsModified(1)
            updateEventCountLabel()
        End If
    End Sub

    Private Sub highlightEvent(Optional theEvent As Integer = -1)
        For a As Integer = 0 To eventsList.Items.Count - 1
            If eventsList.Items(a).BackColor = playingColor Then
                If eventsList.Items(a).SubItems(7).Text <> "" AndAlso Not IO.File.Exists(eventsList.Items(a).SubItems(7).Text) Then
                    eventsList.Items(a).BackColor = errorColor
                    eventsList.Items(a).ForeColor = errorForeColor
                Else
                    eventsList.Items(a).BackColor = foundColor
                    eventsList.Items(a).ForeColor = normalForeColor
                End If
            End If
            eventsList.Items(a).Font = normalFont
        Next

        If theEvent <> -1 Then
            eventsList.Items(theEvent).BackColor = playingColor
            eventsList.Items(theEvent).ForeColor = playingForeColor
            eventsList.Items(theEvent).Font = boldFont
            eventsList.Items(theEvent).EnsureVisible()
        End If
    End Sub

    Public Sub checkAgainstCurrentPlayingEvent()
        If currentPlayingEvent <> -1 Then
            Dim currentPlayingEvent_In As String = eventsList.Items(currentPlayingEvent).SubItems(4).Text
            Dim currentPlayingEvent_Out As String = eventsList.Items(currentPlayingEvent).SubItems(5).Text

            If currentPlayingEvent_In = inTF.Text And currentPlayingEvent_Out = outTF.Text Then
                highlightEvent(currentPlayingEvent) ' if everything matches the IN and OUT of the current event, then keep the highlighitng
            Else
                highlightEvent() ' otherwise, if something doesn't mesh, then clear the highlighting
            End If
        End If
    End Sub

    Private Function loadEvent(ByVal selecteditem As Integer, Optional checkValid As Boolean = False) As Boolean
        If stilLLoading = False Then
            stilLLoading = True ' hold off on loading for a second, because we're trying to load something already

            currentPlayingEvent = selecteditem

            If loopModeButton.Text = "SHUFFLE" Then
                actualPlayingEvent = randomNumberList(selecteditem) ' the actual event to load in Shuffle mode (relative to the current events list)
                selecteditem = actualPlayingEvent ' set the selectedItem to the value from ^^^
            End If

            If eventsList.Items(selecteditem).BackColor = errorColor Then ' and the event we're on has an error, then
                If checkValid = True Then ' if we're forcing a "good event check"
                    stilLLoading = False ' we're not going any fu(a)rther
                    Return False ' if we didn't successfully load an event, then return false
                Else ' we're not forcing the "good event check" (in other words, we clicked on the event directly)
                    Dim fileFound = findFile(selecteditem) ' find the file(s) you're missing, and re-connect any other paths that match that file

                    ' TODO: if we cancel on a series of files to find, we still don't do anything.  Maybe check to see if we're fixed here?
                    If fileFound = DialogResult.Cancel Then ' we cancelled finding a file, so don't do anything
                        If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled finding an event, so keep on keepin' on
                        stilLLoading = False
                        Return False
                    End If
                End If
            End If

            highlightEvent(selecteditem) ' put currently playing event as bold

            loopsRepeatTF.Text = eventsList.Items(selecteditem).SubItems(3).Text ' how many repeats this loop has in Playlist mode

            inTF.Text = eventsList.Items(selecteditem).SubItems(4).Text
            outTF.Text = eventsList.Items(selecteditem).SubItems(5).Text

            If currentPlayingFile <> eventsList.Items(selecteditem).SubItems(7).Text Then ' we need to load another file
                If eventsList.Items(selecteditem).BackColor <> errorColor Then ' if there's no issue, then load the file
                    SendMessage(CMD_SEND.CMD_OPENFILE, eventsList.Items(selecteditem).SubItems(7).Text)
                    clearINOUTPoint = False

                    loadingEvent = True
                    Integer.TryParse(eventsList.Items(selecteditem).SubItems(2).Text, loadingEvent_Speed)
                Else
                    ' do we need to do anything here?
                End If
            Else ' we're still working with the same file
                Dim theSpeed As Integer

                Integer.TryParse(eventsList.Items(selecteditem).SubItems(2).Text, theSpeed)

                SendMessage(CMD_SEND.CMD_SETPOSITION, CStr(TimeStringToNumber(eventsList.Items(selecteditem).SubItems(4).Text) - 0.5))
                setSpeed(theSpeed)

                stilLLoading = False
            End If

            If pausePlaybackOnLoadEvent And eventsList.Items(selecteditem).BackColor <> errorColor Then
                SendMessage(CMD_SEND.CMD_PAUSE)
            End If

            If getMode() Then ' we're in OFF or Loop mode
                SetForegroundWindow(MPCHandle)
            End If
        End If

        Return True ' if we made it here, we were successful in loading a file
    End Function

    Public Sub loadPrevNextEvent(ByVal nextOrPrev As Integer)
        Dim numOfEvents As Integer = eventsList.Items.Count
        Dim successfullyLoaded As Boolean = False

        If numOfEvents > 1 Then ' if we have no events, or only one event, there's no reason to do anything!
            Dim missedEventTicks As Integer = 0

            While successfullyLoaded = False
                missedEventTicks += 1 ' increment the counter

                Dim nextEventToPlay = currentPlayingEvent + nextOrPrev
                nextEventToPlay = returnEventBounds(nextEventToPlay)

                If loopModeButton.Text = "PLAYLIST" Then ' find out if the next item we have is selected - if it isn't, then skip it
                    If eventsList.SelectedItems.Count > 1 Then
                        While True
                            For a As Integer = 0 To eventsList.SelectedItems.Count - 1
                                If nextEventToPlay = eventsList.SelectedItems(a).Index Then
                                    Exit While
                                Else
                                    ' Do nothing, because we haven't found anything
                                End If
                            Next

                            If nextOrPrev = 1 Then
                                nextEventToPlay = returnEventBounds(nextEventToPlay + 1)
                            Else
                                nextEventToPlay = returnEventBounds(nextEventToPlay - 1)
                            End If

                        End While
                    End If
                End If

                successfullyLoaded = loadEvent(nextEventToPlay, True) ' check to see if we loaded an event successfully or not, otherwise go to the next one

                If missedEventTicks = eventsList.Items.Count Then ' if we've tried to load every single event and failed, then quit out of this attempt
                    Exit While
                End If
            End While
        End If
    End Sub

    Private Function returnEventBounds(ByVal nextEventToPlay As Integer) As Integer
        If nextEventToPlay < 0 Then
            nextEventToPlay = eventsList.Items.Count - 1
        ElseIf nextEventToPlay = eventsList.Items.Count Then
            nextEventToPlay = 0
        End If

        Return nextEventToPlay
    End Function

    Private Sub clearEventsList()
        eventsList.Items.Clear() ' delete all the items in the events list
        currentLooperFile = Nothing ' we don't have a looper file loaded now
        setIsModified(0) ' and we're not modified

        totalNumberOfEvents = 100 ' start the incrementing timer back to 100
        currentPlayingEvent = -1 ' we're not currently playing an event

        tallyTotalList() ' re-tabulate the event list tally
        updateEventCountLabel()

        deleteEventButton.Enabled = False
        modifyEventButton.Enabled = False
        saveButton.Enabled = False
    End Sub

    Private Sub tallyTotalList()
        Dim countOfEvents As Integer = eventsList.Items.Count

        If countOfEvents > 0 Then
            Dim timeTally As Double = 0.0

            For a As Integer = 0 To countOfEvents - 1
                timeTally += TimeStringToNumber(eventsList.Items(a).SubItems(6).Text)
            Next

            eventCountLabel.Text = countOfEvents & " events"
        Else
            eventCountLabel.Text = "0 events"
        End If
    End Sub

    Private Sub updateEventCountLabel()
        eventCountLabel.Text = eventsList.Items.Count & " events"
    End Sub

    ' ======================================================================================
    ' ================= PLAYLIST BUTTON / CLICK HANDLERS ===================================
    ' ======================================================================================

    Private Sub prevEventButton_Click(sender As Object, e As EventArgs) Handles prevEventButton.Click
        loadPrevNextEvent(-1)
    End Sub

    Private Sub saveButton_Click(sender As Object, e As EventArgs) Handles saveButton.Click
        If isModified = True Then
            saveLooperFile()
        Else 'if we're not in a modified state, but still click the Save button, if you have something selected (which you'd have to), it'll save a subset of events
            If eventsList.SelectedItems.Count > 0 Then
                menu_Save.Show(MousePosition.X - 20, MousePosition.Y - 10) ' show the context menu to save a subset
            End If
        End If
    End Sub


    Private Sub deleteEventButton_Click(sender As Object, e As EventArgs) Handles deleteEventButton.Click
        deleteEvents()
    End Sub

    Private Sub modifyEventButton_Click(sender As Object, e As EventArgs) Handles modifyEventButton.Click
        If modifyEventButton.Text = "Modify" Then
            modifyEvent()
        ElseIf modifyEventButton.Text = "Merge" Then
            mergeEvents()
        End If
    End Sub

    Private Sub addEventButton_Click(sender As Object, e As EventArgs) Handles addEventButton.Click
        addEvent()
    End Sub


    Private Sub nextEventButton_Click(sender As Object, e As EventArgs) Handles nextEventButton.Click
        loadPrevNextEvent(1)
    End Sub

    ' ======================================================================================
    ' ================== CONTEXT MENU FUNCTIONS ============================================
    ' ======================================================================================

    Private Sub menu_load_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles menu_load.Opening
        If eventsList.Items.Count = 0 Then
            menuItem_importLooperFile.Enabled = False
        Else
            menuItem_importLooperFile.Enabled = True
        End If
    End Sub

    Private Sub menuItem_importLooperFile_Click(sender As Object, e As EventArgs) Handles menuItem_importLooperFile.Click
        loadLooperFile(False)
    End Sub

    Private Sub menuItem_AddAtEnd_Click(sender As Object, e As EventArgs) Handles menuItem_AddAtEnd.Click
        menuItem_AddAtEnd.Checked = True
        menuItem_InsertEvents.Checked = False

        addEventButton.Text = "ADD EVENT"

        insertItem = False
    End Sub

    Private Sub menuItem_InsertEvents_Click(sender As Object, e As EventArgs) Handles menuItem_InsertEvents.Click
        menuItem_AddAtEnd.Checked = False
        menuItem_InsertEvents.Checked = True

        addEventButton.Text = "INS. EVENT"

        insertItem = True
    End Sub

    Private Sub menu_Save_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles menu_Save.Opening
        If eventsList.SelectedIndices.Count > 0 Then
            menuItem_SaveSelection.Enabled = True
        Else
            menuItem_SaveSelection.Enabled = False
        End If
    End Sub

    Private Sub menuItem_SaveSelection_Click(sender As Object, e As EventArgs) Handles menuItem_SaveSelection.Click
        saveLooperFile(True) ' we're only saving a few events, so set this to True to force that
    End Sub

    ' ======================================================================================
    ' ================== EVENT SEARCHING FUNCTIONS =========================================
    ' ======================================================================================


    Private Sub searchTF_KeyDown(sender As Object, e As KeyEventArgs) Handles searchTF.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Return Then
            eventsList.Focus() ' make the event list the focused item to restore hotkeys/take selection off of the text box
            e.SuppressKeyPress = True ' stop the ding after hitting these buttons
        ElseIf e.KeyCode = Keys.Escape Then
            searchTF.Text = Nothing
            restoreFromSearch()
            eventsList.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub searchTF_TextChanged(sender As Object, e As EventArgs) Handles searchTF.TextChanged
        ' Real-time filter as you type
        If searchTF.Text = Nothing Or searchTF.Text = "" Then
            If inSearchMode Then restoreFromSearch() ' restore full list when search is cleared
        Else
            doSearch()
        End If
    End Sub

    Private Sub searchTF_Enter(sender As Object, e As EventArgs) Handles searchTF.Enter
        clearHotKeys()
        hotkeysActive = True 'tell the hotkey thread that hotkeys are still active, so it doesn't try to clear them (this is for text entry ONLY)
    End Sub

    Private Sub searchTF_Leave(sender As Object, e As EventArgs) Handles searchTF.Leave
        setHotKeys()
    End Sub

    Private Sub doSearch()
        If searchTF.Text <> Nothing And searchTF.Text <> "" Then
            ' If we're already in search mode, restore first so we search the full list
            If inSearchMode Then
                restoreFromSearch()
            End If

            If eventsList.Items.Count = 0 Then Exit Sub

            eventsList.SelectedIndices.Clear()

            If UBound(eventsListArray) = 0 Then ' if the master array hasn't been created yet, then create it...
                ReDim eventsListArray(eventsList.Items.Count, 12)

                For a As Integer = 0 To eventsList.Items.Count - 1
                    For b As Integer = 0 To 11
                        eventsListArray(a, b) = eventsList.Items(a).SubItems(b).Text ' get the events into an array
                    Next

                    eventsListArray(a, 12) = eventsList.Items(a).BackColor
                Next
            End If

            Dim foundIndex As New List(Of Integer)
            Dim searchText As String = UCase(searchTF.Text)

            For a As Integer = 0 To eventsList.Items.Count - 1 ' check the entire list view for the search string
                Dim eventName As String = UCase(eventsList.Items(a).SubItems(1).Text)
                Dim eventPath As String = UCase(eventsList.Items(a).SubItems(7).Text)
                Dim eventID As String = UCase(eventsList.Items(a).SubItems(0).Text)

                ' Search in event name, file path (includes directory and file name), and event ID
                If Not (eventName.Contains(searchText) OrElse eventPath.Contains(searchText) OrElse eventID.Contains(searchText)) Then
                    foundIndex.Add(a)
                End If
            Next

            If foundIndex.Count <> eventsList.Items.Count Then ' if we found something, then we need to change the list view
                If currentPlayingEvent > -1 Then
                    Integer.TryParse(eventsList.Items(currentPlayingEvent).SubItems(0).Text, currentSearchPlayingEvent) ' the ID number of the current playing event
                End If

                currentPlayingEvent = -1 ' set this to -1 initially to jump to the beginning of the list if the search doesn't contain ^^^

                eventsList.BeginUpdate() ' stop updating the events list so we can do the next step without glitching the view out

                For a As Integer = foundIndex.Count - 1 To 0 Step -1 ' Delete any items in the list that *don't* contain our search phrase
                    eventsList.Items.RemoveAt(foundIndex(a))
                Next

                eventsList.EndUpdate() ' start updating the events list again after we're done modifying it

                For a As Integer = 0 To eventsList.Items.Count - 1 ' find the current playing event in the new results
                    If eventsList.Items(a).SubItems(0).Text = CStr(currentSearchPlayingEvent) Then
                        currentPlayingEvent = a ' set the current playing event to the position in the current playlist we're in
                    End If
                Next

                inSearchMode = True
                tallyTotalList() ' re-tabulate the event list tally

                If loopModeButton.Text = "SHUFFLE" Then
                    switchToShuffleMode() ' re-initialize the random number list and re-shuffle (to account for the new listing)
                End If
            End If
        End If
    End Sub

    Private Sub restoreFromSearch()
        If UBound(eventsListArray) <> 0 Then ' restore the original events list from the master array
            If currentPlayingEvent > -1 Then
                Integer.TryParse(eventsList.Items(currentPlayingEvent).SubItems(0).Text, currentSearchPlayingEvent) ' the ID number of the current playing event (this time, from search mode)
            End If

            eventsList.BeginUpdate() ' stop updating the events list so we can do the next step without glitching the view out
            eventsList.Items.Clear()

            For a As Integer = 0 To UBound(eventsListArray) - 1
                Dim newItemArray(11) As String

                For b As Integer = 0 To 11
                    newItemArray(b) = CStr(eventsListArray(a, b)) ' re-add all of the original events back to the events list
                Next

                Dim newEvent = New ListViewItem(newItemArray)

                eventsList.Items.Add(newEvent)
                eventsList.Items(eventsList.Items.Count - 1).BackColor = CType(eventsListArray(a, 12), Color) ' the last element of the master array is the original back color
            Next

            eventsList.EndUpdate() ' start updating the events list again after we're done modifying it

            For a As Integer = 0 To eventsList.Items.Count - 1 ' find the current playing event in the new results
                If eventsList.Items(a).SubItems(0).Text = CStr(currentSearchPlayingEvent) Then
                    currentPlayingEvent = a ' set the current playing event to the position in the current playlist we're in
                    checkAgainstCurrentPlayingEvent() ' re-highlight the current playing event (if the IN and OUT points match)
                    Exit For
                End If
            Next

            inSearchMode = False
            tallyTotalList() ' re-tabulate the event list tally

            If loopModeButton.Text = "SHUFFLE" Then
                switchToShuffleMode() ' re-initialize the random number list and re-shuffle (to account for the new listing)
            End If

            ReDim eventsListArray(0, 0) ' delete the master array to clear memory and get it ready for the next search
        End If
    End Sub

    ' ======================================================================================
    ' ================== EVENT NAME EDITING FUNCTIONS ======================================
    ' ======================================================================================

    Private Sub eventsList_MouseDown(sender As Object, e As MouseEventArgs) Handles eventsList.MouseDown
        HideTextEditor()
    End Sub

    Private Sub listViewEditor_Leave(sender As Object, e As EventArgs) Handles listViewEditor.Leave
        HideTextEditor()
    End Sub

    Private Sub HideTextEditor()
        listViewEditor.Visible = False

        If Not (SelectedLSI Is Nothing) Then
            SelectedLSI.Text = listViewEditor.Text
            setIsModified(1) ' if we changed the value of one of the event list's names or loops, then mark it as being modified
            If autoPlayDialogs = True Then SendMessage(CMD_SEND.CMD_PLAY) ' we cancelled or finished editing text, so resume playback
        End If

        SelectedLSI = Nothing
        listViewEditor.Text = Nothing

        setHotKeys()
    End Sub

    Private Sub listViewEditor_KeyDown(sender As Object, e As KeyEventArgs) Handles listViewEditor.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Return Or e.KeyCode = Keys.Escape Then
            e.SuppressKeyPress = True ' stop the ding after hitting these buttons

            If e.KeyCode = Keys.Escape Then
                SelectedLSI = Nothing ' clear the selection, so the edit doesn't take effect
            End If

            HideTextEditor()
            SetForegroundWindow(MPCHandle) ' Make MPC-HC the foreground window
        End If
    End Sub

    Private Sub eventsList_KeyDown(sender As Object, e As KeyEventArgs) Handles eventsList.KeyDown
        If eventsList.Items.Count > 0 Then
            If eventsList.SelectedItems.Count <> 0 Then
                Dim selectedRow = eventsList.SelectedItems(0).Index

                If e.KeyCode = Keys.F2 Then
                    editListViewItem(selectedRow)
                ElseIf e.KeyCode = Keys.F3 Then
                    editListViewItem(selectedRow, 3)
                End If
            End If
        End If
    End Sub

    Private Sub editListViewItem(ByVal selectedItem As Integer, Optional selectedSubItem As Integer = 1)
        SendMessage(CMD_SEND.CMD_PAUSE) ' pause the player when editing text

        Me.Activate() ' make the window frontmost

        clearHotKeys()
        hotkeysActive = True 'tell the hotkey thread that hotkeys are still active, so it doesn't try to clear them (this is for text entry ONLY)

        SelectedLSI = eventsList.Items(selectedItem).SubItems(selectedSubItem)

        Dim cellWidth = SelectedLSI.Bounds.Width
        Dim cellHeight = SelectedLSI.Bounds.Height
        Dim cellLeft = eventsList.Left + SelectedLSI.Bounds.Left
        Dim cellTop = eventsList.Top + SelectedLSI.Bounds.Top

        listViewEditor.Location = New Point(cellLeft, cellTop)
        listViewEditor.Size = New Size(cellWidth, cellHeight)
        listViewEditor.Visible = True
        listViewEditor.BringToFront()
        listViewEditor.Text = SelectedLSI.Text
        listViewEditor.Select()
        listViewEditor.SelectAll()
    End Sub

    ' ======================================================================================
    ' ================== DRAG AND DROP AND SORTING FUNCTIONS ===============================
    ' ======================================================================================

    Private Sub eventsList_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles eventsList.DragDrop
        eventsList.ListViewItemSorter = Nothing ' Clear custom sorting to get the drag operation to work

        'Returns the location of the mouse pointer in the ListView control.
        Dim p As Point = eventsList.PointToClient(New Point(e.X, e.Y))

        'Obtain the item that is located at the specified location of the mouse pointer.
        Dim dragToItem As ListViewItem = eventsList.GetItemAt(p.X, p.Y)
        Dim dragIndex As Integer

        If dragToItem IsNot Nothing Then
            'Obtain the index of the item at the mouse pointer.
            dragIndex = dragToItem.Index
        Else
            'Set the index to zero to put it at the very top
            dragIndex = 0
        End If

        If e.Data.GetDataPresent("System.Windows.Forms.ListView+SelectedListViewItemCollection") Then ' we're dragging events around in the listview
            'Return if the items are not selected in the ListView control.
            If eventsList.SelectedItems.Count = 0 Then Return
            Dim i As Integer
            Dim sel(eventsList.SelectedItems.Count) As ListViewItem
            For i = 0 To eventsList.SelectedItems.Count - 1
                sel(i) = eventsList.SelectedItems.Item(i)
            Next

            For i = 0 To eventsList.SelectedItems.Count - 1
                'Obtain the ListViewItem to be dragged to the target location.
                Dim dragItem As ListViewItem = sel(i)

                Dim itemIndex As Integer = dragIndex
                If itemIndex = dragItem.Index Then Return
                If dragItem.Index < itemIndex Then
                    itemIndex += 1
                Else
                    itemIndex = dragIndex + i
                End If
                'Insert the item in the specified location.
                Dim inserteditem As ListViewItem = CType(dragItem.Clone, ListViewItem)

                eventsList.Items.Insert(itemIndex, inserteditem)
                'Removes the item from the initial location while
                'the item is moved to the new location.
                eventsList.Items.Remove(dragItem)
            Next

            setIsModified(1) ' if we're not already set as isModified, then do it here
        Else ' we dragged something on to the listview that ISN'T a listview item (like a file from Explorer)
            Dim dropFiles() As String = CType(e.Data.GetData("FileDrop", True), String()) ' get the list of files dropped

            If dropFiles.Count > 1 Then
                Array.Sort(dropFiles) ' sort the array of files alphabetically
            End If

            Dim currentInsertedItem As Integer = 0 ' keep track of which item we're currently adding

            For a As Integer = 0 To dropFiles.Count - 1
                If {".mov", ".avi", ".mp4", ".mkv", ".mts", ".m2ts",
                    ".m2t", ".qt", ".ts", ".qt", ".flv", ".wav", ".mp3",
                    ".ogg", ".aif", ".aiff"}.Contains(LCase(My.Computer.FileSystem.GetFileInfo(dropFiles(a)).Extension)) Then

                    Dim draggedEvent = returnListViewItem(My.Computer.FileSystem.GetFileInfo(dropFiles(a)).Name, "100", "1", "0:00", Nothing, dropFiles(a))

                    If insertItem = False Then
                        eventsList.Items.Add(draggedEvent)
                    Else
                        eventsList.Items.Insert(dragIndex + currentInsertedItem, draggedEvent)
                        currentInsertedItem += 1
                    End If

                    totalNumberOfEvents += 1
                    setIsModified(1) ' if we're not already set as isModified, then do it here
                ElseIf My.Computer.FileSystem.GetFileInfo(dropFiles(a)).Extension = ".looper" Then ' we've dragged a looper file into the playlist
                    If insertItem = False Then ' if we're not set to insert .looper events into the current playlist, then just open a new file
                        loadLooperFile(True, dropFiles(a))
                        Exit For ' exit the loop, skipping the rest of the files (this method only opens one file, not a bunch like the above...)
                    Else ' if we're set to insert .looper events
                        If eventsList.Items.Count > 0 Then ' if there's actually something already in the playlist
                            eventsList.SelectedIndices.Clear()

                            If dragIndex <> 0 Then
                                eventsList.SelectedIndices.Add(dragIndex - 1)
                            Else
                                eventsList.SelectedIndices.Add(eventsList.Items.Count - 1)
                            End If

                            loadLooperFile(False, dropFiles(a)) ' load the new looper at the end of the playlist
                        Else ' if there's nothing in the playlist, then just open the one file like the above, and skip the other files
                            loadLooperFile(True, dropFiles(a))
                            Exit For
                        End If
                        ' We can't do anything with this kind of file
                    End If
                End If
            Next
        End If

        updateEventCountLabel()
    End Sub

    Private Sub eventsList_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles eventsList.DragEnter
        If e.Data.GetDataPresent("System.Windows.Forms.ListView+SelectedListViewItemCollection") Then ' we're dragging a listview item to another location in the playlist
            e.Effect = DragDropEffects.Move
        ElseIf e.Data.GetDataPresent(DataFormats.FileDrop) Then ' we're dragging a file to the playlist from Explorer
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub eventsList_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles eventsList.ItemDrag
        eventsList.DoDragDrop(eventsList.SelectedItems, DragDropEffects.Move)
    End Sub

    Private Sub eventsList_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles eventsList.ColumnClick
        If eventsList.Items.Count > 0 Then ' if we have more than one event in the playlist... otherwise do nothing!
            Dim iSortOrder As SortOrder = CType(eventsList.Columns(e.Column).Tag, SortOrder)
            Dim lvcs As New ListViewColumnSorter
            If iSortOrder = SortOrder.Ascending Then
                eventsList.Columns(e.Column).Tag = SortOrder.Descending
                lvcs.SortingOrder = SortOrder.Descending
            Else
                eventsList.Columns(e.Column).Tag = SortOrder.Ascending
                lvcs.SortingOrder = SortOrder.Ascending
            End If
            lvcs.ColumnIndex = e.Column
            eventsList.ListViewItemSorter = lvcs

            setIsModified(1) ' if we re-sorted the events list, then mark it as being modified
        End If
    End Sub

End Class
