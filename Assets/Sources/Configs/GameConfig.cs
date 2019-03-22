using System;
using UnityEngine.Serialization;

[Serializable]
public class Config {
    public GameConfig GameConfig;
}

[Serializable]
public class GameConfig {
    
    public float gameAreaWidth;
    
    public float gameAreaHeight;
    
    public float unitSpawnDelay;
    
    public int numUnitsToSpawn;
    
    public float minUnitRadius;
    
    public float maxUnitRadius;
    
    public float minUnitSpeed;
    
    public float maxUnitSpeed;
}