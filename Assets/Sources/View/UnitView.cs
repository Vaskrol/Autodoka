using UnityEngine;

public class UnitView : MonoBehaviour {
        
    [SerializeField] private SpriteRenderer _sprite;

    private Unit _unit;
    
    public void Init(Unit unit, Color color) {
        _unit = unit;
        _unit.OnDie += OnUnitDie;
        _unit.OnSizeChanged += OnUnitSizeChanged;
        
        _sprite.color = color;
        Update();
        OnUnitSizeChanged(_unit);
    }

    private void Update() {
        transform.position = _unit.Position;
    }

    private void OnUnitSizeChanged(Unit unit) {
        _sprite.size = new Vector2(unit.Size, unit.Size);
    }

    private void OnUnitDie(Unit unit) {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        _unit.OnDie -= OnUnitDie;
        _unit.OnSizeChanged -= OnUnitSizeChanged;
    }
}
