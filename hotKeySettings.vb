Public Class hotKeySettings
    Private Sub HotKeySettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        hotKeysLV.Items.Clear() ' delete all items in the hotkey list view (if they exist from a previous session) to get things started

        ' set the window position to just right of the options window
        Me.Left = optionsWindow.Right
        Me.Top = optionsWindow.Top

        mainWindow.clearHotKeys() ' turn hotkeys off (this forces them to reload when the Options window closes)

        For currentItem = 0 To UBound(mainWindow.hotKeyList)
            Dim itemText As String = mainWindow.hotKeyList(currentItem, 2)

            If hotKeyDisabled(currentItem) Then
                itemText = "[DISABLED] " & itemText
            End If

            hotKeysLV.Items.Add(New ListViewItem(itemText))

            If hotKeyDisabled(currentItem) Then
                hotKeysLV.Items(currentItem).ForeColor = Color.Gray
                hotKeysLV.Items(currentItem).BackColor = Color.FromArgb(240, 240, 240)
            ElseIf Not mainWindow.hotKeyList(currentItem, 1) = Nothing Then
                hotKeysLV.Items(currentItem).Font = mainWindow.boldFont
                hotKeysLV.Items(currentItem).BackColor = Color.LightBlue
            End If
        Next
    End Sub

    Private Sub hotKeysLV_SelectedIndexChanged(sender As Object, e As EventArgs) Handles hotKeysLV.SelectedIndexChanged
        If hotKeysLV.SelectedIndices.Count >= 1 Then
            Dim hkIDX As Integer = hotKeysLV.SelectedIndices(0) ' get the first selected item

            If hotKeysLV.SelectedIndices.Count = 1 Then
                ' Single selection — show details
                If mainWindow.hotKeyList(hkIDX, 1) = Nothing Then
                    currentHKLabel.Text = "CURRENT HOTKEY"
                    currentHKTF.BackColor = Color.Gainsboro
                    currentHKTF.Text = mainWindow.hotKeyList(hkIDX, 0)
                    currentHKDesc.Text = convertKeyCodeToString(mainWindow.hotKeyList(hkIDX, 0))
                    setDefaultHKButton.Enabled = False
                Else
                    currentHKLabel.Text = "CURRENT CUSTOM HOTKEY"
                    currentHKTF.BackColor = Color.LightBlue
                    currentHKTF.Text = mainWindow.hotKeyList(hkIDX, 1)
                    currentHKDesc.Text = convertKeyCodeToString(mainWindow.hotKeyList(hkIDX, 1))
                    setDefaultHKButton.Enabled = True
                End If

                setNewHKButton.Enabled = False
                newHKTF.Text = Nothing
                newHKDesc.Text = Nothing
            Else
                ' Multiple selection — hide single-item details
                currentHKLabel.Text = hotKeysLV.SelectedIndices.Count & " HOTKEYS SELECTED"
                currentHKTF.BackColor = Color.Gainsboro
                currentHKTF.Text = Nothing
                currentHKDesc.Text = Nothing
                setDefaultHKButton.Enabled = False
                setNewHKButton.Enabled = False
                newHKTF.Text = Nothing
                newHKDesc.Text = Nothing
            End If

            ' Update the disable/enable button — check if any selected are enabled
            Dim anyEnabled As Boolean = False
            For i As Integer = 0 To hotKeysLV.SelectedIndices.Count - 1
                If Not hotKeyDisabled(hotKeysLV.SelectedIndices(i)) Then anyEnabled = True
            Next

            If anyEnabled Then
                disableHKButton.Text = "DISABLE"
            Else
                disableHKButton.Text = "ENABLE"
            End If
            disableHKButton.Enabled = True
        Else
            disableHKButton.Enabled = False
        End If
    End Sub

    Private Function convertKeyCodeToString(theKeyCode As String) As String
        Dim outputString As String = Nothing
        Dim currentSettings(2) As String

        currentSettings = Split(theKeyCode, "|")

        If currentSettings(0).Contains("C") Then outputString = "Ctrl+"
        If currentSettings(0).Contains("A") Then outputString += "Alt+"
        If currentSettings(0).Contains("S") Then outputString += "Shift+"

        Dim hkKey As Integer = CType(currentSettings(1), Integer)
        Dim keyName As String = [Enum].GetName(GetType(System.Windows.Forms.Keys), CType(hkKey, System.Windows.Forms.Keys))

        If keyName = "Prior" Then
            keyName = "PageUp"
        ElseIf keyName = "Oem4" Then
            keyName = "["
        ElseIf keyName = "Oem6" Then
            keyName = "]"
        ElseIf keyName = "OemSemicolon" Then
            keyName = "Semicolon"
        ElseIf keyName = "OemQuotes" Then
            keyName = "Quotes"
        End If

        Return outputString & keyName
    End Function

    Private Sub newHKTF_KeyDown(sender As Object, e As KeyEventArgs) Handles newHKTF.KeyDown
        If Not (e.KeyCode = 16 Or e.KeyCode = 17 Or e.KeyCode = 18) Then
            Dim newCode As String = Nothing

            If e.Control Then newCode += "C"
            If e.Alt Then newCode += "A"
            If e.Shift Then newCode += "S"

            newCode += "|" & Int(e.KeyCode).ToString()

            newHKTF.Text = newCode
            newHKDesc.Text = convertKeyCodeToString(newCode)

            setNewHKButton.Enabled = True
        Else
            newHKTF.Text = Nothing
            newHKDesc.Text = Nothing
        End If

        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Private Sub setDefaultHKButton_Click(sender As Object, e As EventArgs) Handles setDefaultHKButton.Click
        If hotKeysLV.SelectedIndices.Count = 1 Then
            Dim hkIDX As Integer = hotKeysLV.SelectedIndices(0)

            ' clear formatting for this hotkey
            hotKeysLV.Items(hkIDX).Font = mainWindow.normalFont
            hotKeysLV.Items(hkIDX).BackColor = Color.White

            mainWindow.hotKeyList(hkIDX, 1) = Nothing ' reset the custom hotkey to nothing

            ' Update the text fields
            currentHKLabel.Text = "CURRENT HOTKEY"
            currentHKTF.BackColor = Color.Gainsboro
            currentHKTF.Text = mainWindow.hotKeyList(hkIDX, 0)
            currentHKDesc.Text = convertKeyCodeToString(mainWindow.hotKeyList(hkIDX, 0))

            newHKTF.Text = Nothing
            newHKDesc.Text = Nothing

            setDefaultHKButton.Enabled = False
            setNewHKButton.Enabled = False
        End If
    End Sub

    Private Sub setNewHKButton_Click(sender As Object, e As EventArgs) Handles setNewHKButton.Click
        If hotKeysLV.SelectedIndices.Count = 1 Then
            Dim hkIDX As Integer = hotKeysLV.SelectedIndices(0)

            ' show custom formatting for this hotkey
            hotKeysLV.Items(hkIDX).Font = mainWindow.boldFont
            hotKeysLV.Items(hkIDX).BackColor = Color.LightBlue

            mainWindow.hotKeyList(hkIDX, 1) = newHKTF.Text ' set the custom hotkey to a new combination

            ' Update the text fields
            currentHKLabel.Text = "CURRENT CUSTOM HOTKEY"
            currentHKTF.BackColor = Color.LightBlue
            currentHKTF.Text = mainWindow.hotKeyList(hkIDX, 1)
            currentHKDesc.Text = convertKeyCodeToString(mainWindow.hotKeyList(hkIDX, 1))

            newHKTF.Text = Nothing
            newHKDesc.Text = Nothing

            setDefaultHKButton.Enabled = True ' if we set a new hotkey, allow resetting immediately
            setNewHKButton.Enabled = False
        End If
    End Sub

    Private Sub disableHKButton_Click(sender As Object, e As EventArgs) Handles disableHKButton.Click
        If hotKeysLV.SelectedIndices.Count >= 1 Then
            ' Determine action: if button says DISABLE, disable all selected; if ENABLE, enable all selected
            Dim disabling As Boolean = (disableHKButton.Text = "DISABLE")

            hotKeysLV.BeginUpdate()
            For i As Integer = 0 To hotKeysLV.SelectedIndices.Count - 1
                Dim hkIDX As Integer = hotKeysLV.SelectedIndices(i)
                hotKeyDisabled(hkIDX) = disabling

                If disabling Then
                    hotKeysLV.Items(hkIDX).Text = "[DISABLED] " & mainWindow.hotKeyList(hkIDX, 2)
                    hotKeysLV.Items(hkIDX).ForeColor = Color.Gray
                    hotKeysLV.Items(hkIDX).BackColor = Color.FromArgb(240, 240, 240)
                Else
                    hotKeysLV.Items(hkIDX).Text = mainWindow.hotKeyList(hkIDX, 2)
                    hotKeysLV.Items(hkIDX).ForeColor = Color.Black

                    If Not mainWindow.hotKeyList(hkIDX, 1) = Nothing Then
                        hotKeysLV.Items(hkIDX).Font = mainWindow.boldFont
                        hotKeysLV.Items(hkIDX).BackColor = Color.LightBlue
                    Else
                        hotKeysLV.Items(hkIDX).Font = mainWindow.normalFont
                        hotKeysLV.Items(hkIDX).BackColor = Color.White
                    End If
                End If
            Next
            hotKeysLV.EndUpdate()

            ' Toggle button text
            If disabling Then
                disableHKButton.Text = "ENABLE"
            Else
                disableHKButton.Text = "DISABLE"
            End If
        End If
    End Sub

    Private Sub resetAllButton_Click(sender As Object, e As EventArgs) Handles resetAllButton.Click
        Dim returnValue = MessageBox.Show(Me, "Are you sure you want to set all hotkeys back to their default values?",
                                          "Reset Hotkeys to Defaults?",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question,
                                          MessageBoxDefaultButton.Button1)

        If returnValue = DialogResult.Yes Then
            For currentItem = 0 To UBound(mainWindow.hotKeyList)
                mainWindow.hotKeyList(currentItem, 1) = Nothing ' clear any special hotkey from the current index
                hotKeyDisabled(currentItem) = False ' re-enable all hotkeys

                ' clear formatting for ALL of the items in the list
                hotKeysLV.Items(currentItem).Text = mainWindow.hotKeyList(currentItem, 2)
                hotKeysLV.Items(currentItem).Font = mainWindow.normalFont
                hotKeysLV.Items(currentItem).ForeColor = Color.Black
                hotKeysLV.Items(currentItem).BackColor = Color.White
            Next

            ' reset all the controls on the page to their default states
            currentHKLabel.Text = "CURRENT HOTKEY"
            currentHKTF.BackColor = Color.Gainsboro
            currentHKTF.Text = Nothing
            currentHKDesc.Text = Nothing

            newHKTF.Text = Nothing
            newHKDesc.Text = Nothing

            setDefaultHKButton.Enabled = False
            setNewHKButton.Enabled = False
        End If
    End Sub

    Private Sub OKButton_Click(sender As Object, e As EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub
End Class