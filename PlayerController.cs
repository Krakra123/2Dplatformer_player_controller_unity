using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerMotion thisMotion;

    float inputHorizontal;

    private void Start()
    {
        thisMotion = GetComponent<PlayerMotion>();
    }

    private void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        thisMotion.Run(inputHorizontal);
    }
}
