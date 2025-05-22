using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesactivateCineCam : MonoBehaviour
{
    IEnumerator rDesactivate()
    {
        yield return new WaitForSeconds(11f);
        Debug.Log("Hacker");
        gameObject.SetActive(false);
    }

    public void DesactivateCam()
    {
        StartCoroutine(rDesactivate());
    }
}
