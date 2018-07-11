Public Class WinRawData

    Public Sub New(ByVal SwipeScanRawData As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
        Me.TextBox1.Text = SwipeScanRawData

    End Sub

End Class