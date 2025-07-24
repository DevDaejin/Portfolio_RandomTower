using Sync;
using UnityEngine;

public class SyncTransform : BaseSync<SyncTransformData>
{
    [SerializeField] private bool isSyncRotation = true;
    [SerializeField] private bool isSyncScale = true;

    public override string SyncType => "transform";

    protected override void FillData(SyncTransformData target)
    {
        target.Position = VectorToProtoVector(transform.position);

        if (isSyncRotation)
        {
            target.Rotation = VectorToProtoVector(transform.eulerAngles);
        }
        if (isSyncScale)
        {
            target.Scale = VectorToProtoVector(transform.localScale);
        }
    }

    protected override void ApplyData(SyncTransformData data)
    {
        transform.position = ProtoVectorToVector(_receivedData.Position);
        if (isSyncRotation)
        {
            transform.eulerAngles = ProtoVectorToVector(_receivedData.Rotation);
        }
        if (isSyncScale)
        {
            transform.localScale = ProtoVectorToVector(_receivedData.Scale);
        }
    }

    protected override bool Equals(SyncTransformData a, SyncTransformData b)
    {
        if (a.Position == null || b.Position == null) return false;
        if (!EqualVector(a.Position, b.Position)) return false;

        if (isSyncRotation)
        {
            if (a.Rotation == null || b.Rotation == null) return false;
            if (!EqualVector(a.Rotation, b.Rotation)) return false;
        }

        if (isSyncScale)
        {
            if (a.Scale == null || b.Scale == null) return false;
            if (!EqualVector(a.Scale, b.Scale)) return false;
        }

        return true;
    }

    private bool EqualVector(ProtoVector3 a, ProtoVector3 b)
    {
        return Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
    }

    private bool Near(float a, float b, float epsilon = 0.00001f)
    {
        return Mathf.Abs(a - b) < epsilon;
    }

    private Vector3 ProtoVectorToVector(ProtoVector3 protoVector3)
    {
        return new Vector3(protoVector3.X, protoVector3.Y, protoVector3.Z);
    }

    private ProtoVector3 VectorToProtoVector(Vector3 vector3)
    {
        return new ProtoVector3
        {
            X = vector3.x,
            Y = vector3.y,
            Z = vector3.z
        };
    }
}

