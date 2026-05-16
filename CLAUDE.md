# CLAUDE.md — Looper 2

## Project Overview

**Looper 2** by Zach Glenwright — a VB.NET WinForms (.NET Framework 4.7.2) A/B looping controller for MPC-HC/BE. Communicates with the media player via `WM_COPYDATA` IPC messages. Forked and enhanced by Qutaiba Khader.

- **Language:** VB.NET (Visual Basic .NET)
- **Framework:** .NET Framework 4.7.2, WinForms
- **Build:** MSBuild (VS 2019 BuildTools or .NET Framework 4.x MSBuild)
- **Config at runtime:** `%LocalAppData%\Zach Glenwright's Looper 2\MPCLooper.ini`
- **Remote:** `github.com/Qutaiba-Khader/Looper-2`

## Build Commands

```bash
# Using .NET Framework MSBuild (from Git Bash)
/c/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe 'MPC-HC Looper VB.vbproj' //p:Configuration=Release //verbosity:minimal

# Output
bin/Release/Looper 2.exe
```

## Architecture

### IPC Model (MPC-HC ↔ Looper)

Looper launches MPC-HC with `/slave <hWnd>` flag. MPC-HC sends `WM_COPYDATA` messages back to Looper's `WndProc`. Looper sends commands to MPC-HC via `SendMessageA` with `WM_COPYDATA`.

**Receive commands** (`CMD_RECEIVED` enum in `MPC_Enum.vb`):
- `CMD_CONNECT` (0x50000000) — MPC-HC connected, gives its hWnd
- `CMD_STATE` (0x50000001) — Load state changes (loaded/loading/closed)
- `CMD_CURRENTPOSITION` (0x50000007) — Position updates (polled every 30ms)
- `CMD_NOWPLAYING` (0x50000003) — File info + duration
- `CMD_DISCONNECT` (0x5000000B) — MPC-HC quit

**Send commands** (`CMD_SEND` enum in `MPC_Enum.vb`):
- `CMD_SETPOSITION` (0xA0002000) — Seek to position
- `CMD_GETCURRENTPOSITION` (0xA0003004) — Request position
- `CMD_GETNOWPLAYING` (0xA0003002) — Request file info
- `CMD_PLAY/PAUSE/STOP` — Playback control
- `CMD_SETSPEED` (0xA0004008) — Speed control
- `CMD_OSDSHOWMESSAGE` (0xA0005000) — Show OSD text (uses binary `MPC_OSDDATA` struct)

### Threading Model

Two background threads run continuously:
- **`posThread` (doTheLoop)** — Polls `CMD_GETCURRENTPOSITION` every 30ms, drives the loop logic
- **`frontThread` (isForeground)** — Checks which window is foreground every 50ms, enables/disables hotkeys

### Loop Modes

- **Off** — No looping, manual control only
- **Loop Mode** — Loops between IN and OUT points continuously
- **Playlist Mode** — Plays events sequentially, respects repeat counts
- **Shuffle Mode** — Randomized event order using pre-shuffled array

## File Map

### Core Forms
| File | Lines | Purpose |
|------|-------|---------|
| `mainWindow.vb` | ~1567 | Main control panel: IPC, hotkeys, IN/OUT points, speed, OSD, loop logic |
| `playlistWindow.vb` | ~1684 | Event list management: load/save .looper files, search, drag-drop, auto-save |
| `optionsWindow.vb` | ~350 | Preferences dialog: loads/saves INI, all settings checkboxes |
| `hotKeySettings.vb` | ~215 | Hotkey customization: per-key disable, multi-select, reassign |
| `findLooperWindow.vb` | ~64 | First-run dialog: locate MPC-HC/BE executable |

### Modules (no UI)
| File | Purpose |
|------|---------|
| `MPC_Enum.vb` | Global constants, enums (CMD_SEND, CMD_RECEIVED, KeyModifier), global settings variables, Log() function |
| `stringWork.vb` | `betweenTheLines()` INI parser, `removeSection()`, `removeParams()` |
| `timeConversions.vb` | `NumberToTimeString()` / `TimeStringToNumber()` — time ↔ seconds conversion |
| `ToolTips.vb` | `loadToolTips()` — sets all tooltip strings for both windows |

### Utility Classes
| File | Purpose |
|------|---------|
| `ListViewColumnSorter.vb` | IComparer for sorting ListView columns (numeric, date, string) |
| `ListViewDoubleBuffered.vb` | ListView subclass with `OptimizedDoubleBuffer` to prevent flicker |

### Designer Files (auto-generated, do NOT hand-edit)
| File | Purpose |
|------|---------|
| `mainWindow.Designer.vb` | Main window layout — buttons, text fields, sliders, timers |
| `playlistWindow.Designer.vb` | Playlist layout — ListView, search bar, buttons |
| `optionsWindow.Designer.vb` | Options layout — all checkboxes, text fields, browse button |
| `hotKeySettings.Designer.vb` | Hotkey settings layout — ListView, buttons |
| `findLooperWindow.Designer.vb` | First-run layout |

