Module Module1

    Sub Main()
        Console.WriteLine("HI")
        testing123()
        Console.Read()

    End Sub

    Public Sub testing123()
        Try
            Console.WriteLine(Today.ToString("yy"))
            'Throw New ArgumentNullException("message")
            'Dim o As Integer = TryCast("TEST", String)
        Catch ex As Exception
            ex.Data.Add("Assembly", "testing123")
            ex.Data.Add("Method", "12345")
            Console.WriteLine("Source.ToString: " & ex.Source.ToString)
            Console.WriteLine("TargetSite.Module: " & ex.TargetSite.Module.ToString)
            Console.WriteLine("TargetSite: " & ex.TargetSite.ToString)
            Console.WriteLine("GetType: " & ex.GetType.ToString)
            Console.WriteLine("Message: " & ex.Message)

            For Each item As DictionaryEntry In ex.Data
                Console.WriteLine("Data: " & item.Key)
                Console.WriteLine("Value: " & item.Value)
            Next
            ' Console.WriteLine("GetType.Name: " & ex.st)

            Dim objTrace As System.Diagnostics.StackTrace = New System.Diagnostics.StackTrace(ex)
            '  Console.WriteLine(objTrace.FrameCoun)
            'Console.WriteLine(ex.GetType.Module)
            'Console.WriteLine(ex.TargetSite)
            'Console.WriteLine(ex.TargetSite)
            'Console.WriteLine(ex.TargetSite)
        End Try
    End Sub
End Module
