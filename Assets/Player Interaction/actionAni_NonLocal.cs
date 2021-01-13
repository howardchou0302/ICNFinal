﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class actionAniNonLocal : MonoBehaviour
{

    public Animator anim;
    public Animation animation;
    public CharacterStats cs;
    public Rigidbody2D rb;
    public int increment;

    public CPB coalSlider;
    public CPB waterSlider;
    public CPB metalSlider;
    // Start is called before the first frame update
    void Start()
    {
      anim = gameObject.GetComponent<Animator>();
      animation = gameObject.GetComponent<Animation>();
      rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
      if(Input.GetButtonDown("Horizontal")||Input.GetButtonDown("Vertical")){
        cs = CharacterStats.Walk;
      }else if(Input.GetButtonUp("Horizontal")||Input.GetButtonUp("Vertical")){
        cs = CharacterStats.Idle;
      }
      if(cs == CharacterStats.Idle){
        anim.SetBool("walk",false);
      }
      if(cs == CharacterStats.Walk){
        anim.SetBool("walk",true);
      }
    }

    IEnumerator actionTime(){
      yield return new WaitForSeconds(2);
      anim.SetBool("coal",false);
      anim.SetBool("metal",false);
      anim.SetBool("water",false);
      anim.SetBool("develop",true);
      yield return new WaitForSeconds(1);
      rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
    }

    // Start the animation while colliding and pressing Space
    void OnCollisionStay2D(Collision2D aaa) //aaa為自定義碰撞事件
    {
        if (aaa.gameObject.tag == "Coal" && Input.GetKeyDown(KeyCode.Space)){
            anim.SetBool("coal",true);
            rb.constraints = RigidbodyConstraints2D.FreezePosition| RigidbodyConstraints2D.FreezeRotation;
            anim.SetBool("increase_coal",true);
            StartCoroutine(actionTime());
        }
        if (aaa.gameObject.tag == "Metal" && Input.GetKeyDown(KeyCode.Space)){
          anim.SetBool("metal",true);
          rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
          anim.SetBool("increase_metal",true);
          StartCoroutine(actionTime());
        }
        if (aaa.gameObject.tag == "lab" && Input.GetKeyDown(KeyCode.Space)){
          anim.SetBool("develop",false);
          if(anim.GetBool("increase_coal")){
            //Vector3 _localCollection = new Vector3(increment,0,0);
            coalSlider.UpdateAmount(increment);
            anim.SetBool("increase_coal",false);
          }
          if(anim.GetBool("increase_metal")){
            //Vector3 _localCollection = new Vector3(0,increment,0);
            metalSlider.UpdateAmount(increment);
            anim.SetBool("increase_metal",false);
          }
          if(anim.GetBool("increase_water")){
            //Vector3 _localCollection = new Vector3(0, 0, increment);
            waterSlider.UpdateAmount(increment);
            anim.SetBool("increase_water",false);
          }
        }

    }
    void OnTriggerStay2D(Collider2D aaa){
      if (aaa.gameObject.tag == "Water" && Input.GetKeyDown(KeyCode.Space)){
        anim.SetBool("water",true);
        rb.constraints = RigidbodyConstraints2D.FreezePosition |RigidbodyConstraints2D.FreezeRotation;
        anim.SetBool("increase_water",true);
        StartCoroutine(actionTime());
      }
    }
}
