using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public Quest axeQuest;

		public GameObject axePrefab;

		public Animator rigAnimator;
		public GameObject equippedCamera;

		public string objectPrimary = "tablet";
		public string objectSecondary = "axe";

		public float cameraToSecondaryTime = 0.5f;
		public float cameraToPrimaryTime = 0.5f;

		public float switchFromSecondaryTime = 0.25f;

		public GameObject mapCamera;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool primaryEquipped;
		public bool secondaryEquipped;

		bool primaryUsed = false;

		bool hasAxe = false;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private void OnEnable()
        {
			EventSystemNew<Quest>.Subscribe(Event_Type.ACTIVATE_QUEST, ActivateAxe);
        }

        private void OnDisable()
        {
			EventSystemNew<Quest>.Unsubscribe(Event_Type.ACTIVATE_QUEST, ActivateAxe);
		}

        private void Start()
        {
			axePrefab.SetActive(false);

			rigAnimator.enabled = false;

			// Load Map Texture
			mapCamera.SetActive(true);

			Invoke("DisableMapCamera", 0.25f);
        }

		private void ActivateAxe(Quest _quest)
        {
			if (axeQuest == _quest)
            {
				axePrefab.SetActive(true);

				hasAxe = true;
            }
        }

		private void DisableMapCamera()
        {
			mapCamera.SetActive(false);
        }
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif
		public void OnMoveInput(InputAction.CallbackContext context)
		{
			move = context.ReadValue<Vector2>();
		} 

		public void OnLookInput(InputAction.CallbackContext context)
		{
			look = context.ReadValue<Vector2>();
		}

		public void OnJumpInput(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Started)
            {
				jump = true;
            }
			else if (context.phase == InputActionPhase.Canceled)
			{
				jump = false;
            }
		}

		public void OnSprintInput(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
			{
				sprint = true;
			}
			else if (context.phase == InputActionPhase.Canceled)
            {
				sprint = false;
            }
		}

		public void OnEquipPrimaryInput(InputAction.CallbackContext context)
        {
			if (context.phase == InputActionPhase.Started)
            {
				rigAnimator.enabled = true;

				StartCoroutine(HolsterObject("holster_primary"));
			}
		}

		public void OnEquipSecondaryInput(InputAction.CallbackContext context)
        {
			if (context.phase == InputActionPhase.Started)
			{
				rigAnimator.enabled = true;

				StartCoroutine(HolsterObject("holster_secondary"));
			}
		}

		public void OnUsePrimaryInput(InputAction.CallbackContext context)
        {
			if (context.phase == InputActionPhase.Started)
			{
				if (primaryEquipped && !primaryUsed)
				{
					if (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
						return;

					primaryUsed = true;

					EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_POSITION, true);

					rigAnimator.SetTrigger("use_primary");

					StartCoroutine(ResetPrimaryUse(rigAnimator.GetCurrentAnimatorStateInfo(0).length));
				}
			}
        }

		public void OnTalkToNPCInput(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Started)
			{
				if (!primaryUsed)
				{
					EventSystemNew.RaiseEvent(Event_Type.TALK_TO_NPC);
				}
			}
		}

		private IEnumerator ResetPrimaryUse(float resetTime)
        {
			yield return new WaitForSeconds(resetTime);

			primaryUsed = false;

			EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_POSITION, false);
		}

		private IEnumerator HolsterObject(string holsterSlot)
        {
			switch (holsterSlot)
            {
				case "holster_primary":
					if (!primaryUsed)
					{
						bool switchFromSecondary = false;

						primaryEquipped = !primaryEquipped;

						if (secondaryEquipped)
						{
							EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, false);
							EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, false);

							switchFromSecondary = true;

							secondaryEquipped = !primaryEquipped;
							rigAnimator.SetBool("holster_secondary", !secondaryEquipped);

							do
							{
								yield return new WaitForEndOfFrame();
							}
							while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < cameraToPrimaryTime);

							equippedCamera.SetActive(secondaryEquipped);
						}

						rigAnimator.SetBool(holsterSlot, !primaryEquipped);

						if (primaryEquipped)
						{
							if (switchFromSecondary)
							{
								do
								{
									yield return new WaitForEndOfFrame();
								}
								while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < switchFromSecondaryTime);
							}

							rigAnimator.Play("equip_" + objectPrimary);
						}
					}
					break;
				case "holster_secondary":
					if (!primaryUsed)
					{
						secondaryEquipped = !secondaryEquipped;

						if (primaryEquipped)
						{
							primaryEquipped = !secondaryEquipped;
							rigAnimator.SetBool("holster_primary", !primaryEquipped);

							do
							{
								yield return new WaitForEndOfFrame();
							}
							while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
						}

						rigAnimator.SetBool(holsterSlot, !secondaryEquipped);

						mapCamera.SetActive(secondaryEquipped);

						if (secondaryEquipped)
						{
							rigAnimator.Play("equip_" + objectSecondary);

							do
							{
								yield return new WaitForEndOfFrame();
							}
							while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < cameraToSecondaryTime);
						}

						equippedCamera.SetActive(secondaryEquipped);

						EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, secondaryEquipped);
						EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, secondaryEquipped);
					}
					break;
            }
		}

#if !UNITY_IOS || !UNITY_ANDROID

#endif

	}
	
}