
Imports System.Data
Imports System.Data.SqlClient



Module modAppUtilities

    Public Const DB_CONNECTION As String = "Server=MTK2;Database=NWTrans;User ID=RptLogin;Password=xray13;Trusted_Connection=False"
    'Public Const DB_CONNECTION As String = "Server=MTBAILEY;Database=NWTrans;User ID=RptLogin;Password=xray13;Trusted_Connection=False"



    Public Function FileExists(ByVal Value As String) As Boolean
        Dim sResults As String
        sResults = Dir$(Value)
        FileExists = IIf(sResults = "", False, True)
    End Function

    Public Function PathExists(ByVal Value As String) As Boolean
        On Error Resume Next
        Dim sCurDir As String
        sCurDir = CurDir()
        ChDir(Value)
        PathExists = (Err.Number = 0)
        ChDir(sCurDir)
        Err.Clear()
    End Function

    Public Function FileName(ByVal rsFileName As String) As String
        On Error Resume Next

        Dim i As Integer
        For i = Len(rsFileName) To 1 Step -1
            If Mid(rsFileName, i, 1) = "\" Then
                Exit For
            End If
        Next
        FileName = Right(rsFileName, Len(rsFileName) - i)
        If Right(FileName, 1) = Chr(0) Then FileName = Left(FileName, Len(FileName) - 1)

    End Function

    Public Function FilePath(ByVal rsFileName As String) As String
        On Error Resume Next

        Dim i As Integer
        For i = Len(rsFileName) To 1 Step -1
            If Mid(rsFileName, i, 1) = "\" Then
                Exit For
            End If
        Next
        FilePath = Mid(rsFileName, 1, i)

    End Function


    Public Function TrimExt(ByVal rsFileName As String) As String
        On Error Resume Next

        Dim i As Integer
        For i = Len(rsFileName) To 1 Step -1
            If Mid(rsFileName, i, 1) = "." Then
                Exit For
            End If
        Next
        TrimExt = Left(rsFileName, i - 1)

    End Function

    Public Function ValidDate(ByVal Value As String) As Boolean

        ValidDate = False

        If (Value Like "#/#/##") Or _
                 (Value Like "#/#/####") Or _
                 (Value Like "#/##/##") Or _
                 (Value Like "#/##/####") Or _
                 (Value Like "##/#/##") Or _
                 (Value Like "##/#/####") Or _
                 (Value Like "##/##/##") Or _
                 (Value Like "##/##/####") Or _
                 (Value Like "#-#-##") Or _
                 (Value Like "#-#-####") Or _
                 (Value Like "#-##-##") Or _
                 (Value Like "#-##-####") Or _
                 (Value Like "##-#-##") Or _
                 (Value Like "##-#-####") Or _
                 (Value Like "##-##-##") Or _
                 (Value Like "##-##-####") Then
            If IsDate(Value) Then ValidDate = True
        End If

    End Function

    Public Function ValidEmail(ByVal Value As String) As Boolean
        Dim varNames As Object
        Dim varName As Object
        Dim intCounter As Integer
        Dim strCharacter As String

        ValidEmail = False
        varNames = Split(Value, "@")

        If UBound(varNames) <> 1 Then Exit Function

        For Each varName In varNames
            If Len(varName) <= 0 Then Exit Function
            For intCounter = 1 To Len(varName)
                strCharacter = LCase(Mid(varName, intCounter, 1))
                If InStr("abcdefghijklmnopqrstuvwxyz_-.", strCharacter) <= 0 And Not IsNumeric(strCharacter) Then Exit Function
            Next
            If Left(varName, 1) = "." Or Right(varName, 1) = "." Then Exit Function
        Next

        If InStr(varNames(1), ".") <= 0 Then Exit Function

        intCounter = Len(varNames(1)) - InStrRev(varNames(1), ".")
        If intCounter <> 2 And intCounter <> 3 Then Exit Function
        If InStr(Value, "..") > 0 Then Exit Function

        ValidEmail = True

    End Function

    Public Sub CustomDoEvents(ByVal NumberOfTimes As Long)
        Dim lngCounter As Long
        For lngCounter = 1 To NumberOfTimes
            Application.DoEvents()
        Next
    End Sub

    Public Sub SetClipboardValues(ByVal Credit As String, ByVal Debit As String)
        Dim strClipboardText As String
        ' Put values on Clipboard
        strClipboardText = "Amount of Credit Transaction is $" & Credit & "."
        If Val(Debit) < 0 Then
            strClipboardText = strClipboardText & "  Amount of Debit Transaction is $" & Debit & "."
        End If

        My.Computer.Clipboard.SetText(strClipboardText)
        Application.DoEvents()
        My.Computer.Clipboard.SetText(strClipboardText)

    End Sub

    Public Sub SetHourglass(ByRef CallingForm As Form, ByVal Value As Boolean)
        CallingForm.Cursor = IIf(Value, Cursors.WaitCursor, Cursors.Arrow)
    End Sub

End Module
