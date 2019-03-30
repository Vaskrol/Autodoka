using UnityEngine;

public class Unit : MonoBehaviour {
    
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _speedModifier = 0.1f;

    public delegate void UnitEventHandler(Unit unit);
    public event UnitEventHandler OnDie;

    private const float DEATH_SIZE = 0.2f;

    public float Size { get; private set; }
    public Vector2 Velocity { get; private set; }
    public int Fraction { get; private set; }

    private bool _isSimulated;

    public void Init(float size, Vector2 velocity, int fraction, Color color) {
        SetSize(size);
        Fraction = fraction;
        Velocity = velocity;
        _sprite.color = color;
    }

    public void StartSimulation() {
        _isSimulated = true;
    }

    public void Die() {
        gameObject.SetActive(false);
        _isSimulated = false;
        if (OnDie != null)
            OnDie(this);
    }

    public void OnCollision(Collision collision) {
        if (!_isSimulated)
            return;
        
        var collidedUnit = collision.Collider;
        if (Fraction == collidedUnit.Fraction) 
            return;
        
        var sizeDelta = (Size / 2f + collidedUnit.Size / 2f - collision.Distance);
        if (sizeDelta > collidedUnit.Size / 2)
            sizeDelta = collidedUnit.Size / 2f;
        SetSize(Size - sizeDelta);
        
        if (Size < DEATH_SIZE)
            Die();
    }

    public void OnEnterCollision(Collision collision) {
        if (!_isSimulated)
            return;
        
        var collidedUnit = collision.Collider;
        if (Fraction != collidedUnit.Fraction) 
            return;
        
        Vector2 collisionNormal = (transform.position - collidedUnit.transform.position).normalized;
        Velocity = Vector2.Reflect(Velocity, collisionNormal);
    }
    
    public void OnWallCollision(Vector2 wallDirection) {
        Velocity = Vector2.Reflect(Velocity, wallDirection);
    }

    private void FixedUpdate() {
        if (!_isSimulated)
            return;
        
        ProcessMovement();
    }

    private void ProcessMovement() {
        transform.Translate(Velocity * Time.fixedDeltaTime * _speedModifier);
    }

    private void SetSize(float size) {
        Size = size;
        _sprite.size = new Vector2(Size, Size);
    }

   
}
