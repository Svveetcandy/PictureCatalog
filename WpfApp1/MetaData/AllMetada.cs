using System.IO;
using System.Collections.Generic;

namespace WpfApp1.MetaData
{
    class AllMetadata
    {
        public List<Metadata> metadata = new List<Metadata>();
        public void SaveAllMetadata(List<Metadata> metadata, string pathToAllMetadata)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(pathToAllMetadata, FileMode.OpenOrCreate)))
            {
                foreach (Metadata item in metadata)
                {
                    writer.Write(item.Path);
                    writer.Write(item.Name);
                    writer.Write(item.CameraManufacturer);
                    writer.Write(item.CameraModel);
                    writer.Write(item.Title);
                    writer.Write(item.DateOfShot);
                    writer.Write(item.Format);
                    writer.Write(item.Weight);
                }
            }
        }
        public List<Metadata> ReadAllMetadata(string pathToAllMetadata)
        {
            List<Metadata> metadata = new List<Metadata>();
            List<Metadata> metadataForDelete = new List<Metadata>();
            using (BinaryReader reader = new BinaryReader(File.Open(pathToAllMetadata, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    Metadata bufMetadata = new Metadata();
                    bufMetadata.Path = reader.ReadString();
                    bufMetadata.Name = reader.ReadString();
                    bufMetadata.CameraManufacturer = reader.ReadString();
                    bufMetadata.CameraModel = reader.ReadString();
                    bufMetadata.Title = reader.ReadString();
                    bufMetadata.DateOfShot = reader.ReadString();
                    bufMetadata.Format = reader.ReadString();
                    bufMetadata.Weight = reader.ReadDouble();
                    if (File.Exists(bufMetadata.Path))
                    {
                        metadata.Add(bufMetadata);
                    }
                    else
                    {
                        metadataForDelete.Add(bufMetadata);
                    }

                }
                reader.BaseStream.Position = 0;
            }

            if (metadataForDelete.Count != 0)
            {
                MetadataInteraction metadataInteraction = new MetadataInteraction();
                foreach (Metadata data in metadataForDelete)
                {
                    metadataInteraction.DeletePictureMetadata(data, pathToAllMetadata);
                }

            }
            return metadata;
        }
    }
}
