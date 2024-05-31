using System;
using System.Collections.ObjectModel;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Collection<UI> _registeredUIs;
    private UI _activeUI;
    private Collection<UI> _activeUIHistory;

    public void RegisterUI(UI ui)
    {

    }

    void Awake()
    {
        _registeredUIs = new();
        _activeUIHistory = new();
    }

    void Update()
    {

    }
}
