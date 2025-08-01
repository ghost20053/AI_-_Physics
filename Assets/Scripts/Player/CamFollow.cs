using System;
using UnityEngine;

public class CamFollow : MonoBehaviour
{

   public Transform player;

   public float playerMouseSens;

   [SerializeField] float cameraVerticalRotation;

   public bool lockCursor;

   private void Start()
   {
        // Setting cursor visibility and camera locking
      Cursor.visible = false;

      Cursor.lockState = CursorLockMode.Locked;

   }


   private void Update()
   {
        // Checking if game is paused
        if(PauseMenu.isPaused)
        {
            return;
        }

        // If Game not pause move camera with mouse
      float inputX = Input.GetAxis("Mouse X") * playerMouseSens;
      float inputY = Input.GetAxis("Mouse Y") * playerMouseSens;


      cameraVerticalRotation -= inputY;
      cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
      transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
      
      player.Rotate(Vector3.up * inputX);
      


   }
}
