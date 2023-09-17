using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //Annimations
    const string IDLE = "idle1";
    const string RUN = "Run";

    //Navigation & Annimation
    private CustomActions input;
    private NavMeshAgent agent;
    Animator animator;

    bool inputHeld = false;

    [Header("Movement")]
    [SerializeField]
    ParticleSystem clickEffect;
    [SerializeField]
    LayerMask clickableLayers;
    float lookRotationSpeed = 8f;

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
            agent.destination = hit.point;
        //Play click effect :
        if (clickEffect != null)
            Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);

    }

    void Update()
    {
        //Face Target :
        if (IsMoving())
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }

        //Set annimation :
        animator.Play(IsMoving() ? RUN : IDLE);
    }

    private bool IsMoving() => agent.velocity.sqrMagnitude > Mathf.Epsilon;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by " + other.gameObject.name);
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
