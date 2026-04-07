using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRMolecularLab.Data;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace VRMolecularLab.Core
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class AtomController : MonoBehaviour
    {
        [Header("Atom Properties")]
        public AtomToken atomData;
        public float proximityRadius = 0.12f;
        public LayerMask atomLayer;
        public Material highlightMaterial;
        
        [Header("References")]
        public BondManager bondManager;
        public GameObject snapZoneIndicator;

        private bool _isGrabbed;
        private GameObject _bondedMolecule;
        private MeshRenderer _renderer;
        private List<AtomController> _nearbyAtoms = new List<AtomController>();
        private XRGrabInteractable _interactable;

        // Public Accessors
        public string ElementSymbol => atomData != null ? atomData.elementSymbol : string.Empty;
        public bool IsGrabbed => _isGrabbed;
        public bool IsConsumed => _bondedMolecule != null;
        public bool IsPlaced { get; set; }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _interactable = GetComponent<XRGrabInteractable>();

            if (_interactable != null)
            {
                _interactable.selectEntered.AddListener(OnGrabbed);
                _interactable.selectExited.AddListener(OnReleased);
            }

            if (snapZoneIndicator != null)
            {
                snapZoneIndicator.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_interactable != null)
            {
                _interactable.selectEntered.RemoveListener(OnGrabbed);
                _interactable.selectExited.RemoveListener(OnReleased);
            }
        }

        public void OnGrabbed(SelectEnterEventArgs args)
        {
            OnGrabbed();
        }

        public void OnReleased(SelectExitEventArgs args)
        {
            OnReleased();
        }

        // Left as public for potential external/event calls per doc
        public void OnGrabbed()
        {
            if (IsConsumed || IsPlaced) return; // Prevent grabbing if already consumed or placed
            
            _isGrabbed = true;
            if (highlightMaterial != null && _renderer != null)
            {
                _renderer.material = highlightMaterial;
            }
            if (BondSocketManager.Instance != null) BondSocketManager.Instance.RegisterGrab();
        }

        public void OnReleased()
        {
            _isGrabbed = false;
            
            if (_renderer != null && atomData != null && atomData.atomMaterial != null)
            {
                _renderer.material = atomData.atomMaterial;
            }

            _nearbyAtoms.Clear();
            if (snapZoneIndicator != null)
            {
                snapZoneIndicator.SetActive(false);
            }
            if (BondSocketManager.Instance != null) BondSocketManager.Instance.UnregisterGrab();
        }

        private void FixedUpdate()
        {
            if (!_isGrabbed || IsConsumed) return;

            _nearbyAtoms.Clear();
            var hits = Physics.OverlapSphere(transform.position, proximityRadius, atomLayer);
            
            foreach (var col in hits)
            {
                var other = col.GetComponent<AtomController>();
                if (other != null && other != this && !other.IsConsumed)
                {
                    _nearbyAtoms.Add(other);
                }
            }

            if (snapZoneIndicator != null)
            {
                snapZoneIndicator.SetActive(_nearbyAtoms.Count > 0);
            }

            if (_nearbyAtoms.Count > 0 && bondManager != null)
            {
                bondManager.OnAtomProximity(this, _nearbyAtoms);
            }
        }

        public void ConsumeIntoMolecule(GameObject mol)
        {
            _bondedMolecule = mol;
            
            if (_renderer != null) _renderer.enabled = false;
            
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            
            if (snapZoneIndicator != null) snapZoneIndicator.SetActive(false);
        }

        public void ResetAtom()
        {
            _bondedMolecule = null;
            IsPlaced = false;
            
            if (_renderer != null) _renderer.enabled = true;
            
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
            
            // Re-assign original material to be safe
            if (_renderer != null && atomData != null && atomData.atomMaterial != null)
            {
                _renderer.material = atomData.atomMaterial;
            }
        }
    }
}
