using UnityEngine;

public class IDGenerator
{
    private readonly string _clientID;
    private ulong _currentID = 0;

    public IDGenerator(string clientID)
    {
        _clientID = clientID;
    }

    public string Get()
    {
        return $"{_clientID}_{_currentID++}";
    }
}
