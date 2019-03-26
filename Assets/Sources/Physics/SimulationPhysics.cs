using UnityEngine;

public class SimulationPhysics {

    private Chunk[,] _chunks;
    private float _chunkSizeModifier = 3f;

    private Vector2 _chunkSize;
    private Vector2 _fieldSize;

    public SimulationPhysics(GameConfig config) {
        _fieldSize = new Vector2(config.gameAreaWidth, config.gameAreaHeight);
        
        var maxUnitSize = config.maxUnitRadius;
        var horizontalChunksCount = Mathf.RoundToInt(_fieldSize.x / (_chunkSizeModifier * maxUnitSize));
        var verticalChunksCount = Mathf.RoundToInt(_fieldSize.y / (_chunkSizeModifier * maxUnitSize));
        
        _chunks = new Chunk[horizontalChunksCount, verticalChunksCount];
        _chunkSize = new Vector2(_fieldSize.x / horizontalChunksCount, _fieldSize.y / verticalChunksCount);
        for (int i = 0; i < horizontalChunksCount; i++)
        for (int j = 0; j < verticalChunksCount; j++) {
            _chunks[i, j] = new Chunk();
        }


        Debug.Log("Created chunks: " + horizontalChunksCount + "x" + verticalChunksCount);
    }

    public void Update() {
        for (int i = 0; i < _chunks.GetLength(0); i++) 
        for (int j = 0; j < _chunks.GetLength(1); j++) {
            foreach (var unit in _chunks[i, j].Units) {
                DetectCollision(unit, i, j);
            }
        }
    }
    
    public void AddUnit(Unit unit) {
        var unitPosition = unit.transform.position;
        var chunkX = (int)Mathf.Floor((unitPosition.x + _fieldSize.x / 2f )/ _chunkSize.x);
        var chunkY = (int)Mathf.Floor((unitPosition.y + _fieldSize.y / 2f )/ _chunkSize.y);
        _chunks[chunkX, chunkY].AddUnit(unit);
    }

    private void DetectCollision(Unit unit, int chunkX, int chunkY) {
        for (int i = chunkX - 1; i <= chunkX + 1; i++)
        for (int j = chunkY - 1; j <= chunkY + 1; j++) {
            if (i < 0 || j < 0 || i >= _chunks.GetLength(0) || j >= _chunks.GetLength(1))
                continue;

            foreach (var nearbyUnit in _chunks[i, j].Units) {
                if (nearbyUnit == null)
                    continue;
                
                var distance = Vector2.Distance(unit.transform.position, nearbyUnit.transform.position);
                if (distance < unit.Size / 2f + nearbyUnit.Size / 2f)
                    unit.OnCollision(nearbyUnit);
            }
        }

    }
}
