using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RessourceGenerator : MonoBehaviour
{
    [SerializeField] private Transform _terrainObject;
    [SerializeField] private GameObject _ressourcePrefab;
    [SerializeField] private int _ressourceCount;

    List<GameObject> Ressources = new();

    void Awake()
    {
        var terrain = _terrainObject.GetComponent<Collider>();
        var surface = _terrainObject.GetComponent<NavMeshSurface>();

        //test object for bounds :
        var ressourceSize = Instantiate(_ressourcePrefab, new(0, -100, 0), Quaternion.identity, this.transform).GetComponent<Collider>().bounds.size;
        
        //terrain bounds shrinked by ressource size :
        var terrainBounds = terrain.bounds;
        terrainBounds.Expand(-1.5f*ressourceSize);

        int attempts = 0;
        for (int i = 0; i < _ressourceCount; i++)
        {
            Vector3 randomPosition = Vector3.zero;
            bool isValid = false;
            while (!isValid)
            {
                isValid = true;
                attempts++;
                randomPosition = new Vector3(Random.Range(terrainBounds.min.x, terrainBounds.max.x), _terrainObject.position.y, Random.Range(terrainBounds.min.z, terrainBounds.max.z));
                Bounds newBounds = new Bounds(randomPosition, ressourceSize);
                
                foreach (var existingRessource in Ressources)
                {
                    if (existingRessource != null &&
                        existingRessource.GetComponent<Collider>().bounds.Intersects(newBounds))
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            var r = Instantiate(_ressourcePrefab, randomPosition, Quaternion.identity, this.transform);


            Ressources.Add(r);
        }
        surface.RemoveData();
        surface.BuildNavMesh();


    }

    void Update()
    {

    }
}
