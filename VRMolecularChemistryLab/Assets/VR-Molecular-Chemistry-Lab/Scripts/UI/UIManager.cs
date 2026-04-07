using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VRMolecularLab.Core;
using VRMolecularLab.Data;

namespace VRMolecularLab.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Library Panel")]
        public Transform libraryGrid;
        public GameObject moleculeCardPrefab;
        private HashSet<string> _discovered = new HashSet<string>();

        [Header("Inspector Panel")]
        public GameObject inspectorPanel;
        public TextMeshProUGUI inspectorNameText;
        public TextMeshProUGUI inspectorFormulaText;
        public TextMeshProUGUI inspectorBondText;
        public TextMeshProUGUI inspectorDescText;
        private GameObject _inspectedMoleculeTarget;

        [Header("Wrist Menu")]
        public CanvasGroup wristMenuCanvasGroup;
        public Transform wristMenuParent;
        private bool _wristMenuVisible = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (inspectorPanel != null) inspectorPanel.SetActive(false);
            if (wristMenuCanvasGroup != null) 
            {
                wristMenuCanvasGroup.alpha = 0f;
                wristMenuCanvasGroup.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (BondManager.Instance != null)
            {
                BondManager.Instance.OnMoleculeFormed += OnMoleculeFormed;
                BondManager.Instance.OnMoleculeReset += OnMoleculeReset;
            }
        }

        private void OnDestroy()
        {
            if (BondManager.Instance != null)
            {
                BondManager.Instance.OnMoleculeFormed -= OnMoleculeFormed;
                BondManager.Instance.OnMoleculeReset -= OnMoleculeReset;
            }
        }

        private void LateUpdate()
        {
            // Billboard the inspector panel to face the camera
            if (_inspectedMoleculeTarget != null && inspectorPanel != null && inspectorPanel.activeSelf)
            {
                var mainCam = Camera.main;
                if (mainCam != null)
                {
                    inspectorPanel.transform.position = _inspectedMoleculeTarget.transform.position + Vector3.up * 0.25f;
                    inspectorPanel.transform.LookAt(mainCam.transform);
                    inspectorPanel.transform.Rotate(0, 180f, 0);
                }
            }
        }

        // --- Library Logic ---
        public void OnMoleculeFormed(MoleculeData data)
        {
            AddDiscovery(data);
        }

        private void OnMoleculeReset(MoleculeData data)
        {
            HideInspector();
            // Optional: refresh library visuals if resetting modifies display state
        }

        public void AddDiscovery(MoleculeData mol)
        {
            if (_discovered.Contains(mol.moleculeName)) return;

            _discovered.Add(mol.moleculeName);

            if (moleculeCardPrefab != null && libraryGrid != null)
            {
                var cardObj = Instantiate(moleculeCardPrefab, libraryGrid);
                var card = cardObj.GetComponent<MoleculeCard>();
                if (card != null)
                {
                    card.Populate(mol);
                }

                // Replacing DOTween pop animation with Coroutine to prevent package missing errors
                StartCoroutine(ScalePunchAnim(cardObj.transform, 0.35f));
            }
        }

        // --- Inspector Logic ---
        public void ShowInspector(MoleculeInstance moleculeInstance)
        {
            if (inspectorPanel == null || moleculeInstance == null) return;

            var data = moleculeInstance.Data;
            _inspectedMoleculeTarget = moleculeInstance.gameObject;

            if (inspectorNameText != null) inspectorNameText.text = data.moleculeName;
            if (inspectorFormulaText != null) inspectorFormulaText.text = data.formula; // Need rich text formatting ideally
            if (inspectorBondText != null) inspectorBondText.text = data.bondType.ToString().ToUpper();
            if (inspectorDescText != null) inspectorDescText.text = data.description;

            inspectorPanel.SetActive(true);
        }

        public void HideInspector()
        {
            if (inspectorPanel != null)
            {
                inspectorPanel.SetActive(false);
            }
            _inspectedMoleculeTarget = null;
        }

        public void UI_Btn_ResetCurrentMolecule()
        {
            if (_inspectedMoleculeTarget != null)
            {
                BondManager.Instance.ResetMolecule(_inspectedMoleculeTarget);
                HideInspector();
            }
        }

        // --- Wrist Menu Logic ---
        public void ToggleWristMenu()
        {
            _wristMenuVisible = !_wristMenuVisible;

            if (wristMenuCanvasGroup != null)
            {
                if (_wristMenuVisible) wristMenuCanvasGroup.gameObject.SetActive(true);
                StartCoroutine(FadeCanvasGroup(wristMenuCanvasGroup, _wristMenuVisible ? 1f : 0f, 0.2f));
            }

            if (wristMenuParent != null)
            {
                StartCoroutine(ScaleLerp(wristMenuParent, _wristMenuVisible ? Vector3.one * 0.0012f : Vector3.zero, 0.2f));
            }
        }

        // --- Animation Coroutine Fallbacks (replaces DOTween dependency) ---
        private IEnumerator ScalePunchAnim(Transform target, float duration)
        {
            target.localScale = Vector3.zero;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Simple ease-out back approximation
                float curve = Mathf.Sin(t * Mathf.PI * 0.5f) + Mathf.Sin(t * Mathf.PI) * 0.2f;
                target.localScale = Vector3.one * Mathf.Clamp01(curve);
                yield return null;
            }
            target.localScale = Vector3.one;
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
        {
            float startAlpha = cg.alpha;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }
            cg.alpha = targetAlpha;

            if (targetAlpha <= 0f)
            {
                cg.gameObject.SetActive(false);
            }
        }

        private IEnumerator ScaleLerp(Transform target, Vector3 targetScale, float duration)
        {
            Vector3 startScale = target.localScale;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
                yield return null;
            }
            target.localScale = targetScale;
        }
    }
}
