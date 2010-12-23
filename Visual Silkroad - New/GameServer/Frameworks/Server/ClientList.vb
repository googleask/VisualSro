﻿Imports Microsoft.VisualBasic
Imports System, System.Net.Sockets, System.Timers
Namespace GameServer

    Public Module ClientList
        Public List(1500) As Socket
        Public LastPingTime(1500) As DateTime
        Public OnCharListing(1500) As cCharListing
        Public WithEvents PingTimer As New Timer

        Public Sub Add(ByVal sock As Socket)
            For i As Integer = 0 To List.Length - 1
                If List(i) Is Nothing Then
                    List(i) = sock
                    Return
                End If
            Next i
        End Sub

        Public Sub Delete(ByVal index As Integer)
            If List(index) IsNot Nothing Then
                List(index) = Nothing
            End If
        End Sub

        Public Function FindIndex(ByVal sock As Socket) As Integer
            For i As Integer = 0 To List.Length - 1
                If sock Is List(i) Then
                    Return i
                End If
            Next i
            Return -1
        End Function

        Public Function GetSocket(ByVal index As Integer) As Socket
            Dim socket As Socket = Nothing
            If (List(index) IsNot Nothing) AndAlso List(index).Connected Then
                socket = List(index)
            End If

            If socket Is Nothing Then
                Debug.Print(1)
            End If
            Return socket
        End Function

        Public Sub SetupClientList(ByVal MaxUser As Integer)
            ReDim List(MaxUser), LastPingTime(MaxUser)

            PingTimer.Interval = 60000
            PingTimer.Start()
        End Sub



        Public Sub CheckUserPings() Handles PingTimer.Elapsed
            PingTimer.Stop()
            Server.OnlineClient = 0

            For i = 0 To Server.MaxClients
                Dim socket As Socket = GetSocket(i)
                If socket IsNot Nothing Then
                    If DateDiff(DateInterval.Second, LastPingTime(i), DateTime.Now) > 30 Then
                        If socket.Connected = True Then
                            Server.Dissconnect(i)
                        End If
                    End If

                    Server.OnlineClient += 1
                End If
            Next
            PingTimer.Interval = 60000
            PingTimer.Start()
        End Sub
    End Module
End Namespace

