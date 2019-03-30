using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationPhysics {

    private readonly Chunk[,] _chunks;
    private float _chunkSizeModifier = 3f;
    
    private readonly int _horizontalChunksCount;
    private readonly int _verticalChunksCount;

    private readonly Vector2 _chunkSize;
    private readonly Vector2 _fieldSize;
    private readonly Dictionary<Unit, HashSet<Unit>> _unitsCollisions; 

    public SimulationPhysics(GameConfig config) {
        _unitsCollisions = new Dictionary<Unit, HashSet<Unit>>();
        _fieldSize = new Vector2(config.gameAreaWidth, config.gameAreaHeight);
        
        var maxUnitSize = config.maxUnitRadius;
        _horizontalChunksCount = Mathf.RoundToInt(_fieldSize.x / (_chunkSizeModifier * maxUnitSize));
        _verticalChunksCount = Mathf.RoundToInt(_fieldSize.y / (_chunkSizeModifier * maxUnitSize));
        
        _chunks = new Chunk[_horizontalChunksCount, _verticalChunksCount];
        _chunkSize = new Vector2(_fieldSize.x / _horizontalChunksCount, _fieldSize.y / _verticalChunksCount);
        for (int i = 0; i < _horizontalChunksCount; i++)
        for (int j = 0; j < _verticalChunksCount; j++) {
            _chunks[i, j] = new Chunk();
        }

        Debug.Log("Created chunks: " + _horizontalChunksCount + "x" + _verticalChunksCount);
    }

    public void Update() {
        for (int i = 0; i < _horizontalChunksCount; i++) 
        for (int j = 0; j < _verticalChunksCount; j++) {
            var chunk = _chunks[i, j];
            for(int unitNumber = 0; unitNumber < chunk.Units.Count; unitNumber++) {
                var unit = chunk.Units[unitNumber];
                DetectBounds(unit);
                if (TryMoveUnitToAnotherChunk(chunk, unit)) 
                    unitNumber--;
                DetectCollision(unit, i, j);
            }
        }
    }

    public void AddUnit(Unit unit) {
        var chunk = DefineUnitChunk(unit);
        chunk.AddUnit(unit);
    }

    private Chunk DefineUnitChunk(Unit unit) {
        var unitPosition = unit.transform.position;
        var chunkX = Mathf.FloorToInt((unitPosition.x + _fieldSize.x / 2f) / _chunkSize.x);
        var chunkY = Mathf.FloorToInt((unitPosition.y + _fieldSize.y / 2f) / _chunkSize.y);
        
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
            if (i < 0 || j < 0 || i >= _horizontalChunksCount || j >= _verticalChunksCount)
                continue;

            foreach (var nearbyUnit in _chunks[i, j].Units) {
                if (nearbyUnit == unit)
                    continue;
                
                if (nearbyUnit == null || !nearbyUnit.isActiveAndEnabled) {
                    StopCollidingUnits(unit, nearbyUnit);
                    continue;
                }

                var distance = Vector2.Distance(unit.transform.position, nearbyUnit.transform.position);
                if (distance > unit.Size / 2f + nearbyUnit.Size / 2f) {
                    StopCollidingUnits(unit, nearbyUnit);
                    continue;
                }

                CollideUnits(unit, nearbyUnit, distance);
                CollideUnits(nearbyUnit, unit, distance);
            }
        }
    }

    private void CollideUnits(Unit unitOne, Unit unitTwo, float distance) {
        var collision = new Collision(distance, unitTwo);
        
        // The unit doesn't have any collisions yet
        if (!_unitsCollisions.ContainsKey(unitOne)) {
            _unitsCollisions.Add(unitOne, new HashSet<Unit>(new[] {unitTwo}));
            unitOne.OnEnterCollision(collision);
            return;
        }

        // The unit doesn't have a collision with this particular unit
        if (!_unitsCollisions[unitOne].Contains(unitTwo)) {
            _unitsCollisions[unitOne].Add(unitTwo);
            unitOne.OnEnterCollision(collision);
            return;
        }

        // The unit have already collided with this unit
        unitOne.OnCollision(collision);
    }

    private void StopCollidingUnits(Unit unit, Unit nearbyUnit) {
        // Call OnCollisionExit here
        
        if (!_unitsCollisions.Any() || !_unitsCollisions.ContainsKey(unit))
            return;

        if (_unitsCollisions[unit].Contains(nearbyUnit)) {
            _unitsCollisions[unit].Remove(nearbyUnit);
        }

        if (!_unitsCollisions[unit].Any()) {
            _unitsCollisions.Remove(unit);
        }
    }

    
    private void DetectBounds(Unit unit) {
        Vector2 unitPosition = unit.transform.position;
        var unitHalfSize = unit.Size / 2f;

        if (unitPosition.x - unitHalfSize < -_fieldSize.x / 2f) {
            unit.OnWallCollision(Vector2.left);
            unit.transform.position = new Vector2(unitHalfSize -_fieldSize.x / 2f, unitPosition.y);
        }

        if (unitPosition.x + unitHalfSize > _fieldSize.x / 2f) {
            unit.OnWallCollision(Vector2.right);
            unit.transform.position = new Vector2(_fieldSize.x / 2f - unitHalfSize, unitPosition.y);
        }

        if (unitPosition.y - unitHalfSize < -_fieldSize.y / 2f) {
            unit.OnWallCollision(Vector2.up);
            unit.transform.position = new Vector2(unitPosition.x, unitHalfSize - _fieldSize.y / 2f);
        }

        if (unitPosition.y + unitHalfSize > _fieldSize.y / 2f) {
            unit.OnWallCollision(Vector2.down);
            unit.transform.position = new Vector2(unitPosition.x, _fieldSize.y / 2f - unitHalfSize);
        }
    }
    
    public void Reset() {
        foreach (var chunk in _chunks) {
            chunk.Clear();
        }
    }
    
    // Let it be here for now
    public void DebugDrawChunks() {
        var startPoint = new Vector2(-_fieldSize.x / 2f, -_fieldSize.y / 2f) + _chunkSize / 2f;
        for (int i = 0; i < _horizontalChunksCount; i++)
        for (int j = 0; j < _verticalChunksCount; j++) {
            Vector3 cubeCenter = startPoint + new Vector2(i * _chunkSize.x, j * _chunkSize.y);
            Gizmos.DrawWireCube(cubeCenter, _chunkSize);
        }
    }
}
