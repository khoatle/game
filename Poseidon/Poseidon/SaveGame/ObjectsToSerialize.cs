using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Poseidon
{
    [Serializable()]
    class ObjectsToSerialize: ISerializable
    {
        public HydroBot hydrobot;

        public ObjectsToSerialize()
        {
        }

        // To deserealize the objects
        public ObjectsToSerialize(SerializationInfo info, StreamingContext context)
        {
            this.hydrobot = (HydroBot)info.GetValue("hydrobot", typeof(HydroBot));
        }

        // To serialize the objects
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("hydrobot", this.hydrobot);
        }
    }
}
