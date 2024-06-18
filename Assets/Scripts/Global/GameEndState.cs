using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameEndState : MonoBehaviour
{
    private List<InputDevice[]> _playerInputList = null;
    private int _winningPlayer = -1;
    
    public IReadOnlyList<InputDevice[]> PlayerInputList => _playerInputList.AsReadOnly();
    public int WinningPlayer => _winningPlayer;

    public void PassState(List<InputDevice[]> playerInputList, int winningPlayer)
    {
        if (_playerInputList != null)
            throw new InvalidOperationException("State is already passed.");
        _playerInputList = playerInputList;
        _winningPlayer = winningPlayer;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}