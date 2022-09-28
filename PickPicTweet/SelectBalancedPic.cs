using System.Drawing;

namespace PickPicTweet;

/// <summary>
/// 最大100枚の画像から撮影時刻と見た目を考慮してバランスよく最大4枚の画像を選ぶ
/// --------------
/// 以下アルゴリズム
/// 最大100枚の画像リストから最近撮られた10枚を選び1枚(Image1)ランダムに取り出す
/// リストを時系列の降順にソート
/// Image1からの距離がthreshold以上のリストをつくり、リストの前半からランダムにImage2を選ぶ
/// 同様にImage3,Image4を選ぶ
/// 途中でリストの要素数が0になるか4枚選ぶかで終了
/// TODO: pathがみつからない場合、threshold - 1 でpathをみつける処理を追加
/// </summary>
public class SelectBalancedPic
{
    private List<string> _paths;
    private PicSimilarity _ps;

    public SelectBalancedPic(List<string> paths)
    {
        _paths = paths;
        _ps = new PicSimilarity();
    }

    /// <summary>
    /// ファイル名のリストを降順名前順でソート
    /// VRChatのスクショには時刻が含まれるので時系列降順になる
    /// TODO:あとでfile infoから作成日時をとってきて並び替える
    /// </summary>
    /// <returns></returns>
    public void Sort()
    {
         _paths =  _paths.OrderBy(filePath => File.GetCreationTime(filePath).Date).Reverse().ToList();
    }
    
    /// <summary>
    /// ハミング距離がthreshold以下(類似度が大きい)ものを取り除いた集合からランダムに要素を一つ選ぶ
    /// 要素数が0の場合はとりあえず空文字を返す
    /// </summary>
    /// <param name="pivot"></param>
    /// <param name="threshold"></param>
    private string EliminateNearsAndPop(string pivot,int threshold)
    {
        if (_paths.Count == 0) return "";
        var pivotImage = new Bitmap(pivot);
        // 削除用のリスト
        var removePaths = new List<string>();
        foreach (string path in _paths)
        {
            var img = new Bitmap(path);
            int d = _ps.ComputeHammingDistance(pivotImage, img);
            if (d <= threshold)
            {
                removePaths.Add(path);
            }
        }
        // 元のリストから削除
        foreach (string path in removePaths)
        {
            _paths.Remove(path);
        }

        if (_paths.Count == 0) return "";
        
        var random = new Random();
        // _pathsは時系列降順なので、前半からランダムに選ぶことでできるだけ最近の画像を選ぶ
        string val= _paths[random.Next(_paths.Count/2)];
        _paths.Remove(val);
        return val;

    }
    
    /// <summary>
    /// 画像同士の距離(距離が近ければ似ていて遠ければ似ていない,0以上64以下)がthreshold(デフォルトは15)よりおおきい画像を最大で4枚選ぶ
    /// </summary>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public List<string> Pick(int threshold = 15)
    {
        var rt = new List<string>();
        Sort();
        // 最近の画像をランダムに1つ取り出しリストから消す
        var random = new Random();
        if (_paths.Count == 0) return new List<string>();
        List<string> recentPaths = _paths.GetRange(0, _paths.Count/3+1);
        string path1= recentPaths[random.Next(recentPaths.Count)];
        // 元のリストから最近の画像を消す
        _paths.Remove(path1);
        rt.Add(path1);
        
        // 適度な類似度をもつ画像パスを取り出す
        string path2 = EliminateNearsAndPop(path1,threshold);
        if (path2 != "") rt.Add(path2);
        string path3 = EliminateNearsAndPop(path2,threshold);
        if (path3 != "") rt.Add(path3);
        string path4 = EliminateNearsAndPop(path3,threshold);
        if (path4 != "") rt.Add(path4);
        
        return rt;
    }

    public void PrintPaths()
    {
        foreach (var path in _paths)
        {
            Console.WriteLine(path);
        }
    }
    
    
}