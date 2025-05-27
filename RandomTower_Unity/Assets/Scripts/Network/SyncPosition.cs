using Newtonsoft.Json;
using UnityEngine;

public class SyncPosition : MonoBehaviour, ISyncable
{
    private Vector3 _synced;
    public string SyncType => "position";

    public string Serialize()
    {
        return JsonConvert.SerializeObject(new Data{ position = transform.position });
    }

    public void Deserialize(string json)
    {
        Data data = JsonConvert.DeserializeObject<Data>(json);
        transform.position = data.position;
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
        public Vector3 position;
    }
}
