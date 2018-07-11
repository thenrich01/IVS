Imports Test_IVSDecode.IVSDecodeService

Class MainWindow

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click

        Try
            Me.TextBox2.Visibility = Windows.Visibility.Hidden
            Me.dgLists.Visibility = Windows.Visibility.Visible

            Dim objIVSWebClient As Test_IVSDecode.IVSDecodeService.IVSDecodeClient = New IVSDecodeClient("SSL")
            Dim strRawData As String = Me.TextBox1.Text
            Dim objIVSLicense As New IVS.License.IVSLicense

            Dim objDecodedData As New DecodedData

            objDecodedData = objIVSWebClient.DecodeData(objIVSLicense.IVSGUID, strRawData)
            Me.dgLists.ItemsSource = GetListOfResults_SwipeScanDetail(objDecodedData)

        Catch ex As Exception
            Me.TextBox2.Visibility = Windows.Visibility.Visible
            Me.dgLists.Visibility = Windows.Visibility.Hidden
            Me.TextBox2.Text = ex.ToString
        End Try

    End Sub

    Private Function GetListOfResults_SwipeScanDetail(ByVal objDecodedData As DecodedData) As List(Of Results)

        Dim ListOfResults As New List(Of Results)
        Dim objResults As New Results

        objResults.ResultsItem = "IDAccountNumber"
        objResults.ResultsValue = objDecodedData.IDAccountNumber
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CardType"
        objResults.ResultsValue = objDecodedData.CardType
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "CCIssuer"
        objResults.ResultsValue = objDecodedData.CCIssuer
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameFirst"
        objResults.ResultsValue = objDecodedData.NameFirst
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameLast"
        objResults.ResultsValue = objDecodedData.NameLast
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "NameMiddle"
        objResults.ResultsValue = objDecodedData.NameMiddle
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfBirth"
        objResults.ResultsValue = objDecodedData.DateOfBirth
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Age"
        objResults.ResultsValue = objDecodedData.Age
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Sex"
        objResults.ResultsValue = objDecodedData.Sex
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Height"
        objResults.ResultsValue = objDecodedData.Height
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Weight"
        objResults.ResultsValue = objDecodedData.Weight
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Eyes"
        objResults.ResultsValue = objDecodedData.Eyes
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "Hair"
        objResults.ResultsValue = objDecodedData.Hair
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfIssue"
        objResults.ResultsValue = objDecodedData.DateOfIssue
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "DateOfExpiration"
        objResults.ResultsValue = objDecodedData.DateOfExpiration
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressStreet"
        objResults.ResultsValue = objDecodedData.AddressStreet
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressCity"
        objResults.ResultsValue = objDecodedData.AddressCity
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressState"
        objResults.ResultsValue = objDecodedData.AddressState
        ListOfResults.Add(objResults)
        objResults = New Results
        objResults.ResultsItem = "AddressZip"
        objResults.ResultsValue = objDecodedData.AddressZip
        ListOfResults.Add(objResults)

        Return ListOfResults

    End Function

End Class

Public Class Results

    Private _ResultsItem As String
    Private _ResultsValue As String

    Property ResultsItem() As String
        Get
            Return _ResultsItem
        End Get
        Set(ByVal Value As String)
            _ResultsItem = Value
        End Set
    End Property

    Property ResultsValue() As String
        Get
            Return _ResultsValue
        End Get
        Set(ByVal Value As String)
            _ResultsValue = Value
        End Set
    End Property

End Class