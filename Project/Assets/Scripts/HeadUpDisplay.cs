﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadUpDisplay : MonoBehaviour
{
    [SerializeField]
    private Button spawnButton1;
    [SerializeField]
    private Button spawnButton2;
    [SerializeField]
    private Button spawnButton3;
    [SerializeField]
    private Button menuButton;

    public PlayerObject player;

    private bool menuIsVisible;
    private bool blue;

    private int resourceCapacity;
    private int availableResources;
    private Text resourceDisplay;

    public void Awake()
    {
        blue = (PlayerPrefs.GetInt("team") == 0);
        
        var players = FindObjectsOfType<PlayerIdentity>();
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i].uniqueIdentity == PlayerPrefs.GetString("uniqueIdentity"))
            {
                player = players[i].GetComponentInParent<PlayerObject>();
                Debug.Log("Unique Identity: " + players[i].uniqueIdentity);
            }
        }

        resourceCapacity = 20;
        availableResources = resourceCapacity;
        resourceDisplay = GetComponentInChildren<Text>();
        string output = availableResources + " / " + resourceCapacity;
        resourceDisplay.text = output;

        menuButton.gameObject.SetActive(true);
        menuIsVisible = true;

        // Add on click functions to buttons
        spawnButton1.onClick.AddListener(SpawnAntOnPosition1);
        spawnButton2.onClick.AddListener(SpawnAntOnPosition2);
        spawnButton3.onClick.AddListener(SpawnAntOnPosition3);
        menuButton.onClick.AddListener(ToggleMenu);

        // Add color to buttons
        Color32 buttonColor;
        if (blue)
            buttonColor = new Color32(0, 128, 255, 255);
        else
            buttonColor = new Color32(255, 155, 155, 255);

        spawnButton1.GetComponent<Image>().color = buttonColor;
        spawnButton2.GetComponent<Image>().color = buttonColor;
        spawnButton3.GetComponent<Image>().color = buttonColor;
        menuButton.GetComponent<Image>().color = buttonColor;
    }

    public void Start ()
    {
        StartCoroutine(IncreaseResourcesTick(2.0f));
    }

    private void SpawnAntOnPosition1 ()
    {
        if (availableResources <= 0)
            return;

        DecreaseResources();
        player.CmdSpawnMyAnt(1, blue);
    }

    private void SpawnAntOnPosition2()
    {
        if (availableResources <= 0)
            return;

        DecreaseResources();
        player.CmdSpawnMyAnt(2, blue);
    }

    private void SpawnAntOnPosition3()
    {
        if (availableResources <= 0)
            return;

        DecreaseResources();
        player.CmdSpawnMyAnt(3, blue);
    }

    private void ToggleMenu()
    {
        ToggleButtonVisibility();
        FindObjectOfType<CustomNetworkManagerUI>().ToggleMenu();
    }

    private void OnDisable()
    {
        ToggleButtonVisibility();
    }

    private void ToggleButtonVisibility ()
    {
        menuIsVisible = menuIsVisible ? false : true;
        spawnButton1.gameObject.SetActive(menuIsVisible);
        spawnButton2.gameObject.SetActive(menuIsVisible);
        spawnButton3.gameObject.SetActive(menuIsVisible);
        resourceDisplay.gameObject.SetActive(menuIsVisible);
    }

    public void DeactivateAllButtons()
    {
        spawnButton1.gameObject.SetActive(false);
        spawnButton2.gameObject.SetActive(false);
        spawnButton3.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        resourceDisplay.gameObject.SetActive(false);
    }

    private void DecreaseResources()
    {
        --availableResources;
        UpdateResourceDisplay();
    }

    private void UpdateResourceDisplay()
    {
        string output = availableResources + " / " + resourceCapacity;
        resourceDisplay.text = output;
    }

    IEnumerator IncreaseResourcesTick(float waitTime)
    {
        if (availableResources <= resourceCapacity)
        {
            availableResources += 2;

            if (availableResources > resourceCapacity)
                availableResources = resourceCapacity;

            UpdateResourceDisplay();
        }
        
        yield return new WaitForSeconds(waitTime);
        yield return IncreaseResourcesTick(waitTime);
    }
}
