using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    float smoothInputMagnitude;
    public float smoothMoveTime=0.1f;
    float smoothMoveVelocity;
    public float  omega=8;
    float angle;
    Rigidbody rigidBody;
    Vector3 velocity;
    bool disabled;
    public static event System.Action OnGameWin;
    public  GameObject particles;
    public float currentTime;
    public  static float lives=1f;
    public Text livesLeft;
    bool status=false;
    public AudioSource healthPick;
    public AudioClip hp;
    public AudioSource heartbeat;
    public AudioClip hb;
    
    void Start()
    {
        rigidBody=GetComponent<Rigidbody>();
        Guard.OnGuardSpot += Disable;
        healthPick.clip=hp;
        heartbeat.clip=hb;
         if(lives==0){
            heartbeat.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction=Vector3.zero;
        if(!disabled){
             direction= new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;
        }
        smoothInputMagnitude=Mathf.SmoothDamp(smoothInputMagnitude,direction.magnitude,ref smoothMoveVelocity,smoothMoveTime);
        float ang=90-Mathf.Atan2(direction.z,direction.x)*Mathf.Rad2Deg;
        angle=Mathf.LerpAngle(angle,ang,omega*Time.deltaTime*direction.magnitude);
        velocity=transform.forward*speed*smoothInputMagnitude;
        if(lives>0){
            heartbeat.Stop();
        }
        
        
    }

    void  FixedUpdate()
    {
        rigidBody.MoveRotation(Quaternion.Euler(Vector3.up*angle));
        //transform.Translate(rigidBody.position+velocity*Time.deltaTime);
        rigidBody.MovePosition(rigidBody.position+velocity*Time.deltaTime);
        if(Time.time>=currentTime+3f){
               particles.SetActive(false);
        }
        if(status){
            lives+=1;
            healthPick.Play();
            status=false;
        }
        livesLeft.text=lives.ToString();
        
    }
    void Disable()
    {
        disabled=true;
    }
    void OnDestroy()
    {
        Guard.OnGuardSpot-=Disable;
       
       // lives-=1;
    }
    void OnTriggerEnter(Collider triggerCOllider)
    {
        if(triggerCOllider.tag=="Finish")
        {
            if(OnGameWin!=null)
            {
                Disable();
                OnGameWin();
            }
        }
        else if (triggerCOllider.tag=="Heart")
        {
            Destroy(triggerCOllider.gameObject);
            particles.SetActive(true);
            status=true;
            currentTime=Time.time;
                      
        }
        
    }
    
}
