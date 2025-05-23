using System.IO;
using UnityEngine;

public class SyncTransform : MonoBehaviour, ISyncable
{
    [SerializeField] private readonly bool setSyncPosition;
    [SerializeField] private readonly bool setSyncRotation;
    [SerializeField] private readonly bool setSyncScale;

    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private Vector3 _lastScale;

    public bool IsDirty
    {
        get => (transform.position != _lastPosition ||
                transform.rotation != _lastRotation ||
                transform.localScale != _lastScale);
    }

    public void Initialize()
    {
        if (setSyncPosition) _lastPosition = transform.position;
        if (setSyncRotation) _lastRotation = transform.rotation;
        if (setSyncScale) _lastScale = transform.localScale;
    }

    public void CollectData(BinaryWriter writer)
    {
        writer.Write(transform.position.x);
        writer.Write(transform.position.y);
        writer.Write(transform.position.z);

        writer.Write(transform.rotation.x);
        writer.Write(transform.rotation.y);
        writer.Write(transform.rotation.z);
        writer.Write(transform.rotation.w);

        writer.Write(transform.localScale.x);
        writer.Write(transform.localScale.y);
        writer.Write(transform.localScale.x);
    }

    public void ApplyData(BinaryReader reader)
    {
        _lastPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        _lastRotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        _lastScale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        transform.SetPositionAndRotation(_lastPosition, _lastRotation);
        transform.localScale = _lastScale;
    }

    public void ResetDirty()
    {
        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
        _lastScale = transform.localScale;
    }
}
