using Newtonsoft.Json;
using UnityEngine;

public class SyncRotation : MonoBehaviour, ISyncable
{
    private Vector3 _synced;
    public string SyncType => "rotation";
  

    public string Serialize()
    {
        return JsonConvert.SerializeObject(new Data
        {
            rotationX = transform.eulerAngles.x,
            rotationY = transform.eulerAngles.y,
            rotationZ = transform.eulerAngles.z,
        });
    }

    public void Deserialize(string json)
    {
        Data data = JsonConvert.DeserializeObject<Data>(json);
        transform.eulerAngles = new Vector3(data.rotationX, data.rotationY, data.rotationZ);
    }

    public bool IsDirty()
    {
        return (transform.eulerAngles - _synced).sqrMagnitude > 0.01f;
    }

    public void ClearDirty()
    {
        _synced = transform.eulerAngles;
    }

    private class Data
    {
        public float rotationX;
        public float rotationY;
        public float rotationZ;
    }
}
