<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class mainWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.controlsPanel = New System.Windows.Forms.Panel()
        Me.filterPanel = New System.Windows.Forms.Panel()
        Me.bottomPanel = New System.Windows.Forms.Panel()
        Me.loopModeButton = New System.Windows.Forms.Button()
        Me.inPointSlipLeftButton = New System.Windows.Forms.Button()
        Me.inTF = New System.Windows.Forms.TextBox()
        Me.inPointButton = New System.Windows.Forms.Button()
        Me.inPointSlipRightButton = New System.Windows.Forms.Button()
        Me.outPointSlipLeftButton = New System.Windows.Forms.Button()
        Me.outTF = New System.Windows.Forms.TextBox()
        Me.outPointButton = New System.Windows.Forms.Button()
        Me.outPointSlipRightButton = New System.Windows.Forms.Button()
        Me.speedDownButton = New System.Windows.Forms.Button()
        Me.speedValueLabel = New System.Windows.Forms.Label()
        Me.speedUpButton = New System.Windows.Forms.Button()
        Me.loadOptionsWindowButton = New System.Windows.Forms.Button()
        Me.showPlayingFileInExplorerButton = New System.Windows.Forms.Button()
        Me.alwaysOnTopButton = New System.Windows.Forms.Button()
        Me.searchTF = New System.Windows.Forms.TextBox()
        Me.eventCountLabel = New System.Windows.Forms.Label()
        Me.prevEventButton = New System.Windows.Forms.Button()
        Me.nextEventButton = New System.Windows.Forms.Button()
        Me.addEventButton = New System.Windows.Forms.Button()
        Me.modifyEventButton = New System.Windows.Forms.Button()
        Me.deleteEventButton = New System.Windows.Forms.Button()
        Me.saveButton = New System.Windows.Forms.Button()
        Me.currentPositionLabel = New System.Windows.Forms.Label()
        Me.fixButton = New System.Windows.Forms.Button()
        Me.fixToolButton = New System.Windows.Forms.Button()
        Me.eventsList = New MPC_HC_Looper_VB.ListViewDoubleBuffered()
        Me.eventPosHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.eventNameHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.speedColumn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.loopsColumn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.eventINPointHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.eventOutPointHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.eventDurHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.eventFilenameHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.listViewEditor = New System.Windows.Forms.TextBox()
        Me.speedSlider = New System.Windows.Forms.TrackBar()
        Me.slipTimer = New System.Windows.Forms.Timer(Me.components)
        Me.toolTips = New System.Windows.Forms.ToolTip(Me.components)
        Me.currentPositionBG = New System.Windows.Forms.Label()
        Me.currentRemainingStatus = New System.Windows.Forms.Label()
        Me.hotkeysTF = New System.Windows.Forms.Label()
        Me.loopsRepeatTF = New System.Windows.Forms.Label()
        Me.menu_Add = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.menuItem_AddAtEnd = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItem_InsertEvents = New System.Windows.Forms.ToolStripMenuItem()
        Me.menu_Save = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.menuItem_SaveSelection = New System.Windows.Forms.ToolStripMenuItem()
        Me.menu_load = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.menuItem_importLooperFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.controlsPanel.SuspendLayout()
        Me.filterPanel.SuspendLayout()
        Me.bottomPanel.SuspendLayout()
        Me.menu_Add.SuspendLayout()
        Me.menu_Save.SuspendLayout()
        Me.menu_load.SuspendLayout()
        CType(Me.speedSlider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'controlsPanel
        '
        Me.controlsPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.controlsPanel.Controls.Add(Me.loopModeButton)
        Me.controlsPanel.Controls.Add(Me.inPointSlipLeftButton)
        Me.controlsPanel.Controls.Add(Me.inTF)
        Me.controlsPanel.Controls.Add(Me.inPointButton)
        Me.controlsPanel.Controls.Add(Me.inPointSlipRightButton)
        Me.controlsPanel.Controls.Add(Me.outPointSlipLeftButton)
        Me.controlsPanel.Controls.Add(Me.outTF)
        Me.controlsPanel.Controls.Add(Me.outPointButton)
        Me.controlsPanel.Controls.Add(Me.outPointSlipRightButton)
        Me.controlsPanel.Controls.Add(Me.speedDownButton)
        Me.controlsPanel.Controls.Add(Me.speedValueLabel)
        Me.controlsPanel.Controls.Add(Me.speedUpButton)
        Me.controlsPanel.Controls.Add(Me.loadOptionsWindowButton)
        Me.controlsPanel.Controls.Add(Me.showPlayingFileInExplorerButton)
        Me.controlsPanel.Controls.Add(Me.alwaysOnTopButton)
        Me.controlsPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.controlsPanel.Location = New System.Drawing.Point(0, 0)
        Me.controlsPanel.Name = "controlsPanel"
        Me.controlsPanel.Size = New System.Drawing.Size(750, 40)
        Me.controlsPanel.TabIndex = 0
        '
        'filterPanel
        '
        Me.filterPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.filterPanel.Controls.Add(Me.searchTF)
        Me.filterPanel.Controls.Add(Me.eventCountLabel)
        Me.filterPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.filterPanel.Location = New System.Drawing.Point(0, 40)
        Me.filterPanel.Name = "filterPanel"
        Me.filterPanel.Size = New System.Drawing.Size(750, 32)
        Me.filterPanel.TabIndex = 1
        '
        'bottomPanel
        '
        Me.bottomPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.bottomPanel.Controls.Add(Me.prevEventButton)
        Me.bottomPanel.Controls.Add(Me.nextEventButton)
        Me.bottomPanel.Controls.Add(Me.addEventButton)
        Me.bottomPanel.Controls.Add(Me.modifyEventButton)
        Me.bottomPanel.Controls.Add(Me.deleteEventButton)
        Me.bottomPanel.Controls.Add(Me.saveButton)
        Me.bottomPanel.Controls.Add(Me.currentPositionLabel)
        Me.bottomPanel.Controls.Add(Me.fixButton)
        Me.bottomPanel.Controls.Add(Me.fixToolButton)
        Me.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.bottomPanel.Location = New System.Drawing.Point(0, 482)
        Me.bottomPanel.Name = "bottomPanel"
        Me.bottomPanel.Size = New System.Drawing.Size(750, 38)
        Me.bottomPanel.TabIndex = 2
        '
        'loopModeButton
        '
        Me.loopModeButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(74, Byte), Integer), CType(CType(58, Byte), Integer))
        Me.loopModeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(136, Byte), Integer))
        Me.loopModeButton.FlatAppearance.BorderSize = 1
        Me.loopModeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.loopModeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.loopModeButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.loopModeButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(136, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer))
        Me.loopModeButton.Location = New System.Drawing.Point(8, 7)
        Me.loopModeButton.Name = "loopModeButton"
        Me.loopModeButton.Size = New System.Drawing.Size(90, 26)
        Me.loopModeButton.TabIndex = 0
        Me.loopModeButton.Text = "LOOP"
        Me.loopModeButton.UseVisualStyleBackColor = False
        '
        'inPointSlipLeftButton
        '
        Me.inPointSlipLeftButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.inPointSlipLeftButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.inPointSlipLeftButton.FlatAppearance.BorderSize = 1
        Me.inPointSlipLeftButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.inPointSlipLeftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.inPointSlipLeftButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.inPointSlipLeftButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.inPointSlipLeftButton.Location = New System.Drawing.Point(104, 7)
        Me.inPointSlipLeftButton.Name = "inPointSlipLeftButton"
        Me.inPointSlipLeftButton.Size = New System.Drawing.Size(26, 26)
        Me.inPointSlipLeftButton.TabIndex = 1
        Me.inPointSlipLeftButton.Text = "-"
        Me.inPointSlipLeftButton.UseVisualStyleBackColor = False
        '
        'inTF
        '
        Me.inTF.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.inTF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.inTF.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.inTF.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.inTF.Location = New System.Drawing.Point(132, 8)
        Me.inTF.Name = "inTF"
        Me.inTF.Size = New System.Drawing.Size(72, 24)
        Me.inTF.TabIndex = 2
        Me.inTF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'inPointButton
        '
        Me.inPointButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(74, Byte), Integer), CType(CType(90, Byte), Integer))
        Me.inPointButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.inPointButton.FlatAppearance.BorderSize = 1
        Me.inPointButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.inPointButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.inPointButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.inPointButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(136, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.inPointButton.Location = New System.Drawing.Point(206, 7)
        Me.inPointButton.Name = "inPointButton"
        Me.inPointButton.Size = New System.Drawing.Size(50, 26)
        Me.inPointButton.TabIndex = 3
        Me.inPointButton.Text = "SET A"
        Me.inPointButton.UseVisualStyleBackColor = False
        '
        'inPointSlipRightButton
        '
        Me.inPointSlipRightButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.inPointSlipRightButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.inPointSlipRightButton.FlatAppearance.BorderSize = 1
        Me.inPointSlipRightButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.inPointSlipRightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.inPointSlipRightButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.inPointSlipRightButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.inPointSlipRightButton.Location = New System.Drawing.Point(258, 7)
        Me.inPointSlipRightButton.Name = "inPointSlipRightButton"
        Me.inPointSlipRightButton.Size = New System.Drawing.Size(26, 26)
        Me.inPointSlipRightButton.TabIndex = 4
        Me.inPointSlipRightButton.Text = "+"
        Me.inPointSlipRightButton.UseVisualStyleBackColor = False
        '
        'outPointSlipLeftButton
        '
        Me.outPointSlipLeftButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.outPointSlipLeftButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.outPointSlipLeftButton.FlatAppearance.BorderSize = 1
        Me.outPointSlipLeftButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.outPointSlipLeftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.outPointSlipLeftButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.outPointSlipLeftButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.outPointSlipLeftButton.Location = New System.Drawing.Point(290, 7)
        Me.outPointSlipLeftButton.Name = "outPointSlipLeftButton"
        Me.outPointSlipLeftButton.Size = New System.Drawing.Size(26, 26)
        Me.outPointSlipLeftButton.TabIndex = 5
        Me.outPointSlipLeftButton.Text = "-"
        Me.outPointSlipLeftButton.UseVisualStyleBackColor = False
        '
        'outTF
        '
        Me.outTF.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.outTF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.outTF.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.outTF.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.outTF.Location = New System.Drawing.Point(318, 8)
        Me.outTF.Name = "outTF"
        Me.outTF.Size = New System.Drawing.Size(72, 24)
        Me.outTF.TabIndex = 6
        Me.outTF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'outPointButton
        '
        Me.outPointButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(74, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.outPointButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(136, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.outPointButton.FlatAppearance.BorderSize = 1
        Me.outPointButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.outPointButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.outPointButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.outPointButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(136, Byte), Integer))
        Me.outPointButton.Location = New System.Drawing.Point(392, 7)
        Me.outPointButton.Name = "outPointButton"
        Me.outPointButton.Size = New System.Drawing.Size(50, 26)
        Me.outPointButton.TabIndex = 7
        Me.outPointButton.Text = "SET B"
        Me.outPointButton.UseVisualStyleBackColor = False
        '
        'outPointSlipRightButton
        '
        Me.outPointSlipRightButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.outPointSlipRightButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.outPointSlipRightButton.FlatAppearance.BorderSize = 1
        Me.outPointSlipRightButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.outPointSlipRightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.outPointSlipRightButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.outPointSlipRightButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.outPointSlipRightButton.Location = New System.Drawing.Point(444, 7)
        Me.outPointSlipRightButton.Name = "outPointSlipRightButton"
        Me.outPointSlipRightButton.Size = New System.Drawing.Size(26, 26)
        Me.outPointSlipRightButton.TabIndex = 8
        Me.outPointSlipRightButton.Text = "+"
        Me.outPointSlipRightButton.UseVisualStyleBackColor = False
        '
        'speedDownButton
        '
        Me.speedDownButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.speedDownButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.speedDownButton.FlatAppearance.BorderSize = 1
        Me.speedDownButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.speedDownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.speedDownButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.speedDownButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.speedDownButton.Location = New System.Drawing.Point(476, 7)
        Me.speedDownButton.Name = "speedDownButton"
        Me.speedDownButton.Size = New System.Drawing.Size(26, 26)
        Me.speedDownButton.TabIndex = 9
        Me.speedDownButton.Text = "-"
        Me.speedDownButton.UseVisualStyleBackColor = False
        '
        'speedValueLabel
        '
        Me.speedValueLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.speedValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.speedValueLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.speedValueLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(218, Byte), Integer))
        Me.speedValueLabel.Location = New System.Drawing.Point(504, 8)
        Me.speedValueLabel.Name = "speedValueLabel"
        Me.speedValueLabel.Size = New System.Drawing.Size(48, 24)
        Me.speedValueLabel.TabIndex = 10
        Me.speedValueLabel.Text = "100%"
        Me.speedValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'speedUpButton
        '
        Me.speedUpButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.speedUpButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.speedUpButton.FlatAppearance.BorderSize = 1
        Me.speedUpButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.speedUpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.speedUpButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.speedUpButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.speedUpButton.Location = New System.Drawing.Point(554, 7)
        Me.speedUpButton.Name = "speedUpButton"
        Me.speedUpButton.Size = New System.Drawing.Size(26, 26)
        Me.speedUpButton.TabIndex = 11
        Me.speedUpButton.Text = "+"
        Me.speedUpButton.UseVisualStyleBackColor = False
        '
        'loadOptionsWindowButton
        '
        Me.loadOptionsWindowButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.loadOptionsWindowButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.loadOptionsWindowButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.loadOptionsWindowButton.FlatAppearance.BorderSize = 1
        Me.loadOptionsWindowButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.loadOptionsWindowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.loadOptionsWindowButton.Font = New System.Drawing.Font("Segoe MDL2 Assets", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.loadOptionsWindowButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.loadOptionsWindowButton.Location = New System.Drawing.Point(644, 6)
        Me.loadOptionsWindowButton.Name = "loadOptionsWindowButton"
        Me.loadOptionsWindowButton.Size = New System.Drawing.Size(34, 28)
        Me.loadOptionsWindowButton.TabIndex = 12
        Me.loadOptionsWindowButton.Text = ChrW(&HE713)
        Me.loadOptionsWindowButton.UseVisualStyleBackColor = False
        '
        'showPlayingFileInExplorerButton
        '
        Me.showPlayingFileInExplorerButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.showPlayingFileInExplorerButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.showPlayingFileInExplorerButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.showPlayingFileInExplorerButton.FlatAppearance.BorderSize = 1
        Me.showPlayingFileInExplorerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.showPlayingFileInExplorerButton.ContextMenuStrip = Me.menu_load
        Me.showPlayingFileInExplorerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.showPlayingFileInExplorerButton.Font = New System.Drawing.Font("Segoe MDL2 Assets", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.showPlayingFileInExplorerButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.showPlayingFileInExplorerButton.Location = New System.Drawing.Point(680, 6)
        Me.showPlayingFileInExplorerButton.Name = "showPlayingFileInExplorerButton"
        Me.showPlayingFileInExplorerButton.Size = New System.Drawing.Size(34, 28)
        Me.showPlayingFileInExplorerButton.TabIndex = 13
        Me.showPlayingFileInExplorerButton.Text = ChrW(&HE838)
        Me.showPlayingFileInExplorerButton.UseVisualStyleBackColor = False
        '
        'alwaysOnTopButton
        '
        Me.alwaysOnTopButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.alwaysOnTopButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.alwaysOnTopButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.alwaysOnTopButton.FlatAppearance.BorderSize = 1
        Me.alwaysOnTopButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.alwaysOnTopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.alwaysOnTopButton.Font = New System.Drawing.Font("Segoe MDL2 Assets", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.alwaysOnTopButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.alwaysOnTopButton.Location = New System.Drawing.Point(716, 6)
        Me.alwaysOnTopButton.Name = "alwaysOnTopButton"
        Me.alwaysOnTopButton.Size = New System.Drawing.Size(34, 28)
        Me.alwaysOnTopButton.TabIndex = 14
        Me.alwaysOnTopButton.Text = ChrW(&HE718)
        Me.alwaysOnTopButton.UseVisualStyleBackColor = False
        '
        'searchTF
        '
        Me.searchTF.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.searchTF.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.searchTF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.searchTF.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.searchTF.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.searchTF.Location = New System.Drawing.Point(8, 5)
        Me.searchTF.Name = "searchTF"
        Me.searchTF.Size = New System.Drawing.Size(620, 24)
        Me.searchTF.TabIndex = 0
        '
        'eventCountLabel
        '
        Me.eventCountLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.eventCountLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.eventCountLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.eventCountLabel.Location = New System.Drawing.Point(634, 5)
        Me.eventCountLabel.Name = "eventCountLabel"
        Me.eventCountLabel.Size = New System.Drawing.Size(110, 24)
        Me.eventCountLabel.TabIndex = 1
        Me.eventCountLabel.Text = "0 events"
        Me.eventCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'prevEventButton
        '
        Me.prevEventButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.prevEventButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.prevEventButton.FlatAppearance.BorderSize = 1
        Me.prevEventButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.prevEventButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.prevEventButton.Font = New System.Drawing.Font("Segoe MDL2 Assets", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.prevEventButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.prevEventButton.Location = New System.Drawing.Point(8, 6)
        Me.prevEventButton.Name = "prevEventButton"
        Me.prevEventButton.Size = New System.Drawing.Size(30, 26)
        Me.prevEventButton.TabIndex = 0
        Me.prevEventButton.Text = ChrW(&HE76B)
        Me.prevEventButton.UseVisualStyleBackColor = False
        '
        'nextEventButton
        '
        Me.nextEventButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.nextEventButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.nextEventButton.FlatAppearance.BorderSize = 1
        Me.nextEventButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.nextEventButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.nextEventButton.Font = New System.Drawing.Font("Segoe MDL2 Assets", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nextEventButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.nextEventButton.Location = New System.Drawing.Point(40, 6)
        Me.nextEventButton.Name = "nextEventButton"
        Me.nextEventButton.Size = New System.Drawing.Size(30, 26)
        Me.nextEventButton.TabIndex = 1
        Me.nextEventButton.Text = ChrW(&HE76C)
        Me.nextEventButton.UseVisualStyleBackColor = False
        '
        'addEventButton
        '
        Me.addEventButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(74, Byte), Integer))
        Me.addEventButton.ContextMenuStrip = Me.menu_Add
        Me.addEventButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(136, Byte), Integer))
        Me.addEventButton.FlatAppearance.BorderSize = 1
        Me.addEventButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.addEventButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.addEventButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.addEventButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.addEventButton.Location = New System.Drawing.Point(84, 6)
        Me.addEventButton.Name = "addEventButton"
        Me.addEventButton.Size = New System.Drawing.Size(60, 26)
        Me.addEventButton.TabIndex = 2
        Me.addEventButton.Text = "+ Add"
        Me.addEventButton.UseVisualStyleBackColor = False
        '
        'modifyEventButton
        '
        Me.modifyEventButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.modifyEventButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.modifyEventButton.FlatAppearance.BorderSize = 1
        Me.modifyEventButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.modifyEventButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.modifyEventButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.modifyEventButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.modifyEventButton.Location = New System.Drawing.Point(148, 6)
        Me.modifyEventButton.Name = "modifyEventButton"
        Me.modifyEventButton.Size = New System.Drawing.Size(68, 26)
        Me.modifyEventButton.TabIndex = 3
        Me.modifyEventButton.Text = "Modify"
        Me.modifyEventButton.UseVisualStyleBackColor = False
        '
        'deleteEventButton
        '
        Me.deleteEventButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(74, Byte), Integer), CType(CType(42, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.deleteEventButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(136, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.deleteEventButton.FlatAppearance.BorderSize = 1
        Me.deleteEventButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.deleteEventButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.deleteEventButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.deleteEventButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.deleteEventButton.Location = New System.Drawing.Point(220, 6)
        Me.deleteEventButton.Name = "deleteEventButton"
        Me.deleteEventButton.Size = New System.Drawing.Size(58, 26)
        Me.deleteEventButton.TabIndex = 4
        Me.deleteEventButton.Text = ChrW(215) & " Del"
        Me.deleteEventButton.UseVisualStyleBackColor = False
        '
        'saveButton
        '
        Me.saveButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.saveButton.ContextMenuStrip = Me.menu_Save
        Me.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.saveButton.FlatAppearance.BorderSize = 1
        Me.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.saveButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.saveButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.saveButton.Location = New System.Drawing.Point(292, 6)
        Me.saveButton.Name = "saveButton"
        Me.saveButton.Size = New System.Drawing.Size(64, 26)
        Me.saveButton.TabIndex = 5
        Me.saveButton.Text = "Save"
        Me.saveButton.UseVisualStyleBackColor = False
        '
        'currentPositionLabel
        '
        Me.currentPositionLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.currentPositionLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.currentPositionLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(218, Byte), Integer))
        Me.currentPositionLabel.Location = New System.Drawing.Point(548, 6)
        Me.currentPositionLabel.Name = "currentPositionLabel"
        Me.currentPositionLabel.Size = New System.Drawing.Size(90, 26)
        Me.currentPositionLabel.TabIndex = 6
        Me.currentPositionLabel.Text = "0:00.000"
        Me.currentPositionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'fixButton
        '
        Me.fixButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.fixButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(90, Byte), Integer))
        Me.fixButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(102, Byte), Integer), CType(CType(136, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.fixButton.FlatAppearance.BorderSize = 1
        Me.fixButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.fixButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.fixButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fixButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.fixButton.Location = New System.Drawing.Point(642, 6)
        Me.fixButton.Name = "fixButton"
        Me.fixButton.Size = New System.Drawing.Size(40, 26)
        Me.fixButton.TabIndex = 7
        Me.fixButton.Text = "Fix"
        Me.fixButton.UseVisualStyleBackColor = False
        '
        'fixToolButton
        '
        Me.fixToolButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.fixToolButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.fixToolButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.fixToolButton.FlatAppearance.BorderSize = 1
        Me.fixToolButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer), CType(CType(68, Byte), Integer))
        Me.fixToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.fixToolButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fixToolButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.fixToolButton.Location = New System.Drawing.Point(686, 6)
        Me.fixToolButton.Name = "fixToolButton"
        Me.fixToolButton.Size = New System.Drawing.Size(60, 26)
        Me.fixToolButton.TabIndex = 8
        Me.fixToolButton.Text = "Fix-Tool"
        Me.fixToolButton.UseVisualStyleBackColor = False
        '
        'eventsList
        '
        Me.eventsList.AllowDrop = True
        Me.eventsList.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.eventsList.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.eventsList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.eventPosHeader, Me.eventNameHeader, Me.speedColumn, Me.loopsColumn, Me.eventINPointHeader, Me.eventOutPointHeader, Me.eventDurHeader, Me.eventFilenameHeader})
        Me.eventsList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.eventsList.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.eventsList.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.eventsList.FullRowSelect = True
        Me.eventsList.GridLines = False
        Me.eventsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.eventsList.HideSelection = False
        Me.eventsList.Location = New System.Drawing.Point(0, 72)
        Me.eventsList.Name = "eventsList"
        Me.eventsList.Size = New System.Drawing.Size(750, 410)
        Me.eventsList.TabIndex = 3
        Me.eventsList.UseCompatibleStateImageBehavior = False
        Me.eventsList.View = System.Windows.Forms.View.Details
        '
        'eventPosHeader
        '
        Me.eventPosHeader.Text = "#"
        Me.eventPosHeader.Width = 35
        '
        'eventNameHeader
        '
        Me.eventNameHeader.Text = "Event Name"
        Me.eventNameHeader.Width = 200
        '
        'speedColumn
        '
        Me.speedColumn.Text = "Spd"
        Me.speedColumn.Width = 38
        '
        'loopsColumn
        '
        Me.loopsColumn.Text = ChrW(215)
        Me.loopsColumn.Width = 30
        '
        'eventINPointHeader
        '
        Me.eventINPointHeader.Text = "IN"
        Me.eventINPointHeader.Width = 72
        '
        'eventOutPointHeader
        '
        Me.eventOutPointHeader.Text = "OUT"
        Me.eventOutPointHeader.Width = 72
        '
        'eventDurHeader
        '
        Me.eventDurHeader.Text = "Duration"
        Me.eventDurHeader.Width = 68
        '
        'eventFilenameHeader
        '
        Me.eventFilenameHeader.Text = "File"
        Me.eventFilenameHeader.Width = 300
        '
        'listViewEditor
        '
        Me.listViewEditor.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.listViewEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.listViewEditor.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.listViewEditor.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.listViewEditor.Location = New System.Drawing.Point(0, 0)
        Me.listViewEditor.Name = "listViewEditor"
        Me.listViewEditor.Size = New System.Drawing.Size(200, 24)
        Me.listViewEditor.TabIndex = 4
        Me.listViewEditor.Visible = False
        '
        'speedSlider
        '
        Me.speedSlider.LargeChange = 10
        Me.speedSlider.Location = New System.Drawing.Point(-200, -200)
        Me.speedSlider.Maximum = 200
        Me.speedSlider.Name = "speedSlider"
        Me.speedSlider.Size = New System.Drawing.Size(100, 45)
        Me.speedSlider.SmallChange = 5
        Me.speedSlider.TabIndex = 5
        Me.speedSlider.TickFrequency = 20
        Me.speedSlider.Value = 100
        Me.speedSlider.Visible = False
        '
        'slipTimer
        '
        '
        'currentPositionBG
        '
        Me.currentPositionBG.Location = New System.Drawing.Point(0, 0)
        Me.currentPositionBG.Name = "currentPositionBG"
        Me.currentPositionBG.Size = New System.Drawing.Size(0, 0)
        Me.currentPositionBG.TabIndex = 6
        Me.currentPositionBG.Visible = False
        '
        'currentRemainingStatus
        '
        Me.currentRemainingStatus.Location = New System.Drawing.Point(0, 0)
        Me.currentRemainingStatus.Name = "currentRemainingStatus"
        Me.currentRemainingStatus.Size = New System.Drawing.Size(0, 0)
        Me.currentRemainingStatus.TabIndex = 7
        Me.currentRemainingStatus.Visible = False
        '
        'hotkeysTF
        '
        Me.hotkeysTF.Location = New System.Drawing.Point(0, 0)
        Me.hotkeysTF.Name = "hotkeysTF"
        Me.hotkeysTF.Size = New System.Drawing.Size(0, 0)
        Me.hotkeysTF.TabIndex = 8
        Me.hotkeysTF.Visible = False
        '
        'loopsRepeatTF
        '
        Me.loopsRepeatTF.Location = New System.Drawing.Point(0, 0)
        Me.loopsRepeatTF.Name = "loopsRepeatTF"
        Me.loopsRepeatTF.Size = New System.Drawing.Size(0, 0)
        Me.loopsRepeatTF.TabIndex = 9
        Me.loopsRepeatTF.Visible = False
        '
        'menu_Add
        '
        Me.menu_Add.ImageScalingSize = New System.Drawing.Size(28, 28)
        Me.menu_Add.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItem_AddAtEnd, Me.menuItem_InsertEvents})
        Me.menu_Add.Name = "menu_Add"
        Me.menu_Add.Size = New System.Drawing.Size(356, 48)
        '
        'menuItem_AddAtEnd
        '
        Me.menuItem_AddAtEnd.Checked = True
        Me.menuItem_AddAtEnd.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuItem_AddAtEnd.Name = "menuItem_AddAtEnd"
        Me.menuItem_AddAtEnd.Size = New System.Drawing.Size(355, 22)
        Me.menuItem_AddAtEnd.Text = "Add New Events to the End..."
        '
        'menuItem_InsertEvents
        '
        Me.menuItem_InsertEvents.CheckOnClick = True
        Me.menuItem_InsertEvents.Name = "menuItem_InsertEvents"
        Me.menuItem_InsertEvents.Size = New System.Drawing.Size(355, 22)
        Me.menuItem_InsertEvents.Text = "Insert New Events After Current Selection"
        '
        'menu_Save
        '
        Me.menu_Save.ImageScalingSize = New System.Drawing.Size(28, 28)
        Me.menu_Save.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItem_SaveSelection})
        Me.menu_Save.Name = "menu_Save"
        Me.menu_Save.ShowImageMargin = False
        Me.menu_Save.Size = New System.Drawing.Size(250, 26)
        '
        'menuItem_SaveSelection
        '
        Me.menuItem_SaveSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.menuItem_SaveSelection.Name = "menuItem_SaveSelection"
        Me.menuItem_SaveSelection.Size = New System.Drawing.Size(249, 22)
        Me.menuItem_SaveSelection.Text = "Save Current Selection as Looper file..."
        '
        'menu_load
        '
        Me.menu_load.ImageScalingSize = New System.Drawing.Size(28, 28)
        Me.menu_load.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItem_importLooperFile})
        Me.menu_load.Name = "menu_load"
        Me.menu_load.ShowImageMargin = False
        Me.menu_load.Size = New System.Drawing.Size(335, 26)
        '
        'menuItem_importLooperFile
        '
        Me.menuItem_importLooperFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.menuItem_importLooperFile.Name = "menuItem_importLooperFile"
        Me.menuItem_importLooperFile.Size = New System.Drawing.Size(334, 22)
        Me.menuItem_importLooperFile.Text = "Import Looper File into Current Playlist..."
        '
        'mainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(750, 520)
        Me.Controls.Add(Me.eventsList)
        Me.Controls.Add(Me.listViewEditor)
        Me.Controls.Add(Me.bottomPanel)
        Me.Controls.Add(Me.filterPanel)
        Me.Controls.Add(Me.controlsPanel)
        Me.Controls.Add(Me.speedSlider)
        Me.Controls.Add(Me.currentPositionBG)
        Me.Controls.Add(Me.currentRemainingStatus)
        Me.Controls.Add(Me.hotkeysTF)
        Me.Controls.Add(Me.loopsRepeatTF)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MinimumSize = New System.Drawing.Size(600, 400)
        Me.Name = "mainWindow"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Looper 2"
        Me.controlsPanel.ResumeLayout(False)
        Me.controlsPanel.PerformLayout()
        Me.filterPanel.ResumeLayout(False)
        Me.filterPanel.PerformLayout()
        Me.bottomPanel.ResumeLayout(False)
        Me.menu_Add.ResumeLayout(False)
        Me.menu_Save.ResumeLayout(False)
        Me.menu_load.ResumeLayout(False)
        CType(Me.speedSlider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents controlsPanel As Panel
    Friend WithEvents filterPanel As Panel
    Friend WithEvents bottomPanel As Panel
    Friend WithEvents loopModeButton As Button
    Friend WithEvents inPointSlipLeftButton As Button
    Friend WithEvents inTF As TextBox
    Friend WithEvents inPointButton As Button
    Friend WithEvents inPointSlipRightButton As Button
    Friend WithEvents outPointSlipLeftButton As Button
    Friend WithEvents outTF As TextBox
    Friend WithEvents outPointButton As Button
    Friend WithEvents outPointSlipRightButton As Button
    Friend WithEvents speedDownButton As Button
    Friend WithEvents speedValueLabel As Label
    Friend WithEvents speedUpButton As Button
    Friend WithEvents loadOptionsWindowButton As Button
    Friend WithEvents showPlayingFileInExplorerButton As Button
    Friend WithEvents alwaysOnTopButton As Button
    Friend WithEvents searchTF As TextBox
    Friend WithEvents eventCountLabel As Label
    Friend WithEvents prevEventButton As Button
    Friend WithEvents nextEventButton As Button
    Friend WithEvents addEventButton As Button
    Friend WithEvents modifyEventButton As Button
    Friend WithEvents deleteEventButton As Button
    Friend WithEvents saveButton As Button
    Friend WithEvents currentPositionLabel As Label
    Friend WithEvents fixButton As Button
    Friend WithEvents fixToolButton As Button
    Friend WithEvents eventsList As ListViewDoubleBuffered
    Friend WithEvents eventPosHeader As ColumnHeader
    Friend WithEvents eventNameHeader As ColumnHeader
    Friend WithEvents speedColumn As ColumnHeader
    Friend WithEvents loopsColumn As ColumnHeader
    Friend WithEvents eventINPointHeader As ColumnHeader
    Friend WithEvents eventOutPointHeader As ColumnHeader
    Friend WithEvents eventDurHeader As ColumnHeader
    Friend WithEvents eventFilenameHeader As ColumnHeader
    Friend WithEvents listViewEditor As TextBox
    Friend WithEvents speedSlider As TrackBar
    Friend WithEvents slipTimer As Timer
    Friend WithEvents toolTips As ToolTip
    Friend WithEvents currentPositionBG As Label
    Friend WithEvents currentRemainingStatus As Label
    Friend WithEvents hotkeysTF As Label
    Friend WithEvents loopsRepeatTF As Label
    Friend WithEvents menu_Add As ContextMenuStrip
    Friend WithEvents menuItem_AddAtEnd As ToolStripMenuItem
    Friend WithEvents menuItem_InsertEvents As ToolStripMenuItem
    Friend WithEvents menu_Save As ContextMenuStrip
    Friend WithEvents menuItem_SaveSelection As ToolStripMenuItem
    Friend WithEvents menu_load As ContextMenuStrip
    Friend WithEvents menuItem_importLooperFile As ToolStripMenuItem
End Class
