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

    private bool isAllowedFileExtension(string extension)
    {
        var list = new List<string>() {".png", ".jpg", ".jpeg",".gif"};
        return list.Contains(extension);
    }

    private enum DistDateFormat
    {
        Month,
        Date
    }

    /// <summary>
    /// 画像フォルダを月別・日付別に割り振る
    /// </summary>
    /// <param name="sourceDirectory"></param>
    /// <param name="dateFormat"></param>
    private void DistributeTime(string sourceDirectory,DistDateFormat dateFormat)
    {
        string format;
        Func<string, bool> isDir; 
        if (dateFormat == DistDateFormat.Month)
        {
            format = "yyyy-MM";
            isDir = IsMonth;
        }
        else if(dateFormat == DistDateFormat.Date)
        {
            format = "yyyy-MM-dd";
            isDir = IsDate;
        }
        else
        {
            return;
        }
        try
        {
            IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory);
            foreach (string currentFile in files)
            {
                FileInfo fileInfo = new FileInfo(currentFile);
                if (!isAllowedFileExtension(fileInfo.Extension)) continue;
                var destDir = sourceDirectory + "\\" + fileInfo.CreationTime.ToString(format);
                var destFilePath = destDir + "\\" + fileInfo.Name;
                // ディレクトリが存在し、かつ名前に月・日付が含まれている場合
                if (isDir(destDir))
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
        switch (flag)
        {
            case 0:
                return;
            case 1:
            {
                DistributeTime(sourceDirectory,DistDateFormat.Month);
                break;
            }
            case 2:
            {
                DistributeTime(sourceDirectory,DistDateFormat.Month);
                IEnumerable<string> files = Directory.EnumerateDirectories(sourceDirectory);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    string monthDirPath = fi.FullName;
                    //ディレクトリが存在し、名前に月が含まれる場合
                    if (IsMonth(monthDirPath))
                    {
                        DistributeTime(monthDirPath,DistDateFormat.Date);
                    }
                }
                break;
            }
            default:
                return;
        }
    }
}