using UnityEngine;

[ExecuteInEditMode]
public class Board : MonoBehaviour
{
	public enum Shape
	{
		Peaks,
		Columns
	}

	Slot[] slots = new Slot[0];
	Vector2 cardSpacing = Vector2.zero, boardBounds = Vector2.zero;
	
	void RefreshSlots()
	{
		slots = GetComponentsInChildren<Slot>();
		System.Array.Sort<Slot>(slots, delegate(Slot a, Slot b){
			int zDiff = Mathf.RoundToInt(b.transform.localPosition.z - a.transform.localPosition.z); if (zDiff != 0) return zDiff;
			int yDiff = Mathf.RoundToInt(b.transform.localPosition.y - a.transform.localPosition.y); if (yDiff != 0) return yDiff;
			int xDiff = Mathf.RoundToInt(a.transform.localPosition.x - b.transform.localPosition.x); return xDiff;
		});

		foreach (Slot slot in slots)
		{
			BoxCollider slotCollider = slot.gameObject.AddComponent<BoxCollider>();
			slotCollider.size = NGUITools.FindActive<Deck>()[0].CardBack.bounds.size;
		}

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

		foreach (Slot slot in slots)
		{
			BoxCollider slotCollider = slot.collider as BoxCollider;
			slotCollider.enabled = false;
		}
	}

	public void ArrangeSlotsAsPeaks(int numPeaks, int peakHeight)
	{
		slots = GetComponentsInChildren<Slot>();
		foreach (Slot slot in slots) NGUITools.Destroy(slot.gameObject);

		cardSpacing = new Vector2(90, 80);
		boardBounds = new Vector2((float)((peakHeight - 1) * numPeaks) * 0.5f * cardSpacing.x, (float)(peakHeight - 1) * 0.5f * cardSpacing.y);

		int numSlotsInBottomRow = (peakHeight - 1) * numPeaks + 1;
		Slot[] slotsInBottomRow = new Slot[numSlotsInBottomRow];
		for (int i = 0; i < numSlotsInBottomRow; i++)
		{
			slotsInBottomRow[i] = Slot.Create(gameObject);
			slotsInBottomRow[i].transform.localPosition = new Vector3(-boardBounds.x + i * cardSpacing.x, -boardBounds.y, 0);
		}

		BetterList<BetterList<Slot>> slotsByPeak = new BetterList<BetterList<Slot>>();
		for (int i = 0; i < numPeaks; i++)
		{
			slotsByPeak.Add(new BetterList<Slot>());
			for (int j = 0; j < peakHeight; j++)
			{
				slotsByPeak[i].Add(slotsInBottomRow[i * (peakHeight - 1) + j]);
			}
		}

		foreach (BetterList<Slot> peakSlots in slotsByPeak)
		{
			for (int rowSize = peakHeight - 1; rowSize > 0; rowSize--)
			{
				for (int i = 0; i < rowSize; i++)
				{
					Slot slot = Slot.Create(gameObject);
					Slot slotBelow = peakSlots[peakSlots.size - rowSize - 1];
					slot.transform.localPosition = new Vector3(slotBelow.transform.localPosition.x + cardSpacing.x * 0.5f,
					                                           slotBelow.transform.localPosition.y + cardSpacing.y,
					                                           slotBelow.transform.localPosition.z + 0.1f);
					peakSlots.Add(slot);
				}
			}
		}

		RefreshSlots();
	}

	public void ArrangeSlotsAsColumns(int numColumns, int columnHeight)
	{
		slots = GetComponentsInChildren<Slot>();
		foreach (Slot slot in slots) NGUITools.Destroy(slot.gameObject);

		cardSpacing = new Vector2(90, 80);
		boardBounds = new Vector2((float)(numColumns - 1) * 0.5f * cardSpacing.x, (float)(columnHeight - 1) * 0.5f * cardSpacing.y);

		BetterList<BetterList<Slot>> slotsByColumn = new BetterList<BetterList<Slot>>();

		for (int i = 0; i < numColumns; i++)
		{
			slotsByColumn.Add(new BetterList<Slot>());

			Slot slot = Slot.Create(gameObject);
			slot.transform.localPosition = new Vector3(-boardBounds.x + i * cardSpacing.x, -boardBounds.y, 0);
			slotsByColumn[i].Add(slot);
		}

		for (int i = 0; i < numColumns; i++)
		{
			for (int j = 1; j < columnHeight; j++)
			{
				Slot slot = Slot.Create(gameObject);
				Slot slotBelow = slotsByColumn[i][j - 1];
				slot.transform.localPosition = new Vector3(slotBelow.transform.localPosition.x,
				                                           slotBelow.transform.localPosition.y + cardSpacing.y,
				                                           slotBelow.transform.localPosition.z + 0.1f);
				slotsByColumn[i].Add(slot);
			}
		}

		RefreshSlots();
	}

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

	public Vector2 Bounds
	{
		get
		{
			return boardBounds + cardSpacing * 0.5f;
		}
	}
}
