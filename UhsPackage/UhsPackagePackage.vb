Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Shell
Imports EnvDTE
Imports EnvDTE80

''' <summary>
''' This is the class that implements the package exposed by this assembly.
'''
''' The minimum requirement for a class to be considered a valid package for Visual Studio
''' is to implement the IVsPackage interface and register itself with the shell.
''' This package uses the helper classes defined inside the Managed Package Framework (MPF)
''' to do it: it derives from the Package class that provides the implementation of the 
''' IVsPackage interface and uses the registration attributes defined in the framework to 
''' register itself and its components with the shell.
''' </summary>
' The PackageRegistration attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class
' is a package.
'
' The InstalledProductRegistration attribute is used to register the information needed to show this package
' in the Help/About dialog of Visual Studio.



<ProvideAutoLoad(UIContextGuids.NoSolution)>
<PackageRegistration(UseManagedResourcesOnly:=True),
    InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400),
    Guid(GuidList.guidUhsPackagePkgString)>
Public NotInheritable Class UhsPackagePackage
    Inherits Package

    ''' <summary>
    ''' Default constructor of the package.
    ''' Inside this method you can place any initialization code that does not require 
    ''' any Visual Studio service because at this point the package object is created but 
    ''' not sited yet inside Visual Studio environment. The place to do all the other 
    ''' initialization is the Initialize method.
    ''' </summary>
    Public Sub New()
        Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", Me.GetType().Name))
    End Sub



    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Overridden Package Implementation
#Region "Package Members"

    Protected Sub RegisterUhsFiletype()
        If UserRegistryRoot IsNot Nothing Then
            Dim filexts = UserRegistryRoot.CreateSubKey("FileExtensionMapping")
            Dim UhsKey = filexts.OpenSubKey("uhs", False)
            If UhsKey Is Nothing Then
                UhsKey = filexts.CreateSubKey("uhs")
                UhsKey.SetValue(Nothing, "{8B382828-6202-11D1-8870-0000F87579D2}")
                UhsKey.SetValue("LogViewID", "{B2F072B0-ABC1-11D0-9D62-00C04FD9DFD9}")
                UhsKey.Close()
            End If
            filexts.Close()
        End If
    End Sub

    Protected Sub SetAutoLoadTrue()
        Dim DTE As EnvDTE.DTE
        DTE = CType(System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0"), DTE)
        DTE.Properties("Environment", "Documents").Item("DetectFileChangesOutsideIDE").Value = True
        DTE.Properties("Environment", "Documents").Item("AutoloadExternalChanges").Value = True
    End Sub



    ''' <summary>
    ''' Initialization of the package; this method is called right after the package is sited, so this is the place
    ''' where you can put all the initialization code that rely on services provided by VisualStudio.
    ''' </summary>
    Protected Overrides Sub Initialize()
        MyBase.Initialize()
        RegisterUhsFiletype()
        SetAutoLoadTrue()
    End Sub
#End Region

End Class
