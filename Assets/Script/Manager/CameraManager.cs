using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    private float speed = (float)0.1;
    private Vector3 start;
    private Vector3 end;
    private Camera mainCamera;
    private float size;
    private bool Moving = false;
    private Vector3 position;
    private GameObject info,head;

    public void ShowInfo(Vector3Int property)
    {
        info.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = property.x.ToString();
        info.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = property.y.ToString();
        info.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = property.z.ToString();
    }

    public void ShowHead(Vector3Int property)
    {
        head.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = property.x.ToString();
        head.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = property.y.ToString();
        head.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = property.z.ToString();
    }

    public void MoveToPoint(Vector3 position)
    {
        Moving = true;
        this.position = position;
    }

    void Start()
    {
        start = GameObject.Find("start").transform.position;
        end = GameObject.Find("end").transform.position;
        mainCamera = GetComponent<Camera>();
        size = mainCamera.orthographicSize;
        GameManager.Instance.cameraManager = this;
        info = GameObject.Find("Info");
        head = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == new Vector3(position.x, position.y, -10))
        {
            Moving = false;
        }
        if (Moving)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, -10), (Time.time - 2f) * 2f);
            return;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0 && mainCamera.orthographicSize>5 && UIManager.Opened == 0)
        {
            mainCamera.orthographicSize -= speed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && mainCamera.orthographicSize < 15 && UIManager.Opened == 0)
        {
            mainCamera.orthographicSize += speed;
        }

        if (Input.mousePosition.x > Screen.width * 0.98 && transform.position.x<end.x - size*1.7)
        {
            transform.position = transform.position + new Vector3(speed, 0, 0);
        }
        if (Input.mousePosition.y > Screen.height * 0.98 && transform.position.y<end.y - size/2)
        {
            transform.position = transform.position + new Vector3(0, speed, 0);
        }
        if (Input.mousePosition.x < Screen.width * 0.03 && transform.position.x>start.x + size*1.7)
        {
            transform.position = transform.position - new Vector3(speed, 0, 0);
        }
        if (Input.mousePosition.y < Screen.height * 0.03 && transform.position.y>start.y + size/2)
        {
            transform.position = transform.position - new Vector3(0, speed, 0);
        }
    }
}
