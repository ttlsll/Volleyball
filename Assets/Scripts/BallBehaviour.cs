using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform transformBall;

    private bool firstPunchDone = false;

    GameObject player;
    Transform playerTransform;
    PlayerController playerController;
    PlayerController.ActionState action;
    float appliedPower;

    bool hitApplied = false;

    Vector3 direction;

    private void Update()
    {

        if (!firstPunchDone && rb.velocity.magnitude > 0)
        {
            firstPunchDone = true;
            rb.gravityScale = 0.5f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CheckForHit(collision);
        }
    }

    void CheckForHit(Collider2D collision)
    {
        player = collision.gameObject;
        playerController = player.GetComponent<PlayerController>();
        playerTransform = playerController.transform;
        action = playerController.actionState;

        if (action != PlayerController.ActionState.None)
        {
            ProcessHit();
        }
    }

    void ProcessHit()
    {
        if (hitApplied) return;

        hitApplied = true;
        appliedPower = playerController.appliedPower;
        Invoke(nameof(ResetHit), 0.2f);

        switch (action)
        {
            case PlayerController.ActionState.Punch:
                Punch();
                break;
            case PlayerController.ActionState.Kick:
                Kick();
                break;
            case PlayerController.ActionState.Defence:
                Defence();
                break;
            default:
                break;
        }
    }

    void Punch()
    {
        direction = transform.position - playerTransform.position;
        direction = direction.normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * appliedPower, ForceMode2D.Impulse);
    }

    void Kick()
    {
        direction = transform.position - playerTransform.position;
        direction = direction.normalized;
        direction.y *= -1;
        direction.y = Mathf.Clamp(direction.y, 0.2f, 1);
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * appliedPower, ForceMode2D.Impulse);
    }

    void Defence()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * appliedPower, ForceMode2D.Impulse);
    }

    void ResetHit() { hitApplied = false; }

    public void ResetBall(Vector3 nextServe)
    {
        firstPunchDone = false;
        rb.gravityScale = 0;

        rb.velocity = Vector2.zero;
        transformBall.position = nextServe;
        
    }
}
