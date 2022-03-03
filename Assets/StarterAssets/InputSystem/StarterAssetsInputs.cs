using UnityEngine;
using System.Collections;
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

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool primaryEquipped;
		public bool secondaryEquipped;

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

        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnEquipPrimary(InputValue value)
        {
			EquipPrimaryInput(value.isPressed);
        }

		public void OnEquipSecondary(InputValue value)
		{
			EquipSecondaryInput(value.isPressed);
		}
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void EquipPrimaryInput(bool newEquipPrimaryState)
        {
			rigAnimator.enabled = true;

			StartCoroutine(HolsterObject("holster_primary"));
		}

		public void EquipSecondaryInput(bool newEquipSecondaryState)
        {
			rigAnimator.enabled = true;

			StartCoroutine(HolsterObject("holster_secondary"));
		}

		private IEnumerator HolsterObject(string holsterSlot)
        {
			switch (holsterSlot)
            {
				case "holster_primary":
					primaryEquipped = !primaryEquipped;

					if (secondaryEquipped)
					{
						secondaryEquipped = !primaryEquipped;
						rigAnimator.SetBool("holster_secondary", !secondaryEquipped);

						do
						{
							yield return new WaitForEndOfFrame();
						}
						while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
					}

					rigAnimator.SetBool(holsterSlot, !primaryEquipped);

					if (primaryEquipped)
					{
						rigAnimator.Play("equip_" + objectPrimary);

						do
						{
							yield return new WaitForEndOfFrame();
						}
						while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
					}

					equippedCamera.SetActive(primaryEquipped);
					break;
				case "holster_secondary":
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

						equippedCamera.SetActive(primaryEquipped);
                    }

					rigAnimator.SetBool(holsterSlot, !secondaryEquipped);

					if (secondaryEquipped)
						rigAnimator.Play("equip_" + objectSecondary);
					break;
            }
		}

#if !UNITY_IOS || !UNITY_ANDROID

#endif

	}
	
}