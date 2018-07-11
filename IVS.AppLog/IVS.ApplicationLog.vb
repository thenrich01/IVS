Imports System.Collections.ObjectModel
Imports System.IO

Public Class ApplicationLog
    Dim Lock As New Object

#Region "Properties"

    Enum DateEncodingType As Integer
        YYYYMMDD = 1
        YYYYDOY = 2
    End Enum

    Public mTotalSize As Long
    ''' <summary>
    ''' Provides the total number of byets in .log files in the Log File directory.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TotalSize() As Long
        Get
            TotalLogFileSize()
            Return mTotalSize
        End Get
    End Property

    Public mFileCount As Long
    ''' <summary>
    ''' Provides a count of all files with .log file extention in the Log File directory.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FileCount() As Long
        Get
            TotalLogFileSize()
            Return mFileCount
        End Get
    End Property
    Private mDateEncoding As DateEncodingType
    Public ReadOnly Property _DateEncoding() As String
        Get
            Return mDateEncoding.ToString
        End Get
    End Property

    Private mMyDateFormat As DateFormat
    Public ReadOnly Property _DateFormat() As String
        Get
            Return mMyDateFormat.ToString
        End Get
    End Property

    Private mPrefixWithDate As Boolean
    Public ReadOnly Property _PrefixWithDate() As Boolean
        Get
            Return mPrefixWithDate
        End Get
    End Property

    Private mPath As String
    Public ReadOnly Property _Path() As String
        Get
            Return mPath
        End Get
    End Property

    Private mFile As String
    Public ReadOnly Property _File() As String
        Get
            Return mFile
        End Get
    End Property

    Private mConnectionString As String
    Public ReadOnly Property _ConnectionString() As String
        Get
            Return mConnectionString
        End Get
    End Property

    Private mLogLevel As Integer
    Public Property _LogLevel() As Integer
        Get
            Return mLogLevel
        End Get

        Set(ByVal Value As Integer)
            mLogLevel = Value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Intializes parameters for how the Log File will be created.
    ''' </summary>
    ''' <param name="DateEncoding">The format of how the file name date code format will be implemented. ex. 20070312 or Day of Year 2008072 </param>
    ''' <param name="PrefixDateTimeStamp">True if you want a fixed length Date/Time stamp prefixed to each each entry.</param>
    ''' <param name="Format"></param>
    ''' <remarks></remarks>

    Public Sub New(ByVal DateEncoding As DateEncodingType, Optional ByVal PrefixDateTimeStamp As Boolean = False, Optional ByVal ProductName As String = Nothing, Optional ByVal Format As DateFormat = DateFormat.GeneralDate, Optional ByVal LogLevel As Integer = 0)
        mDateEncoding = DateEncoding
        mPrefixWithDate = PrefixDateTimeStamp
        mMyDateFormat = Format
        mLogLevel = LogLevel

        If ProductName = Nothing Then
            mPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\" & Trim(My.Application.Info.CompanyName) & "\" & Trim(My.Application.Info.ProductName) & "\Log Files\"
        Else
            mPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\" & Trim(My.Application.Info.CompanyName) & "\" & ProductName & "\Log Files\"
        End If

        mFile = TodaysFileName(mDateEncoding)

        System.Diagnostics.EventLog.WriteEntry("IVS", String.Format("Log Level: {0}, Log file path:{1}, Log Name:{2}", mLogLevel, mPath, mFile), EventLogEntryType.Information)

    End Sub

    ''' <summary>
    ''' Intializes parameters for how the Log File will be created.
    ''' </summary>
    ''' <param name="ConnectionString">The application connection string </param>
    ''' <remarks></remarks>

    'Public Sub New(ByVal ConnectionString As String)
    '    mConnectionString = ConnectionString
    'End Sub

    ''' <summary>
    ''' Provides a date coded file name with optional date formating.
    ''' YYYYMMDD or YYYYDOY
    ''' </summary>
    ''' <param name="DateEncoding"> Valid values: 1 = YYYYMMDD or 2 = YYYYDOY</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 

    Private Function TodaysFileName(ByVal DateEncoding As DateEncodingType) As String
        Select Case DateEncoding
            Case Is = 1
                TodaysFileName = DateTime.Now.Year & DateTime.Now.Month.ToString.PadLeft(2, "0") & DateTime.Now.Day.ToString.PadLeft(2, "0") _
                                    & "_" & My.Application.Info.Title & ".log"
            Case Is = 2
                TodaysFileName = DateTime.Now.Year & DateTime.Now.DayOfYear.ToString.PadLeft(3, "0") _
                                    & "_" & My.Application.Info.Title & ".log"
            Case Else
                TodaysFileName = DateTime.Now.Year & DateTime.Now.Month.ToString.PadLeft(2, "0") & DateTime.Now.Day.ToString.PadLeft(2, "0") _
                                    & "_" & My.Application.Info.Title & ".log"
        End Select
    End Function

    Public Sub WriteToLog(ByVal LogEntry As String)
        'UNCOMMENT TO ENABLE DIAGNOSTIC LOGGING

        'mFile = TodaysFileName(mDateEncoding)

        Dim LogValue As String
        'Check to see if a Date Time stamp should prefix the string
        If mPrefixWithDate Then
            LogValue = FormatDateTime(Now, mMyDateFormat).ToString.PadRight(23, " ") & "- " & LogEntry
        Else
            LogValue = LogEntry
        End If
        LogValue += vbCrLf

        'Check to see if the folder exists
        If Not My.Computer.FileSystem.DirectoryExists(mPath) Then
            'My.Computer.FileSystem.CreateDirectory(mPath)
        End If

        'Check to see if the file exists
        If Not My.Computer.FileSystem.FileExists(mPath & mFile) Then
            'My.Computer.FileSystem.WriteAllText(mPath & mFile, String.Empty, False)
        End If

        'Write the data to the log
        SyncLock Lock
            'My.Computer.FileSystem.WriteAllText(mPath & mFile, LogValue, True)
            System.Diagnostics.EventLog.WriteEntry("IVS", LogValue, EventLogEntryType.Information)
        End SyncLock

    End Sub

    Public Sub WriteToLog(ByVal ex As Exception)

        mFile = TodaysFileName(mDateEncoding)

        Dim LogValue As String = ">" & New String("-", 100) & vbCrLf

        'Check to see if a Date Time stamp should prefix the string
        If mPrefixWithDate Then
            LogValue += FormatDateTime(Now, mMyDateFormat).ToString.PadRight(30, " ") & "Application Exception" & vbCrLf
        Else
            LogValue += "Application Exception" & New String("_", 50) & vbCrLf
        End If
        LogValue += ex.Source & vbCrLf
        LogValue += ex.Message & vbCrLf
        LogValue += ex.ToString & vbCrLf
        LogValue += New String("-", 100) & "<" & vbCrLf


        'Check to see if the folder exists
        If Not My.Computer.FileSystem.DirectoryExists(mPath) Then
            'My.Computer.FileSystem.CreateDirectory(mPath)
        End If

        'Check to see if the file exists
        If Not My.Computer.FileSystem.FileExists(mPath & mFile) Then
            'My.Computer.FileSystem.WriteAllText(mPath & mFile, String.Empty, False)
        End If
        'Write the data to the log
        SyncLock LogValue
            'My.Computer.FileSystem.WriteAllText(mPath & mFile, LogValue, True)
        End SyncLock

        System.Diagnostics.EventLog.WriteEntry(ex.Source, ex.ToString, EventLogEntryType.Error)

    End Sub

    Public Sub WriteToLog(ByVal Source As String, ByVal LogEntry As String, ByVal Type As EventLogEntryType)

        mFile = TodaysFileName(mDateEncoding)

        Dim LogValue As String
        'Check to see if a Date Time stamp should prefix the string
        If mPrefixWithDate Then
            LogValue = FormatDateTime(Now, mMyDateFormat).ToString.PadRight(23, " ") & "- " & LogEntry
        Else
            LogValue = LogEntry
        End If
        LogValue += vbCrLf

        'Check to see if the folder exists
        If Not My.Computer.FileSystem.DirectoryExists(mPath) Then
            My.Computer.FileSystem.CreateDirectory(mPath)
        End If

        'Check to see if the file exists
        If Not My.Computer.FileSystem.FileExists(mPath & mFile) Then
            My.Computer.FileSystem.WriteAllText(mPath & mFile, String.Empty, False)
        End If

        'Write the data to the log
        SyncLock Lock
            My.Computer.FileSystem.WriteAllText(mPath & mFile, LogValue, True)
            System.Diagnostics.EventLog.WriteEntry(Source, LogEntry, Type)
        End SyncLock
    End Sub

    Public Sub WriteToLog(ByVal Source As String, ByVal LogEntry As String, ByVal Type As EventLogEntryType, ByVal MsgLogLevel As String)

        If MsgLogLevel <= mLogLevel Then
            'Only write to log file if message log value is <= application log level - Stored in App config.

            mFile = TodaysFileName(mDateEncoding)

            Dim LogValue As String
            'Check to see if a Date Time stamp should prefix the string
            If mPrefixWithDate Then
                LogValue = FormatDateTime(Now, mMyDateFormat).ToString.PadRight(23, " ") & "- " & LogEntry
            Else
                LogValue = LogEntry
            End If
            LogValue += vbCrLf

            'Check to see if the folder exists
            If Not My.Computer.FileSystem.DirectoryExists(mPath) Then
                My.Computer.FileSystem.CreateDirectory(mPath)
            End If

            'Check to see if the file exists
            If Not My.Computer.FileSystem.FileExists(mPath & mFile) Then
                My.Computer.FileSystem.WriteAllText(mPath & mFile, String.Empty, False)
            End If

            'Write the data to the log

            SyncLock Lock
                My.Computer.FileSystem.WriteAllText(mPath & mFile, LogValue, True)
                'System.Diagnostics.EventLog.WriteEntry(Source, LogEntry, Type)
            End SyncLock
        End If

    End Sub

    Private Sub TotalLogFileSize()
        mTotalSize = 0
        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(mPath, FileIO.SearchOption.SearchTopLevelOnly, "*.log")
        mFileCount = files.Count
        For Each s As String In files
            Dim fileData As FileInfo = My.Computer.FileSystem.GetFileInfo(s)
            mTotalSize += fileData.Length
        Next
    End Sub

    Public Sub DeleteExpiredLogFiles(ByVal LogRetention As Integer)

        Dim objFileInfo As FileInfo
        Dim objDir As DirectoryInfo = New DirectoryInfo(mPath)
        Dim strLogFileDate As String
        Dim intDateDifference As Integer

        Try
            WriteToLog("DeleteExpiredLogFiles() Searching for application log files more than " & LogRetention & " days old")

            For Each objFileInfo In objDir.GetFiles()

                If objFileInfo.Name.Contains(My.Application.Info.Title & ".log") Then

                    strLogFileDate = objFileInfo.Name.Substring(0, 4) & "-" & objFileInfo.Name.Substring(4, 2) & "/" & objFileInfo.Name.Substring(6, 2)

                    intDateDifference = DateDiff(DateInterval.Day, CDate(strLogFileDate), Today.Date)

                    If intDateDifference > LogRetention Then

                        My.Computer.FileSystem.DeleteFile(mPath & objFileInfo.Name)

                    End If

                End If

            Next

        Catch Ex As Exception
            System.Diagnostics.EventLog.WriteEntry("IVS", Ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Class