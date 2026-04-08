using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Data;
using VRMolecularLab.UI;

namespace VRMolecularLab.Core
{
    public class MoleculeInstance : MonoBehaviour
    {
        public MoleculeData Data;
        public List<AtomController> ConsumedAtoms = new List<AtomController>();

        private Vector3 _homePosition;
        private Quaternion _homeRotation;
        private Coroutine _returnHomeCoroutine;
        private XRGrabInteractable _interactable;

        private void Start()
        {
            _homePosition = transform.position;
            _homeRotation = transform.rotation;
            
            _interactable = GetComponent<XRGrabInteractable>();
            if (_interactable != null)
            {
                _interactable.selectExited.AddListener(OnReleased);
                _interactable.selectEntered.AddListener(OnGrabbed);
            }
        }

        private void OnDestroy()
        {
            if (_interactable != null)
            {
                _interactable.selectExited.RemoveListener(OnReleased);
                _interactable.selectEntered.RemoveListener(OnGrabbed);
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            if (_returnHomeCoroutine != null)
            {
                StopCoroutine(_returnHomeCoroutine);
                _returnHomeCoroutine = null;
            }
            if (MoleculeDetailsUI.Instance != null)
            {
                MoleculeDetailsUI.Instance.ShowMolecule(this);
            }
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            if (MoleculeDetailsUI.Instance != null)
            {
                MoleculeDetailsUI.Instance.HideMolecule();
            }
            if (gameObject.activeInHierarchy)
            {
                if (_returnHomeCoroutine != null) StopCoroutine(_returnHomeCoroutine);
                _returnHomeCoroutine = StartCoroutine(ReturnHome());
            }
        }

        private IEnumerator ReturnHome()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            
            while (elapsed < 0.4f)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, _homePosition, elapsed / 0.4f);
                transform.rotation = Quaternion.Lerp(startRot, _homeRotation, elapsed / 0.4f);
                yield return null;
            }
            
            transform.position = _homePosition;
            transform.rotation = _homeRotation;
            _returnHomeCoroutine = null;
        }

        public void Dissolve()
        {
            if (ConsumedAtoms != null)
            {
                foreach (var atom in ConsumedAtoms)
                {
                    if (atom != null)
                    {
                        atom.ResetAtom();
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
