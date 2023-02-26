using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destruir_som : MonoBehaviour
{
    private float tempo_de_existencia = 0;
    private float tempo_para_se_destruir = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tempo_de_existencia += Time.deltaTime;
        if (tempo_de_existencia> tempo_para_se_destruir) Destroy(gameObject);

    }
}
