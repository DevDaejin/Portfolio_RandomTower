using System.IO;
using UnityEngine;

public class SyncTransform : Syncable<Transform>
{
    [SerializeField] private readonly bool setSyncPosition;
    [SerializeField] private readonly bool setSyncRotation;
    [SerializeField] private readonly bool setSyncScale;

    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private Vector3 _lastScale;

    public SyncTransform(Transform target, int id) : base(target, id){}

    public override void Initialize()
    {
        base.Initialize();
        if (setSyncPosition) _lastPosition = Target.position;
        if (setSyncRotation) _lastRotation = Target.rotation;
        if (setSyncScale) _lastScale = Target.localScale;
    }
    public override void Serialize(BinaryWriter writer)
    {

    }
    public override void Deserialize(BinaryReader reader)
    {
    }
}
