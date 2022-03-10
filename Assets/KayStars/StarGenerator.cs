using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject starPrefab;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] Material[] starMaterials;

    [Header("Settings")]
    [SerializeField] LayerMask checkCollisionLayers;

    [SerializeField] int starMinAmount, starMaxAmount;

    [SerializeField] float starMinSize, starMaxSize;

    [SerializeField] int maxAmountOfTries;

    [SerializeField] bool checkForStarsAround = true;

    [SerializeField] float checkForStarsAroundRadius = 1f;

    [SerializeField] bool spawnInsideSphere = true;

    List<GameObject> stars = new List<GameObject>();

    int index = 0;

    int tryAmount = 0;

    private void Start()
    {
        GenerateStars();
    }

    private void GenerateStar()
    {
        tryAmount++;

        float starSize = Random.Range(starMinSize, starMaxSize);

        var r = sphereCollider.radius * (sphereCollider.transform.localScale.x);

        var x = Random.Range(-1f, 1f);
        var y = Random.Range(-1f, 1f);
        var z = Random.Range(-1f, 1f);

        var vec = Vector3.zero;

        if (spawnInsideSphere)
            vec = new Vector3(x, y, z) * r;
        else
            vec = new Vector3(x, y, z).normalized * r;

        if (checkForStarsAround)
        {
            Vector3 position = sphereCollider.transform.position + vec;

            Collider[] hitColliders = Physics.OverlapSphere(position, checkForStarsAroundRadius * starSize, checkCollisionLayers);

            if (hitColliders.Length == 0)
            {
                GameObject star = Instantiate(starPrefab, position, Quaternion.identity);

                stars.Add(star);

                star.GetComponent<MeshRenderer>().material = starMaterials[Random.Range(0, starMaterials.Length)];

                star.transform.localScale = Vector3.one * starSize;

                index++;
            }

            return;
        }

        GameObject spawned = Instantiate(starPrefab, sphereCollider.transform);

        stars.Add(spawned);

        spawned.GetComponent<MeshRenderer>().material = starMaterials[Random.Range(0, starMaterials.Length)];

        spawned.transform.position = vec;

        spawned.transform.localScale = Vector3.one * starSize;

        index++;
    }

    private void GenerateStars()
    {
        int amountOfStars = Random.Range(starMinAmount, starMaxAmount);

        while (index < amountOfStars && tryAmount < maxAmountOfTries)
        {
            GenerateStar();
        }
    }

    private void DestroyStars()
    {
        foreach (var star in stars)
        {
            Destroy(star);
        }

        stars.Clear();

        index = 0;
        tryAmount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyStars();

            GenerateStars();
        }
    }
}
