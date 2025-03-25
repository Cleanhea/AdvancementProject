using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField]
    private EventReference test1;
    bool check = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("??");
            StartCoroutine(test2());
        }
        if (check == false)
        {
            Debug.Log("테스트");
            AudioManager.instance.PlayOneShot(test1, transform.position);
            check = true;
        }
    }

    IEnumerator test2()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("뭐지");
            AudioManager.instance.PlayOneShot(test1, transform.position);
            yield return new WaitForSeconds(1f);
        }
    }
}
