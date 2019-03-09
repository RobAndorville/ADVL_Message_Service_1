﻿
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Multiple, UseSynchronizationContext:=False)>
Public Class MsgService
    Implements IMsgService

    Private Shared ReadOnly connections As New List(Of clsConnection)()
    'connections is a list of connection information.
    'Each item contains the application name and the callback used to send a message to the application, a GetAllWarningsflag and a GetAllMessages flag.

    'Private Shared ReadOnly adminConnections As New List(Of clsConnection)()
    'adminConnections is a list of Admin Connection information.
    'Each item contains the application name, the callback used to send a message to the application, a GetWarnings flag and a GetAllMessages flag.

    'The Main Node is an application associated with this message service. It has a user interface that displays a list of connected applications.
    'UPDATE 6-Oct-2018 - ADVL_Message_Service_1 now hosts the Message Servie. This is a separate application to the ADVL_Application_Network AppNet). - (COULD NOT COMMUNICATE BETWEEN AppNet and other apps when the Message Service was hosted from it.)
    Private Shared _mainNodeName As String = ""
    Property MainNodeName As String
        Get
            Return _mainNodeName
        End Get
        Set(value As String)
            _mainNodeName = value
        End Set
    End Property

    'The Main Node callback - used to send a message to the Main Node application.
    'Private _mainNodeCallback As IMsgServiceCallback
    Private Shared _mainNodeCallback As IMsgServiceCallback
    Property MainNodeCallback As IMsgServiceCallback
        Get
            Return _mainNodeCallback
        End Get
        Set(value As IMsgServiceCallback)
            _mainNodeCallback = value
        End Set
    End Property

    'Public Function Connect(ByVal appName As String, ByVal connectionName As String, ByVal projectName As String, ByVal projectDescription As String, ByVal settingsLocnType As ADVL_Utilities_Library_1.FileLocation.Types, ByVal settingsLocnPath As String, ByVal appType As clsConnection.AppTypes, ByVal getAllWarnings As Boolean, ByVal getAllMessages As Boolean) As String Implements IMsgService.Connect
    Public Function Connect(ByVal appNetName As String, ByVal appName As String, ByVal connectionName As String, ByVal projectName As String, ByVal projectDescription As String, ByVal projectType As ADVL_Utilities_Library_1.Project.Types, ByVal projectPath As String, ByVal getAllWarnings As Boolean, ByVal getAllMessages As Boolean) As String Implements IMsgService.Connect
        'The Connect function adds a connection to the connections list.
        'Debug.Print("Connecting ...")

        Try
            Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)()

            ''Check if appName is already on the connections list:
            Dim conn As clsConnection

            'Dim NewConnectionName As String = NewConnName(connectionName) 'This checks if connectionName is available and modifies the name if required.
            Dim NewConnectionName As String = NewConnName(appNetName, connectionName) 'This checks if connectionName is available and modifies the name if required.

            If NewConnectionName <> "" Then 'NewConnectionName is not already on the list
                'Dim Connection As New clsConnection(appName, NewConnectionName, projectName, projectDescription, settingsLocnType, settingsLocnPath, appType, callback, getAllWarnings, getAllMessages)
                'Dim Connection As New clsConnection(appName, NewConnectionName, projectName, projectDescription, projectType, projectPath, callback, getAllWarnings, getAllMessages)
                Dim Connection As New clsConnection(appNetName, appName, NewConnectionName, projectName, projectDescription, projectType, projectPath, callback, getAllWarnings, getAllMessages)
                connections.Add(Connection)

                'AppType code removed 2Feb19 ================================================================================
                'If Connection.AppType = clsConnection.AppTypes.MainNode Then
                '    MainNodeName = Connection.ConnectionName '12May18 UPDATE
                '    MainNodeCallback = Connection.Callback
                '    'NOTE: Program freezes if an attempt is made to send a message to the Main Node before the Main Note form has been displayed.
                '    'Connection information is not sent for the Main Node connection.
                'Else
                'AppType code removed 2Feb19 --------------------------------------------------------------------------------

                'Add the new connection information to the data grid:
                'If NewConnectionName <> "ApplicationNetwork" Then
                If NewConnectionName <> "MessageService" Then
                    'If Main.ConnectionNameAvailable(NewConnectionName) Then 'UPDATED 12May18
                    If Main.ConnectionNameAvailable(appNetName, NewConnectionName) Then 'UPDATED 12May18

                        'Dont show it on Main.dgvConnections!

                        Main.dgvConnections.Rows.Add()
                        Dim CurrentRow As Integer = Main.dgvConnections.Rows.Count - 2

                        'UPDATED 2Feb19:
                        Main.dgvConnections.Rows(CurrentRow).Cells(0).Value = appNetName 'New connection AppNet Name

                        'Main.dgvConnections.Rows(CurrentRow).Cells(0).Value = appName 'New connection App Name
                        Main.dgvConnections.Rows(CurrentRow).Cells(1).Value = appName 'New connection App Name
                        'Main.dgvConnections.Rows(CurrentRow).Cells(1).Value = NewConnectionName 'New connection Name - UPDATED 12May18
                        Main.dgvConnections.Rows(CurrentRow).Cells(2).Value = NewConnectionName 'New connection Name 
                        'Main.dgvConnections.Rows(CurrentRow).Cells(2).Value = projectName 'New Project Name - UPDATED 12May18
                        Main.dgvConnections.Rows(CurrentRow).Cells(3).Value = projectName 'New Project Name 

                        'AppType code removed 2Feb19 ================================================================================
                        'Select Case appType
                        '    Case clsConnection.AppTypes.Application
                        '        Main.dgvConnections.Rows(CurrentRow).Cells(3).Value = "Application" 'New connection App Type
                        '    Case clsConnection.AppTypes.MainNode
                        '        Main.dgvConnections.Rows(CurrentRow).Cells(3).Value = "Main Node" 'New connection App Type
                        '    Case clsConnection.AppTypes.Node
                        '        Main.dgvConnections.Rows(CurrentRow).Cells(3).Value = "Node" 'New connection App Type
                        'End Select
                        'AppType code removed 2Feb19 --------------------------------------------------------------------------------

                        Main.dgvConnections.Rows(CurrentRow).Cells(4).Value = projectType.ToString 'New Project Type
                        Main.dgvConnections.Rows(CurrentRow).Cells(5).Value = projectPath 'New Project Path

                        Select Case getAllWarnings
                            Case True
                                Main.dgvConnections.Rows(CurrentRow).Cells(6).Value = "True" 'New connection GetAllWarnings is True
                            Case False
                                Main.dgvConnections.Rows(CurrentRow).Cells(6).Value = "False" 'New connection GetAllWarnings is False
                        End Select
                        Select Case getAllMessages
                            Case True
                                Main.dgvConnections.Rows(CurrentRow).Cells(7).Value = "True" 'New connection GetAllMessages is True
                            Case False
                                Main.dgvConnections.Rows(CurrentRow).Cells(7).Value = "False" 'New connection GetAllMessages is False
                        End Select
                        Main.dgvConnections.Rows(CurrentRow).Cells(8).Value = callback.GetHashCode 'New connection Callback hash code
                        Main.dgvConnections.Rows(CurrentRow).Cells(9).Value = Format(Now, "d-MMM-yyyy H:mm:ss") 'New connection start time
                        Main.dgvConnections.Rows(CurrentRow).Cells(10).Value = "0" 'New connection duration
                        Main.dgvConnections.AutoResizeColumns(DataGridViewAutoSizeColumnMode.AllCells)
                    Else
                        'Connection App Name not available.
                    End If
                End If

                'AppType code removed 2Feb19 ================================================================================
                'End If
                'AppType code removed 2Feb19 --------------------------------------------------------------------------------

                Connect = NewConnectionName
            Else
                'appName is already on the list.
                SendMainNodeMessage("WARNING: Connection failed because " & connectionName & " (and modified versions) is already on the connections list." & vbCrLf)
                Return False
            End If
            'End If
        Catch ex As Exception
            SendMainNodeMessage("WARNING: Connection failed: " & ex.Message & vbCrLf)
            Return False
        End Try
    End Function

    'Private Function NewConnName(ByVal reqConnName As String) As String
    '    'Return an available connection name based in the requested name: reqConnName.

    '    'Check if reqConnName is already on the connections list:
    '    Dim conn As clsConnection
    '    conn = connections.Find(Function(item As clsConnection)
    '                                If IsNothing(item) Then
    '                                    'An error is raised if an item of nothing is used in the Return code.
    '                                Else
    '                                    'Return item.ReqConnName = reqConnName
    '                                    Return item.ConnectionName = reqConnName
    '                                End If
    '                            End Function)
    '    If IsNothing(conn) Then 'reqConnName is not already on the list
    '        Return reqConnName 'Return reqConnName as an available connection name.
    '    Else
    '        Dim Imax As Integer = connections.Count + 1
    '        Dim tryConnName As String = ""
    '        Dim I As Integer
    '        For I = 1 To Imax
    '            'tryConnName = reqConnName & I
    '            tryConnName = reqConnName & "-" & I
    '            conn = connections.Find(Function(item As clsConnection)
    '                                        If IsNothing(item) Then
    '                                            'An error is raised if an item of nothing is used in the Return code.
    '                                        Else
    '                                            'Return item.ReqConnName = tryConnName
    '                                            Return item.ConnectionName = tryConnName
    '                                        End If
    '                                    End Function)
    '            If IsNothing(conn) Then 'tryConnName is not already on the list
    '                'tryConnName is not on the list. It can be used for a new connection.
    '                Exit For
    '            Else 'tryConnName is already on the connection list.
    '                tryConnName = ""
    '            End If
    '        Next
    '        Return tryConnName
    '    End If
    'End Function

    'CODE UPDATED TO INCLUDE AppNetName 2Feb19
    Private Function NewConnName(ByVal AppNetName As String, ByVal reqConnName As String) As String
        'Return an available connection name based in the requested name: reqConnName.

        'Check if reqConnName is already on the connections list:
        Dim conn As clsConnection
        conn = connections.Find(Function(item As clsConnection)
                                    'If IsNothing(item) Then
                                    '    'An error is raised if an item of nothing is used in the Return code.
                                    'Else
                                    '    Return item.ConnectionName = reqConnName
                                    'End If
                                    Return item.ConnectionName = reqConnName And item.AppNetName = AppNetName
                                End Function)
        If IsNothing(conn) Then 'reqConnName is not already on the list
            Return reqConnName 'Return reqConnName as an available connection name.
        Else
            Dim Imax As Integer = connections.Count + 1
            Dim tryConnName As String = ""
            Dim I As Integer
            For I = 1 To Imax
                tryConnName = reqConnName & "-" & I
                conn = connections.Find(Function(item As clsConnection)
                                            'If IsNothing(item) Then
                                            '    'An error is raised if an item of nothing is used in the Return code.
                                            'Else
                                            '    Return item.ConnectionName = tryConnName
                                            'End If
                                            Return item.ConnectionName = reqConnName And item.AppNetName = AppNetName
                                        End Function)
                If IsNothing(conn) Then 'tryConnName is not already on the list
                    'tryConnName is not on the list. It can be used for a new connection.
                    Exit For
                Else 'tryConnName is already on the connection list.
                    tryConnName = ""
                End If
            Next
            Return tryConnName
        End If
    End Function

    Public Function ConnectionAvailable(ByVal AppNetName As String, ByVal ConnName As String) As Boolean Implements IMsgService.ConnectionAvailable
        'Return True if a Connection named ConnName is available for use in the Application Network named AppNetName.

        Dim conn As clsConnection
        conn = connections.Find(Function(item As clsConnection)
                                    'If IsNothing(item) Then
                                    '    'An error is raised if an item of nothing is used in the Return code.
                                    'Else
                                    '    Return item.ConnectionName = reqConnName
                                    'End If
                                    Return item.ConnectionName = ConnName And item.AppNetName = AppNetName
                                End Function)
        If IsNothing(conn) Then 'ConnName is available for use in the Application Network named AppNetName.
            Return True
        Else 'ConnName is already in use in the Application Network named AppNetName.
            Return False
        End If

    End Function

    'Public Function AdminConnect(ByVal appName As String, ByVal getWarnings As Boolean, ByVal getAllMessages As Boolean) As Boolean Implements IMsgService.AdminConnect
    '    'The AdminConnect function adds an administration connection to the adminConnections list.
    '    'Admin connections can receive all warnings and all messages.
    '    'The getWarnings and getAllMessages flags are used to indicate which messagese are sent.

    '    Try
    '        Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)()
    '        Dim AdminConnection As New clsConnection(appName, callback, getWarnings, getAllMessages)
    '        If adminConnections.Count = 0 Then
    '            adminConnections.Add(AdminConnection)
    '            Return True
    '        Else
    '            'Check if appName is already on the adminConnections list:
    '            Dim conn As clsConnection
    '            conn = adminConnections.Find(Function(item As clsConnection)
    '                                             If IsNothing(item) Then
    '                                                 'An error is raised if an item of nothing is used in the Return code.
    '                                             Else
    '                                                 Return item.AppName = appName
    '                                             End If
    '                                         End Function)
    '            If IsNothing(conn) Then 'appName is not already on the list.
    '                'adminConnections.Add(conn)
    '                adminConnections.Add(AdminConnection)
    '                Return True
    '            Else
    '                'appName is already on the list.
    '                Return False
    '            End If
    '        End If

    '    Catch ex As Exception
    '        Return False
    '    End Try


    'End Function

    'Public Sub SendMessage(ByVal appName As String, ByVal message As String) Implements IMsgService.SendMessage
    'Public Sub SendMessage(ByVal connName As String, ByVal message As String) Implements IMsgService.SendMessage
    Public Sub SendMessage(ByVal appNetName As String, ByVal connName As String, ByVal message As String) Implements IMsgService.SendMessage
        'Send the message to the application with the connection name appName.

        'Find the connection for the application corresponding to appName:
        Dim conn As clsConnection
        conn = connections.Find(Function(item As clsConnection)
                                    'If IsNothing(item) Then
                                    '    'An error is raised if an item of nothing is used in the Return code.
                                    'Else
                                    '    'Return item.AppName = appName
                                    '    Return item.ConnectionName = connName
                                    'End If
                                    Return item.ConnectionName = connName And item.AppNetName = appNetName
                                End Function)
        If IsNothing(conn) Then
            'The connection is not on the list!

            'If connName = "ApplicationNetwork" Then
            If connName = "MessageService" Then
                Main.InstrReceived = message
            Else
                Main.Message.Add("Connection name: " & connName & " not found." & vbCrLf)
            End If
        Else
            If DirectCast(conn.Callback, ICommunicationObject).State = CommunicationState.Opened Then
                'Send a message showing the callers callback:
                Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)()
                Dim SenderName As String
                Dim connMatch = From conn2 In connections Where conn2.Callback.GetHashCode = callback.GetHashCode
                If connMatch.Count > 0 Then
                    'conn.Callback.OnSendMessage("The message was sent from: " & connMatch(0).AppName & vbCrLf)
                    'conn.Callback.OnSendMessage(connMatch(0).AppName & "> ")
                Else
                    conn.Callback.OnSendMessage("The sender is not on the connection list " & vbCrLf)
                End If
                conn.Callback.OnSendMessage(message) 'The following error was returned when a large list of projected coordinate reference system names was sent by the Coordinates app:
                'An exception of type 'System.ServiceModel.ProtocolException' occurred in
                'mscorlib.dll but was not handled in user code.
                'Additional onformation: The remote server returned an unexpected response:
                '(413) Request Entitly Too Large.

            Else
                connections.Remove(conn)
            End If
        End If
    End Sub

    'Public Sub SendAllMessage(ByVal message As String, ByVal SenderName As String) Implements IMsgService.SendAllMessage
    Public Sub SendAllMessage(ByVal message As String, ByVal SenderConnName As String) Implements IMsgService.SendAllMessage
        'Send the message to all connections in the connections list.
        Dim I As Integer 'Loop index
        For I = 1 To connections.Count
            'If connections(I - 1).AppName = SenderName Then
            If connections(I - 1).ConnectionName = SenderConnName Then
                'Dont send the message back to the sender.
            Else
                If DirectCast(connections(I - 1).Callback, ICommunicationObject).State = CommunicationState.Opened Then
                    connections(I - 1).Callback.OnSendMessage(message)
                Else

                End If
            End If

        Next
    End Sub

    'Public Sub SendAdminMessage(ByVal message As String) Implements IMsgService.SendAdminMessage
    '    'Send the message to all connections in the adminConnections list.
    '    Dim I As Integer 'Loop index
    '    For I = 1 To adminConnections.Count
    '        If DirectCast(adminConnections(I - 1).Callback, ICommunicationObject).State = CommunicationState.Opened Then
    '            adminConnections(I - 1).Callback.OnSendMessage(message)
    '        Else

    '        End If
    '    Next
    'End Sub

    Public Sub SendMainNodeMessage(ByVal message As String) Implements IMsgService.SendMainNodeMessage
        'Send the message to the Main Node.
        'ADD TRY ... CATCH ------------------------------------------------------------
        If MainNodeName <> "" Then
            MainNodeCallback.OnSendMessage(message)
        End If
    End Sub

    'Public Sub GetConnectionList() Implements IMsgService.GetConnectionList
    '    Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)() 'The connection list will be sent back to the requesting connection.
    '    'callback.OnSendMessage("Connection list: (AppName | Connection Code)" & vbCrLf)
    '    'callback.OnSendMessage("Connection list: (AppName | Connection Name | Connection Code)" & vbCrLf)

    '    'Dim decl As New XDeclaration("1.0", "utf-8", "yes")
    '    'Dim doc As New XDocument(decl, Nothing) 'Create an XDocument to store the instructions.
    '    'Dim xmessage As New XElement("XMsg") 'This indicates the start of the message in the XMessage class
    '    'Dim connectionList As New XElement("ConnectionList")

    '    For Each item In connections
    '        'callback.OnSendMessage(item.AppName & " | " & item.Callback.GetHashCode & vbCrLf)
    '        callback.OnSendMessage(item.AppName & " | " & item.ConnectionName & " | " & item.Callback.GetHashCode & vbCrLf)

    '        'Dim applicationInfo As New XElement("ApplicationInfo")
    '        'Dim Name As New XElement("Name", item.AppName)
    '        'applicationInfo.Add(Name)
    '        'Dim Descr As New XElement("Description", item.)
    '    Next

    'End Sub

    Public Sub GetConnectionList() Implements IMsgService.GetConnectionList
        Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)() 'The connection list will be sent back to the requesting connection.

        Dim decl As New XDeclaration("1.0", "utf-8", "yes")
        Dim doc As New XDocument(decl, Nothing) 'Create an XDocument to store the instructions.
        Dim xmessage As New XElement("XMsg") 'This indicates the start of the message in the XMessage class
        Dim connectionList As New XElement("ConnectionList")

        For Each item In connections
            Dim connectionInfo As New XElement("Connection")

            'ADDED 2Feb19:
            Dim appNetName As New XElement("AppNetName", item.AppNetName)
            connectionInfo.Add(appNetName)

            Dim name As New XElement("Name", item.ConnectionName)
            connectionInfo.Add(name)
            Dim appName As New XElement("ApplicationName", item.AppName)
            connectionInfo.Add(appName)

            'REMOVED 2Feb19:
            'Dim appType As New XElement("ApplicationType", item.AppType)
            'connectionInfo.Add(appType)

            Dim getAllMessages As New XElement("GetAllMessages", item.GetAllMessages)
            connectionInfo.Add(getAllMessages)
            Dim getAllMWarnings As New XElement("GetAllWarnings", item.GetAllWarnings)
            connectionInfo.Add(getAllMWarnings)
            Dim projectName As New XElement("ProjectName", item.ProjectName)
            connectionInfo.Add(projectName)
            Dim projectDescription As New XElement("ProjectDescription", item.ProjectDescription)
            connectionInfo.Add(projectDescription)


            'Dim settingsLocnPath As New XElement("SettingsLocationPath", item.SettingsLocnPath)
            'connectionInfo.Add(settingsLocnPath)
            'Dim settingsLocnType As New XElement("SettingsLocationType", item.SettingsLocnType)
            'connectionInfo.Add(settingsLocnType)

            'UPDATED 2Feb19:
            Dim projectType As New XElement("ProjectType", item.ProjectType)
            connectionInfo.Add(projectType)
            Dim projectPath As New XElement("ProjectPath", item.ProjectPath)
            connectionInfo.Add(projectPath)

            connectionList.Add(connectionInfo)
        Next

        xmessage.Add(connectionList)
        doc.Add(xmessage)

        callback.OnSendMessage(doc.ToString)

    End Sub

    Public Sub GetApplicationList() Implements IMsgService.GetApplicationList
        'Get the list of applications from the Message Service.
        Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)() 'The application list will be sent back to the requesting conection.

        Dim decl As New XDeclaration("1.0", "utf-8", "yes")
        Dim doc As New XDocument(decl, Nothing) 'Create an XDocument to store the instructions.
        Dim xmessage As New XElement("XMsg") 'This indicates the start of the message in the XMessage class
        Dim applicationList As New XElement("ApplicationList")

        For Each item In Main.App.List
            Dim applicationInfo As New XElement("Application")
            Dim name As New XElement("Name", item.Name)
            applicationInfo.Add(name)
            Dim description As New XElement("Description", item.Description)
            applicationInfo.Add(description)
            Dim directory As New XElement("Directory", item.Directory)
            applicationInfo.Add(directory)
            Dim executablePath As New XElement("ExecutablePath", item.ExecutablePath)
            applicationInfo.Add(executablePath)
            applicationList.Add(applicationInfo)
        Next
        xmessage.Add(applicationList)
        doc.Add(xmessage)

        callback.OnSendMessage(doc.ToString)

    End Sub

    Public Sub GetApplicationInfo(ByVal appName As String) Implements IMsgService.GetApplicationInfo
        'Get information about an application.
        Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)() 'The application information will be sent back to the requesting conection.

        Dim decl As New XDeclaration("1.0", "utf-8", "yes")
        Dim doc As New XDocument(decl, Nothing) 'Create an XDocument to store the instructions.
        Dim xmessage As New XElement("XMsg") 'This indicates the start of the message in the XMessage class
        Dim applicationInfo As New XElement("ApplicationInfo")

        Dim newName As String = ""
        Dim newDescription As String = ""
        Dim newDirectory As String = ""
        Dim newExePath As String = ""

        Dim Count As Integer = 0

        For Each item In Main.App.List
            If item.Name = appName Then 'The appName has been found in the Application List.
                Count += 1 'Increment the cout of found names. (There should only be one application with this name found.)
                newName = item.Name
                newDescription = item.Description
                newDirectory = item.Directory
                newExePath = item.ExecutablePath
            End If
        Next

        If Count = 0 Then
            Dim name As New XElement("Name", newName)
            applicationInfo.Add(name)
            Dim description As New XElement("Description", "")
            applicationInfo.Add(description)
            Dim directory As New XElement("Directory", "")
            applicationInfo.Add(directory)
            Dim executablePath As New XElement("ExecutablePath", "")
            applicationInfo.Add(executablePath)
            Dim status As New XElement("Status", "Application not found in Message Service list.")
            applicationInfo.Add(status)
            xmessage.Add(applicationInfo)
            doc.Add(xmessage)
            callback.OnSendMessage(doc.ToString)
        ElseIf Count > 1 Then
            Dim name As New XElement("Name", newName)
            applicationInfo.Add(name)
            Dim description As New XElement("Description", "")
            applicationInfo.Add(description)
            Dim directory As New XElement("Directory", "")
            applicationInfo.Add(directory)
            Dim executablePath As New XElement("ExecutablePath", "")
            applicationInfo.Add(executablePath)
            Dim status As New XElement("Status", "More than one Application name matches found in Message Service list.")
            applicationInfo.Add(status)
            xmessage.Add(applicationInfo)
            doc.Add(xmessage)
            callback.OnSendMessage(doc.ToString)
        Else 'Single application found with the name appName.
            Dim name As New XElement("Name", newName)
            applicationInfo.Add(name)
            Dim description As New XElement("Description", newDescription)
            applicationInfo.Add(description)
            Dim directory As New XElement("Directory", newDirectory)
            applicationInfo.Add(directory)
            Dim executablePath As New XElement("ExecutablePath", newExePath)
            applicationInfo.Add(executablePath)
            Dim status As New XElement("Status", "Application information found in Message service list.")
            applicationInfo.Add(status)
            xmessage.Add(applicationInfo)
            doc.Add(xmessage)
            callback.OnSendMessage(doc.ToString)
        End If

    End Sub

    Public Sub GetMessageServiceAppInfo() Implements IMsgService.GetMessageServiceAppInfo
        'Get information about the Message Service application.
        Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)() 'The application information will be sent back to the requesting conection.

        Dim decl As New XDeclaration("1.0", "utf-8", "yes")
        Dim doc As New XDocument(decl, Nothing) 'Create an XDocument to store the instructions.
        Dim xmessage As New XElement("XMsg") 'This indicates the start of the message in the XMessage class
        Dim applicationInfo As New XElement("MessageServiceAppInfo")

        Dim applicationName As New XElement("Name", Main.ApplicationInfo.Name)
        applicationInfo.Add(applicationName)
        Dim applicationPath As New XElement("Path", Main.ApplicationInfo.ApplicationDir)
        applicationInfo.Add(applicationPath)
        Dim applicationExePath As New XElement("ExePath", Main.ApplicationInfo.ExecutablePath)
        applicationInfo.Add(applicationExePath)

        xmessage.Add(applicationInfo)
        doc.Add(xmessage)

        callback.OnSendMessage(doc.ToString)

    End Sub

    'Public Sub GetAdminConnectionList() Implements IMsgService.GetAdminConnectionList
    '    Dim callback As IMsgServiceCallback = OperationContext.Current.GetCallbackChannel(Of IMsgServiceCallback)() 'The connection list will be sent back to the requesting connection.
    '    callback.OnSendMessage("Admin connection list: (AppName | Connection Code | Get Warnings | Get All Messages)" & vbCrLf)
    '    For Each item In adminConnections
    '        callback.OnSendMessage(item.AppName & " | " & item.Callback.GetHashCode & " | " & item.GetWarnings & " | " & item.GetAllMessages & vbCrLf)
    '    Next
    'End Sub

    'Public Sub SendWarning(ByVal warning As String) Implements IMsgService.SendAdminWarning
    '    'Sends the warning to all connections in the adminConnections list with the getWarnings flag set.

    '    Dim I As Integer 'Loop index
    '    For I = 1 To adminConnections.Count
    '        If adminConnections(I - 1).GetWarnings = True Then
    '            If DirectCast(adminConnections(I - 1).Callback, ICommunicationObject).State = CommunicationState.Opened Then
    '                adminConnections(I - 1).Callback.OnSendMessage(warning)
    '            Else

    '            End If

    '        Else
    '            'Warnings are not requested for this Admin Connection.
    '        End If
    '    Next

    'End Sub

    Public Function IsAlive() As Boolean Implements IMsgService.IsAlive
        'Returns True if the service is running
        Return True
    End Function

    Public Function AppNetNameInUse(ByVal AppNetName As String) As Boolean
        'Return True if the specified AppNetName is in use.

        Dim conn As clsConnection
        conn = connections.Find(Function(item As clsConnection)
                                    Return item.AppNetName = AppNetName
                                End Function)
        If IsNothing(conn) Then
            Return False
        Else
            Return True
        End If
    End Function

    'Public Function Disconnect(ByVal appName As String) As Boolean Implements IMsgService.Disconnect
    'Public Function Disconnect(ByVal connName As String) As Boolean Implements IMsgService.Disconnect 'UPDATED 12May18
    Public Function Disconnect(ByVal appNetName As String, ByVal connName As String) As Boolean Implements IMsgService.Disconnect 'UPDATED 2Feb19
        'The Disconnect function removes a connection from the connections list.
        'Find the connection for the application corresponding to connName: 'Find the connection for the application corresponding to appName: 'UPDATED 12May18
        Dim conn As clsConnection
        conn = connections.Find(Function(item As clsConnection)
                                    'If IsNothing(item) Then
                                    '    'An error is raised if an item of nothing is used in the Return code.
                                    'Else
                                    '    Return item.ConnectionName = connName
                                    'End If
                                    Return item.ConnectionName = connName And item.AppNetName = appNetName
                                End Function)
        If IsNothing(conn) Then
            'The connection is not on the list!
            'SendWarning("WARNING: Disconnection failed because " & appName & " is not on the connections list." & vbCrLf)
            'SendMainNodeMessage("WARNING: Disconnection failed because " & appName & " is not on the connections list." & vbCrLf)
            'Main.Message.Add("WARNING: Disconnection failed because " & appName & " is not on the connections list." & vbCrLf)
            Main.Message.AddWarning("WARNING: Disconnection failed because appNetName = " & appNetName & " and connName = " & connName & " is not on the connections list." & vbCrLf)

            'Show the connections in the list:
            Dim I As Integer
            Dim NConn As Integer = connections.Count
            Main.Message.Add("Number of connections: " & NConn & vbCrLf)
            For I = 0 To NConn - 1
                Main.Message.Add(I & "  AppNetName: " & connections(I).AppNetName & "  Connection Name: " & connections(I).ConnectionName & vbCrLf)
            Next



            Return False
        Else
            connections.Remove(conn)
            Main.Message.Add("Connection removed: AppNetName: " & appNetName & "  Connection Name: " & connName & vbCrLf)

            'OLD CODE: Used for application version using a separate WCF service. ------------------------------------------
            'Send removed connection information to the Main Node connection:
            'Dim decl As New XDeclaration("1.0", "utf-8", "yes")
            'Dim doc As New XDocument(decl, Nothing) 'Create an XDocument to store the instructions.
            'Dim xmessage As New XElement("XMsg") 'This indicates the start of the message in the XMessage class
            'Dim removedConnectionInfo As New XElement("RemovedConnectionInfo")
            'Dim applicationName As New XElement("ApplicationName", appName)
            'removedConnectionInfo.Add(applicationName)
            'xmessage.Add(removedConnectionInfo)
            'doc.Add(xmessage)
            'SendMainNodeMessage(doc.ToString)

            'NEW CODE: Uses self hosted WCF service. --------------------------------------------------------------------------
            'Main.RemoveConnectionWithAppName(appName)
            'Main.RemoveConnectionWithName(connName)
            Main.RemoveConnectionWithName(appNetName, connName)

            Return True
        End If
    End Function

    'Public Function AdminDisconnect(ByVal appName As String) As Boolean Implements IMsgService.AdminDisconnect
    '    'The AdminDisconnect function removes an administration connection from the adminConnections list.
    '    'Find the connection for the application corresponding to appName:
    '    Dim conn As clsConnection
    '    conn = adminConnections.Find(Function(item As clsConnection)
    '                                     If IsNothing(item) Then
    '                                         'An error is raised if an item of nothing is used in the Return code.
    '                                     Else
    '                                         Return item.AppName = appName
    '                                     End If
    '                                 End Function)
    '    If IsNothing(conn) Then
    '        'The connection is not on the list!
    '        Return False
    '    Else
    '        adminConnections.Remove(conn)
    '        Return True
    '    End If
    'End Function

    Public Function AppNetNameUsed(ByVal AppNetName As String) As Boolean Implements IMsgService.AppNetNameUsed
        'Return True if the specified Application Network Name is used in the list of Connections.

        Dim conn As clsConnection
        conn = connections.Find(Function(item As clsConnection)
                                    Return item.AppNetName = AppNetName
                                End Function)
        If IsNothing(conn) Then 'No connection found using the Application Network Name AppNetName.
            Return False
        Else 'A connection was found using the Application Network Name AppNetName.
            Return True
        End If

    End Function

End Class
