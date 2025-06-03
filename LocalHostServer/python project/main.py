# main.py

import asyncio
import websockets
import uuid

from client_context import ClientContext
from room_manager import RoomManager
from spawn_manager import SpawnManager
from sync_manager import SyncManager

from proto.net_pb2 import SyncPacketData
from proto.net_pb2 import Envelope
from proto.room_pb2 import RoomPacket
from proto.spawn_pb2 import SpawnEnemyPacket

room_manager = RoomManager()
spawn_manager = SpawnManager(room_manager)
sync_manager = SyncManager(room_manager)

ADDR = "127.0.0.1"
PORT = 8765
client = {}

async def handler(websocket):
    client_id = str(uuid.uuid4())
    context = ClientContext(client_id, websocket)
    client[websocket] = context
    print(f"[Connect] {client_id}")
    try:
        async for message in websocket:
            await handle_message(context, message)
    except websockets.exceptions.ConnectionClosed:
        print(f"[Disconnect] {client_id}")
    finally:
        await room_manager.leave_room(context)
        del client[websocket]

async def handle_message(context, message: bytes):
    try:
        envelope = Envelope()
        envelope.ParseFromString(message)

        if envelope.type == "room":
            room_packet = RoomPacket()
            room_packet.ParseFromString(envelope.payload)
            await handle_room_packet(context, room_packet)

        elif envelope.type == "sync":
            sync_packet = SyncPacketData()
            sync_packet.ParseFromString(envelope.payload)
            await sync_manager.handle_sync(context, sync_packet.sync_type, sync_packet.payload)

        elif envelope.type == "spawn_enemy":
            await spawn_manager.handle_spawn_enemy(context, envelope.payload)

        elif envelope.type == "spawn_tower":
            await spawn_manager.handle_spawn_tower(context, envelope.payload)

        elif envelope.type == "spawn_projectile":
            await spawn_manager.handle_spawn_projectile(context, envelope.payload)

        else:
            print(f"[Warn] Unknown envelope type: {envelope.type}")
    except Exception as e:
        print(f"[Error] Failed to parse envelope: {e}")

async def handle_room_packet(context, packet: RoomPacket):
    if packet.HasField("create_room"):
        print(f"[Room] Client {context.client_id} requested room creation with name: {packet.create_room.name}")
        await room_manager.create_room(context, packet.create_room)
    elif packet.HasField("join_room"):
        print(f"[Room] Client {context.client_id} requested to join room: {packet.join_room.room_id}")
        await room_manager.join_room(context, packet.join_room)
    elif packet.HasField("leave_room"):
        print(f"[Room] Client {context.client_id} requested to leave room")
        await room_manager.leave_room(context)
    elif packet.HasField("list_room"):
        print(f"[Room] Client {context.client_id} requested room list")
        await room_manager.list_rooms(context)
    else:
        print("[Warn] Unknown RoomPacket field")

async def main():
    async with websockets.serve(handler, ADDR, PORT):
        print(f"WebSocket Server running on {ADDR}:{PORT}")
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())
