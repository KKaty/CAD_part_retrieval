using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;



namespace SolidWorksAddinUtility
{
    public partial class Utility
    {

        public static void WriteToBinaryFile(string path, object pc)
        {
            Stream fstream = new FileStream(path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new PreMergeToMergedDeserializationBinder();
            bf.Serialize(fstream, pc);
            fstream.Close();

        }

        public static object ReadToBinaryFile(string path)
        {
            Stream fstream = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new PreMergeToMergedDeserializationBinder();
            var readed = bf.Deserialize(fstream);
            return readed;
        }

        sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;

                // For each assemblyName/typeName that you want to deserialize to
                // a different type, set typeToDeserialize to the desired type.
                String exeAssembly = Assembly.GetExecutingAssembly().FullName;


                // The following line of code returns the type.
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, exeAssembly));

                return typeToDeserialize;
            }
        }
    }
}
