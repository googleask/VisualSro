﻿Namespace GameServer.Functions
    Module Players

        'Players
        Public PlayerData(5000) As [cChar]
        'items
        Public Inventorys(5000) As cInventory
        'Exchange
        Public ExchangeData As New List(Of cExchange)


        ''' <summary>
        ''' For Players
        ''' </summary>
        ''' <param name="Index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateSpawnPacket(ByVal Index As Integer) As Byte()

            Dim chari As [cChar] = PlayerData(Index) 'Only for faster Code writing

            Dim writer As New PacketWriter
            writer.Create(ServerOpcodes.SingleSpawn)
            writer.DWord(chari.Model)
            writer.Byte(chari.Volume)
            writer.Byte(0)
            writer.Byte(chari.HelperIcon)

            'items
            Dim inventory As New cInventory(chari.MaxSlots)
            inventory = GameServer.GameDB.FillInventory(chari)

            Dim PlayerItemCount As Integer = 0
            For b = 0 To 9
                If inventory.UserItems(b).Pk2Id <> 0 And inventory.UserItems(b).Pk2Id <> 62 Then
                    PlayerItemCount += 1
                End If
            Next

            writer.Byte(chari.MaxSlots)
            writer.Byte(PlayerItemCount)

            For b = 0 To 9
                If inventory.UserItems(b).Pk2Id <> 0 Then
                    If inventory.UserItems(b).Pk2Id <> 62 Then 'Dont send arrows
                        writer.DWord(inventory.UserItems(b).Pk2Id)
                        writer.Byte(inventory.UserItems(b).Plus)
                    End If

                End If
            Next

            writer.Byte(4) 'avatar start
            writer.Byte(0) '0 avatars fro now
            writer.Byte(0)

            writer.DWord(chari.UniqueId)
            writer.Byte(chari.Position.XSector)
            writer.Byte(chari.Position.YSector)
            writer.Float(chari.Position.X)
            writer.Float(chari.Position.Z)
            writer.Float(chari.Position.Y)

            writer.Word(chari.Angle)
            writer.Byte(0) 'dest
            writer.Byte(chari.MovementType) 'walk run flag

            writer.Byte(0) 'dest
            writer.Word(chari.Angle)

            writer.Byte(chari.Alive) ' death flag
            writer.Byte(chari.ActionFlag) 'action flag
            writer.Byte(chari.Berserk) 'berserk activated
            writer.Float(chari.WalkSpeed)
            writer.Float(chari.RunSpeed)
            writer.Float(chari.BerserkSpeed)

            writer.Byte(0) 'no buffs for now

            writer.Word(chari.CharacterName.Length)
            writer.String(chari.CharacterName)

            writer.Byte(0) 'job lvl
            writer.Byte(1) 'job type
            writer.DWord(0) 'trader exp
            writer.DWord(0) 'theif exp
            writer.DWord(0) 'hunter exp
            writer.DWord(0)
            writer.DWord(0)
            writer.DWord(0)
            writer.DWord(0)
            writer.Byte(0)
            writer.Byte(chari.PVP)
            writer.Byte(1)

            Return writer.GetBytes
        End Function


        Public Function CreateDespawnPacket(ByVal UniqueID As Integer) As Byte()
            Dim writer As New PacketWriter
            writer.Create(ServerOpcodes.SingleDespawn)
            writer.DWord(UniqueID)
            Return writer.GetBytes
        End Function

        Public Sub DespawnPlayerTeleport(ByVal Index_ As Integer)

            For i = 0 To PlayerData(Index_).SpawnedPlayers.Count - 1
                Dim Other_Index As Integer = PlayerData(Index_).SpawnedPlayers(i)
                If PlayerData(Other_Index) IsNot Nothing And PlayerData(Other_Index).SpawnedPlayers.Contains(Index_) = True Then
                    Server.Send(CreateDespawnPacket(PlayerData(Index_).UniqueId), Other_Index)
                    PlayerData(Other_Index).SpawnedPlayers.Remove(Index_)

                    Server.Send(CreateDespawnPacket(PlayerData(Other_Index).UniqueId), Index_)
                End If
            Next

            For i = 0 To NpcList.Count - 1
                If PlayerData(Index_).SpawnedNPCs.Contains(i) = True Then
                    Dim _npc As cNPC = NpcList(i)
                    Server.Send(CreateDespawnPacket(_npc.UniqueID), Index_)
                    PlayerData(Index_).SpawnedNPCs.Remove(i)
                End If
            Next


            For i = 0 To MobList.Count - 1
                If PlayerData(Index_).SpawnedMonsters.Contains(i) = True Then
                    Dim _mob As cMonster = MobList(i)
                    Server.Send(CreateDespawnPacket(_mob.UniqueID), Index_)
                    PlayerData(Index_).SpawnedMonsters.Remove(i)
                End If
            Next


            For i = 0 To ItemList.Count - 1
                If PlayerData(Index_).SpawnedItems.Contains(i) = True Then
                    Dim _item As cItemDrop = ItemList(i)
                    Server.Send(CreateDespawnPacket(_item.UniqueID), Index_)
                    PlayerData(Index_).SpawnedItems.Remove(i)
                End If
            Next

            CleanUpPlayer(Index_)
        End Sub

        Public Sub DespawnPlayer(ByVal Index_ As Integer)
            For i = 0 To PlayerData(Index_).SpawnedPlayers.Count - 1
                Dim Other_Index As Integer = PlayerData(Index_).SpawnedPlayers(i)
                If PlayerData(Other_Index) IsNot Nothing Then
                    If PlayerData(Other_Index).SpawnedPlayers.Contains(Index_) = True Then
                        Server.Send(CreateDespawnPacket(PlayerData(Index_).UniqueId), Other_Index)
                        PlayerData(Other_Index).SpawnedPlayers.Remove(Index_)
                    End If
                End If

            Next

            PlayerData(Index_).SpawnedPlayers.Clear()
        End Sub

        Public Sub CleanUpPlayer(ByVal Index_ As Integer)
            'Cleanup
            PlayerData(Index_).SpawnedPlayers.Clear()
            PlayerData(Index_).SpawnedNPCs.Clear()
            PlayerData(Index_).SpawnedMonsters.Clear()
            PlayerData(Index_).SpawnedItems.Clear()

            PlayerData(Index_).Busy = False
            PlayerData(Index_).Alive = True
            PlayerData(Index_).Attacking = False
            PlayerData(Index_).AttackedId = 0
            PlayerData(Index_).UsingSkillId = 0
            PlayerData(Index_).AttackType = AttackType_.Normal
            PlayerData(Index_).LastSelected = 0
            PlayerData(Index_).UsedItem = UseItemTypes.None
            PlayerData(Index_).UsedItemParameter = 0
            PlayerData(Index_).PickUpId = 0

            PlayerData(Index_).InExchange = False
            PlayerData(Index_).ExchangeID = 0

            If PlayerData(Index_).InStall = True Then

            End If

            PlayerData(Index_).InStall = False
            PlayerData(Index_).StallID = 0


        End Sub

        Public Sub CheckStall(ByVal Index_ As Integer)
            For i = 0 To Stalls.Count - 1
                If Stalls(i).OwnerID = PlayerData(Index_).UniqueId Then
                    Stall_Close_Own(Index_)
                End If
            Next
        End Sub
    End Module
End Namespace
