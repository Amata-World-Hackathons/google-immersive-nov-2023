using System.Collections;
using System.Collections.Generic;
using AmataWorld.Logging;
using Google.XR.ARCoreExtensions;
using UnityEngine;

namespace AmataWorld.Features.Semantics
{
    public class ARCoreSceneSemantics : MonoBehaviour
    {
        [SerializeField]
        ARSemanticManager _semanticManager;

        Texture2D _semanticImage;

        bool _didCheck = false;

        bool _isSemanticsAvailable = false;

        void Awake()
        {
        }

        void OnEnable()
        {
            if (_didCheck && !_isSemanticsAvailable)
            {
                gameObject.SetActive(false);
                this.LogWarning("cannot enable component, ARCore Semantic API is not available");
            }
        }

        void Update()
        {
            if (!_didCheck)
            {
                RunAPIAvailabilityCheck();
                if (!_isSemanticsAvailable) return;
            }

            if (_semanticManager.TryGetSemanticTexture(ref _semanticImage))
            {
            }
        }

        /// <summary>
        /// For the checks to be run successfully, we need to wait until the
        /// ARCore extensions component is enabled. That is why this function
        /// should only be run after the OnEnable step is complete
        /// </summary>
        void RunAPIAvailabilityCheck()
        {
            _didCheck = true;
            _isSemanticsAvailable = false;

            // can only be run after OnEnable is done
            switch (_semanticManager.IsSemanticModeSupported(SemanticMode.Enabled))
            {
                case FeatureSupported.Supported:
                    _isSemanticsAvailable = true;
                    break;

                case FeatureSupported.Unknown:
                    this.LogWarning("unable to determine if ARCore semantics API is supported");
                    break;

                case FeatureSupported.Unsupported:
                    break;

                default:
                    break;
            }

            if (!_isSemanticsAvailable)
            {
                gameObject.SetActive(false);
                this.LogWarning("ARCore semantic API is not enabled, automatically disabling component");
            }
        }
    }
}