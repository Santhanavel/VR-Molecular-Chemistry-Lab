using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRMolecularLab.Data;
using VRMolecularLab.Core;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRMolecularLab.UI
{
    public class MoleculeDetailsUI : MonoBehaviour
    {
        public static MoleculeDetailsUI Instance { get; private set; }

        [Header("UI References")]
        [Tooltip("The CanvasGroup to fade in and out")]
        [SerializeField] private CanvasGroup uiCanvasGroup;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI formulaText;
        [SerializeField] private TextMeshProUGUI bondTypeText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image iconImage;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (uiCanvasGroup != null)
            {
                uiCanvasGroup.alpha = 0f;
                uiCanvasGroup.interactable = false;
                uiCanvasGroup.blocksRaycasts = false;
            }
        }

        public void ShowMolecule(MoleculeInstance moleculeInstance)
        {
            if (moleculeInstance != null)
            {
                MoleculeData data = moleculeInstance.Data;
                
                if (nameText != null)
                {
                    nameText.text = string.IsNullOrEmpty(data.moleculeName) ? "Unknown" : data.moleculeName;
                    nameText.color = Color.white;
                }

                if (formulaText != null)
                {
                    formulaText.text = FormatFormula(string.IsNullOrEmpty(data.formula) ? "" : data.formula);
                    if (ColorUtility.TryParseHtmlString("#00B4D8", out Color tealColor))
                    {
                        formulaText.color = tealColor;
                    }
                }

                if (bondTypeText != null)
                {
                    bondTypeText.text = "BOND: " + data.bondType.ToString().ToUpper();
                    Color bondCol = Color.white;
                    if (data.bondType == BondType.Single) ColorUtility.TryParseHtmlString("#4CAF50", out bondCol);
                    else if (data.bondType == BondType.Double) ColorUtility.TryParseHtmlString("#FFC107", out bondCol);
                    else ColorUtility.TryParseHtmlString("#F44336", out bondCol);
                    bondTypeText.color = bondCol;
                }

                if (descriptionText != null)
                {
                    descriptionText.text = string.IsNullOrEmpty(data.description) ? "" : data.description;
                }
                
                if (iconImage != null)
                {
                    if (data.discoveryIcon != null)
                    {
                        iconImage.sprite = data.discoveryIcon;
                        iconImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        iconImage.gameObject.SetActive(false);
                    }
                }
            }
            FadeUI(1f);
        }

        private string FormatFormula(string rawFormula)
        {
            if (string.IsNullOrEmpty(rawFormula)) return "";
            string formatted = "";
            foreach (char c in rawFormula)
            {
                if (char.IsDigit(c))
                {
                    formatted += $"<sub>{c}</sub>";
                }
                else
                {
                    formatted += c;
                }
            }
            return formatted;
        }

        public void HideMolecule()
        {
            FadeUI(0f);
        }

        private void FadeUI(float targetAlpha)
        {
            if (uiCanvasGroup == null) return;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            
            _fadeCoroutine = StartCoroutine(FadeCoroutine(targetAlpha));
        }

        private IEnumerator FadeCoroutine(float targetAlpha)
        {
            float startAlpha = uiCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                uiCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            uiCanvasGroup.alpha = targetAlpha;
        }
    }
}
