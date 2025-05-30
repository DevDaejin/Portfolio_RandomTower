using UnityEngine;

public class SyncScale : BaseSync<SyncScale.Data>
{
    public override string SyncType => "scale";

    protected override Data GetCurrentData()
    {
        return new Data
        {
            X = transform.localScale.x,
            Y = transform.localScale.y,
            Z = transform.localScale.z,
        };
    }

    protected override void ApplyData(Data data)
    {
        transform.localScale = new Vector3(data.X, data.Y, data.Z);
    }

    protected override bool Equals(Data a, Data b)
    {
        Vector3 aScale = new Vector3(a.X, a.Y, a.Z);
        Vector3 bScale = new Vector3(b.X, b.Y, b.Z);

        return aScale == bScale;
    }

    public class Data
    {
        public float X;
        public float Y;
        public float Z;
    }
}
