using System.Collections.Generic;
using UnityEngine;

namespace Flusk.Utility
{
	/// <summary>
	/// Factory used to create singleton prefabs for a scene
	/// </summary>
	public class PrefabFactory : MonoBehaviour
	{
		#region Fields
		/// <summary>
		/// List of prefabs to create
		/// </summary>
		[SerializeField]
		protected List<GameObject> prefabs;

		protected static Dictionary<string, GameObject> createdObjects;
		#endregion


		#region Methods
		/// <summary>
		/// Create all prefabs on awwake
		/// </summary>
		protected virtual void Awake()
		{
			CreatePrefabs();
		}

		/// <summary>
		/// Create all prefabs that don't exist already
		/// </summary>
		protected virtual void CreatePrefabs()
		{
			if (createdObjects == null)
			{
				createdObjects = new Dictionary<string, GameObject>();
			}

			foreach (GameObject prefab in prefabs)
			{
				TryCreatePrefab(prefab);
			}
			Object.Destroy(gameObject);
		}

		/// <summary>
		/// Attempt to create a prefab
		/// </summary>
		protected virtual void TryCreatePrefab(GameObject prefab)
		{
			GameObject createdOb;

			if(createdObjects.TryGetValue(prefab.name, out createdOb))
			{
				if(createdOb == null)
				{
					// Object was destroyed, so this is okay. Clear the reference
					createdObjects.Remove(prefab.name);
				}
			}

			// Check again - might've been removed above
			if(!createdObjects.ContainsKey(prefab.name))
			{
				// If the dictionary doesn't contain the object, it means we've never created one before
				// and it's okay to instantiate it

				createdOb = Object.Instantiate(prefab);
				createdObjects.Add(prefab.name, createdOb);
			}
		}
		#endregion
	}
}