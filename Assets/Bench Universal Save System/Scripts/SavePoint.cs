using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Terresquall {
    public class SavePoint : MonoBehaviour {

        [Tooltip("When checked, saving the game does not pause gameplay.")]
        public bool asynchronous = true;
        public enum DetectionMode { tagName, className }
        public DetectionMode detectionMode = DetectionMode.tagName;
        public string detectionTarget = "Player";

        protected List<Component> objectsInRange = new List<Component>();

#if ENABLE_INPUT_SYSTEM
        public Key[] interactKeys = { Key.E };
#else
        public KeyCode[] interactKeys = { KeyCode.E };
#endif

        [Header("Feedback")]
        [Tooltip("Color of the object when someone is in range.")]
        public Color activeColor = new Color(.9f,.9f,.9f);
        [Tooltip("List of all Renderers this feedback should affect.")]
        public Renderer[] feedbackTargets;
        protected readonly Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

        // Delegates for other scripts to attach callbacks to.
        public event Action<Component> OnRangeEntry, OnRangeExit;

        protected virtual void Start() {
            // Get all renderers attached to this save point.
            foreach(Renderer r in feedbackTargets) {
                if(r is SpriteRenderer sr) originalColors.Add(r, sr.color);
                else originalColors.Add(r, r.sharedMaterial.color);
            }

            // Check if there are any trigger colliders. If not, print a messsage.
            Collider2D[] col2D = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D c in col2D)
                if (c.isTrigger) return;
            Collider[] col = GetComponentsInChildren<Collider>();
            foreach (Collider c in col)
                if (c.isTrigger) return;

            Debug.LogWarning($"No collider found in Save Point <{name}>. It will not work.");
        }

        protected virtual void Reset() {
            feedbackTargets = GetComponentsInChildren<Renderer>();
        }

        public virtual bool AreKeysPressed() {
#if ENABLE_INPUT_SYSTEM
            foreach(Key k in interactKeys) {
                if (Keyboard.current[k].IsPressed()) return true;
            }
#else
            foreach (KeyCode k in interactKeys) {
                if (Input.GetKeyDown(k)) return true;
            }
#endif
            return false;
        }

        protected virtual void Update() {
            if (objectsInRange.Count > 0 && AreKeysPressed()) {
                if (asynchronous) Bench.SaveGameAsync();
                else Bench.SaveGame();
            }
        }

        // Checks if a component is a valid target, according to the settings in the component.
        public bool IsValidTarget(Component other) {
            switch(detectionMode) {
                case DetectionMode.tagName:
                    return other.CompareTag(detectionTarget);
                case DetectionMode.className:
                    Type type = Type.GetType(detectionTarget);
                    if (type != null) {
                        return other.GetComponent(type) != null;
                    } else {
                        Debug.LogWarning("Class entered for Save Point was not found. Please ensure you use the fully-qualified class name.");
                        return false;
                    }
            }
            return false;
        }

        protected virtual void HandleRangeEntry(Component other) {
            if (IsValidTarget(other)) {
                if (!objectsInRange.Contains(other)) {
                    objectsInRange.Add(other);
                    foreach(Renderer r in feedbackTargets) {
                        if(r is SpriteRenderer sr) sr.color = activeColor;
                        else r.material.color = activeColor;
                    }

                    // Fire any attached callback events.
                    OnRangeEntry?.Invoke(other);
                }
            }
        }

        protected virtual void HandleRangeExit(Component other) {
            if (IsValidTarget(other)) {
                if (objectsInRange.Contains(other)) {
                    objectsInRange.Remove(other);
                    foreach(Renderer r in feedbackTargets) {
                        if(r is SpriteRenderer sr) sr.color = originalColors[r];
                        else r.material.color = originalColors[r];
                    }

                    // Fire any attached callback events.
                    OnRangeExit?.Invoke(other);
                }
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other) { HandleRangeEntry(other); }
        protected virtual void OnTriggerExit2D(Collider2D other) { HandleRangeExit(other); }
        protected virtual void OnTriggerEnter(Collider other) { HandleRangeEntry(other); }
        protected virtual void OnTriggerExit(Collider other) { HandleRangeExit(other); }
    }
}