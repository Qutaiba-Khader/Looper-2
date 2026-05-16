# Looper 2 - Enhancement Changes

Summary of all modifications made to the original Looper 2 codebase.

---

## Bug Fixes

### 1. Window Position Restore (mainWindow.vb)
- **Bug:** `startPositionT` (top coordinate) was checked against `startPositionL` instead of itself, so the vertical window position was never restored.
- **Fix:** Changed `If startPositionL <> "-1"` to `If startPositionT <> "-1"` for the Top coordinate.

### 2. Slip Length Assignment (optionsWindow.vb)
- **Bug:** When `loopSlipLength` parsed to 0, the fallback incorrectly assigned to `loopPreviewLength` instead of `loopSlipLength`.
- **Fix:** Changed `mainWindow.loopPreviewLength = 0.5` to `mainWindow.loopSlipLength = 0.5`.

### 3. Missing `=` in disableHotkeys INI Key (optionsWindow.vb)
- **Bug:** `betweenTheLines(fileReader, "disableHotkeys", ...)` was missing the trailing `=`, so the setting was never read from the INI file.
- **Fix:** Changed to `"disableHotkeys="`.

### 4. Event Name Template Variable [f] (playlistWindow.vb)
- **Bug:** The `[f]` replacement (filename without extension) was applied against the raw template `mainWindow.newEventString` instead of the already-processed `newEventName`, losing the prior `[F]` substitution.
- **Fix:** Changed `Replace(mainWindow.newEventString, "[f]", ...)` to `Replace(newEventName, "[f]", ...)`.

### 5. IN/OUT Point Timing Race (mainWindow.vb)
- **Bug:** `setInPoint()` and `setOutPoint()` read `currentPosition` immediately after `SendMessage(CMD_GETCURRENTPOSITION)` before the response could arrive, resulting in stale position values.
- **Fix:** Added `Threading.Thread.Sleep(50)` after `SendMessage` in both methods to allow the position response to arrive.

### 6. Set IN Point Clearing OUT (mainWindow.vb)
- **Bug:** When IN > OUT, `clearOutPoint()` was called which reset OUT to end-of-video. This was undesirable when rapidly setting loop points.
- **Fix:** Changed to `outTF.Text = Nothing` (just blanks the field without resetting to duration).

### 7. OUT Point Offset (mainWindow.vb)
- **Bug:** `setOutPoint()` did not apply `outPointOffset`, making the OUT point slightly inaccurate.
- **Fix:** Changed to `NumberToTimeString(currentPosition + outPointOffset)`.

### 8. Search Array Column Count (playlistWindow.vb)
- **Bug:** The search backup array was `ReDim(count, 8)` with loops `0 To 7`, but events have 12 sub-items (0-11). This caused columns like file path (index 11) to be lost after restoring from search.
- **Fix:** Changed to `ReDim(count, 12)` with loops `0 To 11`, and BackColor stored at index 12.

### 9. Missing File Relocator - Wrong Index (playlistWindow.vb)
- **Bug:** When batch-fixing missing files from the same directory, the code updated `missingEvents(a)` (current outer loop) instead of `missingEvents(b)` (the matching inner loop entry).
- **Fix:** Changed to use index `b` consistently.

---

## New Features

### 1. Per-Hotkey Disable/Enable
- Added `hotKeyDisabled(44)` array to track per-hotkey disabled state.
- Hotkey Settings list view now supports **multi-select** for batch operations.
- Added **DISABLE/ENABLE** button to toggle selected hotkeys.
- Disabled hotkeys show `[DISABLED]` prefix, gray text, and light background.
- Disabled state is persisted in INI as `D<id>=1` under `[HotKeys]` section.
- `assignHotKey()` skips registration for disabled hotkeys.

### 2. Skip Save Confirmation Dialogs
- New option: **"Skip save confirmation dialogs"** (`skipSaveConfirmations`).
- When enabled, all save/overwrite/quit confirmation dialogs are bypassed.
- Works with auto-save (below) for a fully non-interactive workflow.

