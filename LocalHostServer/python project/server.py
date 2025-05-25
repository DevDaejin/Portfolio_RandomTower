import asyncio
import websockets
from connection_handler import handle_connection

connected = set()
addr = "127.0.0.1"
port = 8765

async def main():
    async with websockets.serve(handle_connection, addr, port):
        print(f"[Server] Running on ws://{addr}:{port}")
        await asyncio.Future()


if __name__ == "__main__":
    asyncio.run(main())
