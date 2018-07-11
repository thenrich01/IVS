Imports IVS.Data.Decode
Imports IDecode
Imports System.Text
Imports System.ServiceModel.Channels

<ServiceBehavior(InstanceContextMode:=InstanceContextMode.[Single])> _
Public Class IVSDecodeService
    Implements IIVSDecode

    Public objMyIDChecker As IDecode.Net.PC.Checker
    Private MyAppLog As New IVS.AppLog.ApplicationLog(IVS.AppLog.ApplicationLog.DateEncodingType.YYYYMMDD, True, "IVS.WS.Decode", DateFormat.GeneralDate)

    Public Sub New()

        MyAppLog.WriteToLog("IVSService.New()")

        Dim objWebServiceSettings As WebServiceSettings = New WebServiceSettings
        Dim intIDecodeLicenseStatus As Integer
        Dim intIDecodeRegisterStatus As Integer
        Dim intIDecodeResponse As Integer

        Try
            objWebServiceSettings = DataAccess.GetWebServiceSettings

            objMyIDChecker = New IDecode.Net.PC.Checker
            intIDecodeLicenseStatus = objMyIDChecker.LicenseStatus

            MyAppLog.WriteToLog("IVSDecodeService.New() IDecode IDecodeLicense: " & objWebServiceSettings.IDecodeLicense)
            MyAppLog.WriteToLog("IVSDecodeService.New() IDecode LicenseStatus: " & intIDecodeLicenseStatus)
            MyAppLog.WriteToLog("IVSDecodeService.New() IDecode Version: " & objMyIDChecker.Version)
            MyAppLog.WriteToLog("IVSDecodeService.New() IDecode TrialDaysRemaining: " & objMyIDChecker.TrialDaysRemaining)

            DataAccess.UpdateWSIDecode(objMyIDChecker.Version)

            Select Case intIDecodeLicenseStatus

                Case 0
                    MyAppLog.WriteToLog("IVSDecodeService.New() License status OK")
                Case 1
                    MyAppLog.WriteToLog("IVSDecodeService.New() Trial version within evaluation period")
                Case 2
                    MyAppLog.WriteToLog("IVSDecodeService.New() Trial version beyond evaluation")
                Case 3
                    MyAppLog.WriteToLog("IVSDecodeService.New() Unregistered copy")
                    intIDecodeRegisterStatus = objMyIDChecker.RegisterIDecode(objWebServiceSettings.IDecodeLicense)
                    MyAppLog.WriteToLog("IVSDecodeService.New() RegisterIDecode:" & intIDecodeRegisterStatus)
                Case Else
                    MyAppLog.WriteToLog("IVSDecodeService.New() Error retrieving license status")
            End Select

            MyAppLog.WriteToLog("IVS.Application_Startup() objMyIDChecker.TrackFormat()" & objWebServiceSettings.IDecodeTrackFormat)
            MyAppLog.WriteToLog("IVS.Application_Startup() objMyIDChecker.CardTypes()" & objWebServiceSettings.IDecodeCardTypes)

            intIDecodeResponse = objMyIDChecker.Setup(objWebServiceSettings.IDecodeTrackFormat, objWebServiceSettings.IDecodeCardTypes)
            MyAppLog.WriteToLog("IVS.Application_Startup() objMyIDChecker.Setup()" & intIDecodeResponse)

        Catch ex As Exception
            MyAppLog.WriteToLog(ex)
        End Try

    End Sub

    Public Function DecodeData(ByVal LicenseGuid As Guid, ByVal RawData As String) As DecodedData Implements IIVSDecode.DecodeData

        Dim objThisContext As OperationContext
        Dim objThisMessageProperties As MessageProperties
        Dim objThisEndPointProperty As RemoteEndpointMessageProperty
        Dim objDecodedData As New DecodedData
        Dim intDataToProcessStatus As Integer
        Dim intCardType As Integer
        Dim strIssueDate As String
        Dim strExpirationDate As String
        Dim strBirthDate As String
        Dim intAge As Integer = 0
        Dim sbRawData As New StringBuilder

        Try
            objThisContext = OperationContext.Current
            objThisMessageProperties = objThisContext.IncomingMessageProperties
            objThisEndPointProperty = objThisMessageProperties(RemoteEndpointMessageProperty.Name)

            If DataAccess.IsIVSLicenseValid(LicenseGuid, objThisEndPointProperty.Address) Then

                DataAccess.WebServiceActivity(objThisEndPointProperty.Address, "DecodeData", RawData)

                intDataToProcessStatus = objMyIDChecker.ProcessData(RawData)
                MyAppLog.WriteToLog("IVSDecodeService.DecodeData() ProcessData: " & intDataToProcessStatus)
                MyAppLog.WriteToLog("IVSDecodeService.DecodeData() ProcessData: " & intDataToProcessStatus)
                'ProcessData Status: 0=returned,1=NULL, Negative=ERROR

                intCardType = objMyIDChecker.GetCardType

                strIssueDate = objMyIDChecker.AllData.IssueDate.Text

                If Len(strIssueDate) = 8 Then
                    strIssueDate = strIssueDate.Substring(0, 2) & "/" & strIssueDate.Substring(2, 2) & "/" & strIssueDate.Substring(4, 4)
                End If

                strExpirationDate = objMyIDChecker.AllData.ExpiryDate.Text

                If Len(strExpirationDate) = 8 Then
                    strExpirationDate = strExpirationDate.Substring(0, 2) & "/" & strExpirationDate.Substring(2, 2) & "/" & strExpirationDate.Substring(4, 4)
                End If

                strBirthDate = objMyIDChecker.AllData.BirthDate.Text

                If Len(strBirthDate) = 8 Then
                    strBirthDate = strBirthDate.Substring(0, 2) & "/" & strBirthDate.Substring(2, 2) & "/" & strBirthDate.Substring(4, 4)
                    intAge = Math.Floor(DateDiff(DateInterval.Day, CDate(strBirthDate), Today) / 365.25)
                End If

                Select Case intCardType

                    Case 0

                        objDecodedData.IDAccountNumber = objMyIDChecker.AllData.Id.Text
                        objDecodedData.NameFirst = objMyIDChecker.AllData.Name.First.Text
                        objDecodedData.NameLast = objMyIDChecker.AllData.Name.Last.Text
                        objDecodedData.NameMiddle = objMyIDChecker.AllData.Name.Middle.Text
                        objDecodedData.DateOfBirth = strBirthDate
                        objDecodedData.Age = objMyIDChecker.AllData.Age
                        objDecodedData.Sex = objMyIDChecker.AllData.Sex.Text
                        objDecodedData.Height = objMyIDChecker.AllData.Height.FeetInches
                        objDecodedData.Weight = objMyIDChecker.AllData.Weight.Pounds
                        objDecodedData.Eyes = objMyIDChecker.AllData.Eye.Color
                        objDecodedData.Hair = objMyIDChecker.AllData.Hair.Color
                        objDecodedData.DateOfIssue = strIssueDate
                        objDecodedData.DateOfExpiration = strExpirationDate
                        objDecodedData.AddressStreet = objMyIDChecker.AllData.MailAddress.Line1
                        objDecodedData.AddressCity = objMyIDChecker.AllData.MailAddress.City.Text
                        objDecodedData.AddressState = objMyIDChecker.AllData.MailAddress.State.Text
                        objDecodedData.AddressZip = objMyIDChecker.AllData.MailAddress.Zip.Text
                        objDecodedData.CardType = "Drivers License Or State ID"

                        MyAppLog.WriteToLog("IVSDecodeService.DecodeData() CardType: Drivers License Or State ID")

                    Case 1

                        objDecodedData.IDAccountNumber = objMyIDChecker.CreditCardItems.Account.Text
                        objDecodedData.NameFirst = objMyIDChecker.CreditCardItems.Name.First
                        objDecodedData.NameLast = objMyIDChecker.CreditCardItems.Name.Last
                        objDecodedData.NameMiddle = objMyIDChecker.CreditCardItems.Name.Middle
                        objDecodedData.DateOfExpiration = strExpirationDate
                        objDecodedData.CardType = "Credit Card"
                        objDecodedData.CCIssuer = objMyIDChecker.CreditCardItems.Issuer.Text

                        MyAppLog.WriteToLog("IVSDecodeService.DecodeData() CardType: Credit Card")

                    Case 2

                        objDecodedData.IDAccountNumber = objMyIDChecker.MilitaryCardItems.CardInfo.FormNumber.Text
                        objDecodedData.NameFirst = objMyIDChecker.MilitaryCardItems.PersonalInfo.Name.First
                        objDecodedData.NameLast = objMyIDChecker.MilitaryCardItems.PersonalInfo.Name.Last
                        objDecodedData.NameMiddle = objMyIDChecker.MilitaryCardItems.PersonalInfo.Name.Middle
                        objDecodedData.DateOfBirth = strBirthDate
                        objDecodedData.Age = intAge
                        objDecodedData.Height = objMyIDChecker.MilitaryCardItems.PersonalInfo.Height.FeetInches
                        objDecodedData.Weight = objMyIDChecker.MilitaryCardItems.PersonalInfo.Weight.Pounds
                        objDecodedData.Eyes = objMyIDChecker.MilitaryCardItems.PersonalInfo.Eye.Color
                        objDecodedData.Hair = objMyIDChecker.MilitaryCardItems.PersonalInfo.Hair.Color
                        objDecodedData.DateOfIssue = strIssueDate
                        objDecodedData.DateOfExpiration = strExpirationDate
                        objDecodedData.CardType = "Military ID Card"

                        MyAppLog.WriteToLog("IVSDecodeService.DecodeData() CardType: Military ID Card")

                    Case 3

                        objDecodedData.IDAccountNumber = objMyIDChecker.INSEmployeeAuthItems.CardNumber.Text
                        objDecodedData.NameFirst = objMyIDChecker.INSEmployeeAuthItems.Name.First
                        objDecodedData.NameLast = objMyIDChecker.INSEmployeeAuthItems.Name.Last
                        objDecodedData.NameMiddle = objMyIDChecker.INSEmployeeAuthItems.Name.Middle
                        objDecodedData.DateOfBirth = strBirthDate
                        objDecodedData.Age = intAge
                        objDecodedData.Sex = objMyIDChecker.INSEmployeeAuthItems.Sex.Text
                        objDecodedData.DateOfIssue = strIssueDate
                        objDecodedData.DateOfExpiration = strExpirationDate
                        objDecodedData.CardType = "INS Employee Authorization Card"

                        MyAppLog.WriteToLog("IVSDecodeService.DecodeData() CardType: INS Employee Authorization Card")

                End Select

                DataAccess.WebServiceActivity(LicenseGuid.ToString, "DecodeData", objDecodedData.IDAccountNumber)

            Else
                DataAccess.LicenseException(LicenseGuid, objThisEndPointProperty.Address, objThisContext.Channel.LocalAddress.ToString, "DecodeData")
            End If

        Catch ex As Exception
            MyAppLog.WriteToLog(ex)
        End Try

        Return objDecodedData

    End Function

    Public Function KeepAlive() As DateTime Implements IIVSDecode.KeepAlive

        Return DateTime.Now

    End Function

End Class
