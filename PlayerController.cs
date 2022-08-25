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

        motion.Run(inputHorizontal);

        if (Input.GetKeyDown(KeyCode.Space) && motion.onGround)
        {
            motion.Jump();
        }
    }
}
