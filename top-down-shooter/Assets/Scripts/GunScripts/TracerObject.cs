using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerObject : MonoBehaviour
{
    private GameObject TracerEffect;
    private GameObject Impact;
    private Vector3 StartPosition;
    private Vector3 EndPosition;
    public TracerObject(Vector3 StartPosition, Vector3 EndPosition, GameObject Impact)
	{
        this.StartPosition = StartPosition;
        this.EndPosition = EndPosition;
        this.Impact = Impact;
	}
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
