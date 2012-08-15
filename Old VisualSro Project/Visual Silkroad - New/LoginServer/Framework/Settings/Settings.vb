﻿
Namespace Settings
    Module Settings
        'Loading File
        Private File As New SRFramework.cINI(AppDomain.CurrentDomain.BaseDirectory & "settings_login\settings.ini")

        Public Database_IP As String
        Public Database_Port As UShort
        Public Database_User As String
        Public Database_Password As String
        Public Database_Database As String

        Public GlobalManger_Ip As String = "0.0.0.0"
        Public GlobalManger_Port As UShort = 32000
        Public Const GlobalManager_ProtocolVersion As UInteger = 1

        Public Server_Ip As String = "0.0.0.0"
        Public Server_Port As UShort = 15780
        Public Server_NormalSlots As UInteger = 100
        Public Server_MaxClients As UInteger = 105
        Public Server_Id As UShort = 0
        Private _Server_DebugMode As Boolean = False
        Public Property Server_DebugMode 'Only for setting the PingDc on ClientList
            Get
                Return _Server_DebugMode
            End Get
            Set(ByVal value)
                _Server_DebugMode = value
                Server.Server_DebugMode = value
            End Set
        End Property


        Public Auto_Register As Boolean = False
        Public Max_FailedLogins As Integer = 5
        Public Max_RegistersPerDay As Integer = 3
        Public Server_CurrectVersion As UInteger = 0
        Public Server_Local As Byte = 0

        Public Log_Connect As Boolean = False
        Public Log_Register As Boolean = False
        Public Log_Login As Boolean = False
        Public Log_Packets As Boolean = False

        Public Sub LoadSettings()
            Server_Ip = File.Read("SERVER_INTERNAL", "Ip", "0.0.0.0")
            Server_Port = File.Read("SERVER_INTERNAL", "Port", "15880")
            Server_NormalSlots = File.Read("SERVER_INTERNAL", "Max_NormalSlots", "1000")
            Server_MaxClients = File.Read("SERVER_INTERNAL", "Max_Clients", "1050")
            Server_Id = File.Read("SERVER_INTERNAL", "Server_Id", "0")

            Database_IP = File.Read("DATABASE", "Ip", "127.0.0.1")
            Database_Port = File.Read("DATABASE", "Port", "3306")
            Database_Database = File.Read("DATABASE", "Database", "visualsro")
            Database_User = File.Read("DATABASE", "User", "root")
            Database_Password = File.Read("DATABASE", "Password", "")

            GlobalManger_Ip = File.Read("GLOBALMANAGER", "Ip", "127.0.0.1")
            GlobalManger_Port = File.Read("GLOBALMANAGER", "Port", "32000")

            Auto_Register = CBool(File.Read("SERVER", "Auto_Register", "0"))
            Max_FailedLogins = File.Read("SERVER", "Max_FailedLogins", "5")
            Max_RegistersPerDay = File.Read("SERVER", "Max_RegistersPerDay", "3")
            Server_CurrectVersion = File.Read("SERVER", "Server_CurrectVersion", "0")
            Server_Local = File.Read("SERVER", "Server_Local", "0")

            Log_Connect = CBool(File.Read("LOG", "Connect", "0"))
            Log_Login = CBool(File.Read("LOG", "Login", "0"))
            Log_Register = CBool(File.Read("LOG", "Register", "0"))
        End Sub

        Public Sub SetToServer()
            Database.DbIp = Database_IP
            Database.DbPort = Database_Port
            Database.DbDatabase = Database_Database
            Database.DbUsername = Database_User
            Database.DbPassword = Database_Password

            Server.Ip = Server_Ip
            Server.Port = Server_Port
            Server.MaxNormalClients = Server_NormalSlots
            Server.MaxClients = Server_MaxClients
            Server.Server_DebugMode = Server_DebugMode
        End Sub

    End Module
End Namespace
