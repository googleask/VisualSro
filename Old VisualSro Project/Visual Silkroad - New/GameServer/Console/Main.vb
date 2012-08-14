﻿Imports System.Threading
Imports SRFramework

Friend Class Program

    Shared Sub Main()
        AddHandler Server.OnClientConnect, AddressOf Server_OnClientConnect
        AddHandler Server.OnClientDisconnect, AddressOf Server_OnClientDisconnect
        AddHandler Server.OnReceiveData, AddressOf Server_OnReceiveData
        AddHandler Server.OnServerError, AddressOf Server_OnServerError
        AddHandler Server.OnServerStarted, AddressOf Server_OnServerStarted
        AddHandler Server.OnServerStopped, AddressOf Server_OnServerStopped
        AddHandler Server.OnServerLog, AddressOf Server_OnServerLog
        AddHandler Server.OnServerPacketLog, AddressOf Server_OnPacketLog

        AddHandler Database.OnDatabaseError, AddressOf db_OnDatabaseError
        AddHandler Database.OnDatabaseConnected, AddressOf db_OnConnectedToDatabase
        AddHandler Database.OnDatabaseLog, AddressOf Program.db_OnDatabaseLog

        AddHandler GlobalManagerCon.OnGlobalManagerInit, AddressOf Program.gmc_OnGlobalManagerInit
        AddHandler GlobalManagerCon.OnGlobalManagerShutdown, AddressOf Program.gmc_OnGlobalManagerInit
        AddHandler GlobalManagerCon.OnError, AddressOf Program.gmc_OnGlobalManagerError
        AddHandler GlobalManagerCon.OnLog, AddressOf Program.gmc_OnGlobalManagerLog
        AddHandler GlobalManagerCon.OnPacketReceived, AddressOf Functions.Parser.ParseGlobalManager
        AddHandler GlobalManagerCon.OnGameserverUserauthReply, AddressOf Functions.Check_GlobalManagerUserAuthReply

        Console.WindowHeight = 20
        Console.BufferHeight = 50
        Console.WindowWidth = 70
        Console.BufferWidth = 70
        Console.BackgroundColor = ConsoleColor.White
        Console.ForegroundColor = ConsoleColor.DarkGreen
        Console.Clear()
        Console.Title = "GAMESERVER ALPHA"

        Log.WriteSystemLog("Loading Settings.")
        Settings.LoadSettings()
        Settings.SetToServer()

        Log.WriteSystemLog("Connecting Database.")
        Database.Connect()
        Log.WriteSystemLog("Connected Database. Loading Data now.")

        Functions.GlobalGame.GlobalInit(Server.MaxClients)
        GlobalDef.Initalize(Server.MaxClients)

        SilkroadData.DumpDataFiles()
        GameDB.UpdateData()
        Functions.Timers.LoadTimers(Server.MaxClients)
        GameMod.Damage.OnServerStart(Server.MaxClients)

        Log.WriteSystemLog("Inital Loading complete! Waiting for Globalmanager...")
        Log.WriteSystemLog("Slotcount: " & Settings.Server_NormalSlots & "/" & Settings.Server_MaxClients)

        GlobalManagerCon.Connect(Settings.GlobalManger_Ip, Settings.GlobalManger_Port)

        Do While True
            Dim msg As String = Console.ReadLine()
            CheckCommand(msg)
            Thread.Sleep(10)
        Loop
    End Sub

    Private Shared Sub Server_OnClientConnect(ByVal ip As String, ByVal index As Integer)
        If Settings.Log_Detail Then
            Log.WriteSystemLog(String.Format("Client[{0}/{1}] Connected: {2}", Server.OnlineClients, Server.MaxNormalClients, ip))
        End If

        Server.OnlineClients += 1

        SessionInfo(index) = New cSessionInfo_GameServer
        SessionInfo(index).LoginAuthRequired = True
        SessionInfo(index).LoginAuthTimeout = Date.Now.AddSeconds(40)

        Dim packet As New PacketWriter
        packet.Create(ServerOpcodes.HANDSHAKE)
        packet.Byte(1)
        Server.Send(packet.GetBytes, index)
    End Sub

    Private Shared Sub Server_OnClientDisconnect(ByVal ip As String, ByVal index As Integer)
        Try
            Server.OnlineClients -= 1
            If Functions.PlayerData(index) IsNot Nothing Then
                Functions.DespawnPlayer(index)
                Functions.CleanUpPlayerComplete(index)
            End If
        Catch ex As Exception
            Log.WriteSystemLog("Client disconnect error! " & ex.Message & " Stack: " & ex.StackTrace & " Index: " & index)
        Finally
            Functions.CharListing(index) = Nothing
            Functions.PlayerData(index) = Nothing
            SessionInfo(index) = Nothing
        End Try
    End Sub

    Private Shared Sub Server_OnReceiveData(ByVal buffer() As Byte, ByVal index_ As Integer)

        Dim Position As Integer = 0

        Do While True
            Dim length As Integer = BitConverter.ToUInt16(buffer, Position)
            Dim opc As Integer = BitConverter.ToUInt16(buffer, Position + 2)

            If length = 0 And opc = 0 Then 'endless prevention
                Exit Do
            End If

            Dim newbuff(length + 5) As Byte
            Array.ConstrainedCopy(buffer, Position, newbuff, 0, length + 6)
            Position = Position + length + 6

            Dim packet As New PacketReader(newbuff)
            If Settings.Server_DebugMode = True Then
                Log.LogPacket(newbuff, False)
            End If

            Functions.Parser.Parse(packet, index_)
        Loop
    End Sub

    Private Shared Sub Server_OnServerError(ByVal ex As Exception, ByVal index As Integer)
        Log.WriteSystemLog("Server Error: " & ex.Message & " Stack: " & ex.StackTrace & " Index: " & index)
        '-1 = on client connect + -2 = on server start
    End Sub

    Private Shared Sub Server_OnServerStarted(ByVal time As String)
        Log.WriteSystemLog("Server Started: " & time)
    End Sub

    Private Shared Sub Server_OnServerStopped(ByVal time As String)
        Log.WriteSystemLog("Server Stopped: " & time)
    End Sub

    Private Shared Sub Server_OnServerLog(ByVal message As String)
        Log.WriteSystemLog("Server Log: " & message)
    End Sub

    Private Shared Sub Server_OnPacketLog(ByVal buff() As Byte, ByVal fromserver As Boolean, ByVal index As Integer)
        Log.LogPacket(buff, fromserver)
    End Sub

    Private Shared Sub db_OnConnectedToDatabase()
        Log.WriteSystemLog("Connected to database at: " & DateTime.Now.ToString())
    End Sub

    Private Shared Sub db_OnDatabaseLog(ByVal message As String)
        Log.WriteSystemLog("Database Log: " & message)
    End Sub

    Private Shared Sub db_OnDatabaseError(ByVal ex As Exception, ByVal command As String)
        Log.WriteSystemLog("Database error: " & ex.Message & " Command: " & command)
    End Sub

    Private Shared Sub gmc_OnGlobalManagerInit()
        Server.Start()
        Log.WriteSystemLog("GMC: We are ready!")
    End Sub

    Private Shared Sub gmc_OnGlobalManagerShutdown()
        For i = 0 To SessionInfo.Count - 1
            If SessionInfo(i) IsNot Nothing Then
                Server.Disconnect(i)
            End If
        Next
        Server.Stop()
        Database.ExecuteQuerys()

        Log.WriteSystemLog("Server stopped, Data is save. Feel free to close!")
    End Sub

    Private Shared Sub gmc_OnGlobalManagerLog(ByVal message As String)
        Log.WriteSystemLog("GMC Log: " & message)
    End Sub

    Private Shared Sub gmc_OnGlobalManagerError(ByVal ex As Exception, ByVal command As String)
        Log.WriteSystemLog("GMC error: " & ex.Message & " Command: " & command)
    End Sub
End Class



