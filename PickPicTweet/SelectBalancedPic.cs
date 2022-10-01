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
    /// <summary>
    /// _orderedSetは選ばれる画像の候補を含むリスト
    /// </summary>
    private List<string> _orderedSet;
    private readonly PicSimilarity _ps;

    public SelectBalancedPic(List<string> paths)
    {
        _orderedSet = paths;
        _ps = new PicSimilarity();
    }

    /// <summary>
    /// ファイル名のリストを作成日時降順でソート
    /// </summary>
    /// <returns></returns>
    private void DescendingTimeSort()
    {
         _orderedSet =  _orderedSet.OrderBy(filePath => File.GetCreationTime(filePath).Date).Reverse().ToList();
    }
    
    /// <summary>
    /// ハミング距離がthreshold以下(類似度が大きい)ものを取り除いた集合からランダムに要素を一つ選ぶ
    /// 要素数が0の場合はとりあえず空文字を返す
    /// </summary>
    /// <param name="pivot"></param>
    /// <param name="threshold"></param>
    private string EliminateNearsAndPop(string pivot,int threshold)
    {
        if (_orderedSet.Count == 0) return "";
        var pivotImage = new Bitmap(pivot);// 画像類似度計算の基準となる画像
        var removePaths = new List<string>();// 削除用(基準画像と距離が近い画像)のリスト
        // 基準画像から距離が近い画像のリストを作成
        foreach (string path in _orderedSet)
        {
            var img = new Bitmap(path);
            int d = _ps.ComputeHammingDistance(pivotImage, img);
            if (d <= threshold)
            {
                removePaths.Add(path);
            }
        }
        // 画像候補から距離が近い画像を削除
        foreach (string path in removePaths)
        {
            _orderedSet.Remove(path);
        }

        if (_orderedSet.Count == 0) return "";
        
        // _orderedSetは時系列降順なので、前半からランダムに選ぶことでできるだけ最近の画像を選ぶ
        var random = new Random();
        string val= _orderedSet[random.Next(_orderedSet.Count/2)];
        _orderedSet.Remove(val);
        return val;
    }
    
    /// <summary>
    /// 画像同士の距離(距離が近ければ似ていて遠ければ似ていない,0以上64以下)がthreshold(デフォルトは20)よりおおきい画像を最大で4枚選ぶ
    /// </summary>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public List<string> Pick(int threshold = 20)
    {
        // 画像候補のリスト(_orderSet)を時系列降順に並び替える
        DescendingTimeSort();
        if (_orderedSet.Count == 0) return new List<string>();
        
        // 最近の画像をランダムに1つ取り出しリストから消す
        List<string> recentPaths = _orderedSet.GetRange(0, _orderedSet.Count/3+1); // 最近の画像を取り出す
        var random = new Random();
        string path1 = recentPaths[random.Next(recentPaths.Count)]; // ランダムに選ぶ
        _orderedSet.Remove(path1);// 画像候補から最近の画像を消す
        
        // いい感じに選んだ画像パスを追加
        var balancedPaths = new List<string> {path1};

        // いい感じに選んだ画像パスを追加
        string path2 = EliminateNearsAndPop(path1,threshold);
        if (path2 != "") balancedPaths.Add(path2);
        string path3 = EliminateNearsAndPop(path2,threshold);
        if (path3 != "") balancedPaths.Add(path3);
        string path4 = EliminateNearsAndPop(path3,threshold);
        if (path4 != "") balancedPaths.Add(path4);
        
        return balancedPaths;
    }

    public void PrintBalancedPaths()
    {
        foreach (var path in _orderedSet)
        {
            Console.WriteLine(path);
        }
    }
}