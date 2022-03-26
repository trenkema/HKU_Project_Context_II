using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLight : MonoBehaviour
{
    public PipeFixed[] pipes;

    public GameObject[] cables;

    public CheckLight checklightOff;

    public GameObject[] cableFixed;

    public Pipe[] pipeScripts;

    [SerializeField] List<CheckLightList> checkLightPuzzles = new List<CheckLightList>();

    private void OnEnable()
    {
        EventSystemNew<int>.Subscribe(Event_Type.PIPE_FIXED, PipeFixed);
    }

    private void OnDisable()
    {
        EventSystemNew<int>.Unsubscribe(Event_Type.PIPE_FIXED, PipeFixed);
    }

    private void PipeFixed(int _pipeID)
    {
        foreach (var lightPuzzle in checkLightPuzzles)
        {
            if (!lightPuzzle.isSolved)
            {
                lightPuzzle.pipesFixed.Add(_pipeID);

                if (lightPuzzle.pipesFixed.Count == lightPuzzle.pipesCorrectOrder.Count)
                {
                    string currentOrder = string.Empty;
                    string correctOrder = string.Empty;

                    foreach (var pipe in lightPuzzle.pipesFixed)
                    {
                        currentOrder += pipe;
                    }

                    foreach (var pipe in lightPuzzle.pipesCorrectOrder)
                    {
                        correctOrder += pipe;
                    }

                    if (currentOrder == correctOrder)
                    {
                        lightPuzzle.isSolved = true;

                        lightPuzzle.lightOn.SetActive(true);
                        lightPuzzle.lightOff.SetActive(false);

                        foreach (var cable in cables)
                        {
                            cable.SetActive(true);
                        }

                        foreach (var cableFixed in cableFixed)
                        {
                            cableFixed.SetActive(false);
                        }

                        foreach (var pipeScript in pipeScripts)
                        {
                            pipeScript.isGrabbed = false;
                        }

                        foreach (var lightPuzzel in checkLightPuzzles)
                        {
                            if (!lightPuzzle.isSolved)
                            {
                                lightPuzzle.pipesFixed.Clear();
                            }
                        }
                    }
                    else
                    {
                        lightPuzzle.pipesFixed.Clear();

                        foreach (var cable in cables)
                        {
                            cable.SetActive(true);
                        }

                        foreach (var cableFixed in cableFixed)
                        {
                            cableFixed.SetActive(false);
                        }

                        foreach (var pipeScript in pipeScripts)
                        {
                            pipeScript.isGrabbed = false;
                        }
                    }
                }
            }
        }
    }

    private void PipeRemoved(int _pipeID)
    {
        foreach (var lightPuzzle in checkLightPuzzles)
        {
            if (!lightPuzzle.isSolved)
            {
                lightPuzzle.pipesFixed.Remove(_pipeID);
            }
        }
    }
}

[System.Serializable]
public class CheckLightList
{
    public bool isSolved = false;

    public GameObject lightOn;
    public GameObject lightOff;

    public List<int> pipesFixed = new List<int>();

    public List<int> pipesCorrectOrder = new List<int>();
}