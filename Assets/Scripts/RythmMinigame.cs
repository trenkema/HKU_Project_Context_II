using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;

public class RythmMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Quest quest;

    [SerializeField] RythmStart rythmStart;

    [SerializeField] GameObject water;

    [SerializeField] int maxLiters = 100;

    [SerializeField] int minLitersPerPump, maxLitersPerPump;

    [SerializeField] float waterMinHeight, waterMaxHeight;

    [SerializeField] Animator pumpAnimator;

    [SerializeField] Animator hudAnimator;

    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject[] hudComponents;

    [SerializeField] CinemachineImpulseSource rythmShake;

    [SerializeField] GameObject[] buttonPrefabs;
    [SerializeField] Transform[] buttonSpawnPositions;

    [SerializeField] LayerMask layersToHit;

    [SerializeField] TextMeshProUGUI litersPumpedText;

    [Header("Settings")]
    [SerializeField] float delayToStart;

    [SerializeField] float[] destroyButtonTime;

    [SerializeField] float[] timeBetweenSpawning;

    [SerializeField] int minBurstToSpawn, maxBurstToSpawn;

    [SerializeField] float[] buttonMultiplier;

    List<GameObject> spawnedButtons = new List<GameObject>();

    public bool isCompleted { get; private set; }

    public bool spawnButtons = false;

    public bool isStarted = false;

    int litersPumped = 0;

    float heightDifference;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.RYTHM_BUTTON_DESTROYED, RythmButtonDestroyed);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.RYTHM_BUTTON_DESTROYED, RythmButtonDestroyed);
    }

    private void Start()
    {
        isCompleted = false;

        foreach (var hudComponent in hudComponents)
        {
            hudComponent.SetActive(false);
        }

        float heightDifferenceCalculated = Mathf.Max(waterMaxHeight, waterMinHeight) - Mathf.Min(waterMinHeight, waterMaxHeight);

        heightDifference = heightDifferenceCalculated / maxLiters;
    }

    public void StartGame()
    {
        spawnButtons = true;

        pumpAnimator.SetBool("Pump", true);

        StartCoroutine(SpawnButtons());
    }

    public void ExitGame()
    {
        spawnButtons = false;

        pumpAnimator.SetBool("Pump", false);

        audioSource.Stop();

        StopAllCoroutines();

        foreach (var spawnedButton in spawnedButtons)
        {
            Destroy(spawnedButton);
        }

        foreach (var hudComponent in hudComponents)
        {
            hudComponent.SetActive(false);
        }
    }

    public void HitKey(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            hudAnimator.SetTrigger("Hit");

            Vector2 hitValue = _context.ReadValue<Vector2>();
            RythmButton nearestX = null;
            RythmButton nearestY = null;

            switch (hitValue.x)
            {
                case -1: // A Key
                    nearestX = CheckNearest(Vector2.left, 2);

                    if (nearestX == null)
                    {
                        litersPumped--;
                        water.transform.position += new Vector3(0f, heightDifference, 0f);
                    }
                    break;
                case 1: // D Key
                    nearestX = CheckNearest(Vector2.right, 3);

                    if (nearestX == null)
                    {
                        litersPumped--;
                        water.transform.position += new Vector3(0f, heightDifference, 0f);
                    }
                    break;
            }

            if (nearestX != null)
            {
                rythmShake.GenerateImpulse();

                if (nearestX.isInTrigger)
                {
                    int randomLiters = Random.Range(minLitersPerPump, maxLitersPerPump);

                    litersPumped += randomLiters;

                    water.transform.position -= new Vector3(0f, heightDifference * randomLiters, 0f);

                    nearestX.SetRight();
                }
                else
                {
                    litersPumped--;

                    water.transform.position += new Vector3(0f, heightDifference, 0f);

                    nearestX.SetWrong();
                }
            }

            switch (hitValue.y)
            {
                case 1: // W Key
                    nearestY = CheckNearest(Vector2.up, 0);

                    if (nearestY == null)
                    {
                        litersPumped--;
                        water.transform.position += new Vector3(0f, heightDifference, 0f);
                    }
                    break;
                case -1: // S Key
                    nearestY = CheckNearest(Vector2.down, 1);

                    if (nearestY == null)
                    {
                        litersPumped--;
                        water.transform.position += new Vector3(0f, heightDifference, 0f);
                    }
                    break;
            }

            if (nearestY != null)
            {
                rythmShake.GenerateImpulse();

                if (nearestY.isInTrigger)
                {
                    int randomLiters = Random.Range(minLitersPerPump, maxLitersPerPump);

                    litersPumped += randomLiters;

                    water.transform.position -= new Vector3(0f, heightDifference * randomLiters, 0f);

                    nearestY.SetRight();
                }
                else
                {
                    litersPumped--;

                    water.transform.position += new Vector3(0f, heightDifference, 0f);

                    nearestY.SetWrong();
                }
            }

            water.transform.position = new Vector3(water.transform.position.x, Mathf.Clamp(water.transform.position.y, waterMinHeight, waterMaxHeight), water.transform.position.z);

            if (litersPumped >= maxLiters)
            {
                isCompleted = true;

                rythmStart.MiniGameFinished();

                EventSystemNew<Quest>.RaiseEvent(Event_Type.QUEST_COMPLETED, quest);
            }

            litersPumped = Mathf.Clamp(litersPumped, 0, maxLiters);

            litersPumpedText.text = string.Format("Pumped: {0} L", litersPumped);
        }
    }

    private RythmButton CheckNearest(Vector2 _direction, int _spawnPositionID)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, _direction, 450f, layersToHit);

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.gameObject.TryGetComponent(out RythmButton rythmButton))
            {
                if (rythmButton.spawnPositionID == _spawnPositionID)
                {
                    return rythmButton;
                }
            }
        }

        return null;
    }

    public void SpawnButton()
    {
        int spawnPositionID = Random.Range(0, buttonSpawnPositions.Length);
        Transform spawnPosition = buttonSpawnPositions[spawnPositionID];

        int buttonID = Random.Range(0, buttonPrefabs.Length);
        GameObject buttonPrefab = buttonPrefabs[buttonID];

        int multiplierID = Random.Range(0, buttonMultiplier.Length);
        float multiplier = buttonMultiplier[multiplierID];

        GameObject spawnedButton = Instantiate(buttonPrefab, spawnPosition);
        spawnedButton.GetComponent<RythmButton>().Setup(multiplier, multiplierID, spawnPositionID, transform.position);

        spawnedButtons.Add(spawnedButton);
    }

    IEnumerator SpawnButtons()
    {
        audioSource.Play();

        foreach (var hudComponent in hudComponents)
        {
            hudComponent.SetActive(true);
        }

        yield return new WaitForSeconds(delayToStart);

        while (spawnButtons)
        {
            float spawnTime = timeBetweenSpawning[Random.Range(0, timeBetweenSpawning.Length)];

            yield return new WaitForSeconds(spawnTime);

            SpawnButton();
        }
    }

    private void RythmButtonDestroyed()
    {
        litersPumped--;

        litersPumped = Mathf.Clamp(litersPumped, 0, 100);

        litersPumpedText.text = string.Format("Pumped: {0} L", litersPumped);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out RythmButton rythmButton))
        {
            rythmButton.SetInTrigger();

            float destroyTime = destroyButtonTime[rythmButton.multiplierID];

            StartCoroutine(rythmButton.DestroyButton(destroyTime));
        }
    }
}
