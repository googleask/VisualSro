﻿Module Commands

    Public Sub CheckCommand(ByVal msg As String)

        Select Case msg

            Case "/info"
                GlobalManager.Log.WriteSystemLog("This Emulator is from GoneUp. ")
                GlobalManager.Log.WriteSystemLog("Specical Thanks to:")
                GlobalManager.Log.WriteSystemLog("Drew Benton")
                GlobalManager.Log.WriteSystemLog("manneke for the great help")
                GlobalManager.Log.WriteSystemLog("Windrius for the Framework.")
                GlobalManager.Log.WriteSystemLog("SREmu Team")
                GlobalManager.Log.WriteSystemLog("Dickernoob for CSREmu")
                GlobalManager.Log.WriteSystemLog("Cheat-Project Germany [cp-g.net] <-- Best Forum ever")


            Case "/help"
                GlobalManager.Log.WriteSystemLog("Commands: ")
                GlobalManager.Log.WriteSystemLog("/info for the credits")
                GlobalManager.Log.WriteSystemLog("/packets to enable packetGlobalManager.Log")
                GlobalManager.Log.WriteSystemLog("/clear")

            Case "/packets"

                GlobalManager.Settings.Server_DebugMode = True
                GlobalManager.Log.WriteSystemLog("PacketLog started!")

            Case "/debug"
                If Settings.Server_DebugMode Then
                    Settings.Server_DebugMode = False
                    Log.WriteSystemLog("Turned off DebugMode")

                ElseIf Settings.Server_DebugMode = False Then
                    Settings.Server_DebugMode = True
                    Log.WriteSystemLog("Turned on DebugMode")
                End If

            Case "/clear"
                Console.Clear()


            Case "/end"
                For i = 0 To SessionInfo.Count - 1
                    If SessionInfo(i) IsNot Nothing Then
                        Server.Disconnect(i)
                    End If
                Next
                ' GameServer.Server.stop()
                DataBase.ExecuteQuerys()
                End

        End Select



    End Sub
End Module
