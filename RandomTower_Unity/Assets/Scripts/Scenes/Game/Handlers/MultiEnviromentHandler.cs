using System;
using UnityEngine;

public class MultiEnviromentHandler : MonoBehaviour
{
    [Serializable]
    public struct EnviromentData
    {
        public Transform InstallPoints;
        public Transform Routes;
    }

    [Header("Game enviroment")]
    [SerializeField] private EnviromentData _hostEnviroment;
    [SerializeField] private EnviromentData _guestEnviroment;

    [Header("Maps")]
    [SerializeField] private GameObject _singleMap;
    [SerializeField] private GameObject _multiMap;



    public void Initialize(bool isHost)
    {
        _singleMap.SetActive(false);
        _multiMap.SetActive(true);

        GetComponent<EnemyManager>().SetRouteGroup(isHost ? _hostEnviroment.Routes : _guestEnviroment.Routes);
        GetComponent<TowerManager>().SetInstallPoints(points: isHost ? _hostEnviroment.InstallPoints : _guestEnviroment.InstallPoints);
    }
}
