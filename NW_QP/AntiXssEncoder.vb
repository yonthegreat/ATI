Imports System
Imports System.IO
Imports System.Web
Imports Microsoft.Security.Application


Public Class AntiXssEncoder
    Inherits HttpEncoder

    Public AntiXssEncoder()

    Protected Overrides Sub HtmlEncode(value As String, output As TextWriter)
        output.Write(AntiXss.HtmlEncode(value))
    End Sub

    Protected Overrides Sub HtmlAttributeEncode(value As String, output As TextWriter)
        output.Write(AntiXss.HtmlAttributeEncode(value))
    End Sub

    Protected Overrides Sub HtmlDecode(value As String, output As TextWriter)
        MyBase.HtmlDecode(value, output)
    End Sub

    Protected Overrides Function UrlEncode(bytes As Byte(), offset As Integer, count As Integer) As Byte()
        Dim cSpaces As Integer = 0
        Dim cUnsafe As Integer = 0

        For i As Integer = 0 To count - 1
            Dim ch As Char = Chr(bytes(offset + i))
            If ch = " " Then
                cSpaces = cSpaces + 1
            ElseIf Not IsUrlSafeChar(ch) Then
                cUnsafe = cUnsafe + 1
            End If
        Next
        If cSpaces = 0 And cUnsafe = 0 Then
            Return bytes
        End If
        Dim expandedBytes(count + cUnsafe * 2) As Byte
        Dim pos As Integer

        For i As Integer = 0 To count - 1
            Dim b As Byte = bytes(offset + 1)
            Dim ch = Chr(b)
            If IsUrlSafeChar(ch) Then
                expandedBytes(pos) = b
                pos = pos + 1
            ElseIf ch = " " Then
                expandedBytes(pos) = Convert.ToByte("+")
                pos = pos + 1
            Else
                expandedBytes(pos) = Convert.ToByte("%")
                pos = pos + 1
                expandedBytes(pos) = Convert.ToByte(IntToHex((b >> 4) And &HF))
                pos = pos + 1
                expandedBytes(pos) = Convert.ToByte(IntToHex(b And &HF))
                pos = pos + 1
            End If
        Next
        Return expandedBytes
    End Function

    Protected Overrides Function UrlPathEncode(value As String) As String
        Dim parts() As String = value.Split("?".ToCharArray())
        Dim originalPath As String = parts(0)
        Dim originalQueryString As String = Nothing

        If parts.Length = 2 Then
            originalQueryString = "?" + parts(1)
        End If

        Dim pathSegments() As String = originalPath.Split("/".ToCharArray)
        For i As Integer = 0 To pathSegments.Length - 1
            pathSegments(i) = AntiXss.UrlEncode(pathSegments(i))
        Next
        Return String.Join("/", pathSegments) + originalQueryString
    End Function

    Private Function IsUrlSafeChar(ch As Char) As Boolean
        If ch >= Convert.ToChar("a") And ch <= Convert.ToChar("z") Or ch >= Convert.ToChar("A") And ch <= Convert.ToChar("Z") Or ch >= Convert.ToChar("0") And ch <= Convert.ToChar("9") Then
            Return True
        End If
        Select Case ch
            Case Convert.ToChar("-")
            Case Convert.ToChar("_")
            Case Convert.ToChar(".")
                Return True
        End Select
        Return False
    End Function

    Private Function IntToHex(n As Integer) As Char
        If n <= 9 Then
            Return Convert.ToChar(n + Convert.ToInt32("0", 10))
        Else
            Return Convert.ToChar(n - 10 + Convert.ToInt32("a"))
        End If
    End Function
End Class
