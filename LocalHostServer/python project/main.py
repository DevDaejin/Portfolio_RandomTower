import asyncio
import websockets
import json
import uuid
from room_manager import RoomManager
from client_context import ClientContext

room_manager = RoomManager()
ADDR = "127.0.0.1"
PORT = 8765
client = {}

handlers = {
    "create_room": lambda c, d: room_manager.create_room(c, d),
    "join_room": lambda c, d: room_manager.join_room(c, d),
    "leave_room": lambda c, d: room_manager.leave_room(c),
    "room_list": lambda c, d: room_manager.list_rooms(c),
    "sync": lambda c, d: room_manager.relay_sync(c, d),
    "spawn_enemy": lambda c, d: room_manager.spawn_object(c, d, "enemy", "enemy_id"),
    "spawn_tower": lambda c, d: room_manager.spawn_object(c, d, "tower", "tower_id"),
    "spawn_projectile": lambda c, d: room_manager.spawn_object(c, d, "projectile", "projectile_id"),
}

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

async def handle_message(context, message):
    try:
        data = json.loads(message)
        message_type = data.get("type")
        handler = handlers.get(message_type)

        if handler:
            await handler(context, data)
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
