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
            }
            
            if (formulaText != null)
            {
                formulaText.text = FormatFormula(data.formula);
            }
            
            if (bondTypeText != null)
            {
                bondTypeText.text = data.bondType.ToString().ToUpper();
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
