using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class StateMachineSimple : MonoBehaviour
{
    public enum State { Idle, Patrol, Search, Chase, Investigate, Flee };

    [Header("Scene References")]
    public GameObject character;
    public Transform[] waypoints;
    public Transform farAway;

    [Header ("Config Values")]
    public float idleThreshold = 1.0f;
    public float waypointThreshold = 1.5f;
    public float searchThreshold = 5.0f;
    public float investigateThreshold = 5.0f;
    public float fleeThreshold = 1.5f;
    public float viewRadius = 10f;
    public float viewAngle = 60f;
    public float investigateDistance = 1.5f;


    public LayerMask soundLayer;
    public LayerMask lightLayer;

    float idleTime = 0.0f;
    float searchTime = 0f;
    float investigateTime = 0f;
    int waypointIndex = 0;


    NavMeshAgent agent;

    State state = State.Idle;

    bool viewEnabled = false;
    bool canSeePlayer = false;

    bool soundHeard = false;
    bool scared = false;
    Vector3 soundLocation = Vector3.zero;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        state = State.Idle;
        idleTime = Time.time;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Search:
                Search();
                break;
            case State.Investigate:
                Investigate();
                break;
            case State.Flee:
                Flee();
                break;


        }
    }

    public void SoundTrigger (SoundObject soundObject)
    {
        soundHeard = true;
        soundLocation = soundObject.transform.position;
    }

    public void RunAway()
    {
        scared = true;

        state = State.Flee;
    }

    void Idle()
    {
        viewEnabled = false;

        float timeElapsed = Time.time - idleTime;
        if (timeElapsed > idleThreshold)
        {
            EnterPatrol();
        }
    }

    void EnterPatrol()
    {
        state = State.Patrol;
        waypointIndex++;
        if (waypointIndex >= waypoints.Length) waypointIndex = 0;
        agent.speed = 15f;
    }

    void Patrol()
    {
        Vector3 waypoint = waypoints[waypointIndex].position;
        
        agent.SetDestination(waypoint);
        

        viewEnabled = true;
        canSeePlayer = InViewCone();

        if (Vector3.Distance(transform.position, waypoint) < waypointThreshold)
        {

            state = State.Idle;
            idleTime = Time.time;
        }

        if (canSeePlayer)
        {
            state = State.Chase;
        }

        if(soundHeard)
        {
            EnterInvestigate();
        }
    }

    void Chase()
    {
        agent.SetDestination(character.transform.position);
        agent.speed = 25f;

        canSeePlayer = InViewCone();
        if (!canSeePlayer)
        {
            state = State.Search;
            searchTime = Time.time;
        }
    }

    

    void Search()
    {
        agent.SetDestination(transform.position + transform.forward/2 + transform.right/2);

        float elapsedSearchTime = Time.time - searchTime;
        if (elapsedSearchTime > searchThreshold)
        {
            EnterPatrol();
        }

        canSeePlayer = InViewCone();
        if (canSeePlayer)
        {
            state = State.Chase;
        }

        if (soundHeard)
        {
            EnterInvestigate();
        }
    }

    void EnterInvestigate()
    {
        state = State.Investigate;
        investigateTime = Time.time;
        agent.speed = 10f;
    }

    void Investigate()
    {
        agent.SetDestination(soundLocation);

        float distance = Vector3.Distance(transform.position, soundLocation);
        if (distance <= investigateDistance)
        {
            float timeElapsed = Time.time - investigateTime;

            transform.Rotate(Vector3.up, Mathf.Sin(Time.time) * Time.deltaTime * 45f);


            if (timeElapsed > investigateThreshold)
            {
                soundHeard = false;
                EnterPatrol();
            }
        }
        else
        {
            investigateTime = Time.time;
        }

        canSeePlayer = InViewCone();
        if (canSeePlayer )
        {
            soundHeard = false;
            state = State.Chase;
        }
    }

    void Flee()
    {
        //makes the spider run back to their spawn position through the 
        //waypoint they start on.
        Vector3 location = farAway.position;
        agent.SetDestination(location);
        Debug.Log(Vector3.Distance(transform.position, location));
        if (Vector3.Distance(transform.position, location) < fleeThreshold)
        {
            
            //we turn soundHeard off here too to avoid the spider getting 
            //caught in a loop of trying to go to the sound, only to keep
            //running into a safe zone.
            scared = false;
            soundHeard = false;
            state = State.Idle;
        }
        
    }


    bool InViewCone()
    {

        if (Vector3.Distance(transform.position, character.transform.position) > viewRadius) return false;

        Vector3 npcToCharacter = character.transform.position - transform.position;
        if (Vector3.Angle(transform.forward, npcToCharacter) > 0.5f * viewAngle) return false;

        Vector3 toCharacterDir = npcToCharacter.normalized;
        if (Physics.Raycast(transform.position, toCharacterDir, out RaycastHit ray, viewRadius, ~soundLayer | ~lightLayer))
        {
            return ray.transform == character.transform;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            SceneManager.LoadScene("Caught");
        }
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint.position, 0.5f);
        }

        if (viewEnabled)
        {
            Handles.color = new Color(0f, 1f, 1f, 0.25f);

            if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, viewAngle / 2, viewRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -viewAngle / 2, viewRadius);
        }
    }
}
