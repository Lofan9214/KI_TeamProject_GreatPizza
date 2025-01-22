using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PointerManager pointerManager;

    public PizzaCommand PizzaCommand { get; set; }

    private void Start()
    {
        PizzaCommand = PizzaCommand.None;
    }

    public void SetPizzaCommand(string command)
    {
        if (Enum.TryParse(command, true, out PizzaCommand result))
        {
            PizzaCommand = result;
        }
    }
}
