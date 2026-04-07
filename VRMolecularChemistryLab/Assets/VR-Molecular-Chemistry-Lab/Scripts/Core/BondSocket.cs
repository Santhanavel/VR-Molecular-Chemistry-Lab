using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRMolecularLab.Core
{
    public class BondSocket : MonoBehaviour
    {
        public AtomController occupant;
        public bool IsOccupied => occupant != null;
        public GameObject socketVisual;

        private float _lastPlace;
        private Material _visMat;
        private Color _neutCol;
        private Coroutine _flashCo;

        private void Awake()
        {
            if (socketVisual != null)
            {
                var r = socketVisual.GetComponent<MeshRenderer>();
                if (r != null) { _visMat = r.material; _neutCol = _visMat.color; }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Time.time - _lastPlace < 0.3f || IsOccupied) return;
            AtomController atom = other.GetComponentInParent<AtomController>();
            if (atom != null && atom.IsGrabbed && !atom.IsConsumed)
            {
                var xri = atom.GetComponent<XRGrabInteractable>();
                if (xri != null)
                {
                    xri.selectExited.RemoveListener(OnAtomReleased);
                    xri.selectExited.AddListener(OnAtomReleased);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            AtomController atom = other.GetComponentInParent<AtomController>();
            if (atom != null)
            {
                var xri = atom.GetComponent<XRGrabInteractable>();
                if (xri != null) xri.selectExited.RemoveListener(OnAtomReleased);
            }
        }

        private void OnAtomReleased(SelectExitEventArgs args)
        {
            if (args.interactableObject == null) return;
            AtomController atom = args.interactableObject.transform.GetComponent<AtomController>();
            if (atom != null)
            {
                var xri = atom.GetComponent<XRGrabInteractable>();
                if (xri != null) xri.selectExited.RemoveListener(OnAtomReleased);
                if (!IsOccupied) TryAccept(atom);
            }
        }

        public void TryAccept(AtomController atom)
        {
            if (IsOccupied || atom == null) return;
            occupant = atom;
            _lastPlace = Time.time;

            var antiGravity = atom.GetComponent<AntigravityFloat>();
            if (antiGravity != null) antiGravity.enabled = false;

            atom.transform.position = transform.position;
            var rb = atom.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            var xri = atom.GetComponent<XRGrabInteractable>();
            if (xri != null) xri.enabled = false;
            atom.IsPlaced = true;

            if (socketVisual != null) socketVisual.SetActive(false);
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            if (AudioManager.Instance != null) AudioManager.Instance.PlayAtomPlaced();
            if (BondSocketManager.Instance != null) BondSocketManager.Instance.OnSocketOccupied(this, atom);

            if (_flashCo != null) StopCoroutine(_flashCo);
            _flashCo = StartCoroutine(FlashColor(Color.green, 0.2f));
        }

        public void FlashInvalid()
        {
            if (_flashCo != null) StopCoroutine(_flashCo);
            _flashCo = StartCoroutine(FlashColor(Color.red, 0.3f));
        }

        private IEnumerator FlashColor(Color color, float duration)
        {
            if (_visMat != null)
            {
                _visMat.color = color;
                yield return new WaitForSeconds(duration);
                _visMat.color = _neutCol;
            }
        }

        public void ClearSocket()
        {
            if (occupant != null) occupant.IsPlaced = false;
            occupant = null;
            if (_flashCo != null) { StopCoroutine(_flashCo); _flashCo = null; }
            if (_visMat != null) _visMat.color = _neutCol;

            if (socketVisual != null) socketVisual.SetActive(BondSocketManager.Instance != null && BondSocketManager.Instance.IsSockedVisible);
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }
    }
}
