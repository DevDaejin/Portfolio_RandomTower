import asyncio
import websockets
import json
import uuid
from room_manager import RoomManager
from client_context import ClientContext

room_manager = RoomManager()
ADDR = "127.0.0.1"
PORT = 8765
client ={}

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
        room_manager.leave_room(context)
        del client[websocket]

async def handle_message(context, message):
    try:
        data = json.loads(message)
        message_type = data.get("type")

        if message_type == "create_room":
            await room_manager.create_room(context, data)
        
        elif message_type == "join_room":
            await room_manager.join_room(context, data)

        elif message_type == "leave_room":
            await room_manager.leave_room(context)

        elif message_type == "room_list":
            print("[main] room_list message received")
            await room_manager.list_rooms(context)
        
        elif message_type == "spawn_enemy":
            await room_manager.spawn_enemy(context, data)

        elif message_type == "spawn_tower":
            await room_manager.spawn_tower(context, data)

        elif message_type == "sync":
            await room_manager.relay_sync(context, data)

        else:
            print(f"[Warn] Unknown type: {message_type}")

    except Exception as e:
        print(f"[Error] {e}")

async def main():
    async with websockets.serve(handler, ADDR, PORT):
        print(f"WebSocket Server running on {ADDR}:{PORT}")
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())



