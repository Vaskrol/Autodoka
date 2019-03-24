using System;
using UnityEngine;

public class SimulationPhysics {

    private Chunk[,] _chunks;
    private float _chunkSizeModifier = 3f;

    private Vector2 _chunkSize;

    public SimulationPhysics(GameConfig config) {
        var fieldSize = new Vector2(config.gameAreaWidth, config.gameAreaHeight);
        var maxUnitSize = config.maxUnitRadius;
        var horizontalChunksCount = Mathf.RoundToInt(fieldSize.x / (_chunkSizeModifier * maxUnitSize));
        var verticalChunksCount = Mathf.RoundToInt(fieldSize.y / (_chunkSizeModifier * maxUnitSize));
        
        _chunks = new Chunk[horizontalChunksCount, verticalChunksCount];
        _chunkSize = new Vector2(fieldSize.x / horizontalChunksCount, fieldSize.y / verticalChunksCount);
        
        Debug.Log("Created chunks: " + horizontalChunksCount + "x" + verticalChunksCount);
    }

    public void Update() {
        
    }

    public void AddUnit(Unit unit) {
        var unitPosition = unit.transform.position;
        var chunkX = (int)Mathf.Floor(unitPosition.x / _chunkSize.x);
        var chunkY = (int)Mathf.Floor(unitPosition.y / _chunkSize.y);
        _chunks[chunkX, chunkY].AddUnit(unit);
    }
}
