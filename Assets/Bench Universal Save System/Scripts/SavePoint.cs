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

        [Header("Text Hint")]
        [Tooltip("Optional text that appears when the player is inside the Save Point range.")]
        public TMP_Text textHint;

        [Tooltip("When checked, the text hint is hidden while no valid target is inside the Save Point range.")]
        public bool hideTextHintWhenOutOfRange = true;

        [Tooltip("Text shown when a valid target enters the Save Point range.")]
        public string defaultTextHint = "Press E to Save";

        [Tooltip("Text shown after the game has been saved.")]
        public string savedTextHint = "Game Saved";

        protected List<Component> objectsInRange = new List<Component>();

#if ENABLE_INPUT_SYSTEM
        public Key[] interactKeys = { Key.E };
#else
        public KeyCode[] interactKeys = { KeyCode.E };
#endif

        protected virtual void Start() {
            // Check if there are any trigger colliders. If not, print a messsage.
            Collider2D[] col2D = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D c in col2D)
                if (c.isTrigger) return;
            Collider[] col = GetComponentsInChildren<Collider>();
            foreach (Collider c in col)
                if (c.isTrigger) return;

            Debug.LogWarning($"No collider found in Save Point <{name}>. It will not work.");
        }

        protected virtual void OnEnable() {
            ResetTextHint();
            SetTextHintVisible(!hideTextHintWhenOutOfRange);
        }

        public virtual bool AreKeysPressed() {
#if ENABLE_INPUT_SYSTEM
            foreach (Key k in interactKeys) {
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

                ShowSavedTextHint();
            }
        }

        public bool IsValidTarget(Component other) {
            switch (detectionMode) {
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
                if (!objectsInRange.Contains(other))
                    objectsInRange.Add(other);

                ResetTextHint();
                SetTextHintVisible(true);
            }
        }

        protected virtual void HandleRangeExit(Component other) {
            if (IsValidTarget(other)) {
                if (objectsInRange.Contains(other))
                    objectsInRange.Remove(other);

                ResetTextHint();

                if (objectsInRange.Count == 0 && hideTextHintWhenOutOfRange) {
                    SetTextHintVisible(false);
                }
            }
        }

        protected virtual void ResetTextHint() {
            if (textHint != null) {
                textHint.text = defaultTextHint;
            }
        }

        protected virtual void ShowSavedTextHint() {
            if (textHint != null) {
                textHint.text = savedTextHint;
            }
        }

        protected virtual void SetTextHintVisible(bool visible) {
            if (textHint != null) {
                textHint.gameObject.SetActive(visible);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other) { HandleRangeEntry(other); }
        protected virtual void OnTriggerExit2D(Collider2D other) { HandleRangeExit(other); }
        protected virtual void OnTriggerEnter(Collider other) { HandleRangeEntry(other); }
        protected virtual void OnTriggerExit(Collider other) { HandleRangeExit(other); }
    }
}