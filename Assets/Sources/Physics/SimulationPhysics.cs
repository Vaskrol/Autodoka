using UnityEngine;

public class SimulationPhysics {

    private readonly Chunk[,] _chunks;
    private float _chunkSizeModifier = 3f;

    private readonly Vector2 _chunkSize;
    private readonly Vector2 _fieldSize;

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
            var chunk = _chunks[i, j];
            for(int unitNumber = 0; unitNumber < chunk.Units.Count; unitNumber++) {
                var unit = chunk.Units[unitNumber];
                if (TryMoveUnitToAnotherChunk(chunk, unit)) 
                    unitNumber--;
                DetectCollision(unit, i, j);
                DetectBounds(unit);
            }
        }
    }

    public void AddUnit(Unit unit) {
        var chunk = DefineUnitChunk(unit);
        chunk.AddUnit(unit);
    }

    private Chunk DefineUnitChunk(Unit unit) {
        var unitPosition = unit.transform.position;
        var chunkX = (int)Mathf.Floor((unitPosition.x + _fieldSize.x / 2f)/ _chunkSize.x);
        var chunkY = (int)Mathf.Floor((unitPosition.y + _fieldSize.y / 2f)/ _chunkSize.y);
        return _chunks[chunkX, chunkY];
    }

    private bool TryMoveUnitToAnotherChunk(Chunk currentChunk, Unit unit) {
        var unitChunk = DefineUnitChunk(unit);
        if (unitChunk == currentChunk)
            return false;
        
        currentChunk.RemoveUnit(unit);
        unitChunk.AddUnit(unit);
        return true;
    }
    
    private void DetectCollision(Unit unit, int chunkX, int chunkY) {
        for (int i = chunkX - 1; i <= chunkX + 1; i++)
        for (int j = chunkY - 1; j <= chunkY + 1; j++) {
            if (i < 0 || j < 0 || i >= _chunks.GetLength(0) || j >= _chunks.GetLength(1))
                continue;

            foreach (var nearbyUnit in _chunks[i, j].Units) {
                if (nearbyUnit == null || !nearbyUnit.isActiveAndEnabled)
                    continue;
                
                var distance = Vector2.Distance(unit.transform.position, nearbyUnit.transform.position);
                if (distance > unit.Size / 2f + nearbyUnit.Size / 2f) 
                    continue;
                
                var collision = new Collision(distance, nearbyUnit);
                unit.OnCollision(collision);  
                    
                collision = new Collision(distance, unit);
                nearbyUnit.OnCollision(collision);
            }
        }
    }
    
    private void DetectBounds(Unit unit) {
        Vector2 unitPosition = unit.transform.position;
        var unitHalfSize = unit.Size / 2f;
        
        if (unitPosition.x - unitHalfSize < -_fieldSize.x / 2f)
            unit.OnWallCollision(Vector2.left);
        
        if (unitPosition.x + unitHalfSize > _fieldSize.x / 2f)
            unit.OnWallCollision(Vector2.right);
        
        if (unitPosition.y - unitHalfSize < -_fieldSize.y / 2f)
            unit.OnWallCollision(Vector2.up);
        
        if (unitPosition.y + unitHalfSize > _fieldSize.y / 2f)
            unit.OnWallCollision(Vector2.down);
    }
}
