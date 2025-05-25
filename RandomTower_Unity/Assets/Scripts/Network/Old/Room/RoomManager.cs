//using System.Collections.Generic;
//using UnityEngine;

//public class RoomManager : MonoBehaviour
//{
//    [SerializeField] private string roomName = "DefaultRoom";

//    private Dictionary<int, RoomInfo> _roomList = new();

//    public void CreateRoom()
//    {
//        int id = 100;
//        RoomInfo info = new(id, roomName);

//        SyncView view = gameObject.AddComponent<SyncView>();
//        view.SetOwnership(true);
//        view.Register(info);

//        Debug.Log($"[RoomManager] 방 생성 완료 - ID: {id}, 이름: {roomName}");
//    }

//    public void JoinRoom(int id)
//    {
//        SyncView view = gameObject.AddComponent<SyncView>();
//        view.SetOwnership(false);
//        view.GetType()
//            .GetField("syncID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//            .SetValue(view, id);

//        RoomInfo info = new(id, "");
//        view.Register(info);

//        Debug.Log($"[RoomManager] 방 참가 요청 - ID: {id}");
//    }

//    public void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            CreateRoom();
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            JoinRoom(100);
//        }
//    }
//}
