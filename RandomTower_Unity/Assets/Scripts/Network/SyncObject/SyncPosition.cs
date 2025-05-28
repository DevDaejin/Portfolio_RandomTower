using Newtonsoft.Json;
using UnityEngine;

public class SyncPosition : MonoBehaviour, ISyncable
{
    private Vector3 _synced;
    public string SyncType => "position";

    public string Serialize()
    {
        return JsonConvert.SerializeObject(new Data{ 
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
        });
    }

    public void Deserialize(string json)
    {
        Data data = JsonConvert.DeserializeObject<Data>(json);
        transform.position = new Vector3 (data.positionX, data.positionY, data.positionZ);
    }

    public bool IsDirty()
    {
        return (transform.position - _synced).sqrMagnitude > 0.01f;
    }

    public void ClearDirty()
    {
        _synced = transform.position;
    }

    private class Data
    {
        public float positionX;
        public float positionY;
        public float positionZ;
    }
}
