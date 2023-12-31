using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour
{
    [SerializeField]
    private RessourceType type;

    private Bounds? meshBounds = null;
    public RessourceType Type => type;

    public Bounds MeshBounds
    {
        get
        {
            meshBounds ??= GetComponent<MeshRenderer>().bounds;
            return (Bounds)meshBounds;
        }
        private set => meshBounds = value;
    }

    public Bounds MeshBoundsExpanded { get; private set; }


    void Awake()
    {


    }
    

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Utility.DrawBounds(MeshBounds,Color.red);




    }

}
