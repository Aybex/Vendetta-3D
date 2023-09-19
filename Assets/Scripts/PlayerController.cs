using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //Strings :
    //Annimations
    const string Idle = "idle1";
    const string Run = "Run";
    const string Mine = "Attack1";

    public struct Tags
    {
        public const string Terrain = "Terrain";
        public const string Ressource = "MapRessource";
        public const string Building = "Building";
        public const string Entry = "Entry";
    }



    //Navigation & Annimation
    private CustomActions input;
    private NavMeshAgent agent;
    Animator animator;

    //TODO : Come up with a better name than arrived to avoid confusion with HasArrived()
    bool arrived = true;
    string destinationTag = "";

    [Header("Movement")]
    [SerializeField]
    ParticleSystem clickEffect;
    [SerializeField]
    LayerMask clickableLayers;
    float lookRotationSpeed = 8f;

    bool isMinning = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        input = new CustomActions();
        agent = GetComponent<NavMeshAgent>();

        //Assign Inputs :
        input.Main.Move.performed += SetDestination;
    }

    private void SetDestination(InputAction.CallbackContext context)
    {
        //Get Destination from mouse click :
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100, clickableLayers))
        {
            Vector3 destination = hit.point;
            //   Debug.Log("Clicked on " + hit.collider.gameObject.name + " : " + hit.collider.tag);
            //Set destination as the position of child with tag "Entry"
            if (hit.collider.CompareTag(Tags.Ressource) || hit.collider.CompareTag(Tags.Building))
            {
                foreach (Transform t in hit.collider.transform)
                {
                    if (!t.CompareTag(Tags.Entry)) continue;
                    destination = t.position;
                    break;
                }
            }

            agent.destination = destination;
            destinationTag = hit.collider.tag;
        }
        //Play click effect :
        if (clickEffect != null)
            Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);

        isMinning = false;
    }

    void Update()
    {
        //Face Target :
        if (IsMoving())
        {
            arrived = false;
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }

        if (!arrived && (HasArrived(Tags.Ressource) || HasArrived(Tags.Building)))
        {

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(0, Vector3.forward), Time.deltaTime * lookRotationSpeed);

                isMinning = true;
            //Check if rotation is done :
            if (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis(0, Vector3.forward)) < Mathf.Epsilon)
            {
                arrived = true;
            }

        }

        //Set annimation :
        animator.Play(isMinning ? Mine : (IsMoving() ? Run : Idle));
    }

    private bool IsMoving() => agent.velocity.sqrMagnitude > Mathf.Epsilon;
    private bool HasArrived() => agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || !IsMoving());
    private bool HasArrived(string destination)
    {
        return HasArrived() && destinationTag == destination;
    }


    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

}
