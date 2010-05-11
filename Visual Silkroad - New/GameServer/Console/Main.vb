﻿Imports Microsoft.VisualBasic
Imports System
Namespace GameServer

    Friend Class Program
        Private Shared Sub db_OnConnectedToDatabase()
            Console.WriteLine("Connected to database at: " & DateTime.Now.ToString())

        End Sub

        Private Shared Sub db_OnDatabaseError(ByVal ex As Exception)
            Console.WriteLine("Database error: " & ex.Message)
        End Sub

        Shared Sub Main()
            Dim password As String
            AddHandler Server.OnClientConnect, AddressOf Program.Server_OnClientConnect
            AddHandler Server.OnReceiveData, AddressOf Program.Server_OnReceiveData
            AddHandler Server.OnServerError, AddressOf Program.Server_OnServerError
            AddHandler Server.OnServerStarted, AddressOf Program.Server_OnServerStarted
            AddHandler DataBase.OnDatabaseError, AddressOf Program.db_OnDatabaseError
            AddHandler DataBase.OnConnectedToDatabase, AddressOf Program.db_OnConnectedToDatabase


            Console.WindowHeight = 10
            Console.BufferHeight = 30
            Console.WindowWidth = 60
            Console.BufferWidth = 60
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = ConsoleColor.DarkGreen
            Console.Clear()
            Console.Title = "GAMESERVER ALPHA"
            Console.WriteLine("Starting Agent Server")
            password = "sremu"
            DataBase.Connect("127.0.0.1", 3306, "visualsro", "root", password)
            Server.ip = "127.0.0.1"
            Server.port = 15780
            Server.MaxClients = 1500
            Server.OnlineClient = 0
            Server.Start()

            GameServer.DatabaseCore.GameDbUpdate.Interval = 1
            GameServer.DatabaseCore.GameDbUpdate.Start()


read:
            Dim msg As String = Console.ReadLine()
            CheckCommand(msg)
            GoTo read


        End Sub

        Private Shared Sub Server_OnClientConnect(ByVal ip As String, ByVal index As Integer)
            Console.WriteLine("Client Connected : " & ip)
            Server.Send(New Byte() {1, 0, 0, 80, 0, 0, 1}, index)
            Server.OnlineClient += 1
        End Sub

        Private Shared Sub Server_OnReceiveData(ByVal buffer() As Byte, ByVal index As Integer)
            Dim rp As New ReadPacket(buffer, index)
            Parser.Parse(rp)
        End Sub

        Private Shared Sub Server_OnServerError(ByVal ex As Exception)
            Console.WriteLine("Server Error: " & ex.Message)


        End Sub

        Private Shared Sub Server_OnServerStarted(ByVal time As String)
            Console.WriteLine("Server Started: " & time)
        End Sub
    End Class
End Namespace

