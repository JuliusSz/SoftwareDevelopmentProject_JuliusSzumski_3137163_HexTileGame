using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class NewBehaviourScript : MonoBehaviour
{
    public CharacterController controller;
    public float cameraSpeed;
    public float minZoom;
    public float maxZoom;
    public float zoomSensitivity;
    public float screenRim;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        zoomSensitivity *= 100;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 zoom = gameObject.transform.forward * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity;

        if (transform.position.y + zoom.y <= maxZoom && transform.position.y + zoom.y >= minZoom)
        {
            controller.Move(zoom);
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * cameraSpeed);

        if (Input.mousePosition.x >= Screen.width - screenRim)
        {
            controller.Move(new Vector3(Time.deltaTime * cameraSpeed, 0,0));
        }

        if (Input.mousePosition.x <= screenRim)
        {
            controller.Move(new Vector3((Time.deltaTime * cameraSpeed)*-1, 0, 0));
        }

        if (Input.mousePosition.y >= Screen.height - screenRim)
        {
            controller.Move(new Vector3(0, 0, Time.deltaTime * cameraSpeed));
        }

        if (Input.mousePosition.y <= screenRim)
        {
            controller.Move(new Vector3(0, 0, (Time.deltaTime * cameraSpeed) * -1));
        }

    }
}
