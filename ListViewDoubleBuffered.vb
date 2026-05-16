Imports System.Drawing
Imports System.Windows.Forms

Public Class ListViewDoubleBuffered
    Inherits ListView

    Private ReadOnly headerBackColor As Color = Color.FromArgb(26, 26, 46)
    Private ReadOnly headerForeColor As Color = Color.FromArgb(200, 200, 200)
    Private ReadOnly headerFont As New Font("Segoe UI", 9.0!, FontStyle.Bold)
    Private ReadOnly gridLineColor As Color = Color.FromArgb(38, 38, 54)
    Private ReadOnly altRowColor As Color = Color.FromArgb(34, 34, 54)
    Private ReadOnly normalRowColor As Color = Color.FromArgb(30, 30, 48)

    Public Sub New()
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.OwnerDraw = True
    End Sub

    Protected Overrides Sub OnDrawColumnHeader(e As DrawListViewColumnHeaderEventArgs)
        Using bgBrush As New SolidBrush(headerBackColor)
            e.Graphics.FillRectangle(bgBrush, e.Bounds)
        End Using

        Using dividerPen As New Pen(Color.White, 1)
            e.Graphics.DrawLine(dividerPen, 0, e.Bounds.Bottom - 1, Me.ClientSize.Width, e.Bounds.Bottom - 1)
        End Using

        Using colDividerPen As New Pen(Color.White, 1)
            e.Graphics.DrawLine(colDividerPen, e.Bounds.Right - 1, e.Bounds.Top + 4, e.Bounds.Right - 1, e.Bounds.Bottom - 5)
        End Using

        Dim textBounds = New Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height)
        TextRenderer.DrawText(e.Graphics, e.Header.Text, headerFont, textBounds, headerForeColor, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
    End Sub

    Protected Overrides Sub OnDrawItem(e As DrawListViewItemEventArgs)
    End Sub

    Protected Overrides Sub OnDrawSubItem(e As DrawListViewSubItemEventArgs)
        Dim rowColor As Color
        If e.Item.Selected Then
            rowColor = Color.FromArgb(50, 60, 90)
        ElseIf e.Item.BackColor <> Me.BackColor AndAlso e.Item.BackColor <> normalRowColor AndAlso e.Item.BackColor <> altRowColor Then
            rowColor = e.Item.BackColor
        ElseIf e.ItemIndex Mod 2 = 1 Then
            rowColor = altRowColor
        Else
            rowColor = normalRowColor
        End If

        Using bgBrush As New SolidBrush(rowColor)
            e.Graphics.FillRectangle(bgBrush, e.Bounds)
        End Using

        Using borderPen As New Pen(gridLineColor)
            e.Graphics.DrawLine(borderPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right - 1, e.Bounds.Bottom - 1)
            e.Graphics.DrawLine(borderPen, e.Bounds.Right - 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom - 1)
        End Using

        Dim textColor As Color
        If e.Item.Selected Then
            textColor = Color.White
        ElseIf e.SubItem IsNot Nothing AndAlso e.SubItem.ForeColor <> Me.ForeColor Then
            textColor = e.SubItem.ForeColor
        Else
            textColor = e.Item.ForeColor
        End If

        Dim itemFont As Font
        If e.SubItem IsNot Nothing AndAlso e.SubItem.Font IsNot Nothing AndAlso Not e.SubItem.Font.Equals(Me.Font) Then
            itemFont = e.SubItem.Font
        ElseIf e.Item.Font IsNot Nothing Then
            itemFont = e.Item.Font
        Else
            itemFont = Me.Font
        End If
        Dim textBounds = New Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height)
        TextRenderer.DrawText(e.Graphics, e.SubItem.Text, itemFont, textBounds, textColor, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
    End Sub
End Class
