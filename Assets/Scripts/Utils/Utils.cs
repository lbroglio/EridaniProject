using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{   
    // How low the distance between an object and the collider below it for it to be considered grounded
    private static readonly float GROUNDED_THRESHOLD = 0.01f;

    /// <summary>
    /// Cast a ray downward from a transform to see if its on the ground
    /// </summary>
    public static bool IsGrounded(GameObject g){
            Ray downRay = new Ray(g.transform.position, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(downRay, out hit);

            // If the ray hit something and the distance is below the grounded threshold return true
            if(hit.collider != null 
                && hit.distance - g.GetComponent<MeshFilter>().mesh.bounds.size.y / 2 <= GROUNDED_THRESHOLD){
                return true; 
            }

            return false;
    }
}
