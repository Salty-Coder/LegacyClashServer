using System;
using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Network;
using UCS.PacketProcessing.Commands.Server;
using UCS.PacketProcessing.Messages.Server;

namespace UCS.PacketProcessing.Messages
{
	// Token: 0x020000C5 RID: 197
	internal class PromoteAllianceMemberMessage : Message
	{
		// Token: 0x06000936 RID: 2358 RVA: 0x0001597A File Offset: 0x00013B7A
		public PromoteAllianceMemberMessage(Client client, BinaryReader br) : base(client, br)
		{
			//Empty pack
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x0001C278 File Offset: 0x0001A478
		public override void Decode()
		{
			using (BinaryReader packetReader = new BinaryReader(new MemoryStream(base.GetData())))
			{
				this.m_vId = packetReader.ReadInt64WithEndian();
				this.m_vRole = packetReader.ReadInt32WithEndian();
			}
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0001C2CC File Offset: 0x0001A4CC
		public override void Process(Level level)
		{
			Level player = ResourcesManager.GetPlayer(this.m_vId, false);
			ClientAvatar playerAvatar = level.GetPlayerAvatar();
			Alliance alliance = ObjectManager.GetAlliance(playerAvatar.GetAllianceId());
			if ((playerAvatar.GetAllianceRole() == 2 || playerAvatar.GetAllianceRole() == 4) && playerAvatar.GetAllianceId() == player.GetPlayerAvatar().GetAllianceId())
			{
				int allianceRole = player.GetPlayerAvatar().GetAllianceRole();
				player.GetPlayerAvatar().SetAllianceRole(this.m_vRole);
				if (this.m_vRole == 2)
				{
					playerAvatar.SetAllianceRole(4);
					AllianceEventStreamEntry allianceEventStreamEntry = new AllianceEventStreamEntry();
					allianceEventStreamEntry.SetId(alliance.GetChatMessages().Count + 1);
					allianceEventStreamEntry.SetSender(playerAvatar);
					allianceEventStreamEntry.SetEventType(6);
					allianceEventStreamEntry.SetAvatarId(playerAvatar.GetId());
					allianceEventStreamEntry.SetAvatarName(playerAvatar.GetAvatarName());
					alliance.AddChatMessage(allianceEventStreamEntry);
					AllianceEventStreamEntry allianceEventStreamEntry2 = new AllianceEventStreamEntry();
					allianceEventStreamEntry2.SetId(alliance.GetChatMessages().Count + 1);
					allianceEventStreamEntry2.SetSender(player.GetPlayerAvatar());
					allianceEventStreamEntry2.SetEventType(5);
					allianceEventStreamEntry2.SetAvatarId(playerAvatar.GetId());
					allianceEventStreamEntry2.SetAvatarName(playerAvatar.GetAvatarName());
					alliance.AddChatMessage(allianceEventStreamEntry2);
					PromoteAllianceMemberOkMessage promoteAllianceMemberOkMessage = new PromoteAllianceMemberOkMessage(base.Client);
					PromoteAllianceMemberOkMessage promoteAllianceMemberOkMessage2 = new PromoteAllianceMemberOkMessage(player.GetClient());
					AllianceRoleUpdateCommand allianceRoleUpdateCommand = new AllianceRoleUpdateCommand();
					AvailableServerCommandMessage availableServerCommandMessage = new AvailableServerCommandMessage(base.Client);
					AllianceRoleUpdateCommand allianceRoleUpdateCommand2 = new AllianceRoleUpdateCommand();
					AvailableServerCommandMessage availableServerCommandMessage2 = new AvailableServerCommandMessage(player.GetClient());
					promoteAllianceMemberOkMessage.SetID(level.GetPlayerAvatar().GetId());
					promoteAllianceMemberOkMessage.SetRole(4);
					promoteAllianceMemberOkMessage2.SetID(player.GetPlayerAvatar().GetId());
					promoteAllianceMemberOkMessage2.SetRole(2);
					allianceRoleUpdateCommand2.SetAlliance(alliance);
					allianceRoleUpdateCommand.SetAlliance(alliance);
					allianceRoleUpdateCommand2.SetRole(2);
					allianceRoleUpdateCommand.SetRole(4);
					allianceRoleUpdateCommand2.Tick(player);
					allianceRoleUpdateCommand.Tick(level);
					availableServerCommandMessage2.SetCommandId(8);
					availableServerCommandMessage.SetCommandId(8);
					availableServerCommandMessage2.SetCommand(allianceRoleUpdateCommand2);
					availableServerCommandMessage.SetCommand(allianceRoleUpdateCommand);
					if (ResourcesManager.IsPlayerOnline(player))
					{
						PacketManager.ProcessOutgoingPacket(availableServerCommandMessage2);
						PacketManager.ProcessOutgoingPacket(promoteAllianceMemberOkMessage2);
					}
					PacketManager.ProcessOutgoingPacket(availableServerCommandMessage);
					PacketManager.ProcessOutgoingPacket(promoteAllianceMemberOkMessage);
					using (List<AllianceMemberEntry>.Enumerator enumerator = alliance.GetAllianceMembers().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AllianceMemberEntry allianceMemberEntry = enumerator.Current;
							Level player2 = ResourcesManager.GetPlayer(allianceMemberEntry.GetAvatarId(), false);
							if (player2.GetClient() != null)
							{
								AllianceStreamEntryMessage allianceStreamEntryMessage = new AllianceStreamEntryMessage(player2.GetClient());
								AllianceStreamEntryMessage allianceStreamEntryMessage2 = new AllianceStreamEntryMessage(player2.GetClient());
								allianceStreamEntryMessage.SetStreamEntry(allianceEventStreamEntry);
								allianceStreamEntryMessage2.SetStreamEntry(allianceEventStreamEntry2);
								PacketManager.ProcessOutgoingPacket(allianceStreamEntryMessage);
								PacketManager.ProcessOutgoingPacket(allianceStreamEntryMessage2);
							}
						}
						return;
					}
				}
				AllianceRoleUpdateCommand allianceRoleUpdateCommand3 = new AllianceRoleUpdateCommand();
				AvailableServerCommandMessage availableServerCommandMessage3 = new AvailableServerCommandMessage(player.GetClient());
				PromoteAllianceMemberOkMessage promoteAllianceMemberOkMessage3 = new PromoteAllianceMemberOkMessage(player.GetClient());
				AllianceEventStreamEntry allianceEventStreamEntry3 = new AllianceEventStreamEntry();
				allianceEventStreamEntry3.SetId(alliance.GetChatMessages().Count + 1);
				allianceEventStreamEntry3.SetSender(player.GetPlayerAvatar());
				allianceEventStreamEntry3.SetAvatarId(playerAvatar.GetId());
				allianceEventStreamEntry3.SetAvatarName(playerAvatar.GetAvatarName());
				if (this.m_vRole > allianceRole)
				{
					allianceEventStreamEntry3.SetEventType(5);
				}
				else
				{
					allianceEventStreamEntry3.SetEventType(6);
				}
				allianceRoleUpdateCommand3.SetAlliance(alliance);
				allianceRoleUpdateCommand3.SetRole(this.m_vRole);
				allianceRoleUpdateCommand3.Tick(player);
				availableServerCommandMessage3.SetCommandId(8);
				availableServerCommandMessage3.SetCommand(allianceRoleUpdateCommand3);
				promoteAllianceMemberOkMessage3.SetID(player.GetPlayerAvatar().GetId());
				promoteAllianceMemberOkMessage3.SetRole(this.m_vRole);
				alliance.AddChatMessage(allianceEventStreamEntry3);
				if (ResourcesManager.IsPlayerOnline(player))
				{
					PacketManager.ProcessOutgoingPacket(availableServerCommandMessage3);
					PacketManager.ProcessOutgoingPacket(promoteAllianceMemberOkMessage3);
				}
				foreach (AllianceMemberEntry allianceMemberEntry2 in alliance.GetAllianceMembers())
				{
					Level player3 = ResourcesManager.GetPlayer(allianceMemberEntry2.GetAvatarId(), false);
					if (player3.GetClient() != null)
					{
						AllianceStreamEntryMessage allianceStreamEntryMessage3 = new AllianceStreamEntryMessage(player3.GetClient());
						allianceStreamEntryMessage3.SetStreamEntry(allianceEventStreamEntry3);
						PacketManager.ProcessOutgoingPacket(allianceStreamEntryMessage3);
					}
				}
			}
		}

		// Token: 0x0400053E RID: 1342
		public long m_vId;

		// Token: 0x0400053F RID: 1343
		public int m_vRole;
	}
}
