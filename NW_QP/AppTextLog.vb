Imports Microsoft.VisualBasic
Imports System.IO


Public Class AppTextLog


    Public Shared Function WriteLogEntry(ByVal LogDrive As String, ByVal LogTitle As String, ByVal LogText As String, ByVal IsError As Boolean, ByVal IsTest As Boolean) As Boolean
        Dim objSW As StreamWriter

        ' Clean Up the Drive Letter
        LogDrive = LogDrive.Trim.ToUpper.Replace("\", "")
        LogDrive = LogDrive & ":"
        LogDrive = Left(LogDrive, 2)

        Try

            If IsTest Then

                If IsError Then

                    Select Case LogDrive

                        Case "C:"

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "D:"

                            If File.Exists("D:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("D:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("D:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "E:"

                            If File.Exists("E:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("E:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("E:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "F:"

                            If File.Exists("F:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("F:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("F:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "G:"

                            If File.Exists("G:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("G:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("G:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "H:"

                            If File.Exists("H:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("H:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("H:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case Else

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                    End Select


                Else


                    Select Case LogDrive

                        Case "C:"

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "D:"

                            If File.Exists("D:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("D:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("D:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "E:"

                            If File.Exists("E:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("E:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("E:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "F:"

                            If File.Exists("F:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("F:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("F:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "G:"

                            If File.Exists("G:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("G:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("G:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "H:"

                            If File.Exists("H:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("H:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("H:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If

                        Case Else

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\TEST_IPSQUICKPAY_LOG.TXT")
                            End If


                    End Select

                End If

            Else

                If IsError Then

                    Select Case LogDrive

                        Case "C:"

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "D:"

                            If File.Exists("D:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("D:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("D:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "E:"

                            If File.Exists("E:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("E:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("E:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "F:"

                            If File.Exists("F:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("F:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("F:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "G:"

                            If File.Exists("G:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("G:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("G:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case "H:"

                            If File.Exists("H:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("H:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("H:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                        Case Else

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_ERROR_LOG.TXT")
                            End If


                    End Select


                Else


                    Select Case LogDrive

                        Case "C:"

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "D:"

                            If File.Exists("D:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("D:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("D:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "E:"

                            If File.Exists("E:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("E:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("E:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "F:"

                            If File.Exists("F:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("F:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("F:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "G:"

                            If File.Exists("G:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("G:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("G:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If


                        Case "H:"

                            If File.Exists("H:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("H:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("H:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If

                        Case Else

                            If File.Exists("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT") Then
                                objSW = File.AppendText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            Else
                                objSW = File.CreateText("C:\APP_LOGS\IPSQUICKPAY\LIVE_IPSQUICKPAY_LOG.TXT")
                            End If


                    End Select

                End If

            End If

            objSW.WriteLine(Format(DateTime.Now, "MM/dd/yy HH:mm:ss") & " : " & Left(LogTitle.PadRight(25), 25) & " : " & LogText.Trim)
            objSW.Close()

        Catch err As System.Exception

            Return False

        End Try

        Return True

    End Function

    'Public Shared Function Delete(ByVal LogName As String) As Boolean
    '    Dim strLogFile As String

    '    strLogFile = HttpContext.Current.Request.PhysicalApplicationPath & LogName

    '    Try

    '        If File.Exists(strLogFile) Then
    '            File.Delete(strLogFile)
    '        End If

    '    Catch err As System.Exception

    '        Return False

    '    End Try

    '    Return True

    'End Function

End Class

