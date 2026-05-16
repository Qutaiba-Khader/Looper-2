Module MPC_Enum
    ' ======================================================================================
    ' ================= LOOPER CONSTANTS AND ENUMS =========================================
    ' ======================================================================================
    Public ReadOnly INIFile As String = Environment.GetEnvironmentVariable("LocalAppData") & "\Zach Glenwright's Looper 2\MPCLooper.ini"
    Public ReadOnly LogFile As String = Environment.GetEnvironmentVariable("LocalAppData") & "\Zach Glenwright's Looper 2\looper.log"

    Public Sub Log(message As String)
        Try
            If System.IO.File.Exists(LogFile) Then
                Dim fi As New System.IO.FileInfo(LogFile)
                If fi.Length > 2 * 1024 * 1024 Then ' 2MB limit
                    System.IO.File.Delete(LogFile & ".old")
                    System.IO.File.Move(LogFile, LogFile & ".old")
                End If
            End If
            System.IO.File.AppendAllText(LogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " | " & message & vbCrLf)
        Catch
        End Try
    End Sub
    Public pausePlaybackOnLoadEvent As Boolean = False ' whether to force pause when loading new events instead of playing them
    Public skipSaveConfirmations As Boolean = False ' whether to skip all save confirmation dialogs
    Public skipEventNameEditor As Boolean = False ' whether to skip the event name editor when adding events
    Public autoSaveLooper As Boolean = False ' whether to automatically save the looper file on changes
    Public defaultLooperSavePath As String = Nothing ' default directory for saving .looper files
    Public autoCreateEventOnOut As Boolean = False ' whether to auto-create event when OUT point is set
    Public osdOnInPoint As Boolean = False
    Public osdOnOutPoint As Boolean = False
    Public osdOnAddEvent As Boolean = False
    Public osdOnLoopModeChange As Boolean = False
    Public osdOnSave As Boolean = False
    Public hotKeyDisabled() As Boolean = {
        False, False, False, False, False, True, True, True, True, True,
        True, True, True, False, True, True, True, True, True, False,
        True, True, False, False, False, False, True, True, False, False,
        True, True, True, True, True, True
    } ' per-hotkey disabled state (index 0-35, maps to hotkey IDs 100-135)

    Enum KeyModifier
        None = 0
        Alt = &H1
        Control = &H2
        Shift = &H4
        Winkey = &H8
    End Enum

    ' ======================================================================================
    ' ================= MPC-HC ENUMS =======================================================
    ' ======================================================================================
    Public Enum CMD_RECEIVED As Integer
        CMD_CONNECT = &H50000000
        CMD_STATE = &H50000001
        CMD_PLAYMODE = &H50000002
        CMD_NOWPLAYING = &H50000003
        CMD_CURRENTPOSITION = &H50000007
        CMD_NOTIFYSEEK = &H50000008
        CMD_DISCONNECT = &H5000000B
    End Enum

    Public Enum CMD_SEND As Integer
        ' ---------------- FILE COMMANDS ----------------
        CMD_OPENFILE = &HA0000000
        CMD_CLOSEFILE = &HA0000002
        ' ---------------- MEDIA COMMANDS ----------------
        CMD_STOP = &HA0000001
        CMD_PLAY = &HA0000004
        CMD_PAUSE = &HA0000005
        CMD_PLAYPAUSE = &HA0000003
        CMD_SETSPEED = &HA0004008
        CMD_TOGGLEFULLSCREEN = &HA0004000
        CMD_CLOSEAPP = &HA0004006
        CMD_OSDSHOWMESSAGE = &HA0005000
        ' ---------------- INFO COMMANDS ----------------
        CMD_GETNOWPLAYING = &HA0003002
        CMD_GETCURRENTPOSITION = &HA0003004
        CMD_SETPOSITION = &HA0002000
    End Enum

    Public Enum LOADSTATE As Integer
        MLS_CLOSED = 0
        MLS_LOADING = 1
        MLS_LOADED = 2
        MLS_CLOSING = 3
    End Enum

    Public Enum PLAYSTATE As Integer
        PS_PLAY = 0
        PS_PAUSE = 1
        PS_STOP = 2
        PS_UNUSED = 3
    End Enum
End Module
