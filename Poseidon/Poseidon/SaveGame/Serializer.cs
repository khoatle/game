using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Poseidon
{
    class Serializer
    {
        public Serializer()
        {
        }

        public void SerializeObjects(string filename, ObjectsToSerialize objectsToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectsToSerialize);
            stream.Close();
        }

        public ObjectsToSerialize DeSerializeObjects(string filename)
        {
            ObjectsToSerialize objectsToSerialize;
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectsToSerialize = (ObjectsToSerialize)bFormatter.Deserialize(stream);
            stream.Close();
            return objectsToSerialize;
        }
    }
}
