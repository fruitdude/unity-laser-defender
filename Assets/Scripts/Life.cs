using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour{
    float movementThisFrame = 1.0f;
    Vector2 moveTowards;

    private void Update() {
        MoveLife();
        moveTowards = new Vector2(transform.position.x, -10.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        FindObjectOfType<Player>().PlayerGetsHealed();
        Destroy(gameObject);
    }

    public void MoveLife() {
        transform.position = Vector2.MoveTowards(transform.position, moveTowards, movementThisFrame * Time.deltaTime);
    }
}
