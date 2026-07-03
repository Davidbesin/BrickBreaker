using UnityEngine;
using System;
public class Brick : MonoBehaviour
{
    [SerializeField] private int hits = 1;

    [SerializeField] GameObject one;
    [SerializeField] GameObject two;
    [SerializeField] GameObject three;

    [SerializeField] GameObject multiBalls;
    [SerializeField] GameObject UnderNeath;
    [SerializeField] GameObject Rocket;

    public event Action<Brick> OnBrickDestroyed;

    public int Hits
    {
        get => hits;
        set => hits = value;
    }

    public void Damage()
    {
        hits--;
       
        if (hits <= 0)
        {
            OnBrickDestroyed?.Invoke(this);
            Destroy(gameObject);
            if (UnityEngine.Random.Range(1, 10) == 1) // Returns 1, 2, 3, or 4
                GetRandomPowerUp().SetActive(true);
        }
    }
    public void DamageToZero()
    {
        hits = 0;
        OnBrickDestroyed?.Invoke(this);
        
         Destroy(gameObject);
            if (UnityEngine.Random.Range(1, 10) == 1) // Returns 1, 2, 3, or 4
                GetRandomPowerUp().SetActive(true);
    
    }

    private void Update()
    {
         SetActiveBrick(Hits);
    }

    void SetActiveBrick(int hit)
    {
        if (hit == 3)
        {
            one.SetActive(false);
            two.SetActive(false);
            three.SetActive(true);
        }
        else if (hit == 2)
        {
            one.SetActive(false);
            two.SetActive(true);
            three.SetActive(false);
        }
        else if (hit == 1)
        {
            one.SetActive(true);
            two.SetActive(false);
            three.SetActive(false);
        }
    }

    public GameObject GetRandomPowerUp()
    {
        GameObject[] powerUps =
        {
            multiBalls,
            UnderNeath,
            Rocket
        };

        return powerUps[UnityEngine.Random.Range(0, powerUps.Length)];
    }
}