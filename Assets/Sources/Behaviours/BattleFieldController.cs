using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleFieldController : MonoBehaviour {

	[SerializeField] private SpriteRenderer _field;
	[SerializeField] private Camera _camera;
	
	[Header("Units")]
	[SerializeField] private Unit _unitPrefab;
	[SerializeField] private Transform _unitsHolder;
	[SerializeField] private Color[] _unitColors;

	[Header("Debug")] 
	[SerializeField] private bool _debugMode;
	
	private GameConfig _config;
	private Unit[,] _units;
	private int _perFractionCount;
	private SimulationPhysics _physics;
	private bool _isSimulating;

	public void Init(GameConfig config, SimulationPhysics physics) {
		_config = config;
		_field.size = new Vector2(config.gameAreaWidth, config.gameAreaHeight);
		_camera.orthographicSize = _field.size.y / 2f;
		_perFractionCount  = _config.numUnitsToSpawn / _unitColors.Length;
		_physics = physics;
	}

	public void SpawnUnits(Action onComplete) {
		_units = new Unit[_unitColors.Length,_config.numUnitsToSpawn];
		StartCoroutine(ProcessSpawnUnits(onComplete));
	}

	public void StartSimulation() {
		for (int colorIdx = 0; colorIdx < _unitColors.Length; colorIdx++)
		for (int i = 0; i < _perFractionCount; i++) {
			_units[colorIdx, i].StartSimulation();
		}
		
		_isSimulating = true;
	}

	private void FixedUpdate() {
		if (!_isSimulating)
			return;
		
		_physics.Update();
	}

	private IEnumerator ProcessSpawnUnits(Action onComplete) {
		for (int colorIdx = 0; colorIdx < _unitColors.Length; colorIdx++) 
		for (int i = 0; i < _perFractionCount; i++) {
			_units[colorIdx, i] = SpawnUnit(colorIdx);
			yield return new WaitForSeconds(_config.unitSpawnDelay / 1000f);
		}

		onComplete();
	}

	private Unit SpawnUnit(int unitColorIndex) {
		var unit = Instantiate(_unitPrefab, _unitsHolder);
		var unitSize = Random.Range(_config.minUnitRadius, _config.maxUnitRadius);
		var unitSpeed = Random.Range(_config.minUnitSpeed, _config.maxUnitSpeed);
		
		unit.Init(unitSize, unitSpeed, unitColorIndex, _unitColors[unitColorIndex]);
		unit.transform.position = new Vector2(
			(Random.value - 0.5f) * (_field.size.x - unit.Size), 
			(Random.value - 0.5f) * (_field.size.y - unit.Size));
		
		_physics.AddUnit(unit);
		
		return unit;
	}

	private void OnDrawGizmos() {
		if (!_debugMode)
			return;
		
		_physics.DebugDrawChunks();
	}
}

