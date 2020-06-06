using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pathHolder;
    public float speed;
    public float waitTime;
    public float omega;
    public Light spotLight;
    public float viewDistance;
    float viewAngle;
    Transform player;
    public LayerMask viewMask;
    Color OGSpot;
    public float timeToSpotPlayer=0.5f;
    float playerVisibleTimer;
    public static event System.Action OnGuardSpot;
    public AudioSource alarm;
    public AudioClip al;
    int stats=0;
    
    void Start()
    {
     player=GameObject.FindGameObjectWithTag("Player").transform;
     viewAngle=spotLight.spotAngle;
     OGSpot=spotLight.color;
     Vector3[] waypoints=new  Vector3[pathHolder.childCount];
     for(int i=0;i<pathHolder.childCount;i++){
         waypoints[i]=pathHolder.GetChild(i).position;
         }
     StartCoroutine(FollowPath(waypoints));
     alarm.clip=al;

    }

    // Update is called once per frame
    void Update()
    {
       if (CanSeePlayer())
       {
           playerVisibleTimer+=Time.deltaTime;
           if (stats!=1)
           {
            alarm.Play();
            stats=1;
           }
       }
       else{
           playerVisibleTimer-=Time.deltaTime;
           alarm.Stop();
           stats=2;
       }
      
       playerVisibleTimer=Mathf.Clamp(playerVisibleTimer,0,timeToSpotPlayer);
       spotLight.color=Color.Lerp(OGSpot,Color.red,playerVisibleTimer/timeToSpotPlayer);
       if(playerVisibleTimer>=timeToSpotPlayer){
           if(OnGuardSpot!=null){
               OnGuardSpot();
               
           }
       }
    }
    bool CanSeePlayer(){
        if(Vector3.Distance(transform.position,player.position)<viewDistance){
            Vector3 dirToPlayer=(player.position-transform.position).normalized;
            if (Vector3.Angle(transform.forward,dirToPlayer)<viewAngle/2f){
                if (!Physics.Linecast(transform.position,player.position,viewMask)){
                    return true;
                }
            }

        }
        return false;
    
    }
       void OnDrawGizmos(){
        Vector3 startPosition=pathHolder.GetChild(0).position;
        Vector3 previousPosition=startPosition;
        foreach(Transform waypoint in pathHolder){
            Gizmos.DrawSphere(waypoint.position,0.3f);
            Gizmos.DrawLine(previousPosition,waypoint.position);
            previousPosition=waypoint.position;
        }
        Gizmos.DrawLine(previousPosition,startPosition);
        Gizmos.color=Color.red;
        Gizmos.DrawRay(transform.position,transform.forward*viewDistance);

    }
   
    IEnumerator FollowPath(Vector3[] waypoints)
   {
     int i=1;
     transform.position=waypoints[0];
     transform.LookAt(waypoints[1]);
     while (true){
             transform.position=Vector3.MoveTowards(transform.position,waypoints[i],speed*Time.deltaTime);
        if (transform.position==waypoints[i]){
            i++;
            if(i==pathHolder.childCount){
                i=0;
            }
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(RotateTo(waypoints[i])); 
        }
        yield return null; 
     }

   }
   IEnumerator RotateTo(Vector3 rotateTarget)
   {
       Vector3 dirToTarget=(rotateTarget-transform.position).normalized;
       float targetAngle=90-Mathf.Atan2(dirToTarget.z,dirToTarget.x)*Mathf.Rad2Deg;
       while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y,targetAngle))>0.05f){
           float angleToRotate = Mathf.MoveTowardsAngle(transform.eulerAngles.y,targetAngle,omega*Time.deltaTime);
           transform.eulerAngles=Vector3.up*angleToRotate;
           yield return null;
       }
   }
}
