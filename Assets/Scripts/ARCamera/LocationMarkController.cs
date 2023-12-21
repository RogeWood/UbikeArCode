// utf-8 編碼
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationMarkController : MonoBehaviour
{
    private Transform cam;
    [SerializeField] private float lat;
    [SerializeField] private float lon;
    [SerializeField] private TextMeshPro message;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cam.forward);
    }

    public void SetLatitudeAndLongitude(float at, float on)
    {
        lat = at;
        lon = on;
    }

    public void SetText(string text)
    {
        message.text = text;
    }

    public string GetText()
    {
        return message.text;
    }

    public Vector2 GetLatAndLon()
    {
        return new Vector2(lat, lon);
    }
}
