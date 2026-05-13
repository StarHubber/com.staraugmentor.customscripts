using UnityEngine;

namespace StarCooperation
{
	public class EnsureSiblingIndex : MonoBehaviour
	{
		public enum SiblingIndex
		{
			First,
			Last,
			Index
		}
		public SiblingIndex keepAtIndex;

		[Tooltip("Only assign when not First or Last.")]
		public int index;

		// Start is called before the first frame update
		void Start()
		{
			if (keepAtIndex == SiblingIndex.First)
			{
				transform.SetAsFirstSibling();
			}
			else if (keepAtIndex == SiblingIndex.Last)
			{
				transform.SetAsLastSibling();
			}
			else if (keepAtIndex == SiblingIndex.Index)
			{
				transform.SetSiblingIndex(index);
			}
		}
	}
}