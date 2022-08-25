using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerMotion motion;

    float inputHorizontal;

    private void Start()
    {
        motion = GetComponent<PlayerMotion>();
    }

    private void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        motion.Move(inputHorizontal);

        motion.Jump(Input.GetButton("Jump"));
    }
}
