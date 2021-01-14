using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovementNonLocal : MonoBehaviour
{
    public GameObject gun;
    public Transform weaponSpawnPoint;

    Rigidbody2D rigidbody2d;
    public float speed = 3f;
    public Animator animator;
    float horizontal;
    float vertical;
    Vector2 movement;

    private Quaternion Masturbate;
    private bool isUsingWeapon = false;
    private GameObject currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Press G to take out or put back weapon
        if (!isUsingWeapon) {
          if(Masturbate != Quaternion.identity){
              currentWeapon = Instantiate(gun, weaponSpawnPoint.position, Masturbate);
              currentWeapon.transform.parent = gameObject.transform;
                //gun.transform.rotation = Masturbate;
                gun.GetComponent<WeaponNonLocal>().SetRotation(Masturbate);
              isUsingWeapon = true;
            }
        }
        else {
            if(Masturbate == Quaternion.identity){
              Destroy(currentWeapon);
              isUsingWeapon = false;
            }
            else
            {
                gun.GetComponent<WeaponNonLocal>().SetRotation(Masturbate);
                //gun.transform.rotation = Masturbate;

            }
        }
    }

    public void SetMasturbate(Quaternion rotation)
    {
        Masturbate = rotation;
    }
    // void FixedUpdate()
    // {
    //     Vector3 position = rigidbody2d.position;
    //     position.x = position.x + 3.0f * horizontal * Time.deltaTime;
    //     position.y = position.y + 3.0f * vertical * Time.deltaTime;
    //     position.z = 0;
    //     position = new Vector3(
    //       Mathf.Clamp(position.x,-22f,21.5f),
    //       Mathf.Clamp(position.y,-16f,15.5f),
    //       0
    //     );
    //
    //     rigidbody2d.MovePosition(position);
    // }

    /*void FixedUpdate(){
      Vector2 position = rigidbody2d.position;
      position = rigidbody2d.position + movement * speed * Time.fixedDeltaTime;
      position = new Vector2(
      Mathf.Clamp(position.x,-22f,21.5f), Mathf.Clamp(position.y,-16f,15.5f));
      rigidbody2d.MovePosition(position);
    }*/
}
