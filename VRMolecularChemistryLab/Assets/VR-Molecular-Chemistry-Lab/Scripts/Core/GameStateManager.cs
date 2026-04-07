using System.Collections;
using UnityEngine;
using VRMolecularLab.UI;

namespace VRMolecularLab.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public enum GameState { Idle, Active, Resetting }
        private GameState _state;

        [Header("References")]
        public AtomSpawner atomSpawner;
        public BondSocketManager bondSocketManager;
        public MoleculeSpawnController moleculeSpawnController;
        public UIManager uiManager;

        [Header("UI References")]
        public GameObject instructionPanel;
        public GameObject startButton;
        public GameObject spawnPoint;

        private void Awake()
        {
            _state = GameState.Idle;
            if (instructionPanel != null) instructionPanel.SetActive(true);
            if (startButton != null) startButton.SetActive(false);
            if (spawnPoint != null) spawnPoint.SetActive(false);
            StartCoroutine(WaitAndShowStartButton(2f));
        }

        private IEnumerator WaitAndShowStartButton(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (startButton != null) startButton.SetActive(true);
        }

        public void OnStartPressed()
        {
            if (_state != GameState.Idle) return;
            if (instructionPanel != null) instructionPanel.SetActive(false);
            if (startButton != null) startButton.SetActive(false);

            if (atomSpawner != null) atomSpawner.StartSession();
            if (spawnPoint != null) spawnPoint.SetActive(true);

            if (bondSocketManager != null) bondSocketManager.OpenFirstSocket();
            _state = GameState.Active;
        }

        public void ResetSession()
        {
            if (_state == GameState.Resetting) return;
            _state = GameState.Resetting;

            if (bondSocketManager != null) bondSocketManager.ResetAllSockets();
            if (atomSpawner != null) atomSpawner.ResetAll();
            if (moleculeSpawnController != null) moleculeSpawnController.ClearCurrentMolecule();
            if (uiManager != null) uiManager.ClearLibrary();
            if (spawnPoint != null) spawnPoint.SetActive(false);
            if (instructionPanel != null) instructionPanel.SetActive(true);
            if (startButton != null) startButton.SetActive(true);
            _state = GameState.Idle;
        }

        public void ClearDesk()
        {
            if (_state != GameState.Active) return;

            if (moleculeSpawnController != null) moleculeSpawnController.ClearCurrentMolecule();
            if (bondSocketManager != null) bondSocketManager.ResetAllSockets();
            if (atomSpawner != null) atomSpawner.CleanDesk();
            
            if (bondSocketManager != null) bondSocketManager.OpenFirstSocket();
        }
    }
}
