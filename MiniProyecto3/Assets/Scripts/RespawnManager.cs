using UnityEngine;
using System.Collections.Generic;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [System.Serializable]
    public class LevelRespawns
    {
        public string sceneName;
        public Transform spawnPoint;
    }

    public List<LevelRespawns> respawns;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public Transform GetSpawnPoint(string sceneName)
    {
        foreach (var entry in respawns)
        {
            if (entry.sceneName == sceneName)
            {
                return entry.spawnPoint;
            }
        }
        return null;
    }
}
