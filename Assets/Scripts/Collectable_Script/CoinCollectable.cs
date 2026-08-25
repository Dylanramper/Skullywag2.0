using UnityEngine;

public class CoinCollectable : Collectable
{
    protected override void Start()
    {
        // Override base values for coin
        coinValue = 1;
        scoreValue = 5;
        rotationSpeed = 0f;
        floatSpeed = 3f;
        
        // Optional: Randomize rotation direction
        if (Random.value > 0.5f)
        {
            rotationSpeed = -rotationSpeed;
        }
        
       base.Start();
    }
   
}