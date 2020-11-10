using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

    public float moveTime = 0f;
    public LayerMask blockingLayer;
    public LayerMask unitLayer;

    protected BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    //private float inverseMoveTime = 10.0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        //inverseMoveTime = 1.0f/moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit1, out RaycastHit2D hit2)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit1 = Physics2D.Linecast(start, end, blockingLayer);
        hit2 = Physics2D.Linecast(start, end, unitLayer);
        boxCollider.enabled = true;

        if (hit1.transform == null && hit2.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            //Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, 0.2f);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        //rb2D.MovePosition(end);
        //yield return null;
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit1, hit2;
        bool canMove = Move(xDir, yDir, out hit1, out hit2);

        if (hit1.transform == null && hit2.transform == null)
            return;

        if (hit1.transform != null)
        {
            T hitComponent1 = hit1.transform.GetComponent<T>();
            if (!canMove && hitComponent1 != null)
            {
                OnCantMove(hitComponent1);
            }
        }

        if (hit2.transform != null)
        {
            T hitComponent2 = hit2.transform.GetComponent<T>();
            if (!canMove && hitComponent2 != null)
            {
                OnCantMove(hitComponent2);
            }
        }
    }

    public Vector2 CheckRoom()
    {
        float outX;
        float outY;
        
        float x = transform.position.x;
        float y = transform.position.y;

        outX = (x < 0) ? -10 : ((x < 10) ? 0 : 10);
        outY = (y < 0) ? -10 : ((y < 10) ? 0 : 10);

        return new Vector2(outX, outY);

    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
