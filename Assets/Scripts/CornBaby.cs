using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornBaby : MonoBehaviour
{
    public bool dead = false;
    public float health = 100;

    public float speed;
    //public bool attacking = false;

    public Animator anim;
    public Rigidbody myRigid;
    public SkinnedMeshRenderer myRender;
    public PlayerControl _player;

    public AudioSource footstep;
    public AudioSource scream;
    public AudioSource attack;

    public GameObject prefabSpawn;
    public ParticleSystem prefabFade;

    public Cornfield _cf;

    Vector3 _moveDir;
    float dirNoise;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("RandomDir", 0, 5);
    }

    void RandomDir()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        _moveDir = randomDir;
    }

    private void LateUpdate()
    {
        if (health <= 0)
        {
            myRender.SetBlendShapeWeight(0, 100f);
        }
        else
        {
            myRender.SetBlendShapeWeight(0, 0f);
        }
    }

    private void FixedUpdate()
    {
        //gravity
        //_moveDir.y = myRigid.velocity.y;
        //_moveDir.y -= 1 * Time.fixedDeltaTime;
        _moveDir.y = 0;
        //move
        myRigid.velocity = _moveDir * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.dead)
        {
            Destroy(gameObject);
        }
        //death
        if (health <= 0 && dead == false)
        {
            dead = true;
            Invoke("SpawnMeBack", 2.3f);
            Invoke("ScoreMe", 2.3f);
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && !anim.IsInTransition(0))
            {
                anim.CrossFade("Death", 0.1f);
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            _moveDir = Vector3.zero;
            myRigid.transform.rotation = Quaternion.Lerp(myRigid.transform.rotation, Quaternion.LookRotation(_player.transform.position - transform.position), 2f * Time.deltaTime);
            footstep.pitch = 0;
            return;
        }
        //dir noise
        dirNoise = (0.5f - Mathf.PingPong(Time.time, 1f)) * 2;
        //look at
        Vector3 velDir = myRigid.velocity;
        velDir.y = 0;
        if (velDir != Vector3.zero)
        {
            myRigid.transform.rotation = Quaternion.Lerp(myRigid.transform.rotation, Quaternion.LookRotation(velDir), 15f * Time.deltaTime);
        }
        //anim
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), myRigid.velocity.magnitude / speed, 2f * Time.deltaTime));
        //footstep
        footstep.pitch = myRigid.velocity.magnitude / speed;
        //angle
        float angle = Vector3.Angle((transform.transform.position - _player.transform.position), _player.transform.right);
        float angle2 = Vector3.Angle((transform.transform.position - _player.transform.position), _player.transform.forward);
        if (angle2 > 90)
        {
            angle = 360 - angle;
        }
        //print(Mathf.Round(angle));
        //dist
        float dist = (_player.transform.position - transform.position).magnitude;
        //attack
        if (dist < 5)
        {
            _player.health -= 25f * Time.deltaTime;
        }
        //die
        if (dist < 5 && angle > 70 && angle < 110) 
        {
            health -= 50 * Time.deltaTime;
            if (!prefabFade.isPlaying)
            {
                prefabFade.Play();
            }
            scream.volume = Mathf.MoveTowards(scream.volume, 1f, 0.1f * Time.deltaTime);
            attack.volume = Mathf.MoveTowards(attack.volume, 0f, 0.5f * Time.deltaTime);
            _moveDir = (transform.position - _player.transform.position).normalized + (transform.right * dirNoise);
        }
        else
        {
            scream.volume = Mathf.MoveTowards(scream.volume, 0f, 0.5f * Time.deltaTime);
            if (prefabFade.isPlaying)
            {
                prefabFade.Stop();
            }
            //attack
            if (dist < 10 && angle > 110 || angle > 0 && angle < 70)
            {
                attack.volume = Mathf.MoveTowards(attack.volume, 1f, 0.1f * Time.deltaTime);
                _moveDir = (_player.transform.position - transform.position).normalized + (transform.right * dirNoise);
            }
            else
            {
                attack.volume = Mathf.MoveTowards(attack.volume, 0f, 0.5f * Time.deltaTime);
            }
        }
        //spawn back
        if (dist > 30)
        {
            SpawnMeBack();
        }
    }

    void SpawnMeBack()
    {
        health = 100;
        dead = false;
        scream.volume = 0;
        GameObject spark = Instantiate(prefabSpawn, transform.position, Quaternion.identity);
        Destroy(spark, 3);
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            transform.position = _player.transform.position + (_player.transform.forward * 20);
        }
        else
        {
            transform.position = _player.transform.position - (_player.transform.forward * 20);
        }
    }

    void ScoreMe()
    {
        _cf.score++;
    }
}
