Imports System.IO
Imports System.Text

Module LooperFixer

    Public LogPath As String = Path.Combine(Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location), "LooperFixTool.log")

    Public Structure FixResult
        Public TotalEvents As Integer
        Public BrokenEvents As Integer
        Public FixedEvents As Integer
        Public UnfixableEvents As Integer
        Public OutputPath As String
        Public Skipped As Boolean
    End Structure

    Private Sub FixerLog(msg As String)
        Try
            File.AppendAllText(LogPath, DateTime.Now.ToString("HH:mm:ss") & " | " & msg & vbCrLf)
        Catch
        End Try
    End Sub

    Public Function FixLooperFile(inputPath As String, replaceOriginal As Boolean) As FixResult
        Dim result As New FixResult With {
            .TotalEvents = 0,
            .BrokenEvents = 0,
            .FixedEvents = 0,
            .UnfixableEvents = 0,
            .Skipped = False
        }

FixerLog("--- Processing: " & inputPath)

        Dim encoding As Encoding = DetectEncoding(inputPath)
        Dim lines() As String = File.ReadAllLines(inputPath, encoding)

        Dim fixedLines As New List(Of String)
        Dim pathCache As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim anyChanged As Boolean = False

        For lineIdx = 0 To lines.Length - 1
            Dim line = lines(lineIdx)

            If String.IsNullOrWhiteSpace(line) Then
                fixedLines.Add(line)
                Continue For
            End If

            result.TotalEvents += 1
            Dim parts = SplitLooperLine(line)

            If parts Is Nothing OrElse parts.Length < 4 Then
                fixedLines.Add(line)
                Continue For
            End If

            Dim filePath = parts(parts.Length - 1).Trim()

            If File.Exists(filePath) Then
                fixedLines.Add(line)
                Continue For
            End If

            result.BrokenEvents += 1
            Dim filename = Path.GetFileName(filePath)

            ' Check cache first
            If pathCache.ContainsKey(filename) Then
                Dim cachedPath = pathCache(filename)
                If Not String.Equals(cachedPath, filePath, StringComparison.OrdinalIgnoreCase) Then
                    fixedLines.Add(ReplacePathInLine(line, cachedPath))
                    result.FixedEvents += 1
                    anyChanged = True
                Else
                    fixedLines.Add(line)
                    result.UnfixableEvents += 1
                End If
                Continue For
            End If

            ' Search via Everything
            Dim searchResults = SearchFile(filename)

            If searchResults.Length = 0 Then
                fixedLines.Add(line)
                result.UnfixableEvents += 1
                Continue For
            End If

            Dim bestMatch = PickBestMatch(searchResults, filePath, filename)
            If bestMatch IsNot Nothing AndAlso Not String.Equals(bestMatch, filePath, StringComparison.OrdinalIgnoreCase) Then
                pathCache(filename) = bestMatch
                fixedLines.Add(ReplacePathInLine(line, bestMatch))
                result.FixedEvents += 1
                anyChanged = True
FixerLog("    FIXED -> " & bestMatch)
            Else
                pathCache(filename) = filePath
                fixedLines.Add(line)
                result.UnfixableEvents += 1
            End If
        Next

        ' Only write output if something actually changed
        If Not anyChanged Then
            result.Skipped = True
FixerLog("  No changes needed, skipping output file")
            Return result
        End If

        If replaceOriginal Then
            result.OutputPath = inputPath
            Dim originalDate = File.GetLastWriteTime(inputPath)
            File.WriteAllLines(inputPath, fixedLines.ToArray(), encoding)
            File.SetLastWriteTime(inputPath, originalDate)
FixerLog("  Replaced original (kept date " & originalDate.ToString("yyyy-MM-dd HH:mm:ss") & "): " & inputPath)
        Else
            Dim dir = Path.GetDirectoryName(inputPath)
            Dim nameNoExt = Path.GetFileNameWithoutExtension(inputPath)
            result.OutputPath = Path.Combine(dir, nameNoExt & "-fixed.looper")
            File.WriteAllLines(result.OutputPath, fixedLines.ToArray(), encoding)
FixerLog("  Saved: " & result.OutputPath)
        End If

        Return result
    End Function

    Private Function DetectEncoding(filePath As String) As Encoding
        Dim bom(3) As Byte
        Using fs = File.OpenRead(filePath)
            fs.Read(bom, 0, 4)
        End Using
        If bom(0) = &HFF AndAlso bom(1) = &HFE Then Return Encoding.Unicode
        If bom(0) = &HFE AndAlso bom(1) = &HFF Then Return Encoding.BigEndianUnicode
        If bom(0) = &HEF AndAlso bom(1) = &HBB AndAlso bom(2) = &HBF Then Return Encoding.UTF8
        Return Encoding.UTF8
    End Function

    Private Function SplitLooperLine(line As String) As String()
        Dim cleaned = line
        While cleaned.StartsWith("<") AndAlso cleaned.Contains(">")
            Dim closeIdx = cleaned.IndexOf(">")
            cleaned = cleaned.Substring(closeIdx + 1)
        End While
        Return cleaned.Split("|"c)
    End Function

    Private Function ReplacePathInLine(line As String, newPath As String) As String
        ' Replace everything after the last pipe with the new path
        Dim lastPipe = line.LastIndexOf("|"c)
        If lastPipe < 0 Then Return line
        Return line.Substring(0, lastPipe + 1) & newPath
    End Function

    Private Function PickBestMatch(results() As String, originalPath As String, filename As String) As String
        Dim exactMatches As New List(Of String)
        For Each r In results
            If String.Equals(Path.GetFileName(r), filename, StringComparison.OrdinalIgnoreCase) Then
                ' Only accept if the file actually exists on disk (Everything index may be stale)
                If File.Exists(r) Then
                    exactMatches.Add(r)
                End If
            End If
        Next

        If exactMatches.Count = 0 Then Return Nothing
        If exactMatches.Count = 1 Then Return exactMatches(0)

        ' Multiple matches — prefer one with similar parent folder structure
        Dim origParts = originalPath.Split("\"c, "/"c)
        Dim bestScore = -1
        Dim best As String = Nothing
        For Each candidate In exactMatches
            Dim candParts = candidate.Split("\"c, "/"c)
            Dim score = 0
            Dim origIdx = origParts.Length - 2
            Dim candIdx = candParts.Length - 2
            While origIdx >= 0 AndAlso candIdx >= 0
                If String.Equals(origParts(origIdx), candParts(candIdx), StringComparison.OrdinalIgnoreCase) Then
                    score += 1
                End If
                origIdx -= 1
                candIdx -= 1
            End While
            If score > bestScore Then
                bestScore = score
                best = candidate
            End If
        Next
        Return best
    End Function

End Module
