using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

[Serializable] public class MyDictionary1 : SerializableDictionary<GameController.Role, PawnController> { }

public class Test : MonoBehaviour
{
    public int test;
    public MyDictionary1 dictionary;
    //ublic SerializableDictionary<GameController.Role,PawnController> dictionary2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}