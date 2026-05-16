<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub InitializeComponent()
        Me.fileListView = New System.Windows.Forms.ListView()
        Me.colFileName = New System.Windows.Forms.ColumnHeader()
        Me.colPath = New System.Windows.Forms.ColumnHeader()
        Me.colStatus = New System.Windows.Forms.ColumnHeader()
        Me.colResult = New System.Windows.Forms.ColumnHeader()
        Me.colOutput = New System.Windows.Forms.ColumnHeader()
        Me.addButton = New System.Windows.Forms.Button()
        Me.removeButton = New System.Windows.Forms.Button()
        Me.startFixButton = New System.Windows.Forms.Button()
        Me.stopButton = New System.Windows.Forms.Button()
        Me.replaceOriginalCB = New System.Windows.Forms.CheckBox()
        Me.statusLabel = New System.Windows.Forms.Label()
        Me.progressBar = New System.Windows.Forms.ProgressBar()
        Me.openLogButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        ' fileListView
        '
        Me.fileListView.AllowDrop = True
        Me.fileListView.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.fileListView.BackColor = Color.FromArgb(30, 30, 48)
        Me.fileListView.ForeColor = Color.White
        Me.fileListView.BorderStyle = BorderStyle.None
        Me.fileListView.Columns.AddRange(New ColumnHeader() {Me.colFileName, Me.colPath, Me.colStatus, Me.colResult, Me.colOutput})
        Me.fileListView.FullRowSelect = True
        Me.fileListView.GridLines = True
        Me.fileListView.HeaderStyle = ColumnHeaderStyle.Nonclickable
        Me.fileListView.Location = New Point(12, 12)
        Me.fileListView.Name = "fileListView"
        Me.fileListView.Size = New Size(660, 290)
        Me.fileListView.TabIndex = 0
        Me.fileListView.UseCompatibleStateImageBehavior = False
        Me.fileListView.View = View.Details
        '
        ' colFileName
        '
        Me.colFileName.Text = "File Name"
        Me.colFileName.Width = 180
        '
        ' colPath
        '
        Me.colPath.Text = "Location"
        Me.colPath.Width = 170
        '
        ' colStatus
        '
        Me.colStatus.Text = "Status"
        Me.colStatus.Width = 70
        '
        ' colResult
        '
        Me.colResult.Text = "Result"
        Me.colResult.Width = 120
        '
        ' colOutput
        '
        Me.colOutput.Text = "Output"
        Me.colOutput.Width = 120
        '
        ' addButton
        '
        Me.addButton.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.addButton.BackColor = Color.FromArgb(42, 90, 74)
        Me.addButton.FlatAppearance.BorderColor = Color.FromArgb(68, 170, 136)
        Me.addButton.FlatStyle = FlatStyle.Flat
        Me.addButton.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
        Me.addButton.ForeColor = Color.White
        Me.addButton.Location = New Point(12, 310)
        Me.addButton.Name = "addButton"
        Me.addButton.Size = New Size(40, 32)
        Me.addButton.TabIndex = 1
        Me.addButton.Text = "+"
        Me.addButton.UseVisualStyleBackColor = False
        '
        ' removeButton
        '
        Me.removeButton.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.removeButton.BackColor = Color.FromArgb(74, 42, 42)
        Me.removeButton.FlatAppearance.BorderColor = Color.FromArgb(136, 85, 85)
        Me.removeButton.FlatStyle = FlatStyle.Flat
        Me.removeButton.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
        Me.removeButton.ForeColor = Color.White
        Me.removeButton.Location = New Point(58, 310)
        Me.removeButton.Name = "removeButton"
        Me.removeButton.Size = New Size(40, 32)
        Me.removeButton.TabIndex = 2
        Me.removeButton.Text = "-"
        Me.removeButton.UseVisualStyleBackColor = False
        '
        ' replaceOriginalCB
        '
        Me.replaceOriginalCB.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.replaceOriginalCB.AutoSize = True
        Me.replaceOriginalCB.ForeColor = Color.White
        Me.replaceOriginalCB.Location = New Point(110, 317)
        Me.replaceOriginalCB.Name = "replaceOriginalCB"
        Me.replaceOriginalCB.Size = New Size(120, 19)
        Me.replaceOriginalCB.TabIndex = 6
        Me.replaceOriginalCB.Text = "Replace original"
        Me.replaceOriginalCB.UseVisualStyleBackColor = False
        '
        ' openLogButton
        '
        Me.openLogButton.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.openLogButton.BackColor = Color.FromArgb(51, 51, 51)
        Me.openLogButton.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 85)
        Me.openLogButton.FlatStyle = FlatStyle.Flat
        Me.openLogButton.Font = New Font("Segoe UI", 8.0!)
        Me.openLogButton.ForeColor = Color.White
        Me.openLogButton.Location = New Point(442, 310)
        Me.openLogButton.Name = "openLogButton"
        Me.openLogButton.Size = New Size(50, 32)
        Me.openLogButton.TabIndex = 7
        Me.openLogButton.Text = "Log"
        Me.openLogButton.UseVisualStyleBackColor = False
        '
        ' stopButton
        '
        Me.stopButton.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.stopButton.BackColor = Color.FromArgb(74, 42, 42)
        Me.stopButton.FlatAppearance.BorderColor = Color.FromArgb(136, 85, 85)
        Me.stopButton.FlatStyle = FlatStyle.Flat
        Me.stopButton.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.stopButton.ForeColor = Color.White
        Me.stopButton.Location = New Point(498, 310)
        Me.stopButton.Name = "stopButton"
        Me.stopButton.Size = New Size(70, 32)
        Me.stopButton.TabIndex = 4
        Me.stopButton.Text = "Stop"
        Me.stopButton.UseVisualStyleBackColor = False
        Me.stopButton.Visible = False
        '
        ' startFixButton
        '
        Me.startFixButton.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.startFixButton.BackColor = Color.FromArgb(42, 58, 90)
        Me.startFixButton.FlatAppearance.BorderColor = Color.FromArgb(102, 136, 170)
        Me.startFixButton.FlatStyle = FlatStyle.Flat
        Me.startFixButton.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.startFixButton.ForeColor = Color.White
        Me.startFixButton.Location = New Point(572, 310)
        Me.startFixButton.Name = "startFixButton"
        Me.startFixButton.Size = New Size(100, 32)
        Me.startFixButton.TabIndex = 3
        Me.startFixButton.Text = "Start Fix"
        Me.startFixButton.UseVisualStyleBackColor = False
        '
        ' statusLabel
        '
        Me.statusLabel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.statusLabel.ForeColor = Color.White
        Me.statusLabel.Location = New Point(12, 350)
        Me.statusLabel.Name = "statusLabel"
        Me.statusLabel.Size = New Size(660, 20)
        Me.statusLabel.TabIndex = 5
        Me.statusLabel.Text = "Drag && drop .looper files here, or click + to add"
        '
        ' progressBar
        '
        Me.progressBar.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.progressBar.Location = New Point(12, 372)
        Me.progressBar.Name = "progressBar"
        Me.progressBar.Size = New Size(660, 4)
        Me.progressBar.Style = ProgressBarStyle.Continuous
        Me.progressBar.TabIndex = 8
        Me.progressBar.Visible = False
        '
        ' MainForm
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.FromArgb(30, 30, 48)
        Me.ForeColor = Color.White
        Me.ClientSize = New Size(684, 385)
        Me.Controls.Add(Me.progressBar)
        Me.Controls.Add(Me.openLogButton)
        Me.Controls.Add(Me.replaceOriginalCB)
        Me.Controls.Add(Me.statusLabel)
        Me.Controls.Add(Me.stopButton)
        Me.Controls.Add(Me.startFixButton)
        Me.Controls.Add(Me.removeButton)
        Me.Controls.Add(Me.addButton)
        Me.Controls.Add(Me.fileListView)
        Me.Font = New Font("Segoe UI", 9.0!)
        Me.MinimumSize = New Size(600, 320)
        Me.Name = "MainForm"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "Looper Fix Tool"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents fileListView As ListView
    Friend WithEvents colFileName As ColumnHeader
    Friend WithEvents colPath As ColumnHeader
    Friend WithEvents colStatus As ColumnHeader
    Friend WithEvents colResult As ColumnHeader
    Friend WithEvents colOutput As ColumnHeader
    Friend WithEvents addButton As Button
    Friend WithEvents removeButton As Button
    Friend WithEvents startFixButton As Button
    Friend WithEvents stopButton As Button
    Friend WithEvents replaceOriginalCB As CheckBox
    Friend WithEvents statusLabel As Label
    Friend WithEvents progressBar As ProgressBar
    Friend WithEvents openLogButton As Button

End Class
