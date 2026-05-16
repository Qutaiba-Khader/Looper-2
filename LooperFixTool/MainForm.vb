Imports System.IO
Imports System.Diagnostics
Imports System.Threading

Public Class MainForm

    Private Const DEFAULT_EVERYTHING_EXE As String = "C:\Program Files\Everything\Everything.exe"
    Private everythingExePath As String = DEFAULT_EVERYTHING_EXE
    Private startedEverything As Boolean = False
    Private isRunning As Boolean = False
    Private cancelRequested As Boolean = False

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Clear log on startup
        Try
            File.WriteAllText(LooperFixer.LogPath, "=== Looper Fix Tool started " & DateTime.Now.ToString() & " ===" & vbCrLf)
        Catch
        End Try

        ' Accept command-line args (drag-drop onto exe)
        Dim args = Environment.GetCommandLineArgs()
        For i = 1 To args.Length - 1
            If args(i).EndsWith(".looper", StringComparison.OrdinalIgnoreCase) AndAlso File.Exists(args(i)) Then
                AddFileToList(args(i))
            End If
        Next
    End Sub

    Private Sub MainForm_DragEnter(sender As Object, e As DragEventArgs) Handles MyBase.DragEnter, fileListView.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub MainForm_DragDrop(sender As Object, e As DragEventArgs) Handles MyBase.DragDrop, fileListView.DragDrop
        If isRunning Then Return
        Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
        For Each f In files
            If f.EndsWith(".looper", StringComparison.OrdinalIgnoreCase) AndAlso File.Exists(f) Then
                AddFileToList(f)
            ElseIf Directory.Exists(f) Then
                ' If a folder is dropped, add all .looper files in it
                For Each lf In Directory.GetFiles(f, "*.looper", SearchOption.TopDirectoryOnly)
                    AddFileToList(lf)
                Next
            End If
        Next
    End Sub

    Private Sub addButton_Click(sender As Object, e As EventArgs) Handles addButton.Click
        If isRunning Then Return
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select .looper files"
            ofd.Filter = "Looper Files (*.looper)|*.looper"
            ofd.Multiselect = True
            If ofd.ShowDialog() = DialogResult.OK Then
                For Each f In ofd.FileNames
                    AddFileToList(f)
                Next
            End If
        End Using
    End Sub

    Private Sub removeButton_Click(sender As Object, e As EventArgs) Handles removeButton.Click
        If isRunning Then Return
        For Each item As ListViewItem In fileListView.SelectedItems
            fileListView.Items.Remove(item)
        Next
        UpdateStatus()
    End Sub

    Private Sub stopButton_Click(sender As Object, e As EventArgs) Handles stopButton.Click
        cancelRequested = True
        stopButton.Enabled = False
        stopButton.Text = "Stopping"
        LooperFixer.Log("*** STOP REQUESTED ***")
    End Sub

    Private Sub openLogButton_Click(sender As Object, e As EventArgs) Handles openLogButton.Click
        If File.Exists(LooperFixer.LogPath) Then
            Process.Start("notepad.exe", LooperFixer.LogPath)
        Else
            MessageBox.Show("No log file yet.", "Log", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub startFixButton_Click(sender As Object, e As EventArgs) Handles startFixButton.Click
        If isRunning Then Return
        If fileListView.Items.Count = 0 Then
            MessageBox.Show("Add .looper files first.", "No Files", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Find Everything
        If Not FindEverythingExe() Then Return
        If Not EnsureEverythingRunning() Then Return

        ' Start fixing in background
        isRunning = True
        cancelRequested = False
        startFixButton.Visible = False
        stopButton.Visible = True
        stopButton.Enabled = True
        stopButton.Text = "Stop"
        addButton.Enabled = False
        removeButton.Enabled = False
        replaceOriginalCB.Enabled = False
        progressBar.Visible = True
        progressBar.Maximum = fileListView.Items.Count
        progressBar.Value = 0

        Dim replaceOriginal = replaceOriginalCB.Checked

        ' Clear log for this run
        LooperFixer.Log("")
        LooperFixer.Log("========== NEW RUN: " & fileListView.Items.Count & " files, replaceOriginal=" & replaceOriginal & " ==========")

        Dim files As New List(Of String)
        For Each item As ListViewItem In fileListView.Items
            files.Add(item.Tag.ToString())
            item.SubItems(2).Text = "Waiting..."
            item.SubItems(3).Text = ""
            item.SubItems(4).Text = ""
            item.ForeColor = Color.FromArgb(220, 220, 220)
        Next

        Dim worker As New Thread(Sub() RunFix(files, replaceOriginal))
        worker.IsBackground = True
        worker.Start()
    End Sub

    Private Sub RunFix(files As List(Of String), replaceOriginal As Boolean)
        Dim totalFixed = 0
        Dim totalFailed = 0
        Dim totalSkipped = 0

        For i = 0 To files.Count - 1
            If cancelRequested Then
                Me.Invoke(Sub()
                              LooperFixer.Log("*** Cancelled at file " & (i + 1) & " of " & files.Count)
                              statusLabel.Text = "Stopped at " & (i + 1) & " of " & files.Count
                          End Sub)
                Exit For
            End If

            Dim idx = i
            Me.Invoke(Sub()
                          fileListView.Items(idx).SubItems(2).Text = "Fixing..."
                          fileListView.Items(idx).ForeColor = Color.FromArgb(255, 200, 60)
                          fileListView.EnsureVisible(idx)
                          statusLabel.Text = "Processing " & (idx + 1) & " of " & files.Count & ": " & Path.GetFileName(files(idx))
                      End Sub)

            Dim result = FixLooperFile(files(i), replaceOriginal)

            Me.Invoke(Sub()
                          Dim item = fileListView.Items(idx)
                          If result.Skipped Then
                              item.SubItems(2).Text = "OK"
                              item.SubItems(3).Text = "No broken paths"
                              item.SubItems(4).Text = ""
                              item.ForeColor = Color.FromArgb(120, 120, 120)
                              totalSkipped += 1
                          ElseIf result.BrokenEvents = 0 Then
                              item.SubItems(2).Text = "OK"
                              item.SubItems(3).Text = "All valid"
                              item.SubItems(4).Text = ""
                              item.ForeColor = Color.FromArgb(120, 120, 120)
                              totalSkipped += 1
                          Else
                              item.SubItems(2).Text = "Done"
                              item.SubItems(3).Text = result.FixedEvents & " fixed, " & result.UnfixableEvents & " failed"
                              If result.OutputPath IsNot Nothing Then
                                  item.SubItems(4).Text = Path.GetFileName(result.OutputPath)
                              End If
                              If result.UnfixableEvents > 0 AndAlso result.FixedEvents = 0 Then
                                  item.ForeColor = Color.FromArgb(255, 80, 80)
                              ElseIf result.UnfixableEvents > 0 Then
                                  item.ForeColor = Color.FromArgb(255, 180, 60)
                              Else
                                  item.ForeColor = Color.FromArgb(100, 220, 100)
                              End If
                              totalFixed += result.FixedEvents
                              totalFailed += result.UnfixableEvents
                          End If
                          progressBar.Value = idx + 1
                      End Sub)
        Next

        ' Close Everything if we started it
        If startedEverything Then
            Try
                Process.Start(everythingExePath, "-close")
            Catch
            End Try
            startedEverything = False
        End If

        Me.Invoke(Sub()
                      isRunning = False
                      startFixButton.Visible = True
                      stopButton.Visible = False
                      addButton.Enabled = True
                      removeButton.Enabled = True
                      replaceOriginalCB.Enabled = True
                      progressBar.Visible = False
                      statusLabel.Text = "Done! " & totalFixed & " fixed, " & totalFailed & " failed, " & totalSkipped & " already OK"
                      LooperFixer.Log("========== DONE: " & totalFixed & " fixed, " & totalFailed & " failed, " & totalSkipped & " skipped ==========")
                  End Sub)
    End Sub

    Private Sub AddFileToList(filePath As String)
        ' Check if already in list
        For Each existing As ListViewItem In fileListView.Items
            If String.Equals(existing.Tag.ToString(), filePath, StringComparison.OrdinalIgnoreCase) Then
                Return
            End If
        Next

        Dim item As New ListViewItem(Path.GetFileName(filePath))
        item.SubItems.Add(Path.GetDirectoryName(filePath))
        item.SubItems.Add("Pending")
        item.SubItems.Add("")
        item.SubItems.Add("")
        item.Tag = filePath
        fileListView.Items.Add(item)
        UpdateStatus()
    End Sub

    Private Sub UpdateStatus()
        If fileListView.Items.Count = 0 Then
            statusLabel.Text = "Drag && drop .looper files here, or click + to add"
        Else
            statusLabel.Text = fileListView.Items.Count & " file(s) ready"
        End If
    End Sub

    Private Function FindEverythingExe() As Boolean
        If File.Exists(DEFAULT_EVERYTHING_EXE) Then
            everythingExePath = DEFAULT_EVERYTHING_EXE
            Return True
        End If

        Dim x86Path = "C:\Program Files (x86)\Everything\Everything.exe"
        If File.Exists(x86Path) Then
            everythingExePath = x86Path
            Return True
        End If

        Using ofd As New OpenFileDialog()
            ofd.Title = "Locate Everything.exe"
            ofd.Filter = "Everything (Everything.exe)|Everything.exe"
            ofd.FileName = "Everything.exe"
            If ofd.ShowDialog() = DialogResult.OK Then
                everythingExePath = ofd.FileName
                Return True
            End If
        End Using
        Return False
    End Function

    Private Function EnsureEverythingRunning() As Boolean
        Dim procs = Process.GetProcessesByName("Everything")
        If procs.Length > 0 Then
            LooperFixer.Log("Everything already running")
            Return True
        End If

        statusLabel.Text = "Starting Everything..."
        Application.DoEvents()

        Try
            Process.Start(everythingExePath, "-minimized")
            startedEverything = True
            LooperFixer.Log("Started Everything: " & everythingExePath)
        Catch ex As Exception
            MessageBox.Show("Could not start Everything:" & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        For i = 1 To 20
            Thread.Sleep(500)
            Try
                If IsEverythingAvailable() Then
                    LooperFixer.Log("Everything ready")
                    Return True
                End If
            Catch
            End Try
        Next

        MessageBox.Show("Everything started but is not responding. Try again.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Return False
    End Function

End Class
