# main.py

import asyncio
import datetime
import websockets
import uuid
import sys

from client_context import ClientContext
from room_manager import RoomManager
from spawn_manager import SpawnManager
from sync_manager import SyncManager
from game_state_manager import GameStateManager
from despawn_manager import DespawnManager

from proto.net_pb2 import SyncPacketData, Envelope
from proto.spawn_pb2 import SpawnPacketData
from proto.despawn_pb2 import DespawnPacketData
from proto.game_pb2 import GameStatePacket
from proto.room_pb2 import RoomPacket

def get_server_config():
    if len(sys.argv) >= 3:
        ip = sys.argv[1]
        port = int(sys.argv[2])
    else:
        ip = input("IP Address (default 0.0.0.0): ") or "0.0.0.0"
        port_input = input("Port (default 6112): ") or "6112"
        port = int(port_input)
    return ip, port

room_manager = RoomManager()
spawn_manager = SpawnManager(room_manager)
despawn_manager = DespawnManager(room_manager)
sync_manager = SyncManager(room_manager)
game_state_manager = GameStateManager(room_manager)

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

def dump_raw_message(data: bytes, label: str):
    now = datetime.datetime.now().strftime("%Y%m%d_%H%M%S_%f")
    with open(f"error_dump_{label}_{now}.bin", "wb") as f:
        f.write(data)
    print(f"[ErrorDump] Raw data written to error_dump_{label}_{now}.bin")

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
            await sync_manager.handle_sync(context, sync_packet)
        
        elif envelope.type == "spawn":
            spawn_packet = SpawnPacketData()
            spawn_packet.ParseFromString(envelope.payload)
            await spawn_manager.handle_spawn(context, spawn_packet)

        elif envelope.type == "despawn":
            despawn_packet = DespawnPacketData()
            despawn_packet.ParseFromString(envelope.payload)
            await despawn_manager.handle_despawn(context, despawn_packet)

        elif envelope.type == "game_state":
            packet = GameStatePacket()
            packet.ParseFromString(envelope.payload)
            await game_state_manager.handle_game_state(context, packet)

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
    ip, port = get_server_config()
    async with websockets.serve(handler, ip, port):
        print(f"WebSocket Server running on {ip}:{port}")
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())
