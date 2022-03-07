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
		public Animator rigAnimator;
		public GameObject equippedCamera;

		public string objectPrimary = "tablet";
		public string objectSecondary = "axe";

		public float cameraToSecondaryTime = 0.5f;
		public float cameraToPrimaryTime = 0.5f;

		public float switchFromSecondaryTime = 0.25f;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool primaryEquipped;
		public bool secondaryEquipped;

		bool primaryUsed = false;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private void Start()
        {
			rigAnimator.enabled = false;
        }
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif
		public void OnMoveInput(InputAction.CallbackContext context)
		{
			move = context.ReadValue<Vector2>();
			//move = newMoveDirection;
		} 

		public void OnLookInput(InputAction.CallbackContext context)
		{
			look = context.ReadValue<Vector2>();
			//look = newLookDirection;
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

			//jump = newJumpState;
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

			//sprint = newSprintState;
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

					rigAnimator.SetTrigger("use_primary");

					StartCoroutine(ResetPrimaryUse(rigAnimator.GetCurrentAnimatorStateInfo(1).length));
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