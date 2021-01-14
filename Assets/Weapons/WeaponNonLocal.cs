using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponNonLocal : MonoBehaviour {

    public float offset;

    public GameObject projectile;
    public GameObject shotEffect;
    public Transform shotPoint;
    // public GameObject Player;
    // public Animator camAnim;

    private float timeBtwShots;
    public float startTimeBtwShots;
    private Quaternion rotation;
    private void start()
    {
        transform.rotation = Quaternion.identity;
    }
    private void Update()
    {
        // Handles the weapon facing direction
        // facingDirection facingDirection = Player.GetComponent<facingDirection>();
        // if (facingDirection.facing == "left")
        // {
        //     transform.Rotate(0, 0, 0);
        // }
        // else
        // {
        //     transform.Rotate(0, 180, 0);
        // }

        // Handles the weapon rotation

        //transform.rotation = rotation;
        transform.gameObject.transform.localRotation = rotation;
        /*
        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                Instantiate(shotEffect, shotPoint.position, Quaternion.identity);
                // camAnim.SetTrigger("shake");
                Instantiate(projectile, shotPoint.position, transform.rotation);
                timeBtwShots = startTimeBtwShots;
            }
        }
        else {
            timeBtwShots -= Time.deltaTime;
        }
       */
    }

    public void SetRotation(Quaternion _rotation)
    {
        //transform.rotation.z = _rotation.z * 100;
        //transform.rotation.w = _rotation.w * 100;
        rotation = _rotation;
        transform.gameObject.transform.localRotation = _rotation;
        
    }
}