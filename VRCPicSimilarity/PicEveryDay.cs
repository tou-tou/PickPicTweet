namespace VRCPicSimilarity;

public class PicEveryDay
{
    public void Distribute(string sourceDirectory)
    {
        try
        {
            var files = Directory.EnumerateFiles(sourceDirectory);
            foreach (string currentFile in files)
            {
                var fileInfo = new FileInfo(currentFile);
                var creationDate = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                var destDir = sourceDirectory + "\\" + creationDate;
                var destFilePath = destDir + "\\" + fileInfo.Name;
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                    if (!File.Exists(destFilePath))
                    {
                        fileInfo.MoveTo(destFilePath);
                    }
                }
                else
                {
                    if (!File.Exists(destFilePath))
                    {
                        fileInfo.MoveTo(destFilePath);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}