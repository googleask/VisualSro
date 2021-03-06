﻿Public Class GameServer
    Inherits cGM_Server_Base

    Public ServerName As String
    Public State As _ServerState = _ServerState.Check

    'General Server Data
    Public MobCount As UInt32 = 0
    Public ItemCount As UInt32 = 0
    Public NpcCount As UInt32 = 0
    'Statistic Data (Todo??)
    Public AllItemsCount As UInt32 = 0
    Public AllPlayersCount As UInt32 = 0
    Public AllSkillsCount As UInt32 = 0
    'Settings
    Public Server_XPRate As Long = 1
    Public Server_SPRate As Long = 1
    Public Server_GoldRate As Long = 1
    Public Server_DropRate As Long = 1
    Public Server_SpawnRate As Long = 1
    Public Server_Debug As Boolean = False


    Enum _ServerState
        Check = 0
        Online = 1
    End Enum
End Class