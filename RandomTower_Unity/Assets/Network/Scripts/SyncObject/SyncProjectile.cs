using Sync;
using UnityEngine;

public class SyncProjectile : BaseSync<SyncProjectileData>
{
    private Projectile projectile;
    private LineRenderer _lineRenderer;
    private bool _isLaser => _lineRenderer != null;

    public override string SyncType => "projectile";

    private void Awake()
    {
        _currentData = new();
        _receivedData = new();

        _lineRenderer = GetComponentInChildren<LineRenderer>();
        if (_isLaser)
        {    
            _lineRenderer.positionCount = 2;
        }
        projectile = GetComponent<Projectile>();
    }

    protected override SyncProjectileData GetCurrentData()
    {
        if (_isLaser)
        {
            _currentData.Start.X = _lineRenderer.GetPosition(0).x;
            _currentData.Start.Y = _lineRenderer.GetPosition(0).y;
            _currentData.Start.Z = _lineRenderer.GetPosition(0).z;

            _currentData.End.X = _lineRenderer.GetPosition(1).x;
            _currentData.End.Y = _lineRenderer.GetPosition(1).y;
            _currentData.End.Z = _lineRenderer.GetPosition(1).z;
        }

        return _currentData;
    }

    protected override void ApplyData(SyncProjectileData data)
    {
        if (_isLaser)
        {
            Vector3 start = new Vector3(data.Start.X, data.Start.Y, data.Start.Z);
            _lineRenderer.SetPosition(0, start);

            Vector3 end = new Vector3(data.End.X, data.End.Y, data.End.Z);
            _lineRenderer.SetPosition(1, end);
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
                Near(a.Start.X, b.Start.X)
                && Near(a.Start.Y, b.Start.Y)
                && Near(a.Start.Z, b.Start.Z)
                && Near(a.End.X, b.End.X)
                && Near(a.End.Y, b.End.Y)
                && Near(a.End.Z, b.End.Z);

            return isDataEqual;
        }

        return true;
    }

    private bool Near(float a, float b, float threshold = 0.01f)
    {
        return Mathf.Abs(a - b) < threshold;
    }
}
