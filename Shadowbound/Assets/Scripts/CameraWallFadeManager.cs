using UnityEngine;
using System.Collections.Generic;

public class CameraWallFadeManager : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleMask;

    private List<WallFadeController> fadedWalls = new List<WallFadeController>();

    void LateUpdate()
    {
        // Fade back any previously faded walls
        foreach (var wall in fadedWalls)
            wall.FadeIn();
        fadedWalls.Clear();

        Vector3 dir = player.position - transform.position;
        float distance = Vector3.Distance(transform.position, player.position);

        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, distance, obstacleMask);

        foreach (RaycastHit hit in hits)
        {
            WallFadeController fade = hit.collider.GetComponent<WallFadeController>();
            if (fade != null)
            {
                fade.FadeOut();
                fadedWalls.Add(fade);
            }
        }
    }
}
