using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class ThirdPersonController : MonoBehaviour
	{
		//プレイヤーヘッダー
		public float MoveSpeed = 2.0f;              //キャラクターの移動速度
		public float SprintSpeed = 5.335f;          //キャラクターの全力疾走速度
		public float RotationSmoothTime = 0.12f;    //キャラクターが顔の移動方向に回転する速度
		public float SpeedChangeRate = 10.0f;       //加速と減速
		public float Sensitivity = 1f;

		public float JumpHeight = 1.2f;             //プレイヤーがジャンプできる高さ
		public float Gravity = -15.0f;				//重力

		public float JumpTimeout = 0.50f;           //再びジャンプできるようになるまでに経過するのに必要な時間
		public float FallTimeout = 0.15f;           //落下状態に入るまでの経過時間

		public bool Grounded = true;                //キャラクターが接地されているかどうか
		public float GroundedOffset = -0.14f;       //荒れた地面に便利
		public float GroundedRadius = 0.28f;        //接地されたチェックの半径
		public LayerMask GroundLayers;              //キャラクターが地面として使用するレイヤー
		public int PlayerHP = 100;

		//シネマシンヘッダー
		public GameObject CinemachineCameraTarget;  //カメラがフォローするCinemachine仮想カメラに設定されたフォローターゲット
		public float TopClamp = 70.0f;              //カメラをどれだけ上に動かすか
		public float BottomClamp = -30.0f;          //カメラをどれだけ下に動かすか
		public float CameraAngleOverride = 0.0f;    //カメラをオーバーライドするための追加
		public bool LockCameraPosition = false;     //すべての軸でカメラの位置をロックするため

		// シネマシン
		private float cinemachineTargetYaw;
		private float cinemachineTargetPitch;

		//プレイヤー
		private float speed;
		private float animationBlend;
		private float targetRotation = 0.0f;
		private float rotationVelocity;
		private float verticalVelocity;
		private float terminalVelocity = 53.0f;

		//デルタタイム
		private float jumpTimeoutDelta;
		private float fallTimeoutDelta;

		//アニメーションID
		private int animIDSpeed;
		private int animIDGrounded;
		private int animIDJump;
		private int animIDFreeFall;
		private int animIDMotionSpeed;
		private int animIDDeath;

		private Animator animator;
		private CharacterController controller;
		private StarterAssetsInputs input;
		private GameObject mainCamera;
		private bool rotateOnMove = true;

		private const float threshold = 0.01f;

		private bool hasAnimator;

		Slider slider;

		private void Awake()
		{
			//メインカメラへの参照を取得します
			if (mainCamera == null)
			{
				mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			hasAnimator = TryGetComponent(out animator);
			controller = GetComponent<CharacterController>();
			input = GetComponent<StarterAssetsInputs>();

			AssignAnimationIDs();

			//開始時にタイムアウトをリセットする
			jumpTimeoutDelta = JumpTimeout;
			fallTimeoutDelta = FallTimeout;

			slider = GameObject.Find("PlayerHP").GetComponent<Slider>();
			slider.value = 1;
		}

		private void Update()
		{
			hasAnimator = TryGetComponent(out animator);
			
			JumpAndGravity();
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void AssignAnimationIDs()
		{
			animIDSpeed       = Animator.StringToHash("Speed");
			animIDGrounded    = Animator.StringToHash("Grounded");
			animIDJump        = Animator.StringToHash("Jump");
			animIDFreeFall    = Animator.StringToHash("FreeFall");
			animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
			animIDDeath       = Animator.StringToHash("Death");
		}

		private void GroundedCheck()
		{
			//球の位置をオフセット付きで設定
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

			//キャラクターを使用している場合はアニメーターを更新する
			if (hasAnimator)
			{
				animator.SetBool(animIDGrounded, Grounded);
			}
		}

		private void CameraRotation()
		{
			//入力があり、カメラの位置が固定されていない場合
			if (input.look.sqrMagnitude >= threshold && !LockCameraPosition)
			{
				cinemachineTargetYaw   += input.look.x * Time.deltaTime * Sensitivity;
				cinemachineTargetPitch += input.look.y * Time.deltaTime * Sensitivity;
			}

			//回転をクランプして、値が360度に制限されるようにします
			cinemachineTargetYaw   = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
			cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

			//Cinemachineはこの目標に従います
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
		}

		private void Move()
		{
			//移動速度、スプリント速度、およびスプリントが押された場合に基づいて目標速度を設定します
			float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;

			//取り外し、交換、反復が簡単にできるように設計された単純な加速と減速

			//入力がない場合は、目標速度を0に設定します
			if (input.move == Vector2.zero) targetSpeed = 0.0f;

			//プレーヤーの現在の水平速度への参照
			float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

			//目標速度まで加速または減速します
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				//より有機的な速度変化を与える線形ではなく湾曲した結果を作成します
				//LerpのTはクランプされているため、速度をクランプする必要はありません
				speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				//小数点以下第3位までの丸め速度
				speed = Mathf.Round(speed * 1000f) / 1000f;
			}
			else
			{
				speed = targetSpeed;
			}
			animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

			//入力方向を正規化する
			Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

			//移動入力がある場合、プレーヤーが移動しているときにプレーヤーを回転させます
			if (input.move != Vector2.zero)
			{
				targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

				//カメラの位置を基準にして入力方向を向くように回転します
				if (rotateOnMove)
				{
					transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
				}
			}

			Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

			//プレーヤーを動かす
			controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

			//キャラクターを使用している場合はアニメーターを更新する
			if (hasAnimator)
			{
				animator.SetFloat(animIDSpeed, animationBlend);
				animator.SetFloat(animIDMotionSpeed, inputMagnitude);
			}
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				//落下タイムアウトタイマーをリセットする
				fallTimeoutDelta = FallTimeout;

				//キャラクターを使用している場合はアニメーターを更新する
				if (hasAnimator)
				{
					animator.SetBool(animIDJump, false);
					animator.SetBool(animIDFreeFall, false);
				}

				//接地時に速度が無限に低下するのを防ぎます
				if (verticalVelocity < 0.0f)
				{
					verticalVelocity = -2f;
				}

				//ジャンプ
				if (input.jump && jumpTimeoutDelta <= 0.0f)
				{
					//Hの平方根* -2 * G =目的の高さに到達するために必要な速度
					verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

					//キャラクターを使用している場合はアニメーターを更新する
					if (hasAnimator)
					{
						animator.SetBool(animIDJump, true);
					}
				}

				//ジャンプタイムアウト
				if (jumpTimeoutDelta >= 0.0f)
				{
					jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				//ジャンプタイムアウトタイマーをリセットする
				jumpTimeoutDelta = JumpTimeout;

				//タイムアウト
				if (fallTimeoutDelta >= 0.0f)
				{
					fallTimeoutDelta -= Time.deltaTime;
				}
				else
				{
					//キャラクターを使用している場合はアニメーターを更新する
					if (hasAnimator)
					{
						animator.SetBool(animIDFreeFall, true);
					}
				}

				//接地されていない場合はジャンプしないでください
				input.jump = false;
			}

			//ターミナルの下にある場合は、時間の経過とともに重力を適用します
			if (verticalVelocity < terminalVelocity)
			{
				verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed   = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			//選択したら、接地されたコライダーの位置とそれに一致する半径にギズモを描画します
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
		public void SetSensitivity(float newSensitivity)
        {
			Sensitivity = newSensitivity;
        }

		public void SetRotateOnMove(bool newRotateOnMove)
        {
			rotateOnMove = newRotateOnMove;
        }

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			// 敵の弾に当たったら
            if (hit.gameObject.tag == "Projectile")
            {
				// スライダー(HP)を-0.05
				slider.value -= 0.05f;
				// デバッグ表示
                Debug.Log("Hit"); // ログを表示する
				// スライダー(HP)が0になったら
				if (slider.value <= 0f)
                {
					// Deathアニメーション再生
					animator.SetTrigger("death");
					// 4秒後に実行
					Invoke("SceneChange", 4.0f);
				}
            }
			// アイテムを拾ったら
			if (hit.gameObject.tag == "Item")
            {
				// スライダー(HP)を+5
				slider.value += 0.5f;
				// デバッグ表示
				Debug.Log("Heel");
				// オブジェクトを破壊
				hit.gameObject.SetActive(false);
            }
		}
		void SceneChange()
        {
			// ゲームオーバーに移行
			SceneManager.LoadScene("GameOver");
		}
	}
}