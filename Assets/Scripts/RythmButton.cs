using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class RythmButton : MonoBehaviour
{
    [SerializeField] Animator animator;

    public bool isHitInTrigger { private set; get; }

    public bool isInTrigger { private set; get; }

    public int multiplierID { private set; get; }

    public int spawnPositionID { private set; get; }

    [SerializeField] float baseButtonSpeed;

    [SerializeField] TextMeshProUGUI buttonText;

    float pointerLerpTimeElapsed = 0f;

    bool destroyButton = false;

    Vector3 currentValue;

    Vector3 startPos;

    Vector3 endPos;

    public void Setup(float _buttonMultiplier, int _multiplierID, int _spawnPositionID, Vector3 _endPos)
    {
        isHitInTrigger = false;

        startPos = gameObject.transform.position;
        endPos = _endPos;

        multiplierID = _multiplierID;

        spawnPositionID = _spawnPositionID;

        baseButtonSpeed *= _buttonMultiplier;

        switch (_spawnPositionID)
        {
            case 0:
                buttonText.text = "W";
                break;
            case 1:
                buttonText.text = "S";
                break;
            case 2:
                buttonText.text = "A";
                break;
            case 3:
                buttonText.text = "D";
                break;
        }
    }

    private void Update()
    {
        if (pointerLerpTimeElapsed < baseButtonSpeed)
        {
            currentValue = Vector3.Lerp(startPos, endPos, pointerLerpTimeElapsed / baseButtonSpeed);

            pointerLerpTimeElapsed += Time.deltaTime;
        }

        transform.position = currentValue;
    }

    public void SetInTrigger()
    {
        isInTrigger = true;
    }

    public void SetWrong()
    {
        destroyButton = false;

        animator.SetTrigger("Wrong");
    }
    
    public void SetRight()
    {
        destroyButton = false;

        animator.SetTrigger("Right");
    }

    public IEnumerator DestroyButton(float _destroyTime)
    {
        destroyButton = true;

        yield return new WaitForSeconds(_destroyTime);

        if (destroyButton)
        {
            EventSystemNew.RaiseEvent(Event_Type.RYTHM_BUTTON_DESTROYED);

            Destroy(gameObject);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
