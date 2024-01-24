using Exoa.Designer;
using UnityEngine;
using static Exoa.Designer.DataModel;

namespace Exoa.Designer
{
    public class SpaceMaterialController : MonoBehaviour
    {
        public MeshRenderer walls;
        public MeshRenderer floor;
        public MeshRenderer ceiling;

        private string floorMatName;
        private string ceilingMatName;
        private string wallMatName;

        public void ApplyCeilingMaterial(Material mat)
        {
            ceiling.material = mat;
            ceilingMatName = mat.name;
        }
        public void ApplyFloorMaterial(Material mat)
        {
            floor.material = mat;
            floorMatName = mat.name;
        }

        public void ApplyInteriorWallMaterial(Material mat)
        {
            walls.material = mat;
            wallMatName = mat.name;
        }

#if INTERIOR_MODULE
        public RoomSetting GetRoomSetting()
        {
            RoomSetting rs = new RoomSetting();
            rs.wall = wallMatName;
            rs.floor = floorMatName;
            rs.ceiling = ceilingMatName;
            rs.floorMapItemId = transform.GetSiblingIndex();
            return rs;
        }
        public void SetRoomSetting(RoomSetting roomSetting)
        {
            HDLogger.Log("SetRoomSetting " + roomSetting.wall + " " + roomSetting.floor, HDLogger.LogCategory.Interior);

            if (!string.IsNullOrEmpty(roomSetting.floor))
            {
                Material m = Resources.Load<Material>(HDSettings.FLOOR_MATERIALS_FOLDER + roomSetting.floor);
                if (m == null)
                    m = Resources.Load<Material>(HDSettings.OUTSIDE_MATERIALS_FOLDER + roomSetting.floor);
                if (m != null)
                    ApplyFloorMaterial(m);
            }
            if (!string.IsNullOrEmpty(roomSetting.wall))
            {
                Material m = Resources.Load<Material>(HDSettings.WALL_MATERIALS_FOLDER + roomSetting.wall);
                if (m != null) ApplyInteriorWallMaterial(m);
            }
            if (!string.IsNullOrEmpty(roomSetting.ceiling))
            {
                Material m = Resources.Load<Material>(HDSettings.CEILING_MATERIALS_FOLDER + roomSetting.ceiling);
                if (m != null) ApplyCeilingMaterial(m);
            }
        }
#else
    public object GetRoomSetting()
    {
        return null;
    }

    public void SetRoomSetting(object roomSetting)
    {

    }
#endif
    }
}
