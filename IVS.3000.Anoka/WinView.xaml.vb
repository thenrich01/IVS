Imports System.IO

Public Class WinView

    Private intSwipeScanID As Integer

    Public Sub New(ByVal SwipeScanID As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim imgDocToDisplay As New BitmapImage

        Try

            Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            Me.Width = "764"

            intSwipeScanID = SwipeScanID

            imgDocToDisplay.BeginInit()


            If File.Exists(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg") = True Then

                imgDocToDisplay.UriSource = New Uri(WinMain.objClientSettings.ImageLocation & "\" & intSwipeScanID & "f.jpg", UriKind.Absolute)
            Else

                imgDocToDisplay.UriSource = New Uri("./Resources/NoImage.jpg", UriKind.Relative)

            End If

            imgDocToDisplay.EndInit()

            Me.imgScannedDocument.Source = imgDocToDisplay

        Catch ex As Exception
            WinMain.MyAppLog.WriteToLog("IVS", ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class