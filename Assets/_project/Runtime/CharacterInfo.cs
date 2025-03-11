using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class CharacterInfo : NetworkBehaviour
{
    [SerializeField] private float floatValue = 10f;
    [SerializeField] private String stance;

    public FloatField velocityDisplay;
    public TextField stateDisplay;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        velocityDisplay = root.Q<FloatField>("Velocity");
        stateDisplay = root.Q<TextField>("State");


    }

    public void SetFloatValue(float newValue)
    {
        floatValue = newValue;
        if (velocityDisplay != null)
        {
            velocityDisplay.value = floatValue;
        }
        else
        {
            Debug.LogWarning("FloatField is null! Ensure it is initialized correctly.");
        }
    }

    public void SetStateText(string state)
    {
        stance = state;

        stateDisplay.value = state;
    }
}
