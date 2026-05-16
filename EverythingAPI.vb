Imports System.Runtime.InteropServices
Imports System.Text

Module EverythingAPI

    Private Const EVERYTHING_OK As UInt32 = 0
    Private Const EVERYTHING_ERROR_MEMORY As UInt32 = 1
    Private Const EVERYTHING_ERROR_IPC As UInt32 = 2
    Private Const EVERYTHING_ERROR_REGISTERCLASSEX As UInt32 = 3
    Private Const EVERYTHING_ERROR_CREATEWINDOW As UInt32 = 4
    Private Const EVERYTHING_ERROR_CREATETHREAD As UInt32 = 5
    Private Const EVERYTHING_ERROR_INVALIDINDEX As UInt32 = 6
    Private Const EVERYTHING_ERROR_INVALIDCALL As UInt32 = 7

    Private Const EVERYTHING_REQUEST_FILE_NAME As UInt32 = &H1UI
    Private Const EVERYTHING_REQUEST_PATH As UInt32 = &H2UI

    <DllImport("Everything.dll", CharSet:=CharSet.Unicode)>
    Private Function Everything_SetSearchW(search As String) As UInt32
    End Function

    <DllImport("Everything.dll")>
    Private Function Everything_SetRequestFlags(dwRequestFlags As UInt32) As UInt32
    End Function

    <DllImport("Everything.dll")>
    Private Function Everything_QueryW(bWait As Integer) As Integer
    End Function

    <DllImport("Everything.dll")>
    Private Function Everything_GetNumResults() As UInt32
    End Function

    <DllImport("Everything.dll", CharSet:=CharSet.Unicode)>
    Private Function Everything_GetResultFullPathNameW(index As UInt32, buf As StringBuilder, size As UInt32) As UInt32
    End Function

    <DllImport("Everything.dll")>
    Private Function Everything_GetLastError() As UInt32
    End Function

    <DllImport("Everything.dll")>
    Private Sub Everything_SetMatchWholeWord(bEnable As Integer)
    End Sub

    <DllImport("Everything.dll")>
    Private Sub Everything_SetMatchPath(bEnable As Integer)
    End Sub

    <DllImport("Everything.dll")>
    Private Sub Everything_SetMax(max As UInt32)
    End Sub

    Public Function SearchFile(filename As String) As String()
        Everything_SetMatchWholeWord(0)
        Everything_SetMatchPath(0)
        Everything_SetMax(10UI)
        Everything_SetRequestFlags(EVERYTHING_REQUEST_FILE_NAME Or EVERYTHING_REQUEST_PATH)
        Everything_SetSearchW("""" & filename & """")

        Dim queryResult = Everything_QueryW(1)

        If queryResult = 0 Then
            Dim err = Everything_GetLastError()
            Select Case err
                Case EVERYTHING_ERROR_IPC
                    Console.WriteLine("  ERROR: Everything is not running. Please start Everything first.")
                Case Else
                    Console.WriteLine("  ERROR: Everything query failed (code " & err & ")")
            End Select
            Return New String() {}
        End If

        Dim count = Everything_GetNumResults()
        If count = 0UI Then Return New String() {}

        Dim results(CInt(count) - 1) As String
        Dim buf As New StringBuilder(260)
        For i As UInt32 = 0 To count - 1UI
            buf.Clear()
            Everything_GetResultFullPathNameW(i, buf, CUInt(buf.Capacity))
            results(CInt(i)) = buf.ToString()
        Next
        Return results
    End Function

    Public Function IsEverythingAvailable() As Boolean
        Try
            Everything_SetSearchW("__looperfixtest__")
            Everything_QueryW(1)
            Dim err = Everything_GetLastError()
            Return err = EVERYTHING_OK
        Catch ex As DllNotFoundException
            Return False
        Catch ex As BadImageFormatException
            Return False
        End Try
    End Function

    Public Function GetErrorMessage() As String
        Dim err = Everything_GetLastError()
        Select Case err
            Case EVERYTHING_OK : Return "OK"
            Case EVERYTHING_ERROR_IPC : Return "Everything is not running"
            Case EVERYTHING_ERROR_MEMORY : Return "Out of memory"
            Case Else : Return "Error code " & err
        End Select
    End Function

End Module