### 3. Skip Event Name Editor
- New option: **"Auto-name events (skip name editor on add)"** (`skipEventNameEditor`).
- When enabled, newly added events use the template name directly without opening the inline text editor.

### 4. Auto-Save .looper File
- New option: **"Auto-save .looper file on changes"** (`autoSaveLooper`).
- Automatically saves the current `.looper` file whenever events are added, deleted, or modified.
- Uses efficient `StringBuilder`-based serialization with `FileStream` for performance.
- Only triggers if a file is already loaded (doesn't prompt for new files).

### 5. Clear IN/OUT Points After Adding Event
- New option: **"Clear IN/OUT points after adding an event"** (`clearPointsAfterAdd`).
- Resets both IN and OUT fields after adding, ready for the next loop capture.

### 6. Default .looper Save Path
- New option: **"Default .looper Save Location"** with a text field and Browse button.
- When set, the Save dialog defaults to this directory instead of requiring navigation each time.
- Persisted as `defaultLooperSavePath=<path>` in INI.

### 7. Real-Time Search Filter (playlistWindow.vb)
- Search now filters **as you type** (live `TextChanged` handler) instead of requiring Enter.
- Pressing **Escape** in the search field clears it and restores the full list.
- Search now matches against **event name**, **file path**, and **event ID** (previously only name).
- Removed "No Results Found" popup - the empty list is self-explanatory.
- Each new search properly restores the full list first before re-filtering.

### 8. Smarter Missing File Relocator (playlistWindow.vb)
- The file picker now starts in the **original file's directory** (if it still exists) or the last-found path, instead of always defaulting to Desktop.
- The file picker pre-fills the **filename** in the dialog for easier identification.

---

## UI/UX Improvements

### 1. MessageBox Z-Order Fix
- Added `ShowTopMostMessageBox()` helper that temporarily sets the playlist window as TopMost before showing a dialog, ensuring it appears **above MPC-HC** instead of behind it.
- After dismissal, focus is returned to MPC-HC via `SetForegroundWindow`.
- All playlist confirmation dialogs now use this helper.
- Removed `MessageBoxOptions.DefaultDesktopOnly` from all dialogs (it caused issues with owner window association).

### 2. Options Window - MessageBox Owner
- All `MessageBox.Show()` calls in options/hotkey settings now pass `Me` as owner, ensuring proper parent-child window behavior.

### 3. Event List Performance
- Used `eventsList.BeginUpdate()` / `EndUpdate()` around batch operations (add event, bulk delete) to prevent UI flicker.
- Replaced manual loop to clear selections with `eventsList.SelectedIndices.Clear()`.

### 4. Options Window Expanded
- Window height increased from 613 to 765 to accommodate new options.
- New checkboxes and controls added in a logical group below existing options.

---

## Build/Project Changes

### 1. Removed Pre-Build Event (vbproj)
- Cleared the `<PreBuildEvent>` that referenced `C:\Users\ZachGlenwright\Documents\Pre-Compile.vbs` (original author's machine-specific script).

### 2. BOM Removal
- Removed UTF-8 BOM from `hotKeySettings.vb`, `optionsWindow.vb`, and `playlistWindow.vb`.

---

## Files Modified

| File | Type of Change |
|------|---------------|
| `MPC-HC Looper VB.vbproj` | Removed pre-build event |
| `MPC_Enum.vb` | Added new global settings variables |
| `hotKeySettings.Designer.vb` | Added DISABLE button, enabled multi-select |
| `hotKeySettings.vb` | Per-hotkey disable/enable logic, multi-select support |
| `mainWindow.vb` | Bug fixes (position, timing, offsets), new settings load, hotkey skip |
| `optionsWindow.Designer.vb` | New UI controls for all new options |
| `optionsWindow.vb` | Bug fixes, new options save/load, browse button |
| `playlistWindow.vb` | Bug fixes, auto-save, real-time search, TopMost dialogs, file relocator |
