using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
	public static class TransformExtensions
	{
		public static Transform FindChildByContainsName(this Transform transform, string name)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);

				if (child.gameObject.name.Contains(name))
				{
					return child;
				}

				child = FindChildByContainsName(child, name);
				if (child != null)
				{
					return child;
				}
			}

			return null;
		}

		public static Transform FindChildByName(this Transform transform, string name)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);

				if (child.gameObject.name == name)
				{
					return child;
				}

				child = FindChildByName(child, name);
				if (child != null)
				{
					return child;
				}
			}

			return null;
		}
	}
}
