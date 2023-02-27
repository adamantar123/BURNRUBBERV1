using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;


namespace GercStudio.DragRacingFramework
{
	public class SpawnPoint : MonoBehaviour
	{
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			var gameManager = FindObjectOfType<GameManager>();

			if (gameManager && Selection.gameObjects.Contains(gameManager.gameObject) ||
			    Selection.gameObjects.Contains(gameObject))
			{
				var isRaycast = Physics.Raycast(transform.position, Vector3.down, out var hitInfo);

				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 255, 0, 255);
				Handles.ArrowHandleCap(0, transform.position, transform.rotation, 4, EventType.Repaint);
				Handles.SphereHandleCap(0, transform.position, Quaternion.identity, 0.5f, EventType.Repaint);

				if (isRaycast)
				{
					Handles.color = new Color32(255, 255, 0, 150);
					Handles.DrawSolidDisc(hitInfo.point + Vector3.up * 0.01f, Vector3.up, 2);
				}

				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 255, 0, 100);
				Handles.ArrowHandleCap(0, transform.position, transform.rotation, 4, EventType.Repaint);
				Handles.SphereHandleCap(0, transform.position, Quaternion.identity, 0.5f, EventType.Repaint);

				if (isRaycast)
				{
					Handles.color = new Color32(255, 255, 0, 50);
					Handles.DrawSolidDisc(hitInfo.point + Vector3.up * 0.01f, Vector3.up, 2);
				}
			}
		}
#endif
	}
}


