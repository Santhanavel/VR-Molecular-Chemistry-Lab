using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRMolecularLab.Core
{
    [RequireComponent(typeof(AtomController), typeof(Rigidbody))]
    public class AntigravityFloat : MonoBehaviour
    {
        private Vector3 _homePosition;
        private AtomController _atomController;
        private float _phase;
        private Rigidbody _rb;
        private Coroutine _rtnH;
        private XRGrabInteractable _xri;

        public bool IsReturning => _rtnH != null;

        private void Awake()
        {
            _atomController = GetComponent<AtomController>();
            _rb = GetComponent<Rigidbody>();
            _phase = Random.Range(0f, Mathf.PI * 2f);
            _xri = GetComponent<XRGrabInteractable>();
        }

        private void Update()
        {
            if (_atomController == null) return;
            if (_atomController.IsConsumed || _atomController.IsPlaced) { this.enabled = false; return; }
            if (_atomController.IsGrabbed)
            {
                if (_rtnH != null) { StopCoroutine(_rtnH); _rtnH = null; }
                return;
            }
            if (!_atomController.IsGrabbed && !_atomController.IsConsumed && !_atomController.IsPlaced && _rtnH == null)
            {
                float yOff = Mathf.Sin(Time.time * 1.2f + _phase) * 0.04f;
                // Force X and Z to stay locked to home columns in case they drifted
                transform.position = new Vector3(_homePosition.x, _homePosition.y + yOff, _homePosition.z);
            }
        }

        public void StartReturnHome(float delay = 0f)
        {
            if (this != null && gameObject.activeInHierarchy && this.enabled)
            {
                if (_rtnH != null) StopCoroutine(_rtnH);
                _rtnH = StartCoroutine(ReturnHome(delay));
            }
        }

        private IEnumerator ReturnHome(float delay)
        {
            if (_rb != null) _rb.isKinematic = true; // Lock physics immediately to prevent drops

            // Float in the air where we left it for the delay period
            if (delay > 0f)
            {
                float waitElapsed = 0f;
                // Capture initial spot
                Vector3 holdPos = transform.position;
                while (waitElapsed < delay)
                {
                    if (_atomController == null || _atomController.IsGrabbed) yield break;
                    
                    float yOff = Mathf.Sin(Time.time * 2f + _phase) * 0.005f;
                    transform.position = holdPos + new Vector3(0, yOff, 0);

                    waitElapsed += Time.deltaTime;
                    yield return null;
                }
            }

            float elapsed = 0f;
            Vector3 startPos = transform.position;
            while (elapsed < 0.4f)
            {
                if (_atomController == null || _atomController.IsGrabbed) yield break;
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, _homePosition, elapsed / 0.4f);
                yield return null;
            }
            if (this != null) transform.position = _homePosition;
            _rtnH = null;
        }

        public void SetHome(Vector3 pos) { _homePosition = pos; }
    }
}
