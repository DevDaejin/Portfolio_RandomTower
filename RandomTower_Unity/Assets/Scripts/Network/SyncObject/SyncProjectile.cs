using Sync;
using UnityEngine;

public class SyncProjectile : BaseSync<SyncProjectileData>
{
    private Projectile projectile;
    private LineRenderer _lineRenderer;
    private bool _isLaser => _lineRenderer != null;

    public override string SyncType => NetworkConst.Projectile;

    protected override void Awake()
    {
        base.Awake();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        if (_isLaser)
        {    
            _lineRenderer.positionCount = 2;
        }
        projectile = GetComponent<Projectile>();
    }

    private void OnEnable()
    {
        _temp.IsReturned = false;
        _currentData.IsReturned = false;
        _receivedData.IsReturned = false;
    }

    protected override void FillData(SyncProjectileData target)
    {
        if (_isLaser)
        {
            Vector3 start = _lineRenderer.GetPosition(0);
            Vector3 end = _lineRenderer.GetPosition(1);

            target.Start = new ProtoVector3
            {
                X = start.x,
                Y = start.y,
                Z = start.z
            };

            target.End = new ProtoVector3
            {
                X = end.x,
                Y = end.y,
                Z = end.z
            };
        }

        target.IsReturned = false;
    }

    protected override void ApplyData(SyncProjectileData data)
    {
        if (_isLaser)
        {
            if (data.Start != null)
            {
                _lineRenderer.SetPosition(0,
                    new Vector3(data.Start.X, data.Start.Y, data.Start.Z));
            }

            if (data.Start != null)
            {
                _lineRenderer.SetPosition(1,
                    new Vector3(data.End.X, data.End.Y, data.End.Z));
            }
        }

        if (data.IsReturned)
        {
            projectile.ForceReturn();
        }
    }

    protected override bool Equals(SyncProjectileData a, SyncProjectileData b)
    {
        if (a.IsReturned != b.IsReturned)
        {
            return false;
        }

        if (_isLaser)
        {
            if (a.Start == null || b.Start == null || a.End == null || b.End == null)
                return false;

            bool isDataEqual =
                CheckVectorEqauls(a.Start, b.Start)
                && CheckVectorEqauls(a.End, b.End);

            return isDataEqual;
        }

        return true;
    }

    private bool CheckVectorEqauls(ProtoVector3 a, ProtoVector3 b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }
}
