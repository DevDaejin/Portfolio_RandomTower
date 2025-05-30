using UnityEngine;

public class SyncRotation : BaseSync<SyncRotation.Data>
{
    public override string SyncType => "rotation";

    protected override Data GetCurrentData()
    {
        return new Data
        {
            X = transform.eulerAngles.x,
            Y = transform.eulerAngles.y,
            Z = transform.eulerAngles.z,
        };
    }

    protected override void ApplyData(Data data)
    {
        transform.eulerAngles = new Vector3(data.X, data.Y, data.Z);
    }

    protected override bool Equals(Data a, Data b)
    {
        Vector3 aRotation = new Vector3(a.X, a.Y, a.Z);
        Vector3 bRotation = new Vector3(b.X, b.Y, b.Z);

        return aRotation == bRotation;
    }

    public class Data
    {
        public float X;
        public float Y;
        public float Z;
    }
}
