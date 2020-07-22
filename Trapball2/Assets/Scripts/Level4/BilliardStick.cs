using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliardStick : MonoBehaviour
{
    Vector3 xRecoil = new Vector3(1.65f, 0, 0);
    Vector3 target;
    Vector3 initPos;
    float speed = 1;
    Player plScript;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        target = transform.position - xRecoil;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if(transform.position == target)
        {
            //Significa que ha retrocedido
            if(transform.position.x < initPos.x)
            {
                target = transform.position + xRecoil;
                speed = 12;
            }
            else
            {
                target = transform.position - xRecoil;
                speed = 1;
            }
        }
        //transform.Translate(transform.up * 2 * Time.deltaTime, Space.World);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
    //        plScript = collision.gameObject.GetComponent<Player>();
    //        rb.AddForce(transform.up * 20, ForceMode.Impulse);
    //        StopAllCoroutines();
    //        StartCoroutine(UnlimitBallSpeed());
    //    }
    //}
    //IEnumerator UnlimitBallSpeed()
    //{
    //    plScript.limited = false;
    //    yield return new WaitForSeconds(1.5f);
    //    plScript.limited = true;
    //}
}
