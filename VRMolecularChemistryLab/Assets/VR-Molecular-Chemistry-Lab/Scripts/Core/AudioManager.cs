using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRMolecularLab.Data;

namespace VRMolecularLab.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Clips")]
        public AudioClip bondSuccessClip;
        public AudioClip bondFailClip;
        public AudioClip ambientClip;
        public AudioClip atomPlacedClip;

        // An array of references to controllers in the scene for haptic feedback
        // Use XRBaseController (since Doc mentions SendHapticImpulse on XRBaseController)
        public UnityEngine.XR.Interaction.Toolkit.XRBaseController[] controllers;

        private AudioSource _audioSource;
        private AudioSource _ambientSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _audioSource = GetComponent<AudioSource>();
            
            // Adding a secondary AudioSource for ambient loop
            _ambientSource = gameObject.AddComponent<AudioSource>();
            _ambientSource.loop = true;
            _ambientSource.spatialBlend = 0f; // 2D ambient
            _ambientSource.volume = 0.2f;
            _ambientSource.clip = ambientClip;

            if (ambientClip != null)
            {
                _ambientSource.Play();
            }
        }

        private void Start()
        {
            // Subscribe to BondManager events
            if (BondManager.Instance != null)
            {
                BondManager.Instance.OnMoleculeFormed += HandleMoleculeFormed;
                BondManager.Instance.OnBondFailed += HandleBondFailed;
            }
        }

        private void OnDestroy()
        {
            if (BondManager.Instance != null)
            {
                BondManager.Instance.OnMoleculeFormed -= HandleMoleculeFormed;
                BondManager.Instance.OnBondFailed -= HandleBondFailed;
            }
        }

        private void HandleMoleculeFormed(MoleculeData data)
        {
            PlayBondSuccess();
        }

        private void HandleBondFailed()
        {
            PlayBondFail();
        }

        public void PlayBondSuccess()
        {
            if (bondSuccessClip != null)
            {
                _audioSource.PlayOneShot(bondSuccessClip);
            }
            
            TriggerHaptics(0.8f, 0.15f);
        }

        public void PlayBondFail()
        {
            if (bondFailClip != null)
            {
                _audioSource.PlayOneShot(bondFailClip);
            }
            
            TriggerHaptics(0.3f, 0.05f);
        }

        public void PlayAtomPlaced()
        {
            if (atomPlacedClip != null)
            {
                _audioSource.PlayOneShot(atomPlacedClip);
            }
        }

        private void TriggerHaptics(float amplitude, float duration)
        {
            if (controllers == null) return;
            
            foreach (var controller in controllers)
            {
                if (controller != null)
                {
                    controller.SendHapticImpulse(amplitude, duration);
                }
            }
        }
    }
}
