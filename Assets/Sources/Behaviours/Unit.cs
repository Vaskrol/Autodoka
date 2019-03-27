
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _speedModifier = 0.1f;

    private const float DIE_SIZE = 0.2f;

    public float Size { get { return _size; } }
    public int Fraction { get; private set; }

    private bool _isSimulated;
    
    private Vector2 _velocity;
    private float _size;
    
    public void Init(float size, float speed, int fraction, Color color) {
        SetSize(size);
        _velocity = Random.insideUnitCircle.normalized * speed;
        Fraction = fraction;
        _sprite.color = color;
    }

    public void StartSimulation() {
        _isSimulated = true;
    }

    public void Die() {
        gameObject.SetActive(false);
        _isSimulated = false;
    }

    public void OnCollision(Collision collision) {
        var collidedUnit = collision.Collider;
        if (Fraction == collidedUnit.Fraction) 
            return;
        
        var sizeDelta = (_size + collidedUnit.Size - collision.Distance) / 2f;
        SetSize(_size - sizeDelta);
        
        if (_size < DIE_SIZE)
            Die();
    }

    public void OnEnterCollision(Collision collision) {
        var collidedUnit = collision.Collider;
        if (Fraction != collidedUnit.Fraction) 
            return;
        
        Vector2 collisionNormal = (transform.position - collidedUnit.transform.position).normalized;
        _velocity = Vector2.Reflect(_velocity, collisionNormal);
        return;
    }
    
    public void OnWallCollision(Vector2 wallDirection) {
        _velocity = Vector2.Reflect(_velocity, wallDirection);
    }

    private void FixedUpdate() {
        if (!_isSimulated)
            return;
        
        ProcessMovement();
    }

    private void ProcessMovement() {
        transform.Translate(_velocity * Time.fixedDeltaTime * _speedModifier);
    }

    private void SetSize(float size) {
        _size = size;
        _sprite.size = new Vector2(_size, _size);
    }

   
}
