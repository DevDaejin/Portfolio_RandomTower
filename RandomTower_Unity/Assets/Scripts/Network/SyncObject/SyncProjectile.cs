using UnityEngine;

public class SyncProjectile : BaseSync<SyncProjectile.Data>
{
    [SerializeField] private bool _isLaserType;
    private Projectile projectile;
    private LineRenderer _lineRenderer;

    public override string SyncType => "projectile";

    private void Awake()
    {
        if (_isLaserType)
        {
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }
        projectile = GetComponent<Projectile>();
    }

    protected override Data GetCurrentData()
    {
        Data data = new Data();

        if (_isLaserType)
        {
            data.StartX = _lineRenderer.GetPosition(0).x;
            data.StartY = _lineRenderer.GetPosition(0).y;
            data.StartZ = _lineRenderer.GetPosition(0).z;

            data.EndX = _lineRenderer.GetPosition(1).x;
            data.EndY = _lineRenderer.GetPosition(1).y;
            data.EndZ = _lineRenderer.GetPosition(1).z;
        }

        return data;
    }

    protected override void ApplyData(Data data)
    {
        if (_isLaserType)
        {
            Vector3 start = new Vector3(data.StartX, data.StartY, data.StartZ);
            _lineRenderer.SetPosition(0, start);

            Vector3 end = new Vector3(data.EndX, data.EndY, data.EndZ);
            _lineRenderer.SetPosition(1, end);
        }

        if(data.IsReturned)
        {
            projectile.ForceReturn();
        }
    }

    protected override bool Equals(Data a, Data b)
    {
        bool isLaser =
             a.PositionCount == b.PositionCount

            && a.StartX == b.StartX
            && a.StartY == b.StartY
            && a.StartZ == b.StartZ

            && a.EndX == b.EndX
            && a.EndY == b.EndY
            && a.EndZ == b.EndZ;

        bool isReturn = a.IsReturned == b.IsReturned;

        if (_isLaserType)
        {
            return isLaser && isReturn;
        }
        else
        {
            return isReturn;
        }
           
    }

    public class Data
    {
        public int PositionCount = 0;

        public float StartX = 0;
        public float StartY = 0;
        public float StartZ = 0;

        public float EndX = 0;
        public float EndY = 0;
        public float EndZ = 0;

        public bool IsReturned = false;
    }
}
