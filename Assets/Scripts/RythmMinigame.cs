using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;

public class RythmMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource audioSource;

    [SerializeField] CinemachineImpulseSource rythmShake;

    [SerializeField] Animator animator;

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

    public bool spawnButtons = true;

    int litersPumped = 0;

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
        StartCoroutine(SpawnButtons());
    }

    public void HitKey(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            animator.SetTrigger("Hit");

            Vector2 hitValue = _context.ReadValue<Vector2>();
            RythmButton nearestX = null;
            RythmButton nearestY = null;

            Debug.Log("Hit Key: " + hitValue);

            switch (hitValue.x)
            {
                case -1: // A Key
                    nearestX = CheckNearest(Vector2.left, 2);

                    if (nearestX == null)
                        litersPumped--;
                    break;
                case 1: // D Key
                    nearestX = CheckNearest(Vector2.right, 3);

                    if (nearestX == null)
                        litersPumped--;
                    break;
            }

            if (nearestX != null)
            {
                rythmShake.GenerateImpulse();

                if (nearestX.isInTrigger)
                {
                    litersPumped++;

                    nearestX.SetRight();
                }
                else
                {
                    litersPumped--;

                    nearestX.SetWrong();
                }
            }

            switch (hitValue.y)
            {
                case 1: // W Key
                    nearestY = CheckNearest(Vector2.up, 0);

                    if (nearestY == null)
                        litersPumped--;
                    break;
                case -1: // S Key
                    nearestY = CheckNearest(Vector2.down, 1);

                    if (nearestY == null)
                        litersPumped--;
                    break;
            }

            if (nearestY != null)
            {
                rythmShake.GenerateImpulse();

                if (nearestY.isInTrigger)
                {
                    litersPumped++;

                    nearestY.SetRight();
                }
                else
                {
                    litersPumped--;

                    nearestY.SetWrong();
                }
            }

            litersPumped = Mathf.Clamp(litersPumped, 0, 100);

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
    }

    IEnumerator SpawnButtons()
    {
        audioSource.Play();

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
