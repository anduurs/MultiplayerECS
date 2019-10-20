﻿using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Entity.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Model.Entity.Components
{
    public class CropComponentServer : FNEComponent<CropComponentData>
    {
        public void Tick()
        {
            GrowCrop();
        }

        private void GrowCrop()
        {
            if (m_Data.cropReady)
                return;

            float temperature = ServerApp.World.GetTileTemperature(parent.position);

            if (temperature > m_Data.growthTemperatureMinimum && temperature < m_Data.growthTemperatureMaximum)
            {
                float diff = m_Data.growthTemperatureMaximum - m_Data.growthTemperatureMinimum;
                // percent = 0.5 means it is the optimal temperature
                float percent = (temperature - m_Data.growthTemperatureMinimum) / diff;
                // convert 0.5 to 1
                percent = percent * 2f;
                if (percent > 1f)
                    percent = 2f - percent;

                if (percent > 0.75f)
                    m_Data.growthStatus = CropComponentData.GrowthStatus.Optimal;
                else
                    m_Data.growthStatus = CropComponentData.GrowthStatus.Growing;

                m_Data.health += 0.2f;

                m_Data.growth += percent;

                ServerApp.NetAPI.BR_UpdateComponents(parent, m_Data);
            }
            else if (temperature < m_Data.deathTemperatureLow)
            {
                m_Data.health -= (1 - (m_Data.enduranceInPercent / 100f));
                m_Data.growthStatus = CropComponentData.GrowthStatus.Freezing;

                ServerApp.NetAPI.BR_UpdateComponents(parent, m_Data);
            }
            else if (temperature > m_Data.deathTemperatureHigh)
            {
                m_Data.health -= (1 - (m_Data.enduranceInPercent / 100f));
                m_Data.growthStatus = CropComponentData.GrowthStatus.Overheating;

                ServerApp.NetAPI.BR_UpdateComponents(parent, m_Data);
            }
            else if (temperature < m_Data.growthTemperatureMinimum)
            {
                bool sendUpdate = m_Data.growthStatus != CropComponentData.GrowthStatus.ToCold;
                m_Data.growthStatus = CropComponentData.GrowthStatus.ToCold;

                if (sendUpdate)
                    ServerApp.NetAPI.BR_UpdateComponents(parent, m_Data);
            }
            else if (temperature > m_Data.growthTemperatureMaximum)
            {
                bool sendUpdate = m_Data.growthStatus != CropComponentData.GrowthStatus.ToWarm;
                m_Data.growthStatus = CropComponentData.GrowthStatus.ToWarm;

                if (sendUpdate)
                    ServerApp.NetAPI.BR_UpdateComponents(parent, m_Data);
            }

            if (m_Data.health > 100)
                m_Data.health = 100;

            if (m_Data.growth > m_Data.growthTimeTicks)
                CropReady();
        }

        private void CropReady()
        {
            m_Data.cropReady = true;
            m_Data.growthStatus = CropComponentData.GrowthStatus.Harvestable;

            ServerApp.NetAPI.BR_UpdateComponents(parent, m_Data);
        }

        public void HarvestCrop(FNEEntity harvsestingPlayer)
        {
            /*GridInventoryComponent gic = harvsestingPlayer.GetComponent<GridInventoryComponent>();

            int amount = 0;
            for (int i = 0; i < data.produceDice; i++)
                amount += FNERandom.GetRandomIntInRange(1, data.produceDieSize + 1);

            Item produce = Item.GenerateItem(
                data.produceRef,
                amount
            );

            if (gic.GetFirstAvailableSlot(produce) == null)
            {
                return;
            }

            gic.AutoPlaceIfPossible(produce);

            if (data.consumedOnHarvest)
            {
                EntityFactory.Server_DestroyEntity(parent);
                parent.OnDeath();
                parent.isDead = true;

                ServerApp.NetAPI.UnsyncEntity(parent);

                var destroyPacket = ServerApp.NetAPI.GetPacket<Packet_DestroyEntity>();
                ServerApp.NetAPI.BroadCastToAllRelevantClients(
                    parent.position,
                    destroyPacket.WritePacket(parent)
                );
            }

            gic.Server_Broadcast_All_Relevant_Update();*/
        }
    }
}