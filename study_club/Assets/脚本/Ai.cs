using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ai : MonoBehaviour
{


    public NavMeshAgent Nav;
    public GameObject gab;
    // Start is called before the first frame update
    void Start()
    {
        Nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Nav.SetDestination(gab.transform.position);
    }
}
