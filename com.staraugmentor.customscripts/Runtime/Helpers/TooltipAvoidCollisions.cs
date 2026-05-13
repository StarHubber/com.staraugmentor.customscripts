using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TooltipAvoidCollisions : MonoBehaviour
{
    public LayerMask tooltipLayer;
    public float pushAmount = 0.02f;
    public int maxIterations = 10;

    private BoxCollider col;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    private void LateUpdate()
    {
        Resolve3DCollision();
    }

    void Resolve3DCollision()
    {
        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Collider in aktueller Ausrichtung abfragen
            Collider[] hits = Physics.OverlapBox(
                col.bounds.center,
                col.bounds.extents,
                transform.rotation,
                tooltipLayer
            );

            if (hits.Length <= 1)
                return; // nur unser eigener Collider → kein Problem

            foreach (Collider other in hits)
            {
                if (other == col) continue;

                Vector3 direction = (transform.position - other.transform.position).normalized;

                // Falls genau gleiche Position (kommt selten vor)
                if (direction == Vector3.zero)
                    direction = transform.right;

                // Nur auf X & Y verschieben (keine Tiefe!)
                direction.z = 0;

                transform.position += direction * pushAmount;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!col) return;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(col.center, col.size);
    }
}

