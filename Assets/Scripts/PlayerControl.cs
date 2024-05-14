using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
using UnityStandardAssets.ImageEffects;

public class PlayerControl : MonoBehaviour
{
    public bool dead = false;
    public float health = 100;
    bool running = false;
    public float playerSpeed = 5;
    float mouseX, mouseY;
    public float inputX, inputY;
    public CharacterController myChar;
    public Animator camAnim;
    public Transform weaponCam;

    public CanvasGroup viggnette;
    public GameObject deadObject;


    public AudioSource footstep;
    public AudioSource pantFast;
    public AudioSource pantSlow;

    public BloomOptimized bloom;

    Vector2 _lookDir;
    Vector3 _moveDir;

    // Start is called before the first frame update
    void Start()
    {
        //fix offset
        _lookDir.y = transform.eulerAngles.y;
        //graphics
        if (PlayerPrefs.HasKey("Graphics"))
        {
            if (PlayerPrefs.GetInt("Graphics") > 0)
            {
                bloom.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //dead
        if (health <= 0 && dead == false)
        {
            dead = true;
            deadObject.SetActive(true);
            Invoke("ResetGame", 5f);
        }
        //return
        if (health <= 0)
        {
            footstep.volume = 0;
            pantFast.volume = 0;
            pantSlow.volume = 0;
            return;
        }
        Movement();
        CameraAnimation();
        //regen
        health = Mathf.MoveTowards(health, 100, 10f * Time.deltaTime);
    }

    void CameraAnimation()
    {
        float joy = _moveDir.magnitude / playerSpeed;
        camAnim.SetLayerWeight(1, joy);
        //footstep
        if (joy > 0.3f)
        {
            footstep.pitch = Mathf.Clamp(joy, 0.8f, 1f) + 0.2f;
        }
        else
        {
            footstep.pitch = 0;
        }
        //panting
        if (health > 90)
        {
            pantFast.volume = Mathf.MoveTowards(pantFast.volume, 0f, 0.5f * Time.deltaTime);
            pantSlow.volume = Mathf.MoveTowards(pantSlow.volume, 0f, 0.5f * Time.deltaTime);
        }
        else if (health < 70)
        {
            pantFast.volume = Mathf.MoveTowards(pantFast.volume, 1f, 0.5f * Time.deltaTime);
            pantSlow.volume = Mathf.MoveTowards(pantSlow.volume, 0f, 0.5f * Time.deltaTime);
        }
        else
        {
            pantFast.volume = Mathf.MoveTowards(pantFast.volume, 0f, 0.5f * Time.deltaTime);
            pantSlow.volume = Mathf.MoveTowards(pantSlow.volume, 1f, 0.5f * Time.deltaTime);
        }
        //viggente
        viggnette.alpha = 1 - (health / 100);
    }

    void Movement()
    {
        float tempSens = 1;
        //dir to rot
        if (Application.isMobilePlatform)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width / 2)
                {
                    _lookDir += new Vector2(-touch.deltaPosition.y, touch.deltaPosition.x) * (tempSens / 2);
                    //delta touch
                    mouseX = Mathf.Lerp(mouseX, touch.deltaPosition.x, 10f * Time.fixedDeltaTime);
                    mouseY = Mathf.Lerp(mouseY, -touch.deltaPosition.y, 10f * Time.fixedDeltaTime);
                }
            }
            //drop
            mouseX = Mathf.Lerp(mouseX, 0, 30f * Time.fixedDeltaTime);
            mouseY = Mathf.Lerp(mouseY, 0, 30f * Time.fixedDeltaTime);
        }
        else
        {
            _lookDir += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * (tempSens * 2);
            //delta mouse
            mouseX = Input.GetAxis("Mouse X");
            mouseY = -Input.GetAxis("Mouse Y");
        }
        //clamp
        _lookDir.x = Mathf.Clamp(_lookDir.x, -90, 60);
        if (_lookDir.y > 360)
        {
            _lookDir.y = 0;
        }
        else if (_lookDir.y < 0)
        {
            _lookDir.y = 360;
        }
        mouseX = Mathf.Clamp(mouseX, -3, 3);
        mouseY = Mathf.Clamp(mouseY, -3, 3);
        //get input
        inputX = CnInputManager.GetAxis("Horizontal");
        inputY = CnInputManager.GetAxis("Vertical");
        //run
        if (CnInputManager.GetButton("LeftShift") && _moveDir.magnitude > 0.5f)
        {
            if (running == false)
            {
                running = true;
            }
        }
        else
        {
            if (running == true)
            {
                running = false;
            }
        }
        //if (myChar.isGrounded)
        //{
            //make dir
            _moveDir = Vector3.ClampMagnitude(transform.right * inputX + transform.forward * inputY, 1f);
            //run
            if (running)
            {
                _moveDir *= playerSpeed * 1.5f;
            }
            else
            {
                _moveDir *= playerSpeed;
            }
            if (Input.GetButton("Jump"))
            {
                //_moveDir.y = 5;
            }
        //}
        //gravity
        //_moveDir.y -= 20 * Time.deltaTime;
        // Move the controller
        myChar.Move(_moveDir * Time.deltaTime);
        //rotate
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, _lookDir.y, 0), 30f * Time.fixedDeltaTime);
        //camera
        weaponCam.localRotation = Quaternion.Lerp(weaponCam.localRotation, Quaternion.Euler(_lookDir.x, 0, 0), 30f * Time.fixedDeltaTime);
    }
}
