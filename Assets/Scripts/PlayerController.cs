using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public enum State
{
    Idle,
    Moving,
    Minning
}
public struct Tags
{
    public const string Terrain = "Terrain";
    public const string Ressource = "MapRessource";
    public const string Building = "Building";
    public const string Entry = "Entry";
}

public struct Annimations
{
    public const string Idle = "idle1";
    public const string Run = "Run";
    public const string Mine = "Attack1";
}

public class PlayerController : MonoBehaviour
{
    //Navigation & Annimation
    private InputActions input;
    private NavMeshAgent agent;
    Animator animator;

    State state, lastState = State.Idle;

    string destinationTag = "";
    Ressource destinationRessource = null;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;
    [SerializeField] Inventory inventory;
    float lookRotationSpeed = 8f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        input = new();

        //Assign Inputs :
        input.Main.RightClick.performed += SetDestination;
    }


    void Update()
    {
        if (HasArrived(Tags.Terrain))
        {
            state = State.Idle;
        }
        else if (HasArrived(new[] { Tags.Ressource, Tags.Building }))
        {
            state = State.Minning;
            StartCoroutine(FaceEntry());
            StartCoroutine(Mine());
        }

        if (StateChanged())
            UpdateAnnimation();

        lastState = state; //Save last state
    }
    private void SetDestination(InputAction.CallbackContext context)
    {
        //Get Destination from mouse click :
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100, clickableLayers))
        {
            Vector3 destination = hit.point;
            //Set destination as the position of child with tag "Entry"
            if (hit.collider.CompareTag(Tags.Ressource) || hit.collider.CompareTag(Tags.Building))
            {
                foreach (Transform t in hit.collider.transform)
                {
                    if (!t.CompareTag(Tags.Entry)) continue;
                    destination = t.position;
                    break;
                }
                if (hit.collider.CompareTag(Tags.Ressource))
                    destinationRessource = hit.collider.GetComponent<Ressource>();
            }
            agent.destination = destination;
            destinationTag = hit.collider.tag;
        }
        //Play click effect :
        if (clickEffect != null)
            Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);

        //check if already at destination :
        if (Vector3.SqrMagnitude(transform.position - agent.destination) < 0.1f) return;
        state = State.Moving;
        StartCoroutine(FaceDestination());

    }

    private void UpdateAnnimation()
    {
        switch (state)
        {
            case State.Idle:
                animator.Play(Annimations.Idle);
                break;
            case State.Moving:
                animator.Play(Annimations.Run);
                break;
            case State.Minning:
                animator.Play(Annimations.Mine);
                break;
            default:
                animator.Play(Annimations.Idle);
                break; ;
        }
    }
    // Bools 
    private bool IsMoving() => agent.velocity.sqrMagnitude > Mathf.Epsilon;
    private bool HasArrived() => agent.remainingDistance <= agent.stoppingDistance && !agent.hasPath && !IsMoving() && state == State.Moving;
    private bool HasArrived(string destination)
    {
        return HasArrived() && destinationTag == destination;
    }
    private bool HasArrived(IEnumerable<string> destinations)
    {
        return HasArrived() && destinations.Any(destination => destinationTag == destination);
    }
    private bool StateChanged() => state != lastState;

    private bool StartedMoving() => state == State.Moving && lastState != State.Moving;

    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }

    //Rotation Coroutine :
    IEnumerator FaceDestination()
    {
        while (state == State.Moving)
        {
            Quaternion lookRotation = agent.velocity.sqrMagnitude > Mathf.Epsilon ? Quaternion.LookRotation(agent.velocity.normalized) : transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
            yield return null;
        }
    }

    //Entry ROTATION COROUTINE :
    IEnumerator FaceEntry()
    {
        while (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis(0, Vector3.forward)) > .01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(0, Vector3.forward), Time.deltaTime * lookRotationSpeed);
            yield return null;
        }
    }

    IEnumerator Mine()
    {
        Debug.Log("Start mining corotine");
        while (state == State.Minning)
        {
            Debug.Log("Mining");
            if (destinationRessource != null)
            {
                inventory.AddRessource(destinationRessource.Type, 1);
                Debug.Log("Ressource mined : " + destinationRessource.Type);
            }
            yield return new WaitForSeconds(1);
        }

    }

}
