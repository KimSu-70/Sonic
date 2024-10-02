using UnityEngine;

public class AnimalMove : MonoBehaviour
{
    [SerializeField] Rigidbody2D anim;
    [SerializeField] float moveSpeed;
    [SerializeField] SpriteRenderer flip;
    [SerializeField] GameObject animal;
    private Vector2 moveDirectionl;

    private void Start()
    {
        animal = this.gameObject;
        anim = GetComponent<Rigidbody2D>();
        flip = GetComponent<SpriteRenderer>();
        moveSpeed = 15f;

        Destroy(animal, 4f);
        RandomMove();
    }

    private void FixedUpdate()
    {
        anim.AddForce(moveDirectionl * moveSpeed);
    }

    private void RandomMove()
    {
        float iput = Random.Range(0, 6);

        if (iput > 3)
        {
            flip.flipX = false;
            moveDirectionl = Vector2.right;
        }
        else
        {
            flip.flipX = true;
            moveDirectionl = Vector2.left;
        }
    }
}
