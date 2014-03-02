using UnityEngine;

[ExecuteInEditMode]
public class Board : MonoBehaviour
{
#if UNITY_EDITOR
	public bool refreshSlots = false;
#endif

	Slot[] slots;

	void Awake()
	{
		InitializeSlots();

#if UNITY_EDITOR
		RefreshSlots();
#endif
	}

#if UNITY_EDITOR
	void Update()
	{
		if (refreshSlots)
		{
			refreshSlots = false;
			InitializeSlots();
			RefreshSlots();
		}
	}
#endif

	void InitializeSlots()
	{
		slots = GetComponentsInChildren<Slot>();
		System.Array.Sort<Slot>(slots, delegate(Slot a, Slot b){
			int zDiff = Mathf.RoundToInt(b.transform.localPosition.z - a.transform.localPosition.z); if (zDiff != 0) return zDiff;
			int yDiff = Mathf.RoundToInt(b.transform.localPosition.y - a.transform.localPosition.y); if (yDiff != 0) return yDiff;
			int xDiff = Mathf.RoundToInt(a.transform.localPosition.x - b.transform.localPosition.x); return xDiff;
		});
	}

#if UNITY_EDITOR
	void RefreshSlots()
	{
		foreach (Slot slot in slots)
		{
			BoxCollider slotCollider = slot.collider as BoxCollider;
			Vector3 slotCenter = slotCollider.transform.localPosition + slotCollider.center;

			Vector3[] slotCorners = new Vector3[4];
			slotCorners[0] = slotCenter + new Vector3(-slotCollider.size.x, -slotCollider.size.y, -slotCollider.size.z) * 0.5f;
			slotCorners[1] = slotCenter + new Vector3(-slotCollider.size.x, slotCollider.size.y, -slotCollider.size.z) * 0.5f;
			slotCorners[2] = slotCenter + new Vector3(slotCollider.size.x, slotCollider.size.y, -slotCollider.size.z) * 0.5f;
			slotCorners[3] = slotCenter + new Vector3(slotCollider.size.x, -slotCollider.size.y, -slotCollider.size.z) * 0.5f;

			slot.OverlappingSlots.Clear();

			foreach (Vector3 slotCorner in slotCorners)
			{
				Vector3 slotCornerWorld = slot.transform.parent.TransformPoint(slotCorner);
				RaycastHit[] hits = Physics.RaycastAll(slotCornerWorld - Vector3.forward * 10f, Vector3.forward);
				foreach (RaycastHit hit in hits)
				{
					Slot hitSlot = hit.collider.gameObject.GetComponent<Slot>();
					if (hitSlot != null && hitSlot != slot && hitSlot.transform.localPosition.z < slot.transform.localPosition.z)
					{
						slot.OverlappingSlots.Add(hitSlot);
					}
				}
			}
		}
	}
#endif

	public Slot this[int index]
	{
		get
		{
			return slots[index];
		}
	}

	public int Size
	{
		get
		{
			return slots.Length;
		}
	}
}
