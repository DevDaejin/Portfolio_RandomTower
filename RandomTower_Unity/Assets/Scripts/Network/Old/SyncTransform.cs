//using System.IO;
//using UnityEngine;

//public class SyncTransform : MonoBehaviour, ISyncable
//{
//    [SerializeField] private bool syncPosition;
//    [SerializeField] private bool syncRotation;
//    [SerializeField] private bool syncScale;

//    private Vector3 _lastPosition;
//    private Quaternion _lastRotation;
//    private Vector3 _lastScale;

//    private void Awake()
//    {
//        GetComponent<SyncView>()?.Register(this);
//        if (syncPosition) _lastPosition = transform.position;
//        if (syncRotation) _lastRotation = transform.rotation;
//        if (syncScale) _lastScale = transform.localScale;
//    }

//    public bool IsDirty
//    {
//        get => (transform.position != _lastPosition ||
//                transform.rotation != _lastRotation ||
//                transform.localScale != _lastScale);
//    }

//    public void CollectData(BinaryWriter writer)
//    {
//        if (syncPosition)
//        {
//            writer.Write(transform.position.x);
//            writer.Write(transform.position.y);
//            writer.Write(transform.position.z);
//        }

//        if (syncRotation)
//        {
//            writer.Write(transform.rotation.x);
//            writer.Write(transform.rotation.y);
//            writer.Write(transform.rotation.z);
//            writer.Write(transform.rotation.w);
//        }

//        if (syncScale)
//        {
//            writer.Write(transform.localScale.x);
//            writer.Write(transform.localScale.y);
//            writer.Write(transform.localScale.z);
//        }
//    }

//    public void ApplyData(BinaryReader reader)
//    {
//        if (syncPosition)
//        {
//            _lastPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
//            transform.position = _lastPosition;
//        }

//        if (syncRotation)
//        {
//            _lastRotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
//            transform.rotation = _lastRotation;
//        }

//        if (syncScale)
//        {
//            _lastScale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
//            transform.localScale = _lastScale;
//        }
//    }

//    public void ResetDirty()
//    {
//        _lastPosition = transform.position;
//        _lastRotation = transform.rotation;
//        _lastScale = transform.localScale;
//    }
//}
