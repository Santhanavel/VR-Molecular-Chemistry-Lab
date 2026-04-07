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

        private void Awake()
        {
            _atomController = GetComponent<AtomController>();
            _rb = GetComponent<Rigidbody>();
            _phase = Random.Range(0f, Mathf.PI * 2f);
            _xri = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable() { if (_xri != null) _xri.selectExited.AddListener(OnReleased); }
        private void OnDisable() { if (_xri != null) _xri.selectExited.RemoveListener(OnReleased); }

        private void Update()
        {
            if (_atomController == null) return;
            if (_atomController.IsConsumed) { this.enabled = false; return; }
            if (_atomController.IsGrabbed)
            {
                if (_rtnH != null) { StopCoroutine(_rtnH); _rtnH = null; }
                return;
            }
            if (!_atomController.IsGrabbed && !_atomController.IsConsumed && _rtnH == null)
            {
                float yOff = Mathf.Sin(Time.time * 1.2f + _phase) * 0.04f;
                transform.position = new Vector3(transform.position.x, _homePosition.y + yOff, transform.position.z);
            }
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            if (_atomController != null && !_atomController.IsConsumed && this.enabled && gameObject.activeInHierarchy)
            {
                if (_rtnH != null) StopCoroutine(_rtnH);
                _rtnH = StartCoroutine(ReturnHome());
            }
        }

        private IEnumerator ReturnHome()
        {
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
