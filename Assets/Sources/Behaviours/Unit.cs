
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private SpriteRenderer _sprite;

    private bool _isSimulated { get; set; }
    
    private int _fraction;
    private float _size;
    private float _speed;
    private Rect _fieldRect;
    
    public void Init(float size, float speed, Rect areaRect, int fraction, Color color) {
        _size = size;
        _speed = speed;
        _fraction = fraction;
        _fieldRect = areaRect;
        _sprite.color = color;
    }

    public void Fight() {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        _isSimulated = true;
    }

    private void FixedUpdate() {
        if (!_isSimulated)
            return;
        
        
    }

    private bool ProcessMovement() {
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * _speed);
        
        return true;
    }
}
