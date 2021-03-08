using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamCtrl : MonoBehaviour
{
    public float zoomOutMax = 10;
    public float zoomInMax = 5;

    public bool isStartDrag = false;
    public CinemachineVirtualCamera cineCamera;

    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    // Start is called before the first frame update
    void Start()
    {
    }
    public Vector3? startPos;
    // Update is called once per frame
    void Update()
    {
        if (cineCamera.Follow == null)
        {

            if (Input.GetMouseButtonDown(0) && !isStartDrag)
            {
                dragOrigin = Input.mousePosition;
                isStartDrag = true;
                return;
            }

            if (!Input.GetMouseButton(0) && isStartDrag)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

                Vector3 posMain = Camera.main.transform.position;
                if ((posMain - cineCamera.transform.position).sqrMagnitude > 1)
                {
                    cineCamera.transform.position = posMain;
                }
                cineCamera.transform.Translate(move, Space.World);
                isStartDrag = false;
            }


            // if (Input.GetMouseButton(0))
            // {
            //     if (startPos != null)
            //     {
            //         Vector3 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //         Vector3 deltaMove = (Vector3)startPos - endPos;
            //         deltaMove.z = 0;
            //         Debug.Log("Camera Move " + deltaMove);

            //         // Vector3 posMain = Camera.main.transform.position;
            //         // if ((posMain - cineCamera.transform.position).sqrMagnitude > 1)
            //         // {
            //         //     cineCamera.transform.position = posMain;
            //         // }
            //         cineCamera.transform.Translate(deltaMove);
            //         startPos = endPos;
            //         // cineCamera.transform.position = new Vector3(
            //         //     cineCamera.transform.position.x + deltaMove.x,
            //         //     cineCamera.transform.position.y + deltaMove.y,
            //         //     cineCamera.transform.position.z
            //         // );
            //     }
            //     else
            //     {
            //         startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //         Debug.Log("Camera First Start Pos");
            //     }


            // }
            // else if (startPos != null)
            // {
            //     startPos = null;
            //     Debug.Log("Camera Reset ");
            // }
        }
        // if (Input.touchCount == 1)
        // {
        //     // touch on screen
        //     if (Input.GetTouch(0).phase == TouchPhase.Began)
        //     {
        //         startPos = Input.mousePosition;
        //         Debug.Log("Camera Start Pos");
        //     }


        //     // release touch/dragging
        //     if ((Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
        //     {
        //         Debug.Log("Camera End Pos");
        //         Vector3 endPos = Input.mousePosition;
        //         cineCamera.Follow = null;
        //         cineCamera.transform.position = new Vector3(
        //             cineCamera.transform.position.x + endPos.x - startPos.x,
        //             cineCamera.transform.position.y + endPos.y - startPos.y,
        //             cineCamera.transform.position.z
        //         );
        //     }
        // }
    }
}
