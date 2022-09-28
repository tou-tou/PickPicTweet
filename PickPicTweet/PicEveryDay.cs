namespace PickPicTweet;

/// <summary>
/// 画像を月別フォルダ、日付別フォルダに割り振りなおす
/// </summary>
public class PicEveryDay
{
    /// <summary>
    /// ファイル名が月で終わるかどうかをを判定
    /// ex:2022-09
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    private bool IsMonth(string dirPath)
    {
        //ディレクトリが存在しなかったらfalseを返す
        if (!Directory.Exists(dirPath)) return false;
        FileInfo fi = new FileInfo(dirPath);
        string[] sMonth = fi.Name.Split("-");
        if (sMonth.Length==2)
        {
            return int.TryParse(sMonth[0], out _) && int.TryParse(sMonth[1],out _);
        }
        return false;
    }
    
    /// <summary>
    /// ファイル名が日付で終わるかどうかを判定
    /// ex: 2022-09-27
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    private bool IsDate(string dirPath)
    {
        //ディレクトリが存在しなかったらfalseを返す
        if (!Directory.Exists(dirPath)) return false;
        FileInfo fi = new FileInfo(dirPath);
        string[] sDate = fi.Name.Split("-");
        if (sDate.Length == 3)
        {
            return int.TryParse(sDate[0], out _) && int.TryParse(sDate[1],out _) && int.TryParse(sDate[2],out _);
        }
        return false;
    }
    /// <summary>
    /// 画像フォルダを月別に割りふる
    /// </summary>
    /// <param name="sourceDirectory"></param>
    private void DistributeMonth(string sourceDirectory)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory);
        foreach (string currentFile in files)
        {
            FileInfo fileInfo = new FileInfo(currentFile);
            if (fileInfo.Extension != ".png") continue;
            var creationMonth = fileInfo.CreationTime.ToString("yyyy-MM");
            var destDir = sourceDirectory + "\\" + creationMonth;
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
    
    /// <summary>
    /// 画像フォルダを日付別に割り振る
    /// </summary>
    /// <param name="sourceDirectory"></param>
    private void DistributeDate(string sourceDirectory)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory);
        foreach (string currentFile in files)
        {
            FileInfo fileInfo = new FileInfo(currentFile);
            if (fileInfo.Extension != ".png") continue;
            var creationDate = fileInfo.CreationTime.ToString("yyyy-MM-dd");
            var destDir = sourceDirectory + "\\" + creationDate;
            var destFilePath = destDir + "\\" + fileInfo.Name;
            // ディレクトリが存在し、かつ名前に日付が含まれている場合
            if (IsDate(destDir))
            {
                if (!File.Exists(destFilePath))
                {
                    fileInfo.MoveTo(destFilePath);
                }
            }
            else
            {
                Directory.CreateDirectory(destDir);
                if (!File.Exists(destFilePath))
                {
                    fileInfo.MoveTo(destFilePath);
                }
            }
        }
    }
    
    /// <summary>
    /// flagが0の時は何もしない、1の時は月別、2の時は日付別
    /// 月別ディレクトリはソースディレクトリの直下に作られる
    /// 日別ディレクトリは月別ディレクトリ直下に作られる
    /// </summary>
    /// <param name="sourceDirectory"></param>
    /// <param name="flag"></param>
    public void Distribute(string sourceDirectory,int flag=0)
    {
        try
        {
            switch (flag)
            {
                case 0:
                    return;
                case 1:
                {
                    DistributeMonth(sourceDirectory);
                    break;
                }
                case 2:
                {
                    DistributeMonth(sourceDirectory);
                    IEnumerable<string> files = Directory.EnumerateDirectories(sourceDirectory);
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        var monthDirPath = fi.FullName;
                        Console.WriteLine(monthDirPath);

                        //ディレクトリが存在し、名前に月が含まれる場合
                        if (IsMonth(monthDirPath))
                        {
                            DistributeDate(monthDirPath);
                        }
                    }
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}