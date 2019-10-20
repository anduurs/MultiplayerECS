using FNZ.Shared.Model.Entity.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Model.Entity.Components
{
    public class CropComponentClient : FNEComponent<CropComponentData>
    {
        public float GetGrowthProgress()
        {
            return m_Data.growth / m_Data.growthTimeTicks;
        }
    }

}
