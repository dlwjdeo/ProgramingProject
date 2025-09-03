using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private Collider2D groundChecker;

    public bool IsGrounded {  get; private set; }

    private void Awake()
    {
        groundChecker = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagName.Ground))
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(TagName.Ground))
        {
            IsGrounded = false;
        }
    }
}
