Public Class IVSLicense

    Private _IVSGUID As Guid

    Property IVSGUID() As Guid
        Get
            'Grand Rapids
            'File Version 1.0.1.0
            'Return Guid.Parse("64B41D64-4BCD-4A80-9C02-89868269F643")

            'Spring Lake Park
            'File Version 1.0.3.0
            Return Guid.Parse("82603392-621A-4D1A-B4F0-0D879F79F070")

            'IVS_Demo IVS_TEPDemo
            'File Version 1.0.2.0
            'Return Guid.Parse("74E5A12A-238A-453E-9567-08F194B6A8DB")
        End Get
        Set(value As Guid)

        End Set
    End Property

End Class
