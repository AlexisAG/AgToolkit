using UnityEngine;
using UnityEngine.Animations;

namespace FramaToolkit
{
	[RequireComponent(typeof(LookAtConstraint))]
	public class LookAtMainCamera : MonoBehaviour
	{
		[SerializeField]
		private float _MinDistance = .5f;

		private LookAtConstraint _LookAt;
		private Vector3 _OldPosition;
		private Vector3 _OldCamPosition;

		private void OnDisable()
		{
			_LookAt.constraintActive = true;
		}

		private void Start()
		{
			_LookAt = GetComponent<LookAtConstraint>();
			if (_LookAt.sourceCount == 0)
			{
				_LookAt.AddSource(new ConstraintSource { sourceTransform = Camera.main.transform, weight = 1f });
			}

			_LookAt.rotationOffset = new Vector3(0, 180, 0);
			_LookAt.constraintActive = true;
		}

		private void Update()
		{
			if (_OldCamPosition == Camera.main.transform.position && _OldPosition == transform.position)
			{
				return;
			}

			_LookAt.constraintActive = !((Camera.main.transform.position - transform.position).magnitude < _MinDistance);
			_OldPosition = transform.position;
			_OldCamPosition = Camera.main.transform.position;
		}
	}
}
