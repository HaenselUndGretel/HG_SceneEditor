using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneEditor.GameContent
{
	public enum EditorState
	{
		Standard,
		PlaceMoveArea,
		PlaceWayPoint,
		PlaceInteractiveObject,
		PlaceSprites,
		PlaceCollectable,
		PlaceItem,
		PlaceLight
	}
}
