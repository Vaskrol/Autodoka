using System;
using UnityEngine;

public class SimulationPhysics {

    private Chunk[,] _chunks;
    private float _chunkSizeModifier = 3f;

    public SimulationPhysics(GameConfig config) {
        var fieldSize = new Vector2(config.gameAreaWidth, config.gameAreaHeight);
        var maxUnitSize = config.maxUnitRadius;
        var horizontalChunksCount = Mathf.RoundToInt(fieldSize.x / (_chunkSizeModifier * maxUnitSize));
        var verticalChunksCount = Mathf.RoundToInt(fieldSize.y / (_chunkSizeModifier * maxUnitSize));
        
        _chunks = new Chunk[horizontalChunksCount, verticalChunksCount];
        
        Debug.Log("Created chunks: " + horizontalChunksCount + "x" + verticalChunksCount);
    }

    public void Update() {
        
    }
}
