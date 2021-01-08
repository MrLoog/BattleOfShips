using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindInfoCtrl : MonoBehaviour
{
    public Image WindDirection;
    public float DefaultRotate;
    public Text WindForce;

    private Quaternion StartAnim;
    private Quaternion EndAnim;
    private float animateSpeed = 1f;
    private float accumAnimTime = 0f;
    private bool isAnimate = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimate)
        {
            accumAnimTime += Time.deltaTime;
            WindDirection.transform.rotation = Quaternion.Lerp(StartAnim, EndAnim, accumAnimTime / animateSpeed);
            if (accumAnimTime >= animateSpeed)
            {
                isAnimate = false;
            }
        }
    }

    public void StartAnimate(Quaternion newRotation)
    {
        StartAnim = WindDirection.transform.rotation;
        EndAnim = newRotation;
        accumAnimTime = 0;
        isAnimate = true;
    }
    public void UpdateWindForceInfo()
    {
        Vector2 windForce = GameManager.instance.windForce;
        WindForce.text = string.Format((windForce.magnitude * 10).ToString());

        Vector2 realCtrlDirection = VectorUtils.Rotate(Vector2.up, DefaultRotate, true);
        float angel = Vector2.Angle(realCtrlDirection, windForce);
        Vector3 cross = Vector3.Cross(realCtrlDirection, windForce);
        int sign = cross.z > 0 ? 1 : -1;
        
        // Debug.Log("wind force " + windForce + "/" + realCtrlDirection + "/" + sign * angel + "/" + DefaultRotate);
        StartAnimate(Quaternion.Euler(0, 0, sign * angel));
    }
}
