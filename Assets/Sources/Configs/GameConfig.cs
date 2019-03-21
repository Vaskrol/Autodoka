using System;
using UnityEngine.Serialization;

[Serializable]
public class GameConfig {
    
    [FormerlySerializedAs("gameAreaWidth")] 
    public float GameAreaWidth;
    
    [FormerlySerializedAs("gameAreaHeight")] 
    public float GameAreaHeight;
    
    [FormerlySerializedAs("unitSpawnDelay")] 
    public float UnitSpawnDelay;
    
    [FormerlySerializedAs("numUnitsToSpawn")] 
    public int NumUnitsToSpawn;
    
    [FormerlySerializedAs("minUnitRadius")] 
    public float MinUnitRadius;
    
    [FormerlySerializedAs("maxUnitRadius")] 
    public float MaxUnitRadius;
    
    [FormerlySerializedAs("minUnitSpeed")] 
    public float MinUnitSpeed;
    
    [FormerlySerializedAs("maxUnitSpeed")] 
    public float MaxUnitSpeed;
}