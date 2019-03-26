
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _speedModifier = 0.1f;

    public float Size { get; private set; }
    public int Fraction { get; private set; }
    
    private bool _isSimulated { get; set; }
    
    private float _speed;
    private Rect _fieldRect;
    
    public void Init(float size, float speed, Rect areaRect, int fraction, Color color) {
        Size = size;
        _speed = speed;
        Fraction = fraction;
        _fieldRect = areaRect;
        _sprite.color = color;
    }

    public void Fight() {
        _isSimulated = true;
    }

    
    public void OnCollision(Unit nearbyUnit) {
        if (Fraction == nearbyUnit.Fraction)
            return;
        
        gameObject.SetActive(false);
    }
    
    private void FixedUpdate() {
        if (!_isSimulated)
            return;
        
        ProcessMovement();
    }

    private bool ProcessMovement() {
        transform.Translate(Vector2.up * Time.fixedDeltaTime * _speed * _speedModifier);
        return true;
    }
}
