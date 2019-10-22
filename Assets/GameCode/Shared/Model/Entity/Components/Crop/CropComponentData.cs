using System.Xml.Serialization;
using Lidgren.Network;

namespace FNZ.Shared.Model.Entity.Components
{
    [XmlType("CropComponentData")]
    public class CropComponentData : FNEComponentData
    {
        [XmlElement("produceRef")]
        public string produceRef { get; set; }

        [XmlElement("produceDice")]
        public byte produceDice { get; set; }

        [XmlElement("produceDieSize")]
        public byte produceDieSize { get; set; }

        [XmlElement("additionalSeedChance")]
        public float additionalSeedChance { get; set; }

        [XmlElement("consumedOnHarvest")]
        public bool consumedOnHarvest { get; set; }

        [XmlElement("growthTimeTicks")]
        public float growthTimeTicks { get; set; }

        [XmlElement("growthTemperatureMinimum")]
        public float growthTemperatureMinimum { get; set; }

        [XmlElement("growthTemperatureMaximum")]
        public float growthTemperatureMaximum { get; set; }

        [XmlElement("deathTemperatureLow")]
        public float deathTemperatureLow { get; set; }

        [XmlElement("deathTemperatureHigh")]
        public float deathTemperatureHigh { get; set; }

        [XmlElement("enduranceInPercent")]
        public float enduranceInPercent { get; set; }

        public enum GrowthStatus
        {
            Optimal,
            Growing,
            ToCold,
            ToWarm,
            Freezing,
            Overheating,
            Harvestable
        }

        public float health = 100f;
        public float growth = 0f;
        public GrowthStatus growthStatus;

        public bool cropReady = false;

        public override void Serialize(NetBuffer bw)
        {
            bw.Write(growth);
            bw.Write(health);
            bw.Write((byte)growthStatus);
            bw.Write(cropReady);
        }

        public override void Deserialize(NetBuffer br)
        {
            growth = br.ReadSingle();
            health = br.ReadSingle();
            growthStatus = (GrowthStatus)br.ReadByte();
            cropReady = br.ReadBoolean();
        }

        public override ushort GetSizeInBytes()
        {
            return 0;
        }
    }
}