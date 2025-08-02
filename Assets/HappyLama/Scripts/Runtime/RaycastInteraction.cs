using UnityEngine;

namespace HappyLama
{
    public class RaycastInteraction : MonoBehaviour
    {
        [Header("Raycast Settings")]
        public float interactionDistance = 3f;
        public LayerMask interactionLayer;

        private Camera playerCamera;
        private DialogueTrigger currentTarget;
        private DialogUIManager dialogUIManager;

        private void Start()
        {
            playerCamera = GetComponentInChildren<Camera>();
            dialogUIManager = FindObjectOfType<DialogUIManager>();
        }

        private void Update()
        {
            HandleRaycast();
        }

        private void HandleRaycast()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (currentTarget != null)
            {
                currentTarget = null;
            }

            if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
            {
                DialogueTrigger trigger = hit.collider.GetComponent<DialogueTrigger>();
                if (trigger != null && trigger.useRaycast)
                {
                    currentTarget = trigger;


                    if (Input.GetKeyDown(trigger.interactionKey) &&
                        (dialogUIManager == null || !dialogUIManager.IsDialogueActive()))
                    {
                        trigger.StartDialogue();
                    }
                }
            }
        }
    }
}