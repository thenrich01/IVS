﻿Imports System.IO
Imports System.Windows.Threading
'Imports IVS.AppLog

Public Class WinView

    'Private MyAppLog As New ApplicationLog(ApplicationLog.DateEncodingType.YYYYMMDD, True)
    Private WithEvents timerViewTime As DispatcherTimer = New DispatcherTimer()
    'Private intViewingTime As Integer
    Private intViewTimeCountDown As Integer
    Private strDocDisplayedSide As String
    Private intSwipeScanID As Integer
    'Private strImageLocation As String

    Public Sub New(ByVal SwipeScanID As String, ByVal SwipeScanType As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim imgDocToDisplay As New BitmapImage

        Try

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

            Select Case SwipeScanType

                Case "Check"

                    Me.Width = "1027"

                Case "Drivers License Or State ID"

                    Me.Width = "764"

            End Select

            intSwipeScanID = SwipeScanID
            'strImageLocation = ImageLocation

            imgDocToDisplay.BeginInit()


            If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then

                imgDocToDisplay.UriSource = New Uri(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg", UriKind.Absolute)
            Else

                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

            End If

            strDocDisplayedSide = "F"

            imgDocToDisplay.EndInit()

            Me.imgScannedDocument.Source = imgDocToDisplay

            'intViewingTime = ViewingTime
            intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

            timerViewTime.IsEnabled = True
            timerViewTime.Interval = TimeSpan.FromSeconds(1)
            timerViewTime.Start()

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

    Private Sub imgScannedDocument_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgScannedDocument.MouseDown

        If e.ChangedButton = MouseButton.Left And e.ClickCount = 2 Then

            Me.Close()

        End If

        If e.ChangedButton = MouseButton.Right Then
            Dim imgDocToDisplay As New BitmapImage

            Try
                imgDocToDisplay.BeginInit()

                Select Case strDocDisplayedSide

                    Case "F"

                        If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg") = True Then

                            imgDocToDisplay.UriSource = New Uri(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "b.jpg", UriKind.Absolute)
                        Else

                            imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

                        End If

                        strDocDisplayedSide = "B"

                    Case "B"

                        If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then

                            imgDocToDisplay.UriSource = New Uri(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg", UriKind.Absolute)
                        Else

                            imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

                        End If

                        strDocDisplayedSide = "F"

                End Select

                imgDocToDisplay.EndInit()

                Me.imgScannedDocument.Source = imgDocToDisplay

                intViewTimeCountDown = WinMain.objClientSettings.ViewingTime

            Catch ex As Exception
                WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
            End Try

        End If

    End Sub

    Private Sub WinView_KeyDown() Handles Me.KeyDown, Me.MouseDown
        intViewTimeCountDown = WinMain.objClientSettings.ViewingTime
    End Sub

    Private Sub timerViewTime_Tick(sender As Object, e As System.EventArgs) Handles timerViewTime.Tick

        Try

            If intViewTimeCountDown > 0 Then

                intViewTimeCountDown -= 1

            Else

                timerViewTime.Stop()
                Me.Close()

            End If

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class