## Settings System

### Global Variables (`MPC_Enum.vb`)

All settings are public module-level fields:
```
INIFile                  — path to MPCLooper.ini
LogFile                  — path to looper.log (2MB rotation)
pausePlaybackOnLoadEvent — force pause when loading events
skipSaveConfirmations    — bypass all save/quit dialogs
skipEventNameEditor      — auto-name events without editor
autoSaveLooper           — auto-save .looper on changes
defaultLooperSavePath    — default save directory (supports %ENV_VAR% paths)
autoCreateEventOnOut     — auto-create event when OUT point is set
osdOnInPoint             — show OSD on Set In Point
osdOnOutPoint            — show OSD on Set Out Point
osdOnAddEvent            — show OSD on Add Event
osdOnLoopModeChange      — show OSD on loop mode change
osdOnSave                — show OSD on save
hotKeyDisabled(35)       — per-hotkey disabled state array (IDs 100-135)
```

### INI File Format

Flat `key=value` pairs under section headers, parsed by `betweenTheLines(source, key, delimiter, default)`.

```ini
[Prefs]
loopPreviewLength=0.25
loopSlipLength=0.5
defaultSpeed=100
disableHotkeys=1
skipSaveConfirmations=1
skipEventNameEditor=1
autoSaveLooper=1
autoCreateEventOnOut=1
osdOnInPoint=1
osdOnOutPoint=1
osdOnAddEvent=1
osdOnLoopModeChange=1
osdOnSave=1
defaultLooperSavePath=%USERPROFILE%\OneDrive\Desktop\Loops
autoloadLastLooper=C:\file.looper|3
newEventName=[#] [F]

[System]
MPCEXE=C:\Program Files\MPC-HC\mpc-hc64.exe
inPointOffset=0
outPointOffset=0.15

[StartPos]
startPositionL=100
startPositionT=200
startPLPositionL=100
startPLPositionT=400
startPLPositionW=500
startPLPositionH=538

[HotKeys]
100=C|73
D100=1
105=S|219
```

### Settings Load Flow

1. **Startup:** `mainWindow.loadINIFile()` (line ~528) reads INI → populates global vars + local startup vars → launches MPC-HC → sets up hotkeys/windows
2. **Options open:** `optionsWindow.options_loadINIFile()` (line ~12) re-reads INI → sets checkbox states
3. **Options save:** `optionsWindow.options_saveINIFile()` (line ~104) builds INI string → writes file → updates globals in memory

### Adding a New Setting

1. Add public variable in `MPC_Enum.vb`
2. Add `Boolean.TryParse(betweenTheLines(...))` in `mainWindow.loadINIFile()` (~line 569)
3. Add checkbox + Designer control in `optionsWindow.Designer.vb`
4. Add load logic in `optionsWindow.options_loadINIFile()` (~line 93)
5. Add save logic in `optionsWindow.options_saveINIFile()` (~line 212)
6. Add checkbox name to the `[Prefs]` header condition (~line 120-123)
7. Use the variable wherever needed in `playlistWindow.vb` or `mainWindow.vb`

## Event ListView Structure

Each event is a `ListViewItem` with sub-items:

| Index | Field | Example |
|-------|-------|---------|
| 0 | Event # (ordering) | `100` |
| 1 | Event Name | `Chorus Loop` |
| 2 | Speed | `100` |
| 3 | Repeat Count | `1` |
| 4 | IN Point | `1:23.456` |
| 5 | OUT Point | `2:34.567` |
| 6 | Duration (calculated) | `1:11.111` |
| 11 | File Path | `C:\video.mp4` |

## .looper File Format

One event per line, pipe-delimited. Optional prefix tags `<S:speed>`, `<L:loops>` before the event name. `<Z:>` tags are read for backward compatibility but no longer written:

```
<S:75><L:3>Chorus|1:23.456|2:34.567|C:\video.mp4
Bridge|3:00.000|3:30.000|C:\video.mp4
```

## Hotkey System

- 36 hotkeys (IDs 100-135), defined in `mainWindow.hotKeyList(,)` array
- Format: `"modifiers|keycode"` — modifiers: C=Ctrl, S=Shift, A=Alt
- Registered via Win32 `RegisterHotKey` / `UnregisterHotKey`
- Only active when Looper, playlist, or MPC-HC window is foreground
- Per-hotkey disable via `hotKeyDisabled()` array (initialized with defaults), stored as `D<id>=1` in INI

## Key Function Reference

