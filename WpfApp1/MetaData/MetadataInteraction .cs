using System.IO;
using System.Windows.Media.Imaging;

namespace WpfApp1.MetaData
{
    class MetadataInteraction : Metadata
    {
        public Metadata ExtractMetadata(string picturePath)
        {
            Metadata file = new Metadata();

            using (FileStream Foto = File.Open(picturePath, FileMode.Open, FileAccess.Read))
            {
                BitmapDecoder decoder = JpegBitmapDecoder.Create(Foto, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default); //"распаковали" снимок и создали объект decoder
                BitmapMetadata ImgEXIF = (BitmapMetadata)decoder.Frames[0].Metadata.Clone(); //считали и сохранили метаданные
                file.Path = picturePath;
                file.Name = System.IO.Path.GetFileNameWithoutExtension(picturePath);
                file.Weight = new FileInfo(picturePath).Length;
                if (file.Title == null) file.Title = "";
                file.Format = ImgEXIF.Format;
                try
                {
                    if (ImgEXIF.CameraModel == null) file.CameraModel = "Unknown";
                    else file.CameraModel = ImgEXIF.CameraModel;
                }
                catch
                {
                    file.CameraModel = "Unknown";
                }
                try
                {
                    if (ImgEXIF.CameraManufacturer == null) file.CameraManufacturer = "Unknown";
                    else file.CameraManufacturer = ImgEXIF.CameraManufacturer;
                }
                catch
                {
                    file.CameraManufacturer = "Unknown";
                }
                try
                {
                    if (ImgEXIF.DateTaken == null) file.DateOfShot = "01.12.1998";
                    else file.DateOfShot = ImgEXIF.DateTaken;
                }
                catch { }
            }

            return file;
        }
        public void SavePictureMetadata(Metadata metadata, string pathToAllMetadata)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(pathToAllMetadata, FileMode.Append)))
            {

                writer.Write(metadata.Path);
                writer.Write(metadata.Name);
                writer.Write(metadata.CameraManufacturer);
                writer.Write(metadata.CameraModel);
                writer.Write(metadata.Title);
                writer.Write(metadata.DateOfShot);
                writer.Write(metadata.Format);
                writer.Write(metadata.Weight);
            }
        }
        public void DeletePictureMetadata(Metadata metadata, string pathToAllMetadata)
        {
            Metadata bufMetadata = new Metadata();
            string updatedFile = pathToAllMetadata.Insert(pathToAllMetadata.LastIndexOf("\\") + 1, "buf");
            using (BinaryWriter writer = new BinaryWriter(File.Open(updatedFile, FileMode.Create)))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(pathToAllMetadata, FileMode.Open)))
                {
                    while (reader.PeekChar() > -1)
                    {
                        bufMetadata.Path = reader.ReadString();
                        bufMetadata.Name = reader.ReadString();
                        bufMetadata.CameraManufacturer = reader.ReadString();
                        bufMetadata.CameraModel = reader.ReadString();
                        bufMetadata.Title = reader.ReadString();
                        bufMetadata.DateOfShot = reader.ReadString();
                        bufMetadata.Format = reader.ReadString();
                        bufMetadata.Weight = reader.ReadDouble();
                        if (bufMetadata.Path != metadata.Path)
                        {
                            writer.Write(bufMetadata.Path);
                            writer.Write(bufMetadata.Name);
                            writer.Write(bufMetadata.CameraManufacturer);
                            writer.Write(bufMetadata.CameraModel);
                            writer.Write(bufMetadata.Title);
                            writer.Write(bufMetadata.DateOfShot);
                            writer.Write(bufMetadata.Format);
                            writer.Write(bufMetadata.Weight);
                        }
                    }
                }
            }
            File.Delete(pathToAllMetadata);
            File.Move(updatedFile, pathToAllMetadata);
        }
    }
}
