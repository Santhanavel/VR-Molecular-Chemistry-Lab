using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRMolecularLab.Data;

namespace VRMolecularLab.UI
{
    public class MoleculeCard : MonoBehaviour
    {
        [Header("UI References")]
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI formulaText;
        public TextMeshProUGUI bondTypeText;

        /// <summary>
        /// Populates the card UI with MoleculeData
        /// </summary>
        public void Populate(MoleculeData data)
        {
            if (iconImage != null && data.discoveryIcon != null)
            {
                iconImage.sprite = data.discoveryIcon;
                iconImage.gameObject.SetActive(true);
            }
            
            if (nameText != null)
            {
                nameText.text = data.moleculeName;
                nameText.color = Color.white; // Enforce high-contrast white
            }
            
            if (formulaText != null)
            {
                formulaText.text = FormatFormula(data.formula);
                
                // Set to Teal as per UI specifications
                if (ColorUtility.TryParseHtmlString("#00B4D8", out Color tealColor))
                {
                    formulaText.color = tealColor;
                }
            }
            
            if (bondTypeText != null)
            {
                bondTypeText.text = data.bondType.ToString().ToUpper();
                
                // Color-code the bond text (Green for Single, Yellow for Double, Red for Triple)
                Color bondCol = Color.white;
                if (data.bondType == BondType.Single) ColorUtility.TryParseHtmlString("#4CAF50", out bondCol);
                else if (data.bondType == BondType.Double) ColorUtility.TryParseHtmlString("#FFC107", out bondCol);
                else ColorUtility.TryParseHtmlString("#F44336", out bondCol);
                
                bondTypeText.color = bondCol;
            }
        }

        private string FormatFormula(string rawFormula)
        {
            // Simple logic to set numbers to subscript via TMP rich text:
            // e.g. H2O -> H<sub>2</sub>O
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
    }
}
