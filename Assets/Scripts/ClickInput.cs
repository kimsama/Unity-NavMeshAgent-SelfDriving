using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class ClickInput : MonoBehaviour 
{
    public string LayerGround = "Ground";
    public NavAgentController navController;

    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

	void Update () 
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            
            int groundLayer = 1 << LayerMask.NameToLayer(LayerGround);

            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                navController.Destination = hit.point;
            }
        }
	}
}
