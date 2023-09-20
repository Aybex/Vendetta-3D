using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RessourceGenerator : MonoBehaviour
{
	[SerializeField] private Collider terrain;
	[SerializeField] private int maxRessourcesCount;

	[Header("Ressource prefabs")]
	[SerializeField] private List<Ressource> ressourcesPrefabs = new();

    List<Ressource> Ressources = new List<Ressource>();

	void Start()
	{
        var surface = terrain.GetComponent<NavMeshSurface>();

		int i = 0;
		while (Ressources.Count < maxRessourcesCount)
		{
			//Choose a random ressource from prefabs
			Ressource randomRessource = ressourcesPrefabs[Random.Range(0, ressourcesPrefabs.Count)];

            float x_min = terrain.bounds.min.x + randomRessource.MeshBounds.size.x;
			float x_max = terrain.bounds.max.x - randomRessource.MeshBounds.size.x;
			float z_min = terrain.bounds.min.z + randomRessource.MeshBounds.size.z;
			float z_max = terrain.bounds.max.z - randomRessource.MeshBounds.size.z;
			Vector3 randomPosition = new Vector3(Random.Range(x_min, x_max), terrain.transform.position.y, Random.Range(z_min, z_max));
            
			Bounds newBounds = new Bounds(randomPosition, randomRessource.MeshBounds.size);

			//Check if the new ressource intersects with an existing one
            bool intersects = false;
            foreach (var existingRessource in Ressources)
            {
                if (existingRessource.MeshBounds.Intersects(newBounds))
                {
                    intersects = true;
                    break;
                }
            }

			i++;

            if (intersects)
            {
				continue;
            }

			var r = Instantiate(randomRessource.gameObject, randomPosition, Quaternion.identity, this.transform);
			var ressource = r.GetComponent<Ressource>();

			Ressources.Add(ressource);
			if (i > 10000)
			{
				Debug.LogError("Too many iterations");
				break;
			}
		}
		Debug.Log("Ressources count : " + Ressources.Count);
		Debug.Log("Iterations : " + i);

		surface.RemoveData();
		surface.BuildNavMesh();
	}

	void Update()
	{

	}
}
