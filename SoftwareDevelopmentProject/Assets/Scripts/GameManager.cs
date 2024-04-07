using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject SelectedTile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(SelectedTile != null)
                {
                    SelectedTile.transform.parent.GetChild(1).gameObject.SetActive(false);
                }
                SelectedTile = hit.collider.gameObject;
                SelectedTile.transform.parent.GetChild(1).gameObject.SetActive(true);
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            SelectedTile.transform.parent.GetChild(1).gameObject.SetActive(false);
            SelectedTile = null;
        }
    }
}
