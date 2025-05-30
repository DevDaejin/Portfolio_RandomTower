using UnityEngine;

public class SyncPosition : BaseSync<SyncPosition.Data>
{
    public override string SyncType => "position";

    protected override Data GetCurrentData()
    {
        return new Data
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z,
        };
    }

    protected override void ApplyData(Data data)
    {
        transform.position = new Vector3(data.X, data.Y, data.Z);
    }

    protected override bool Equals(Data a, Data b)
    {
        Vector3 aPosition = new Vector3(a.X, a.Y, a.Z);
        Vector3 bPosition = new Vector3(b.X, b.Y, b.Z);

        return aPosition == bPosition;
    }

    public class Data
    {
        public float X;
        public float Y;
        public float Z;
    }
}