### mainWindow.vb
| Function | Line | Purpose |
|----------|------|---------|
| `WndProc()` | ~178 | Message pump: handles WM_HOTKEY + WM_COPYDATA from MPC-HC |
| `mainWindow_Load()` | ~349 | Startup: load INI, start threads, set hotkeys |
| `loadINIFile()` | ~528 | Full INI load with all settings |
| `loadINIFileForDefaults()` | ~620 | Minimal INI load (MPC-HC path only) |
| `SendMessage()` | ~157 | Send WM_COPYDATA command to MPC-HC |
| `SendOSD()` | ~160 | Send OSD message via binary MPC_OSDDATA struct (position, duration, text) |
| `setInPoint()` | ~1250 | Capture current position as IN point |
| `setOutPoint()` | ~1257 | Capture current position as OUT point |
| `assignHotKey()` | ~668 | Register single hotkey with Windows |
| `setHotKeys()` | ~688 | Register all 36 hotkeys |
| `clearHotKeys()` | ~755 | Unregister all hotkeys |
| `handleHotKeyEvent()` | ~823 | Dispatch hotkey ID to action |
| `setSpeed()` | ~1387 | Set MPC-HC playback speed |
| `doTheLoop()` | ~471 | Background thread: poll position every 30ms |
| `isForeground()` | ~485 | Background thread: check active window every 50ms |

### playlistWindow.vb
| Function | Line | Purpose |
|----------|------|---------|
| `ShowTopMostMessageBox()` | ~33 | MessageBox helper that stays above MPC-HC |
| `unInitialize()` | ~126 | Quit handler: ask save, handle auto-save |
| `loadLooperFile()` | ~225 | Load/import .looper file into event list |
| `saveLooperFile()` | ~467 | Save events to .looper file |
| `autoSaveCurrentFile()` | ~943 | Silent save using StringBuilder (no dialogs) |
| `addEvent()` (public) | ~757 | Add new event from current IN/OUT/speed |
| `addEvent()` (private) | ~714 | Add event from parsed .looper file data |
| `deleteEvents()` | ~827 | Delete selected events with confirmation |
| `modifyEvent()` | ~919 | Update event with current IN/OUT/speed |
| `mergeEvents()` | ~982 | Merge contiguous selected events into one |
| `loadEvent()` | ~1048 | Load specific event (seek + set speed) |
| `loadPrevNextEvent()` | ~1125 | Navigate to next/prev event in playlist |
| `findFile()` | ~368 | Relocate missing files dialog |
| `doSearch()` | ~1357 | Filter events by name/path/ID |
| `restoreFromSearch()` | ~1426 | Restore full event list from backup array |
| `checkAgainstCurrentPlayingEvent()` | ~1035 | Highlight/unhighlight based on IN/OUT match |

### optionsWindow.vb
| Function | Line | Purpose |
|----------|------|---------|
| `options_loadINIFile()` | ~12 | Read INI and set all checkbox states |
| `options_saveINIFile()` | ~104 | Build INI string and write to disk |
| `browseSavePathButton_Click()` | ~335 | FolderBrowserDialog for default save path |

### stringWork.vb
| Function | Line | Purpose |
|----------|------|---------|
| `betweenTheLines()` | ~2 | Parse value between key and delimiter; handles bool conversion (0/1 → False/True) |
| `removeSection()` | ~39 | Strip a `<tag:value>` section from string |
| `removeParams()` | ~51 | Strip all param tags (`<S:>`, `<L:>`, `<Z:>`) from event name |

### timeConversions.vb
| Function | Line | Purpose |
|----------|------|---------|
| `NumberToTimeString()` | ~6 | Convert seconds (Double) → `H:MM:SS.mmm` string |
| `TimeStringToNumber()` | ~42 | Convert `H:MM:SS.mmm` string → seconds (Double) |

## OSD System

Uses MPC-HC's `CMD_OSDSHOWMESSAGE` (0xA0005000) with a binary struct (NOT pipe-delimited text):

```vb
<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
Public Structure MPC_OSDDATA
    Public nMsgPos As Integer        ' 0=clear, 1=top-left, 2=top-right
    Public nDurationMS As Integer    ' milliseconds (default 1500)
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
    Public strMsg As String          ' max 127 chars
End Structure
```

- Only one OSD message visible at a time (each replaces the previous)
- When auto-chain fires (OUT → Add → Save), messages are combined: `"Event Added: name — Saved"`
- The Out Point OSD is suppressed when `autoCreateEventOnOut` is enabled

## Logging

- Log file: `%LocalAppData%\Zach Glenwright's Looper 2\looper.log`
- 2MB rotation (renames to `.old` when limit reached)
- Format: `yyyy-MM-dd HH:mm:ss | message`
- Logged events: startup, MPC-HC connect/disconnect, now playing, IN/OUT points, file operations

## Conventions

- INI parser: always use `betweenTheLines(fileReader, "key=", vbCrLf, default)` — returns default if key missing
- Boolean INI values: stored as `1` (true) or absent (false); parsed via `Boolean.TryParse` with `"B_False"/"B_True"` defaults
- Event numbering starts at 100 and increments
- Time format throughout: `M:SS.mmm` or `H:MM:SS.mmm` (no leading zeros on minutes unless hours present)
- All MessageBox calls in playlistWindow use `ShowTopMostMessageBox()` helper
- All MessageBox calls in optionsWindow/hotKeySettings pass `Me` as owner
- `BeginUpdate()`/`EndUpdate()` wrap any batch ListView operations
- Environment variables in paths: use `Environment.ExpandEnvironmentVariables()` when loading from INI
