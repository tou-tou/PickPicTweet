using System.Drawing;
using System.Numerics;
using CoreTweet;

namespace VRCPicSimilarity;

/// <summary>
/// 最大100枚の画像から最近撮られた10枚を選び1枚(A)ランダムに取り出す
/// ImageSetを時系列の降順にソート
/// Aからthreshold以上のImageSetをつくる
/// ImageSetからランダムにBを選ぶ
/// Bからthreshold以上のImageSetを作る
/// ImageSetからランダムにCを選ぶ
/// Cからthreshold以上のImageSetを作る
/// ImageSetからランダムにDを選ぶ
/// 途中でImageSetの大きさが0になるか4毎選ぶかで終了
/// </summary>
public class SelectBalancedPic
{
    private List<string> _paths;
    private PicSimilarity _ps;

    public SelectBalancedPic(params string[] paths)
    {
        _paths = paths.ToList();
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
        _paths.Sort();
        _paths.Reverse();
    }
    
    /// <summary>
    /// ハミング距離がthreshold以下(類似度が大きい)ものを取り除く
    /// </summary>
    /// <param name="pivot"></param>
    /// <param name="threshold"></param>
    private string EliminateNearsAndPop(string pivot,int threshold)
    {
        var pivotImage = new Bitmap(pivot);
        foreach (var path in _paths)
        {
            var img = new Bitmap(path);
            var d = _ps.ComputeHammingDistance(pivotImage, img);
            if (d <= threshold)
            {
                _paths.Remove(path);
            }
        }
        
        var random = new Random();
        var val= _paths[random.Next(_paths.Count)];
        _paths.Remove(val);
        return val;

    }
    public List<string> Pick(int threshold)
    {
        Sort();
        // ランダムに1つ取り出しリストから消す
        var random = new Random();
        var path1= _paths[random.Next(_paths.Count)];
        _paths.Remove(path1);
        
        // TODO: pathがみつからない場合、threshold - 1 でpathをみつける処理を追加
        var path2 = EliminateNearsAndPop(path1,threshold);
        var path3 = EliminateNearsAndPop(path2,threshold);
        var path4 = EliminateNearsAndPop(path3,threshold);

        return new List<string> {path1, path2, path3, path4};
    }

    public void PrintPaths()
    {
        foreach (var path in _paths)
        {
            Console.WriteLine(path);
        }
    }
    
    
}