using UnityEngine;

public class PrinterPlasticSpawner : MonoBehaviour
{
    public bool isPrinting;
    [SerializeField] private GameObject plasticPrefabRef;
    public float spawnCooldown = 0.5f;
    private float _timeBeforeCanSpawn;

    public void StartPrinting()
    {
        isPrinting = true;
    }

    public void FinishPrinting()
    {
        isPrinting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPrinting)
        {
            if (_timeBeforeCanSpawn <= 0f)
            {
                PrintPlasticParticle();
            }
        }
        _timeBeforeCanSpawn -= Time.deltaTime;
        if (_timeBeforeCanSpawn < 0f)
        {
            _timeBeforeCanSpawn = 0f;
        }
    }

    void PrintPlasticParticle()
    {
        Instantiate(plasticPrefabRef, transform.position, Quaternion.identity);
        _timeBeforeCanSpawn = spawnCooldown;
    }
}
