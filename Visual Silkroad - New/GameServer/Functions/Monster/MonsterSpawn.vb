﻿Namespace GameServer.Functions
    Module MonsterSpawn

        Public MobList As New List(Of cMonster)

        Public Function CreateMonsterSpawnPacket(ByVal _mob As cMonster, ByVal obj As Object_) As Byte()
            Dim writer As New PacketWriter
            writer.Create(ServerOpcodes.SingleSpawn)
            writer.DWord(_mob.Pk2ID)
            writer.DWord(_mob.UniqueID)
            writer.Byte(_mob.Position.XSector)
            writer.Byte(_mob.Position.YSector)
            writer.Float(_mob.Position.X)
            writer.Float(_mob.Position.Z)
            writer.Float(_mob.Position.Y)

            writer.Word(_mob.Angle)
            If _mob.Walking = False Then
                writer.Byte(0) 'dest
                writer.Byte(1) 'walk run flag
                writer.Byte(0) 'dest   
                writer.Word(_mob.Angle)
            Else
                writer.Byte(1) 'dest
                writer.Byte(1) 'walk run flag
                writer.Byte(_mob.Position_ToPos.XSector)
                writer.Byte(_mob.Position_ToPos.YSector)
                writer.Word(_mob.Position_ToPos.X)
                writer.Byte(BitConverter.GetBytes(CShort(_mob.Position_ToPos.Z)))
                writer.Word(_mob.Position_ToPos.Y)
            End If

            writer.Byte(0) 'unknown
            writer.Byte(0) 'death flag
            writer.Byte(0) 'berserker

            writer.Float(obj.WalkSpeed) 'walkspeed
            writer.Float(obj.RunSpeed) 'runspeed
            writer.Float(obj.BerserkSpeed) 'berserkerspeed

            writer.Word(0) 'unknwown  
            writer.Byte(_mob.Mob_Type)
            writer.Byte(0) 'mhm

            Return writer.GetBytes
        End Function

        Public Sub SpawnMob(ByVal MobID As UInteger, ByVal Type As Byte, ByVal Position As Position, ByVal Angle As UInteger, ByVal SpotID As Long)
            Dim mob_ As Object_ = GetObjectById(MobID)
            Dim tmp As New cMonster
            tmp.UniqueID = GameDB.GetUnqiueID
            tmp.Pk2ID = mob_.Pk2ID
            tmp.Position = Position
            tmp.Position_Spawn = Position
            tmp.SpotID = SpotID
            tmp.Mob_Type = Type
            tmp.HP_Cur = mob_.Hp


            Select Case Type
                Case 0
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Normal
                Case 1
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Champion
                Case 3
                    SendUniqueSpawn(MobID)
                Case 4
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Giant
                Case 6
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Elite
                Case 16
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Party_Normal
                Case 17
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Party_Champ
                Case 20
                    tmp.HP_Cur = mob_.Hp * MobMultiplierHP.Party_Giant
            End Select

            tmp.HP_Max = tmp.HP_Cur
            MobList.Add(tmp)

            Dim MyIndex As UInteger = MobList.IndexOf(tmp)
            Dim range As Integer = Settings.ServerRange

            For refindex As Integer = 0 To Server.MaxClients
                Dim socket As Net.Sockets.Socket = ClientList.GetSocket(refindex)
                Dim player As [cChar] = PlayerData(refindex) 'Check if Player is ingame
                If (socket IsNot Nothing) AndAlso (player IsNot Nothing) AndAlso socket.Connected Then
                    If CheckRange(player.Position, Position) Then
                        If PlayerData(refindex).SpawnedNPCs.Contains(tmp.UniqueID) = False Then
                            Server.Send(CreateMonsterSpawnPacket(tmp, mob_), refindex)
                            PlayerData(refindex).SpawnedMonsters.Add(tmp.UniqueID)
                        End If
                    End If
                End If
            Next refindex
        End Sub

        Public Sub RemoveMob(ByVal MobIndex As Integer)
            Dim _mob As cMonster = MobList(MobIndex)
            Server.SendIfMobIsSpawned(CreateDespawnPacket(_mob.UniqueID), _mob.UniqueID)
            MobList.RemoveAt(MobIndex)


            For i = 0 To Server.MaxClients
                If PlayerData(i) IsNot Nothing Then
                    If PlayerData(i).SpawnedMonsters.Contains(_mob.UniqueID) = True Then
                        PlayerData(i).SpawnedMonsters.Remove(_mob.UniqueID)
                    End If
                End If
            Next

        End Sub

        Public Sub SendUniqueSpawn(ByVal PK2ID As UInteger)
            Dim writer As New PacketWriter
            writer.Create(ServerOpcodes.UniqueAnnonce)
            writer.Byte(5) 'Spawn
            writer.Byte(&HC)
            writer.DWord(PK2ID)
            Server.SendToAllIngame(writer.GetBytes)
        End Sub

        Public Sub SendUniqueKill(ByVal PK2ID As UInteger, ByVal KillName As String)
            Dim writer As New PacketWriter
            writer.Create(ServerOpcodes.UniqueAnnonce)
            writer.Byte(6) 'Kill
            writer.Byte(&HC)
            writer.DWord(PK2ID)
            writer.Word(KillName.Length)
            writer.String(KillName)
            Server.SendToAllIngame(writer.GetBytes)
        End Sub

        Enum MobMultiplierHP
            Normal = 1
            Champion = 2
            Unique = 2
            Giant = 20
            Elite = 10
            Party_Normal = 10
            Party_Champ = 20
            Party_Giant = 200
        End Enum

        Enum MobMultiplierExp
            Normal = 1
            Champion = 2
            Unique = 7
            Giant = 3
            Elite = 4
            Party_Normal = 5
            Party_Champ = 6
            Party_Giant = 8
        End Enum
    End Module
End Namespace
