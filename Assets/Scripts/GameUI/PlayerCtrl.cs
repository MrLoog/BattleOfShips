using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float Speed = 1f;
    public Vector2 Target;
    private Vector2 LastPos;
    private Vector2 Direction = Vector2.down;
    private bool _isMove = false;

    public float RotateSpeed = 1f;

    private Rigidbody2D _rigidBody;


    // Start is called before the first frame update
    void Start()
    {
        LastPos = transform.position;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // FaceTarget(Target);
            _isMove = true;
        }
    }

    void FixedUpdate()
    {
        if (_isMove)
        {
            if (!transform.position.Equals(LastPos))
            {
                Debug.DrawLine(transform.position, LastPos, Color.red, 3f);
                Debug.DrawLine(transform.position, LastPos, Color.green, 3f);
                LastPos = transform.position;
            }
            if (Vector2.SqrMagnitude(Target - (Vector2)transform.position) > 0.1f)
            {
                Vector2 targetDir = Target - (Vector2)transform.position;
                float angle = Vector2.Angle(targetDir, Direction);
                Vector3 cross = Vector3.Cross(targetDir, Direction);

                int sign = cross.z > 0 ? -1 : 1;
                float deltaAngel = 0;
                if (angle > 0)
                {
                    deltaAngel = 180 * Time.deltaTime / RotateSpeed;
                    if (deltaAngel > angle) deltaAngel = angle;
                    // int sign = 1;
                    deltaAngel *= sign;
                    // Debug.Log("detal Angel " + deltaAngel);
                    Direction = Quaternion.AngleAxis(deltaAngel, new Vector3(0, 0, 1)) * Direction;
                    // _rigidBody.angularVelocity = sign * 180f / RotateSpeed;
                    transform.localRotation *= Quaternion.Euler(0, 0, deltaAngel);
                    // transform.localRotation = Quaternion.Euler(0, 0, sign * Vector2.Angle(Vector2.down,  Direction));
                }
                // else
                // {
                //     _rigidBody.angularVelocity = 0f;
                // }
                // Debug.DrawLine(transform.position, transform.position + (Vector3)Direction.normalized, Color.green, 1f);
                // Debug.Log("Direction " + Direction.normalized.ToString());

                // Debug.DrawLine(transform.position, transform.position + (Vector3)Direction.normalized, Color.red, 1f);
                _rigidBody.velocity = Direction.normalized * Speed;
                // _rigidBody.AddForce(targetDir.normalized * Speed);
                // _rigidBody.velocity = targetDir.normalized * Speed;
                // transform.position = Vector2.Lerp(transform.position, Target, Time.deltaTime * Speed / Vector2.Distance(Target, transform.position));
            }
            else
            {
                _rigidBody.velocity = Vector2.zero;
                _isMove = false;
            }
        }
    }

    public void FaceTarget(Vector2 target)
    {
        if (Vector2.SqrMagnitude(target - (Vector2)transform.position) > 0f)
        {
            Vector2 targetDir = target - (Vector2)transform.position;
            // Debug.Log(targetDir);
            // Debug.Log(transform.rotation.eulerAngles);
            float angle = Vector2.Angle(targetDir, Vector2.down);
            // Debug.Log(angle);
            // Debug.DrawLine(transform.position, target, Color.green, 1f);
            // Debug.DrawLine(transform.position, transform.position + new Vector3(0, -1, 0), Color.red, 1f);
            // Debug.Log(string.Format("calculate angle {0} of {1} and {2}", angle, targetDir, Vector3.down));
            // Debug.Log(string.Format("calculate {0} target {1}/self{2}", target.x - transform.position.x, target.x, transform.position.x));
            transform.localRotation = Quaternion.Euler(0, 0, angle * (target.x - transform.position.x > 0 ? 1 : -1));

        }
    }


}
