Imports System

Class GuidList
    Private Sub New()
    End Sub

    Public Const guidUhsPackagePkgString As String = "470d2cfa-f5a5-4792-8b60-7e333060745a"
    Public Const guidUhsPackageCmdSetString As String = "31e6b45b-cc36-4f91-bdb9-b28ee28fd42f"

    Public Shared ReadOnly guidUhsPackageCmdSet As New Guid(guidUhsPackageCmdSetString)
End Class