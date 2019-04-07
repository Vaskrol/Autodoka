using UnityEngine;

public class Unit  {

    public delegate void UnitEventHandler(Unit unit);
    public event UnitEventHandler OnDie;
    public event UnitEventHandler OnSizeChanged;

    private const float DEATH_SIZE = 0.2f;

    public Vector2 Position { get; set; }
    
    public float Size { get; private set; }
    public Vector2 Velocity { get; private set; }
    public int Fraction { get; private set; }
    public bool IsSimulated { get; private set; }

    public Unit(Vector2 position, float size, Vector2 velocity, int fraction) {
        Position = position;
        SetSize(size);
        Fraction = fraction;
        Velocity = velocity;
    }

    public void StartSimulation() {
        IsSimulated = true;
    }

    public void Update() {
        if (!IsSimulated)
            return;
        
        ProcessMovement();
    }
    
    public void Die() {
        IsSimulated = false;
        if (OnDie != null)
            OnDie(this);
    }

    public void OnCollision(Collision collision) {
        if (!IsSimulated)
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
        if (!IsSimulated)
            return;
        
        var collidedUnit = collision.Collider;
        if (Fraction != collidedUnit.Fraction) 
            return;
        
        Vector2 collisionNormal = (Position - collidedUnit.Position).normalized;
        Velocity = Vector2.Reflect(Velocity, collisionNormal);
    }
    
    public void OnWallCollision(Vector2 wallDirection) {
        Velocity = Vector2.Reflect(Velocity, wallDirection);
    }

    private void ProcessMovement() {
        var moveDelta = Velocity * Time.fixedDeltaTime;
        Position += moveDelta;
    }

    private void SetSize(float size) {
        Size = size;
        if (OnSizeChanged != null)
           OnSizeChanged(this);
    }
}
