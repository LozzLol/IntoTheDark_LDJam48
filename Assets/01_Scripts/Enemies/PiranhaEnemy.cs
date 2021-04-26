using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaEnemy : Enemy
{
    public Transform graphicsTransform;

    private void Update()
    {
        graphicsTransform.right = -rb2d.velocity.normalized;
    }
}
